using UnityEngine;

public class ButterflyAnimationController : MonoBehaviour
{
    [Header("Hotkeys")]
    public KeyCode keyAlpha = KeyCode.Alpha1;
    public KeyCode keyWing = KeyCode.Alpha2;
    public KeyCode keyRandom = KeyCode.Alpha3;
    public KeyCode keyStop = KeyCode.Alpha0;

    [Header("Playback")]
    public float targetFps = 12f;

    void Update()
    {
        if (Input.GetKeyDown(keyAlpha))
            BroadcastPlay(ButterflyAnimator.PlayMode.AlphaOnly, targetFps);
        else if (Input.GetKeyDown(keyWing))
            BroadcastPlay(ButterflyAnimator.PlayMode.WingOnly, targetFps);
        else if (Input.GetKeyDown(keyRandom))
            BroadcastPlay(ButterflyAnimator.PlayMode.Random, targetFps);
        else if (Input.GetKeyDown(keyStop))
            BroadcastStop();
    }

    private void BroadcastPlay(ButterflyAnimator.PlayMode mode, float fps)
    {
        foreach (var b in ButterflyAnimator.All)
            if (b != null) b.Play(mode, fps);
    }

    private void BroadcastStop()
    {
        foreach (var b in ButterflyAnimator.All)
            if (b != null) b.SetIdle();
    }
}
