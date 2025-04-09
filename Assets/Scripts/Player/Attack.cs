using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.gameObject.GetComponent<Damageable>();// Find object damageable 

        if (damageable != null)
        {
            float direction = Mathf.Sign(collision.transform.position.x - transform.position.x); // Math new direction 
            Vector2 deliveredKnockBack = new Vector2(knockback.x * direction, knockback.y);// Create a new position

            bool goHit = damageable.Hit(attackDamage, deliveredKnockBack); // Update damage and new position
            

            if (goHit)
            {
                Debug.Log(collision.gameObject.name + " hit for " + attackDamage);
            }
        }
    }
}
