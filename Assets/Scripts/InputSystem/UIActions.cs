using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActions : Controls.IUIActions
{
    public Action closeInventoryEvent;
    public void OnCloseInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) closeInventoryEvent?.Invoke();
    }
}
