using System;

public class TickService : ITicker
{
	private event Action<float> _onTickEvent;

   public void AddListener(Action<float> onEvent)
	{
		_onTickEvent += onEvent;
	}

	public void RemoveListener(Action<float> onEvent)
	{
		_onTickEvent -= onEvent;
	}

	public void TickEvent(float deltaTime)
	{
		_onTickEvent?.Invoke(deltaTime);
	}
}
