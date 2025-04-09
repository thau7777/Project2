using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetZone : MonoBehaviour
{
    private ItemWorldControl itemWorld;

    [SerializeField]
    private float _ogSpeed;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _acceleration;
    private void Awake()
    {
        itemWorld = GetComponentInParent<ItemWorldControl>();
        _speed = _ogSpeed;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && itemWorld.CanPickup)
        {
            Vector2 playerPos = collision.transform.position;

            itemWorld.transform.position = Vector2.MoveTowards(itemWorld.transform.position, playerPos, _speed);

            _speed = Mathf.Min(_speed + _acceleration * Time.deltaTime, 2f);
          
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")){
            _speed = _ogSpeed;
        }
    }
}
