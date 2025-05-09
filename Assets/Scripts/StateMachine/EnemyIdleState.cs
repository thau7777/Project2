using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : IState
{
    private readonly EnemyAI _enemyAI;
    public AnimationClip IdleAnimation;
    public EnemyIdleState(EnemyAI enemyAI)
    {
        _enemyAI = enemyAI;
    }
    public void Enter()
    {
        Debug.Log("Enter Idle State");
    }

    public void Exit()
    {
        
    }

    public void StateFixedUpdate()
    {
        
    }

    public void StateUpdate()
    {
        
    }
}
