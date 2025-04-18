using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/ItemDropableEntitySO")]
[System.Serializable]
public class ItemDropableEntitySO : ScriptableObject
{
    [Header("For enity that has Animation")]
    public AnimatorController animators;

    [Header("For enity doesn't have Animation")]
    public Sprite mineBlockIdleSprite;
    public Sprite mineBlockHitSprite;

    [Header("For Both")]
    public Item ItemToDrop;
    public int[] numOfItemCouldDrop;
    public float[] ratioForEachNum;


}
