using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class EnvironmentalResourceHandler : MonoBehaviour
{
    public string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private Animator animator;
    private Damageable damageable;

    [SerializeField]
    private int numItem = 2;

    [SerializeField]
    private GameObject item;

    private bool _hasBeenCut = false;
    public bool HasBeenCut
    {
        get { return _hasBeenCut; }
        private set {  _hasBeenCut = value; }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable()
    {
        EnviromentalStatusManager.ChangeSeasonEvent += ChangeBySeason;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.ChangeSeasonEvent -= ChangeBySeason;
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        animator.SetTrigger(AnimationStrings.hitTrigger);

        if (damageable.Health == 20 || damageable.Health == 0)
        {
            HasBeenCut = true;
            animator.Play(AnimationStrings.rootIdle);
            SpawnItem();
        }
    }

    public void ChangeBySeason(ESeason season)
    {
        if (!HasBeenCut)
        {
            switch (season)
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
    }

    public void SpawnItem()
    {
        if (numItem > 0)
        {
            for (int i = 0; i < numItem; i++)
            {
                Vector3 randomDir = UtilsClass.GetRandomDir();
                Vector3 position = this.transform.position + randomDir * 0.2f;
                GameObject transform = Instantiate(item, position, Quaternion.identity);

                transform.gameObject.GetComponent<Rigidbody2D>().AddForce(randomDir * 5f, ForceMode2D.Impulse);
                ItemWorld itemWorld = transform.GetComponent<ItemWorldControl>().GetItemWorld();
                itemWorld.SetId();
                ItemWorldManager.Instance.AddItemWorld(itemWorld);
            }
        }
    }
}

[Serializable]
public class EnvironmentalResource
{
    [SerializeField] private string id;
    [SerializeField] private string name;
    [SerializeField] private int health;
    

    public EnvironmentalResource() 
    {
        id = System.Guid.NewGuid().ToString();
    }
}