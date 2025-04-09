using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedScript : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private bool _isVertical = false;
    public bool IsVertical
    {
        get { return _isVertical; }
        private set
        {
            _isVertical = value;
        }
    }

    [SerializeField]
    private bool _isBeingUsed = false;
    public bool IsBeingUsed
    {
        get { return _isBeingUsed; }
        private set
        {
            _isBeingUsed = value;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSleep(bool toUse)
    {
        IsBeingUsed = toUse;
        if (toUse)
        {
            playerController.transform.SetParent(transform);
            if (!IsVertical)
            {
                playerController.transform.position = transform.position + new Vector3(0.5f, 0.5f, 0);
                playerController.transform.rotation = Quaternion.Euler(0, 0, 90);
                playerController.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            else
            {
                playerController.transform.position = transform.position;
            }
            playerController.movement = Vector2.zero;
            playerController.GetComponent<Collider2D>().isTrigger = true;
        }
        else
        {
            playerController.transform.SetParent(null);
            playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
            playerController.GetComponent<SpriteRenderer>().sortingOrder = 0;
            playerController.GetComponent<Collider2D>().isTrigger = false;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Player"))
        {
            if (IsBeingUsed) return;
            playerController = collision.collider.GetComponent<PlayerController>();
            playerController.SetCurrentBed(this);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerController.ClearBed();
            playerController = null;
        }
    }
}
