using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;

public class SerialManager : MonoBehaviour
{
    public static SerialManager Instance;

    [Header("Serial Config")]
    public string portName = "COM5";     // Mac ตัวอย่าง: "/dev/tty.usbmodem2101"
    public int baudRate = 115200;
    public bool autoConnect = true;

    SerialPort serial;
    Thread readThread;
    bool isRunning = false;

    string latestLine = "";
    object lockObject = new object();

    // ====== EVENT สำหรับทีมอื่นใช้ ======
    public Action<float[]> OnDataReceived;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (autoConnect)
            OpenPort();
    }

    void OnDestroy()
    {
        ClosePort();
    }

    // ===============================
    //         Open / Close Port
    // ===============================
    public void OpenPort()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 1;
            serial.DtrEnable = true;
            serial.RtsEnable = true;
            serial.Open();

            isRunning = true;
            readThread = new Thread(ReadSerialLoop);
            readThread.Start();

            Debug.Log($"[SerialManager] Connected to {portName}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SerialManager] Cannot open port: {ex.Message}");
        }
    }

    public void ClosePort()
    {
        try
        {
            isRunning = false;

            if (readThread != null && readThread.IsAlive)
                readThread.Join();

            if (serial != null && serial.IsOpen)
                serial.Close();
        }
        catch { }
    }

    // ===============================
    //        Thread: Read Serial
    // ===============================
    void ReadSerialLoop()
    {
        while (isRunning)
        {
            try
            {
                string line = serial.ReadLine();

                lock (lockObject)
                {
                    latestLine = line;
                }
            }
            catch { }
        }
    }

    // ===============================
    //        Update: Parse Data
    // ===============================
    void Update()
    {
        if (string.IsNullOrEmpty(latestLine)) return;

        string line;
        lock (lockObject)
        {
            line = latestLine;
            latestLine = "";
        }

        float[] values = ParsePacket(line);

        if (values != null && OnDataReceived != null)
            OnDataReceived(values);
    }

    // ===============================
    //      PARSE "<v1,v2,v3>"
    // ===============================
    float[] ParsePacket(string packet)
    {
        // รูปแบบที่ต้องการ: <1,2,3,4>
        if (!packet.StartsWith("<") || !packet.EndsWith(">"))
            return null;

        try
        {
            string inner = packet.Substring(1, packet.Length - 2);   // ตัด <>
            string[] tokens = inner.Split(',');

            float[] results = new float[tokens.Length];

            for (int i = 0; i < tokens.Length; i++)
            {
                if (float.TryParse(tokens[i], out float value))
                    results[i] = value;
                else
                    results[i] = 0;
            }

            return results;
        }
        catch
        {
            return null;
        }
    }
}
