using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State _currentState;
    [SerializeField] private State _idleState;

    private void Start()
    {
        _idleState.Initialize(GetComponent<Rigidbody2D>(), GetComponent<Animator>(), null);
        if(_currentState == null)
        {
            _currentState = _idleState;
        }
        _currentState.Enter();
    }
    private void Update()
    {
        if (_currentState != null)
        {
            _currentState.StateUpdate();
        }
    }
}
