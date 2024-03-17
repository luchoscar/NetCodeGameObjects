
using System;

public interface IInput
{
    void AddInputListener(Action<KeyInput> onInputChange);
    void RemoveInputListener(Action<KeyInput> onInputChange);
}