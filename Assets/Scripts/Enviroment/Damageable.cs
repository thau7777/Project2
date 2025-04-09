using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent<int> changeState;
    private Animator animator;
    [SerializeField]
    private int _maxHealth = 100;
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
    

    

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;

            if (_health <= 0)
            {
                IsAlive = false;
            }
            else if (_health == 20)
            {
                changeState?.Invoke(20);
            }

        }
    }

    

    //[SerializeField]
    //private bool isInvincible = false;

    //private float timeSinceHit = 0;
    //public float invincibilityTime = 0.25f;

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool("IsAlive", value);
            Debug.Log("IsAlive set " + value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //if (isInvincible)
        //{
        //    if (timeSinceHit >= invincibilityTime)
        //    {
        //        isInvincible = false;
        //        timeSinceHit = 0;
        //    }

        //    timeSinceHit += Time.deltaTime;
        //}

        //if (!IsAlive) Destroy(gameObject);
    }

    public bool Hit(int damage, Vector2 knockback)
    {
        //if (IsAlive && !isInvincible)
        //{
        //    Health -= damage;
        //    isInvincible = true;

        //    damageableHit?.Invoke(damage, knockback);

        //    return true;
        //}
        if (IsAlive)
        {
            Health -= damage;

            damageableHit?.Invoke(damage, knockback);

            return true;
        }

        return false;
    }
}
