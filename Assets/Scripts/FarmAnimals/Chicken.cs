using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : FarmAnimal
{
    [SerializeField] private GameObject chickenPrefab;
    [SerializeField] private int _timesNeededToBecomeBaby = 15;
    [SerializeField] private int _currentTime;
  
    public void UpdateEggTime(int minute)
    {
        
        _currentTime += minute;
        if(_currentTime >= _timesNeededToBecomeBaby)
        {
            _currentTime = 0;
            _animator.Play("AboutToHatch");
        }
    }
    protected override void MakeProduct()
    {
        var newAnimal = Instantiate(chickenPrefab, transform.position, Quaternion.identity);
        //newAnimal.GetComponent<Chicken>().Initial();
    }

    protected override void ApplyStage()
    {

        isMoving = false;
        if(CurrentStage == "Egg")
        {
            canMove = false;
            GetComponent<CapsuleCollider2D>().isTrigger = true;
        }
        else
        {
            GetComponent<CapsuleCollider2D>().isTrigger = false;
            canMove = true;
            _animator.SetTrigger(CurrentStage);
        }

        
    }

}
