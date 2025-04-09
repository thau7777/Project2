using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviromentTree : EnvironmentalResource
{
    protected override void Awake()
    {
        base.Awake();
        animator.runtimeAnimatorController = destructibleBlockInfo.animators;
    }
    public override void OnHit(int damage, Vector2 knockback)
    {
        if (!damageable.IsAlive)
        {
            DropItem(!damageable.IsAlive);
            Destroy(gameObject);
        }else if (damageable.Health == 20)
        {
            animator.Play("Root_Idle");
            DropItem(!damageable.IsAlive);
        }
        else animator.SetTrigger("Hit");
    }
}
