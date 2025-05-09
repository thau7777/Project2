using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObject/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [SerializeField] public string id { get; private set; }
    private void OnValidate()
    {
#if UNITY_EDITOR
        id = this.name;
#endif
    }

    public string displayName { get; private set; }
    public int playerLevelRequirement { get; private set; }
    QuestInfoSO[] questPrerequisites;
    GameObject[] questSteps;

    public int goldReward;
    public int experienceReward;
}
