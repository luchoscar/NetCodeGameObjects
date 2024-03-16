using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientAutoritativeSync : NetworkBehaviour
{
    public NetworkVariable<Vector3[]> Position = new NetworkVariable<Vector3[]>();

    public enum Option
    {
        Position,
        Rotation,
        LocalScale
    }
    [SerializeField]
    private Option[] _syncOptions;

    public override void OnNetworkSpawn()
    {
        transform.name += IsHost ? "_Host" : IsServer ? "_Server" : "_Client";
        if (IsOwner)
        {
            Camera.main.GetComponent<FollowTarget>().SetTarget(transform);
        }
    }

    /*
    [Rpc(SendTo.ClientsAndHost)]
    private void SubmitPositionRequestClientRpc(Vector3[] syncOptions, RpcParams rpcParams = default)
    {
        if (!IsOwner)
		{
            return;
		}

        if (syncOptions == null)
        {
            Debug.LogError($"Missing {nameof(syncOptions)} in parameters");
            return;
        }

        if (_syncOptions == null)
        {
            Debug.LogError($"Missing {nameof(_syncOptions)} in prefab definition");
            return;
        }

        foreach (Option syncOption in _syncOptions)
        {
            if (syncOptions.TryGetValue(syncOption, out Vector3 vectorData))
            {
                switch (syncOption)
                {
                    case Option.Position:
                        transform.position = vectorData;
                        break;

                    case Option.Rotation:
                        transform.rotation = Quaternion.Euler(vectorData.x, vectorData.y, vectorData.z);
                        break;

                    case Option.LocalScale:
                        transform.localScale = vectorData;
                        break;
                }
            }
        }

    }
    
    [Rpc(SendTo.Server)]
    private void SubmitPositionRequestServerRpc(Dictionary<Option, Vector3> syncOptions, RpcParams rpcParams = default)
    {
        if (syncOptions == null)
        {
            Debug.LogError($"Missing {nameof(syncOptions)} in parameters");
            return;
        }

        if (_syncOptions == null)
        {
            Debug.LogError($"Missing {nameof(_syncOptions)} in prefab definition");
            return;
        }

        foreach (Option syncOption in _syncOptions)
        {
            if (syncOptions.TryGetValue(syncOption, out Vector3 vectorData))
            {
                switch (syncOption)
                {
                    case Option.Position:
                        transform.position = vectorData;
                        break;

                    case Option.Rotation:
                        transform.rotation = Quaternion.Euler(vectorData.x, vectorData.y, vectorData.z);
                        break;

                    case Option.LocalScale:
                        transform.localScale = vectorData;
                        break;
                }
            }
        }
    }
    */
}