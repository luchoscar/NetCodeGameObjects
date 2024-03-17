
using System;
using UnityEngine;

public class InputService : IInput
{
    private KeyInput _inputCollected = KeyInput.None;

    private event Action<KeyInput> _onInputCollected;

    private ITicker _tick;

    public void Initialize(IServiceProvider provider)
	{
        _tick = provider.GetService<ITicker>();
        _tick.AddListener(OnTickEvent);
	}

    ~InputService()
	{
        _tick?.RemoveListener(OnTickEvent);
    }

    #region IInput

    public void AddInputListener(Action<KeyInput> onInputChange)
    {
        _onInputCollected += onInputChange;
    }

    public void RemoveInputListener(Action<KeyInput> onInputChange)
    {
        _onInputCollected -= onInputChange;
    }

    #endregion

    #region ITicker

    private void OnTickEvent(float deltaTime)
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

        _onInputCollected?.Invoke(_inputCollected);
    }

    #endregion
}
