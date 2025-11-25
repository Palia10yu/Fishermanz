using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcaDamage : MonoBehaviour
{
    // Start is called before the first frame update
    static string enemyname = "Orca";
    public int damage = 2;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            PlayerHealth ph = collision.gameObject.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }
    }
}
