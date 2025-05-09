using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : ScriptableObject
{
    protected Rigidbody2D rb;
    protected Animator animator;
    protected AnimationClip animClip;

    public virtual void Initialize(Rigidbody2D rb, Animator animator, AnimationClip animClip)
    {
        this.rb = rb;
        this.animator = animator;
        this.animClip = animClip;
    }
    public virtual void Enter()
    {
        // Called when the state is entered
    }

    public virtual void StateUpdate()
    {
        // Called every frame while in the state
    }
    public virtual void StateFixedUpdate()
    {
        // Called every fixed frame-rate frame while in the state
    }
    public virtual void Exit()
    {
        // Called when the state is exited
    }
}
