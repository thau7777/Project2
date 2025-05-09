using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyIdleState", menuName = "StateMachine/EnemyIdleState")]
public class EnemyIdleState : State
{

    public override void Enter()
    {
        animator.Play(animClip.name);
    }

    public override void StateUpdate()
    {

        Debug.Log("Calling Idle State Update");
        // Check for player detection or other conditions to transition to another state
        // For example:
        // if (PlayerDetected())
        // {
        //     stateMachine.ChangeState(stateMachine.attackState);
        // }
    }
}
