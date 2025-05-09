using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }

    private void Awake()
    {
        StateMachine = new StateMachine();

        IdleState = new EnemyIdleState(this);
    }

    private void Start()
    {
        StateMachine.ChangeState(IdleState);
    }
    private void Update()
    {

    }

    public void StopMoving()
    {

    }
}
