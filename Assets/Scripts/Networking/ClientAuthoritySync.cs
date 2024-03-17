using System;
using Unity.Netcode;
using UnityEngine;

namespace Networking.ClientAutority
{
    public class ClientAuthoritySync : NetworkBehaviour
    {
        public NetworkVariable<Vector3> PositionSync = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> RotationSync = new NetworkVariable<Quaternion>();
        public NetworkVariable<Vector3> LocalScaleSync = new NetworkVariable<Vector3>();

        private IServiceProvider _provider = null;
        private ISimulation _simulation = null;

        public ProcessType Process
        {
            get
            {
                return IsHost
                    ? ProcessType.Host
                    : IsServer
                        ? ProcessType.Server
                        : ProcessType.Client;
            }
        }

        private Action<Vector3, Quaternion, Vector3> _onSyncTransform;

        #region Callbacks

        public void SetSyncTransformCallback(Action<Vector3, Quaternion, Vector3> callback)
        {
            _onSyncTransform = callback;
        }

        #endregion

        #region Setup

        public override void OnNetworkDespawn()
		{
            _simulation?.RemoveSimulationObject(OwnerClientId);
        }

        public override void OnNetworkSpawn()
        {
            if (_simulation == null)
			{
                _simulation = _provider.GetService<ISimulation>();

            }

            IPlayableCharacter playable = transform.GetComponent<IPlayableCharacter>();
            if (playable != null) {
                playable.Initialize(_provider);

                transform.name += $"_{OwnerClientId}";
                if (IsOwner)
                {
                    transform.name += "_Owner";
                }
            }

            _simulation?.AddSimulationObject(OwnerClientId, playable as ISimulationObject);
        }

		#endregion

		#region Network Sync

		public Vector3 GetPositionSync() => PositionSync.Value;
        public Quaternion GetRotationSync() => RotationSync.Value;
        public Vector3 GetLocalSync() => LocalScaleSync.Value;

        public void SyncClientWithServer(
            Vector3 syncPosition,
            Quaternion syncRotation,
            Vector3 syncLocalScale
        )
        {
            ClientAndHostSyncTransformRpc(
                syncPosition,
                syncRotation,
                syncLocalScale
            );
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientAndHostSyncTransformRpc(
            Vector3 syncPosition,
            Quaternion syncRotation,
            Vector3 syncLocalScale,
            RpcParams rpcParams = default
        )
        {
            _onSyncTransform?.Invoke(
                syncPosition,
                syncRotation,
                syncLocalScale
            );
        }

        public void SyncServerWithClient(
            Vector3 positionSync,
            Quaternion rotationSync,
            Vector3 localScaleSync
        )
		{
            ServerSyncTransformRpc(
                positionSync,
                rotationSync,
                localScaleSync
            );
        }

        [Rpc(SendTo.Server)]
        private void ServerSyncTransformRpc(
            Vector3 positionSync,
            Quaternion rotationSync,
            Vector3 localScaleSync,
            RpcParams rpcParams = default
        )
        {
            bool validMove = _simulation.ValidateMove(
                OwnerClientId,
                positionSync,
                out Vector3 validatedPosition
            );

            if (!validMove)
            {
                positionSync = validatedPosition;
                ClientAndHostRewindRpc(
                    OwnerClientId,
                    positionSync
                );
            }

            PositionSync.Value = positionSync;
            RotationSync.Value = rotationSync;
            LocalScaleSync.Value = localScaleSync;

            _onSyncTransform?.Invoke(
                positionSync,
                rotationSync,
                localScaleSync
            );
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void ClientAndHostRewindRpc(
            ulong clientId,
            Vector3 syncPosition,
            RpcParams rpcParams = default
        )
        {
           if (OwnerClientId == clientId)
			{
                transform.position = syncPosition;
			}
        }

        #endregion

        #region Monobehavior

        private void Awake()
		{
            _provider = GameObject.FindFirstObjectByType<GameManager>();
            _simulation = _provider.GetService<ISimulation>();

        }

        public override void OnDestroy()
		{
            _provider = null;
            _simulation = null;

            base.OnDestroy();
        }

		#endregion
	}
}