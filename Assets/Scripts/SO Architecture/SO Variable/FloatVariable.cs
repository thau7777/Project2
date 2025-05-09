using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatVariable : ScriptableObject
{
    public float Value;
    public event Action OnValueChanged;
}
