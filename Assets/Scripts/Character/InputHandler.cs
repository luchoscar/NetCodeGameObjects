using System;
using UnityEngine;


public class InputHandler : MonoBehaviour
{
    private KeyInput _inputCollected = KeyInput.None;
    private readonly KeyInput[] _inputs = new KeyInput[]
    {
            KeyInput.Up,
            KeyInput.Down,
            KeyInput.Left,
            KeyInput.Right
    };

    private event Action<KeyInput> _onInputChange;

	#region Event Listeners

	public void AddInputListener(Action<KeyInput> onInputChange)
	{
        _onInputChange += onInputChange;
    }

    public void RemoveInputListener(Action<KeyInput> onInputChange)
    {
        _onInputChange -= onInputChange;
    }

	#endregion

	#region Monobehavior

	private void Update()
	{
        if (Input.GetKeyDown(KeyCode.W))
        {
            _inputCollected |= KeyInput.Up;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            _inputCollected &= (~KeyInput.Up);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _inputCollected |= KeyInput.Down;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            _inputCollected &= (~KeyInput.Down);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _inputCollected |= KeyInput.Left;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _inputCollected &= (~KeyInput.Left);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _inputCollected |= KeyInput.Right;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _inputCollected &= (~KeyInput.Right);
        }

        _onInputChange?.Invoke(_inputCollected);
    }

	#endregion
}
