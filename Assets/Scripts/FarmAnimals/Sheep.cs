using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : FarmAnimal
{
    [SerializeField] private GameObject sheepPrefab;
    protected override void MakeProduct()
    {
        _canMakeProduct = true;
        ApplyStage();

    }
    [ContextMenu("shave hair")]
    protected override void GetProduct()
    {
        if (_canMakeProduct)
        {
            _canMakeProduct = false;
            Debug.Log("Got hair");
            ApplyStage();
        }
    }
    protected override void InteractWithAnimal()
    {

    }

    protected override void ApplyStage()
    {
        if(_currentStage == "Mature")
        {
            if(_canMakeProduct)
                _animator.SetTrigger("Haired");
            else
                _animator.SetTrigger("Shaved");

        }


    }
}
