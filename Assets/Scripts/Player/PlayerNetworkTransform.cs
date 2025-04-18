using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkTransform : NetworkBehaviour
{
    [SerializeField] private bool _serverAuth;
    [SerializeField] private float _cheapInterpolationTime = 0.1f;

    private NetworkVariable<PlayerNetworkState> _playerState;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        var permission = _serverAuth ? NetworkVariableWritePermission.Server : NetworkVariableWritePermission.Owner;
        _playerState = new NetworkVariable<PlayerNetworkState>(writePerm: permission);
    }

    //public override void OnNetworkSpawn()
    //{
    //    if (!IsOwner) Destroy(transform.GetComponent<PlayerController>());
    //}

    private void Update()
    {
        if (IsOwner) TransmitState();
        
    }

    private void FixedUpdate()
    {
        if (!IsOwner) ConsumeState();
    }

    #region Transmit State

    private void TransmitState()
    {
        var state = new PlayerNetworkState
        {
            Position = _rb.position,
            Scale = transform.localScale
        };

        if (IsServer || !_serverAuth)
            _playerState.Value = state;
        else
            TransmitStateServerRpc(state);
    }

    [ServerRpc]
    private void TransmitStateServerRpc(PlayerNetworkState state)
    {
        _playerState.Value = state;
    }

    #endregion

    #region Interpolate State

    private Vector3 _posVel;
    private float _rotVelY;

    private void ConsumeState()
    {
        if (Vector2.Distance(transform.position, _playerState.Value.Position) > 0.01f)
        {
            Debug.Log("consuming");
            transform.position = Vector3.SmoothDamp(transform.position, _playerState.Value.Position, ref _posVel, _cheapInterpolationTime);
        }

        transform.localScale = _playerState.Value.Scale;
    }

    #endregion

    private struct PlayerNetworkState : INetworkSerializable
    {
        private float _posX, _posY;
        private short _scaleX;
    
        internal Vector3 Position
        {
            get => new(_posX, _posY, 0);
            set
            {
                _posX = value.x;
                _posY = value.y;
            }
        }

        internal Vector3 Scale
        {
            get => new(_scaleX, 1, 1);
            set => _scaleX = (short)value.x;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _posX);
            serializer.SerializeValue(ref _posY);

            serializer.SerializeValue(ref _scaleX);
        }
    }
}