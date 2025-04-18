using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();

    public static void Register<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"Service of type {type.Name} is already registered. Overwriting.");
            services[type] = service; // 기존 인스턴스를 덮어씁니다.
        }
        else
        {
            services.Add(type, service);
            Debug.Log($"Service of type {type.Name} registered.");
        }
    }

    public static void Unregister<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var currentService) && currentService == service)
        {
            services.Remove(type);
            Debug.Log($"Service of type {type.Name} unregistered.");
        }
        else if (services.ContainsKey(type))
        {
             Debug.LogWarning($"Trying to unregister a service of type {type.Name}, but the registered instance is different.");
        }
        else
        {
            Debug.LogWarning($"Trying to unregister a service of type {type.Name}, but it was not registered.");
        }
    }

    // 특정 타입의 서비스만 명시적으로 해제할 때 사용
    public static void UnregisterType<T>() where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
        {
            services.Remove(type);
            Debug.Log($"Service of type {type.Name} unregistered by type.");
        }
    }


    public static T Get<T>() where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
        {
            return service as T;
        }

        Debug.LogError($"Service of type {type.Name} not found.");
        return null; // 또는 예외를 던질 수 있습니다.
    }

    // 모든 서비스 클리어 (예: 게임 종료 시)
    public static void Clear()
    {
        services.Clear();
        Debug.Log("All services cleared.");
    }
}