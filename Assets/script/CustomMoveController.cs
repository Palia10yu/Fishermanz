using UnityEngine;

public class PlayerMovementSmooth : MonoBehaviour
{
    [Header("Step Settings")]
    public float stepSize = 0.3f;       // ขยับทีละนิด
    public float smoothTime = 0.08f;    // ยิ่งน้อย = ยิ่งตอบสนองเร็ว

    [Header("Custom Keys")]
    public KeyCode keyUp = KeyCode.W;
    public KeyCode keyDown = KeyCode.S;
    public KeyCode keyLeft = KeyCode.A;
    public KeyCode keyRight = KeyCode.D;

    private Vector3 targetPos;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        // input up-down
        if (Input.GetKeyDown(keyUp))
            targetPos += Vector3.up * stepSize;

        if (Input.GetKeyDown(keyDown))
            targetPos += Vector3.down * stepSize;

        // input left
        if (Input.GetKeyDown(keyLeft))
        {
            targetPos += Vector3.left * stepSize;

            // 🌟 กลับหน้าปลาไปทางซ้าย
            transform.localScale = new Vector3(1, 1, 1);
        }

        // input right
        if (Input.GetKeyDown(keyRight))
        {
            targetPos += Vector3.right * stepSize;

            // 🌟 กลับหน้าปลาไปทางขวา
            transform.localScale = new Vector3(-1, 1, 1);
        }

        // Smooth slide ไปหา targetPos
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
