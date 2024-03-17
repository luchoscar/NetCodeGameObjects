using Networking;
using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class NetworkGuiHelper : MonoBehaviour
    {
        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();

                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        static void StartButtons()
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }

        static void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        static void SubmitNewPosition()
        {
            if (GUILayout.Button(
                NetworkManager.Singleton.IsServer 
                    ? "Move" 
                    : "Request Position Change"
                )
            )
            {
                NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
                if (NetworkManager.Singleton.IsServer && !NetworkManager.Singleton.IsClient)
                {
                    foreach (ulong uid in NetworkManager.Singleton.ConnectedClientsIds)
                    {
                        NetworkObject networkObject = spawnManager.GetPlayerNetworkObject(uid);
                        CharacterMovement character = networkObject.GetComponent<CharacterMovement>();
                        character.ForceServerResetTransform();
                    }
                }
                else
                {
                    NetworkObject playerObject = spawnManager.GetLocalPlayerObject();
                    CharacterMovement character = playerObject.GetComponent<CharacterMovement>();
                    character.ForceClientResetTransform();
                }
            }
        }
    }
}


