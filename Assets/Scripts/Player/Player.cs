using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public enum EState
    {
        Ilde,
        Sleep,
        RidingVehicle,
        Fishing,
        HoldingItem
    }

    [SerializeField] private EState _curentState;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;
    [SerializeField] private float _money;
    [SerializeField] private Vector3 _position;

    public float MaxHealth
    { get { return _maxHealth; } }

    public float CurrentHealth 
    { get { return _currentHealth; } }

    public float MaxMana
    { get { return _maxMana; } }

    public float CurrentMana
    { get { return _currentMana; } }

    public float MaxStamina
    { get { return _maxStamina; } }

    public float CurrentStamina
    { get { return _currentStamina; } }

    public float Money
    { get { return _money; } }

    public Vector3 Position
    { get { return _position; } }

    public Player() 
    {
        this._curentState = EState.Ilde;
        this._maxHealth = 100;
        this._currentHealth = 100;
        this._maxMana = 100;
        this._currentMana = 100;
        this._maxStamina = 100;
        this._currentStamina = 100;
        this._money = 0;
        this._position = Vector3.zero;
    }

    // State
    public EState GetState()
    {
        return _curentState;
    }

    public void SetState(EState state)
    {
        _curentState = state;
    }

    // Health
    public void SetMaxHealth(float health)
    {
        this._maxHealth = health;
    }

    public void SetCurrentHealth(float health)
    {
        this._currentHealth = health;
    }

    // Money
    public void SetMoney(float money)
    {
        this._money = money;
    }

    // Position
    public void SetPosition(Vector3 position)
    {
        this._position = position;
    }
}
