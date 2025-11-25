using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    // Start is called before the first frame update

    public string playername = "ungung";
    public int health;
    public int maxHealth = 3;
    public Rigidbody rb;
    bool _isAttacking = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int amount)
    {
        Debug.Log($"attack {playername}");
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
