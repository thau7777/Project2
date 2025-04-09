using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAndMineral : EnvironmentalResource
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _onHitTime;
    private Coroutine _hitCoroutine;
    protected override void Awake()
    {
        base.Awake();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = destructibleBlockInfo.mineBlockIdleSprite;
    }
    public override void OnHit(int damage, Vector2 knockback)
    {
        if (!damageable.IsAlive)
        {
            DropItem(false);
            Destroy(gameObject);
        }
        else
        {
            ChangeSpriteOnHit();
        }
    }
    private void ChangeSpriteOnHit()
    {
        if (_hitCoroutine != null)
        {
            StopCoroutine(_hitCoroutine);  
        }
        _hitCoroutine = StartCoroutine(ChangeSpriteRoutine());
    }
    private IEnumerator ChangeSpriteRoutine()
    {
        _spriteRenderer.sprite = destructibleBlockInfo.mineBlockHitSprite;
        yield return new WaitForSeconds(_onHitTime);
        _spriteRenderer.sprite = destructibleBlockInfo.mineBlockIdleSprite;
        _hitCoroutine = null;  
    }
}
