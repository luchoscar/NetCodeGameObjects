using System;
using Unity.Netcode;
using UnityEngine;

namespace Networking.ClientAutority
{
    public class ClientOwnerSync : NetworkBehaviour
    {
        public NetworkVariable<Vector3> PositionSync = new NetworkVariable<Vector3>();
        public NetworkVariable<Quaternion> RotationSync = new NetworkVariable<Quaternion>();
        public NetworkVariable<Vector3> LocalScaleSync = new NetworkVariable<Vector3>();

        public enum OwnerType
		{
            Server,
            Host,
            Client
		}

        public OwnerType Owner
        {
            get
            {
                return IsHost
                    ? OwnerType.Host
                    : IsServer
                        ? OwnerType.Server
                        : OwnerType.Client;
            }
        }

        private event Action<Vector3, Quaternion, Vector3> _onSyncTransformEvent;
        
		#region Events

		public void AddTransformListener(Action<Vector3, Quaternion, Vector3> eventListener)
        {
            _onSyncTransformEvent += eventListener;
        }

        public void RemoveTransformListener(Action<Vector3, Quaternion, Vector3> eventListener)
        {
            _onSyncTransformEvent -= eventListener;
        }

        #endregion

        #region Setup

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                transform.name += "_Owner";
            }
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
            _onSyncTransformEvent?.Invoke(
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
            _onSyncTransformEvent?.Invoke(
                positionSync,
                rotationSync,
                localScaleSync
            );

            PositionSync.Value = positionSync;
            RotationSync.Value = rotationSync;
            LocalScaleSync.Value = localScaleSync;
        }

		#endregion

	}
}