using UnityEngine;

public class ForcePadReader : MonoBehaviour
{
    public float f1, f2, f3, f4, f5;

    void Start()
    {
        SerialManager.Instance.OnDataReceived += OnSensorUpdate;
    }

    void OnSensorUpdate(float[] data)
    {
        if (data.Length < 5) return;

        f1 = data[0];
        f2 = data[1];
        f3 = data[2];
        f4 = data[3];
        f5 = data[4];

        // DEBUG
        Debug.Log($"[ForcePad] {f1}, {f2}, {f3}, {f4}, {f5}");
    }
}
