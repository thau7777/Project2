using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class TargetZone : NetworkBehaviour
{
    private ItemWorldControl _itemWorldControl;
    private Rigidbody2D _parentRb;
    [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _maxSpeed = 10f;

    private Vector2 _currentVelocity = Vector2.zero;

    public NetworkVariable<PlayerChasingTransform> chasingTransform = new NetworkVariable<PlayerChasingTransform>();
    private void Awake()
    {
        _parentRb = GetComponentInParent<Rigidbody2D>();
        _itemWorldControl = GetComponentInParent<ItemWorldControl>();
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (!_itemWorldControl.CanPickup.Value || !IsServer) return;
        if (collision.CompareTag("Player") && !chasingTransform.Value.IsChasing)
        {
            var newTransform = new PlayerChasingTransform
            {
                Position = collision.transform.position,
                IsChasing = true
            };
            chasingTransform.Value = newTransform;

            
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && chasingTransform.Value.IsChasing)
        {
            var newTransform = new PlayerChasingTransform
            {
                Position = collision.transform.position,
                IsChasing = false
            };
        }
    }

    private void FixedUpdate()
    {
        if (chasingTransform.Value.IsChasing)
        {
            Vector2 targetPos = chasingTransform.Value.Position;
            Vector2 currentPos = _parentRb.position;
            Vector2 direction = (targetPos - currentPos).normalized;

            // Accelerate in the direction
            _currentVelocity += direction * _acceleration * Time.fixedDeltaTime;

            // Clamp the speed
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, _maxSpeed);

            _parentRb.velocity = _currentVelocity;
        }
        else
        {
            _currentVelocity = Vector2.zero;
            _parentRb.velocity = Vector2.zero;
        }
    }
    public struct PlayerChasingTransform : INetworkSerializable
    {
        private float _posX, _posY;
        private bool _isChasing;
        internal Vector3 Position
        {
            get => new(_posX, _posY, 0);
            set
            {
                _posX = value.x;
                _posY = value.y;
            }
        }

        internal bool IsChasing
        {
            get => _isChasing;
            set => _isChasing = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _posX);
            serializer.SerializeValue(ref _posY);
            serializer.SerializeValue(ref _isChasing);
        }
    }
}


