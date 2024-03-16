
using System;
using Unity.Netcode;
using UnityEngine;

namespace Networking
{
    public interface ICharacterSync
	{

	}

	public class CharacterMovementSync : NetworkBehaviour
    {
        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        [SerializeField]
        private float _speed = 10f;
        private Vector3 _direction = Vector3.zero;

        [Flags]
        private enum KeyInput
		{
            None = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8
		}
        private KeyInput _collectedInputs = KeyInput.None;
        private readonly KeyInput[] _inputs = new KeyInput[]
        {
            KeyInput.Up,
            KeyInput.Down,
            KeyInput.Left,
            KeyInput.Right
        };


		#region Network

		[Rpc(SendTo.Server)]
        private void SubmitPositionRequestServerRpc(Vector3 newPoition, RpcParams rpcParams = default)
        {
            Debug.Log($"LS Test => SendTo.Server {transform.position} => {newPoition}");
            //transform.position += _direction * _speed * Time.deltaTime;
            transform.position = newPoition;
        }

        public override void OnNetworkSpawn()
		{
            transform.name += IsHost ? "_Host" : IsServer ? "_Server" : "_Client";
            if (IsOwner)
			{
                //GetComponent<NetworkObject>().s(NetworkManager.LocalClient.ClientId);
                Camera.main.GetComponent<FollowTarget>().SetTarget(transform);
            }

            //Position.Value = transform.position;
        }

		#endregion

		private void Update()
        {
            if (IsOwner)
            {
                float theta = Time.frameCount / 10.0f;
                transform.position = new Vector3((float)Math.Cos(theta), 0.0f, (float)Math.Sin(theta));
                SubmitPositionRequestServerRpc(transform.position);
                if (TrySetInputDirection())
                {

                    SubmitPositionRequestServerRpc(transform.position);
                }
            }

            //transform.position = Position.Value;
        }

        private bool TrySetInputDirection()
		{
            if (Input.GetKeyDown(KeyCode.W))
            {
                _collectedInputs |= KeyInput.Up;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                _collectedInputs &= (~KeyInput.Up);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _collectedInputs |= KeyInput.Down;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                _collectedInputs &= (~KeyInput.Down);
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                _collectedInputs |= KeyInput.Left;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                _collectedInputs &= (~KeyInput.Left);
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                _collectedInputs |= KeyInput.Right;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                _collectedInputs &= (~KeyInput.Right);
            }

            if (_collectedInputs == KeyInput.None)
            {
                return false;
            }

            _direction = Vector3.zero;

            foreach (KeyInput input in _inputs)
            {
                KeyInput keyInput = input & _collectedInputs;
                switch (keyInput)
                {
                    case KeyInput.Up:
                        _direction += Vector3.up;
                        break;

                    case KeyInput.Down:
                        _direction -= Vector3.up;
                        break;

                    case KeyInput.Left:
                        _direction -= Vector3.right;
                        break;

                    case KeyInput.Right:
                        _direction += Vector3.right;
                        break;
                }
            }

            Debug.Log($"from input: {_collectedInputs} => {_direction}");

            return true;
        }
    }
}