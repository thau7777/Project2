using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : FarmAnimal
{
    [SerializeField] private GameObject cowPrefab;
    protected override void MakeProduct()
    {
        _canMakeProduct = true;
        Debug.Log("Can get milk");
    }
    [ContextMenu("Get milk")]
    protected override void GetProduct()
    {
        if (_canMakeProduct)
        {
            _canMakeProduct = false;
            Debug.Log("Got milk");
            
        }
    }
    protected override void InteractWithAnimal()
    {

    }


    protected override void ApplyStage()
    {
        _animator.SetBool("IsMature", IsMature);

    }
}
