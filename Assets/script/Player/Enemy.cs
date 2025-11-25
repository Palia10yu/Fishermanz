using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Enemy : MonoBehaviour
{
    // === Stats and Core Components ===
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 5f;
    public float damage = 10f; 
    
    // *** NEW: ระยะโจมตี ***
    public float attackRange = 1.5f; 
    
    // *** NEW: คูลดาวน์การโจมตี ***
    public float attackCooldown = 1f; 
    
    // Components
    private Rigidbody2D rb2D; 
    private SpriteRenderer spriteRenderer; 
    private Transform targetPlayer; 
    
    // สถานะปัจจุบัน
    private float currentHealth;
    private float attackTimer; // ตัวจับเวลาคูลดาวน์

    // === Input and State ===
    private bool _isAttacking = false; 

    // === Unity Lifecycle Methods ===
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError($"{gameObject.name} requires a Rigidbody2D component for 2D movement.");
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"{gameObject.name} does not have a SpriteRenderer. Visual feedback for damage might be limited.");
        }
        
        Player playerComponent = FindObjectOfType<Player>();
        if (playerComponent != null)
        {
            targetPlayer = playerComponent.transform;
        } else {
            Debug.LogWarning("Player not found in the scene! Enemy cannot move.");
        }
        
        currentHealth = maxHealth;
        attackTimer = 0f; // เริ่มคูลดาวน์เป็น 0 พร้อมโจมตี
    }

    void Update()
    {
        if (targetPlayer == null) return;
        
        // นับถอยหลังคูลดาวน์
        attackTimer -= Time.deltaTime; 

        // 1. คำนวณระยะห่าง
        float distanceToPlayer = Vector3.Distance(targetPlayer.position, transform.position);

        if (distanceToPlayer <= attackRange)
        {
            // อยู่ในระยะโจมตี: หยุดและโจมตี
            Move(Vector3.zero); 
            TryAttack(); // ลองโจมตี (ถ้าคูลดาวน์หมด)
        }
        else
        {
            // ไม่อยู่ในระยะโจมตี: เคลื่อนที่เข้าหาผู้เล่น
            Vector3 direction = targetPlayer.position - transform.position;
            Vector3 moveDirection = new Vector3(direction.x, direction.y, 0).normalized;
            
            Move(moveDirection);
            Turn(moveDirection);
        }
    }

    void FixedUpdate()
    {
        // Attack(_isAttacking); // เราจะย้าย logic การโจมตีไปที่ TryAttack/OnCollision เพื่อให้ง่ายต่อการจัดการคูลดาวน์
        // แต่ถ้าต้องการใช้โครงสร้างเดิม คือให้ Attack ทำงานใน FixedUpdate และ TryAttack เซ็ต _isAttacking = true
    }

    // *** NEW: เมธอดสำหรับลองโจมตี (มีคูลดาวน์) ***
    private void TryAttack()
    {
        if (attackTimer <= 0)
        {
            // 1. ตั้งค่าสถานะโจมตีเป็นจริง (เพื่อเรียกใช้ Attack() ใน FixedUpdate)
            // หรือ โจมตีตรงนี้เลย
            
            // โจมตีโดยตรง (ง่ายกว่า)
            // สมมติว่า Player มี TakeDamage และ Enemy ตัวนี้ต้องมี Collider2D เป็น Trigger
            
            // ใช้การทำดาเมจโดยตรงแทนการเซ็ต _isAttacking เพื่อความง่าย
            
            // *** โจมตีตรงนี้เลย ***
            Player target = targetPlayer.GetComponent<Player>();
            if (target != null)
            {
                // ** ให้ดาเมจจริง **
                target.TakeDamage(damage); 
                Debug.Log($"Enemy attacks Player for {damage} damage.");
            }
            
            // 2. รีเซ็ตคูลดาวน์
            attackTimer = attackCooldown;
        }
    }


    // === Movement and Turning ===
    public void Move(Vector3 direction)
    {
        if (rb2D != null)
        {
            rb2D.velocity = direction * moveSpeed;
        }
    }

    public void Turn(Vector3 direction)
    {
        if (spriteRenderer != null && direction.x != 0)
        {
            if (direction.x < 0) {
                spriteRenderer.flipX = true; 
            } else if (direction.x > 0) {
                spriteRenderer.flipX = false; 
            }
        }
    }

    // === Combat System ===
    public void Attack(bool isAttacking) 
    {
        // โค้ดนี้ถูกแทนที่ด้วย TryAttack() แล้ว เพื่อให้จัดการคูลดาวน์ได้ง่าย
        // หากต้องการใช้ Attack() ใน FixedUpdate เหมือนเดิม ต้องเปลี่ยน TryAttack() 
        // ให้แค่เซ็ต _isAttacking = true; แทนการทำดาเมจโดยตรง
        if (isAttacking) 
        {
            // ส่วนนี้ถูกละเลยในการแก้ไขนี้ เพราะ TryAttack ทำงานแทนแล้ว
            // แต่ถ้าต้องการให้มันทำงาน ให้ใส่ logic การทำดาเมจ/Raycast ในนี้
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

    private IEnumerator FlashColor(Color color, float duration)
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