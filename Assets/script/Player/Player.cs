using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    // === Stats and Core Components ===
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float damage = 10f; 
    
    // สถานะปัจจุบัน
    private float currentHealth;
    
    // Components
    // *** เปลี่ยนเป็น Rigidbody2D ***
    private Rigidbody2D rb2D; 
    private SpriteRenderer spriteRenderer; 

    // === Input and State ===
    private Vector3 _inputDirection;
    private bool _isAttacking = false;

    // === Unity Lifecycle Methods ===
    void Start()
    {
        // *** รับ Component Rigidbody2D ***
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Player requires a Rigidbody2D component for 2D movement.");
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Player does not have a SpriteRenderer. Visual feedback for damage might be limited.");
        }
        
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        Move(_inputDirection);
        // ตัด Turn ออกไป เพราะไม่จำเป็นใน 2D (หรือต้องปรับเป็นการ Flip Sprite)
        Attack(_isAttacking); 
    }

    // === Input Handling ===
    private void HandleInput()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // *** การแก้ไข: ใช้ (x, y, 0) สำหรับ 2D (XY Plane) ***
        _inputDirection = new Vector3(x, y, 0).normalized; 
        
        if (Input.GetMouseButtonDown(0))
        {
            _isAttacking = true;
        }
    }

    // === Movement and Turning ===
    public void Move(Vector3 direction)
    {
        // *** การแก้ไข: ใช้ rb2D ***
        if (rb2D != null)
        {
            // การเคลื่อนที่โดยใช้ Rigidbody2D
            rb2D.velocity = direction * moveSpeed;
        }
    }

    // ตัดเมธอด Turn ออกไป หรือคงไว้แต่ไม่มีการทำงาน
    public void Turn(Vector3 direction)
    {
        // หากต้องการให้ตัวละครหันซ้าย/ขวา ให้ใช้ SpriteRenderer.flipX แทนการหมุน Transform
        if (direction.x < 0) {
            spriteRenderer.flipX = true; // หันซ้าย
        } else if (direction.x > 0) {
            spriteRenderer.flipX = false; // หันขวา
        }
    }

    // === Combat System ===
    public void Attack(bool isAttacking) 
    {
        if (isAttacking) 
        {
            Debug.Log($"{gameObject.name} attacks for {damage} damage.");
            _isAttacking = false;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"{gameObject.name} takes {damageAmount} damage. Health remaining: {currentHealth}");
        
        if (spriteRenderer != null)
        {
            StartCoroutine(FlashColor(Color.red, 0.1f));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private System.Collections.IEnumerator FlashColor(Color color, float duration)
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = originalColor;
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        gameObject.SetActive(false); 
    }
}