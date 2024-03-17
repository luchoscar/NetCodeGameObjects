
using System;

public interface ITicker
{
	void AddListener(Action<float> onEvent);
	void RemoveListener(Action<float> onEvent);
	void TickEvent(float deltaTime);
}
