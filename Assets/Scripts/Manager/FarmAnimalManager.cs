using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimalManager : Singleton<FarmAnimalManager>
{
    [SerializeField]
    private List<FarmAnimal> farmAnimals = new List<FarmAnimal>();
    private List<Chicken> eggs = new List<Chicken>();

    private void OnEnable()
    {
        EnviromentalStatusManager.OnTimeIncrease += UpdateChickenEggTime;
    }

    private void OnDisable()
    {
        EnviromentalStatusManager.OnTimeIncrease -= UpdateChickenEggTime;
    }

    private void UpdateChickenEggTime(int minute)
    {
        if(eggs.Count > 0)
        {
            foreach (var egg in eggs)
            {
                egg.UpdateEggTime(minute);
            }
        }
    }


    public void RegisterAnimal(FarmAnimal animal)
    {
        if (!farmAnimals.Contains(animal))
            farmAnimals.Add(animal);
        if(animal.CurrentStage == "Egg")
        {
            RegisterEgg(animal as Chicken);
        }

    }

    public void UnregisterAnimal(FarmAnimal animal)
    {
        if (farmAnimals.Contains(animal))
            farmAnimals.Remove(animal);

        if (animal.CurrentStage == "Egg")
        {
            UnregisterEgg(animal as Chicken);
        }
    }

    public void RegisterEgg(Chicken chicken)
    {
        eggs.Add(chicken);

    }

    public void UnregisterEgg(Chicken chicken)
    {
        if (chicken.CurrentStage == "Egg")
        {
            eggs.Remove(chicken);
        }
    }
}
