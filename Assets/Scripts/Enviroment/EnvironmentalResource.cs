using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Animator))]
public abstract class EnvironmentalResource : MonoBehaviour
{
    protected Animator animator;
    protected Damageable damageable;
    [SerializeField] protected DestructibleEnviromentResources destructibleBlockInfo;
    [SerializeField] protected GameObject itemToDrop;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    protected void Start()
    {
        
    }
    protected void OnEnable()
    {
        if (this is StoneAndMineral) return;
        EnviromentalStatusManager.ChangeSeasonEvent += ChangeBySeason;
    }

    protected void OnDisable()
    {
        if (this is StoneAndMineral) return;
        EnviromentalStatusManager.ChangeSeasonEvent -= ChangeBySeason;
    }

    public virtual void OnHit(int damage, Vector2 knockback) { }


    protected void ChangeBySeason(ESeason season)
    {
        switch(season)
        {
            case ESeason.Spring:
                {
                    animator.Play(AnimationStrings.springIdle);
                    break;
                }
            case ESeason.Summer:
                {
                    animator.Play(AnimationStrings.summerIdle);
                    break;
                }
            case ESeason.Autumn:
                {
                    animator.Play(AnimationStrings.autumnIdle);
                    break;
                }
            case ESeason.Winter:
                {
                    animator.Play(AnimationStrings.winterIdle);
                    break;
                }
        }
    }

    public void DropItem(bool makeLessDrop)
    {
        itemToDrop.GetComponent<ItemWorldControl>().item = destructibleBlockInfo.ItemToDrop;
        Debug.Log(itemToDrop.GetComponent<ItemWorldControl>().item.itemName);
        int numItem = 0;
        numItem = UtilsClass.GetRandomValue(destructibleBlockInfo.numOfItemCouldDrop, destructibleBlockInfo.ratioForEachNum);
        if(makeLessDrop) numItem /= 2;
        if (numItem > 0)
        {
            for (int i = 0; i < numItem; i++)
            {
                Vector3 randomDir = UtilsClass.GetRandomDir();
                Vector3 position = this.transform.position + randomDir * 0.2f;
                GameObject transform = Instantiate(itemToDrop, position, Quaternion.identity);

                transform.gameObject.GetComponent<Rigidbody2D>().AddForce(randomDir * 5f, ForceMode2D.Impulse);
                transform.GetComponent<ItemWorldControl>().StartWaitForPickedup();
                ItemWorld itemWorld = transform.GetComponent<ItemWorldControl>().GetItemWorld();
                itemWorld.SetId();
                ItemWorldManager.Instance.AddItemWorld(itemWorld);

            }
        }      
    }
}
