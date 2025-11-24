using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SimpleFrameAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 12f;
    public bool loop = true;

    SpriteRenderer sr;
    int index = 0;
    float timer = 0f;

    void Awake() { sr = GetComponent<SpriteRenderer>(); if (frames.Length > 0) sr.sprite = frames[0]; }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;
        timer += Time.deltaTime;
        float interval = 1f / Mathf.Max(0.0001f, fps);
        if (timer >= interval)
        {
            timer -= interval;
            index++;
            if (index >= frames.Length)
            {
                if (loop) index = 0;
                else { index = frames.Length - 1; enabled = false; } // หยุดอัพเดต
            }
            sr.sprite = frames[index];
        }
    }
}
