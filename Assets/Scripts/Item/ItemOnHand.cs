using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class ItemOnHand : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItemSprite(Sprite image)
    {
        transform.GetComponent<SpriteRenderer>().sprite = image;
    }
}
