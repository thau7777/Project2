using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/FarmAnimal")]
public class FarmAnimalSO : ScriptableObject
{
    public int EatTimesNeededToGrow;
    public int EatTimesNeededToMakeProduct;
    public string[] GrowStages;
    public Gender Gender;   
}
public enum Gender
{
    None,
    Male,
    Female
}
