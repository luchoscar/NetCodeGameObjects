
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Networking.Test
{
    public class NetworkCommandLine : MonoBehaviour
    {
        [SerializeField]
        private NetworkManager _networkManager;

        private const string MODE_KEY = "-mode";
        private const string CLIENT = "client";
        private const string HOST = "host";
        private const string SERVER = "server";

        private void Start()
        {
            if (Application.isEditor) return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue(MODE_KEY, out string mode))
            {
                Debug.Log($"LS Test => {MODE_KEY}: {mode}");
                switch (mode)
                {
                    case SERVER:
                        _networkManager.StartServer();
                        break;

                    case HOST:
                        _networkManager.StartHost();
                        break;

                    case CLIENT:
                        _networkManager.StartClient();
                        break;
                }
            }
        }

        private Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argDictionary.Add(arg, value);
                }
            }
            return argDictionary;
        }
    }
}