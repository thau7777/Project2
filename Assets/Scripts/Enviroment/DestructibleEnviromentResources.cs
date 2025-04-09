using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/MineOrTree")]
[System.Serializable]
public class DestructibleEnviromentResources : ScriptableObject
{
    [Header("For Tree")]
    public AnimatorController animators;

    [Header("For Mine")]
    public Sprite mineBlockIdleSprite;
    public Sprite mineBlockHitSprite;
    public ObjectType KindOfMine;

    [Header("For Both")]
    public Item ItemToDrop;
    public int[] numOfItemCouldDrop;
    public float[] ratioForEachNum;

    public enum ObjectType 
    {
        Quartz,
        SmallMine,
        BigMine
    }

}
