using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;
    public LayerMask LayerToHit;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & LayerToHit) != 0)
        {
            Damageable damageable = collision.gameObject.GetComponent<Damageable>();

            if (damageable != null)
            {
                float direction = Mathf.Sign(collision.transform.position.x - transform.position.x);
                Vector2 deliveredKnockBack = new Vector2(knockback.x * direction, knockback.y);

                bool goHit = damageable.Hit(attackDamage, deliveredKnockBack);

                if (goHit)
                {
                    Debug.Log(collision.gameObject.name + " hit for " + attackDamage);
                }
            }
        }
        
    }
}
