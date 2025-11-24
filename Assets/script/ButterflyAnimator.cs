using UnityEngine;
using System.Collections.Generic;

public class ButterflyAnimator : MonoBehaviour
{
    public enum PlayMode { Idle = 0, AlphaOnly = 1, WingOnly = 2, Random = 3 }

    [Header("Sprite Frames")]
    public Sprite[] alphaWingFrames;
    public Sprite[] normalWingFrames;

    [Header("Config")]
    [Tooltip("เวลาที่จะเล่นแล้วหยุดอัตโนมัติ (วินาที)")]
    public float playDuration = 10f;

    [Header("Status (Read-Only)")]
    [SerializeField] private PlayMode currentMode = PlayMode.Idle;
    [SerializeField] private float currentFps = 0f;
    [SerializeField] private bool playingReverse = false;

    private SpriteRenderer sr;
    private Sprite[] activeFrames;
    private int frameIndex;
    private float timer;
    private bool isActive = false;
    private float timeLeft = 0f; // ตัวจับเวลา

    // Register with controller
    private static readonly HashSet<ButterflyAnimator> s_All = new HashSet<ButterflyAnimator>();
    public static IReadOnlyCollection<ButterflyAnimator> All => s_All;

    private void OnEnable() { s_All.Add(this); }
    private void OnDisable() { s_All.Remove(this); }

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!sr) sr = gameObject.AddComponent<SpriteRenderer>();
    }

    void Start()
    {
        SetIdle();
    }

    void Update()
    {
        if (!isActive || activeFrames == null || activeFrames.Length == 0 || currentFps <= 0f) return;

        // นับเวลาถอยหลัง
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            SetIdle();
            return;
        }

        // เล่นเฟรม
        timer += Time.deltaTime;
        float frameDur = 1f / Mathf.Max(1f, currentFps);

        while (timer >= frameDur)
        {
            timer -= frameDur;
            StepFrame();
        }

        if (frameIndex >= 0 && frameIndex < activeFrames.Length)
            sr.sprite = activeFrames[frameIndex];
    }

    private void StepFrame()
    {
        if (!playingReverse)
        {
            frameIndex++;
            if (frameIndex >= activeFrames.Length) frameIndex = 0;
        }
        else
        {
            frameIndex--;
            if (frameIndex < 0) frameIndex = activeFrames.Length - 1;
        }
    }

    public void SetIdle()
    {
        currentMode = PlayMode.Idle;
        isActive = false;
        currentFps = 0f;
        activeFrames = null;
        frameIndex = 0;
        timer = 0f;
        timeLeft = 0f;
        if (sr) { sr.enabled = false; sr.flipX = false; sr.flipY = false; }
    }

    public void Play(PlayMode mode, float fps)
    {
        currentMode = mode;
        currentFps = Mathf.Max(1f, fps);
        isActive = true;
        timeLeft = playDuration; // เล่นตามเวลาที่กำหนด

        if (sr) sr.enabled = true;

        switch (mode)
        {
            case PlayMode.AlphaOnly:
                activeFrames = alphaWingFrames;
                playingReverse = false;
                sr.flipX = false; sr.flipY = false;
                frameIndex = 0;
                break;

            case PlayMode.WingOnly:
                activeFrames = normalWingFrames;
                playingReverse = false;
                sr.flipX = false; sr.flipY = false;
                frameIndex = 0;
                break;

            case PlayMode.Random:
                bool pickAlpha = Random.value < 0.5f;
                activeFrames = pickAlpha ? alphaWingFrames : normalWingFrames;
                playingReverse = (Random.value < 0.5f);
                sr.flipX = (Random.value < 0.5f);
                sr.flipY = false;
                frameIndex = (activeFrames != null && activeFrames.Length > 0)
                             ? Random.Range(0, activeFrames.Length) : 0;
                break;

            default:
                SetIdle();
                return;
        }

        if (activeFrames == null || activeFrames.Length == 0)
        {
            SetIdle();
            return;
        }

        timer = 0f;
        if (frameIndex >= 0 && frameIndex < activeFrames.Length)
            sr.sprite = activeFrames[frameIndex];
    }
}
