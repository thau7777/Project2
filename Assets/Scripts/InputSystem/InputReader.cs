using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    public Action<Vector2> moveEvent;
    public Action attackEvent;
    public Action interactEvent;
    public Action<InputAction.CallbackContext> runEvent;

    private Controls input;
    private void OnEnable()
    {
        if(input == null)
        {
            input = new Controls();
            input.Player.SetCallbacks(this);
        }
        input.Player.Enable();
    }

    private void OnDisable()
    {
        if (input != null) input.Player.Disable();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) attackEvent?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) interactEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        runEvent?.Invoke(context);

    }
}
