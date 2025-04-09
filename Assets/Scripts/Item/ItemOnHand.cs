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
    public void MoveUp(float value)
    {
        if(value == 0)
        transform.localPosition = new Vector3(0, 0.9f, 0);
        else transform.localPosition = new Vector3(0, value, 0);
    }

    public void MoveDown(float value)
    {
        if(value == 0)
        transform.localPosition = new Vector3(0, 0.85f, 0);
        else transform.localPosition = new Vector3(0, value, 0);
    }
}
