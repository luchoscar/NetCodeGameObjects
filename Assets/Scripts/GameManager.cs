using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IServiceProvider
{
    private Dictionary<Type, object> _serviceMap = new Dictionary<Type, object>();

    #region IServiceProvide

    public TService GetService<TService>() where TService : class
    {
        return _serviceMap.TryGetValue(typeof(TService), out object service)
            ? (TService)service
            : default(TService);
    }

    public void AddService<TService>(TService service) where TService : class
    {
        _serviceMap.Add(typeof(TService), service);
    }

    private TService AddService<TIService, TService>() where TService : TIService, new() where TIService : class
    {
        TService service = new TService();
        _serviceMap.Add(typeof(TIService), service);

        return service;
    }

    private TService AddService<TService>() where TService : class, new()
    {
        TService service = new TService();
        _serviceMap.Add(typeof(TService), service);

        return service;
    }

    #endregion

    private TickService _tick;

    #region Monobehavior

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        AddService<ISimulation, SimulationService>();

        _tick = AddService<ITicker, TickService>();
        InputService inputService = AddService<IInput, InputService>();
        inputService.Initialize(this);
    }

    void Update()
    {
        _tick?.TickEvent(Time.deltaTime);
    }

	#endregion
}
