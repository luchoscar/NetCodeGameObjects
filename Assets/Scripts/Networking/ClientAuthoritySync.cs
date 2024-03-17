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

        public enum ProcessType
		{
            Server,
            Host,
            Client
		}

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
        
		#region Events

		public void SetSyncTransformCallback(Action<Vector3, Quaternion, Vector3> callback)
        {
            _onSyncTransform = callback;
        }
        /*
        public void RemoveTransformListener(Action<Vector3, Quaternion, Vector3> eventListener)
        {
            _onSyncTransformEvent -= eventListener;
        }
        */
        #endregion

        #region Setup

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                transform.name += "_Owner";
                IPlayableCharacter playable = transform.GetComponent<IPlayableCharacter>();
                if (playable != null)
				{
                    playable.SetupAsLocalPlayer();
                }
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
            _onSyncTransform?.Invoke(
                syncPosition,
                syncRotation,
                syncLocalScale
            );
        }

        public void SyncServerWithClient(
            Vector3 positionSync,
            Quaternion rotationSync,
            Vector3 localScaleSync,
            float deltaTime
        )
		{
            ServerSyncTransformRpc(
                positionSync,
                rotationSync,
                localScaleSync,
                deltaTime
            );
        }

        [Rpc(SendTo.Server)]
        private void ServerSyncTransformRpc(
            Vector3 positionSync,
            Quaternion rotationSync,
            Vector3 localScaleSync,
            float deltaTime,
            RpcParams rpcParams = default
        )
        {
            _onSyncTransform?.Invoke(
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