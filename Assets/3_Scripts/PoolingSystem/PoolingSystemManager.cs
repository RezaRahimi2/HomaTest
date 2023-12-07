using System;
using System.Collections.Generic;
using Events;
using UnityEngine;

// Singleton that manages object pools
public class PoolingSystemManager : MonoBehaviour
{
    static PoolingSystemManager instance;
    public static PoolingSystemManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("PoolingSystemManager").AddComponent<PoolingSystemManager>();
                if(Application.isPlaying)
                    DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    
    // Dictionary to store object pools
    private Dictionary<Type, object> _pools = new Dictionary<Type, object>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Get or create a pool for a given prototype
    public ObjectPool<T> GetOrCreatePool<T>(T prototype, int initialSize = 100) where T : MonoBehaviour
    {
        Type poolType = typeof(T);

        if (_pools.TryGetValue(poolType, out var pool))
        {
            return pool as ObjectPool<T>;
        }
        else
        {
            ObjectPool<T> newPool = new ObjectPool<T>();
            newPool.EnsureQuantity(prototype, initialSize);
            _pools.Add(poolType, newPool);
            return newPool;
        }
    }

    // Get a pooled object
    public ObjectPool<T> GetPool<T>() where T : MonoBehaviour
    {
        Type poolType = typeof(T);

        if (_pools.TryGetValue(poolType, out var pool))
        {
            return pool as ObjectPool<T>;
        }

        Debug.LogError($"Pool for type {poolType} does not exist.");
        return null;
    }

    public T GetPooledObject<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent = null) where T : MonoBehaviour
    {
        ObjectPool<T> pool = GetOrCreatePool(prototype); // Adjust the initialSize as needed
        return pool.GetPooled(prototype, position, rotation, parent);
    }
}
