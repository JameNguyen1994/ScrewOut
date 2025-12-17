using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central Pool Manager (Singleton).
/// Handles Spawn and Despawn of pooled objects.
/// Spawn/Despawn by key.
/// Provides a generic "super" Spawn() API that looks like Instantiate(),
/// but reuses objects when possible.
/// PoolManager singleton: register pools via PoolConfig ScriptableObjects (by drag in inspector or create file config at Resources/Config/Pool),
/// </summary>

public class PoolManager : Singleton<PoolManager>
{
    [Tooltip("Register PoolConfig assets here")]
    [SerializeField]
    private PoolConfig poolConfig;

    private Dictionary<string, Queue<PooledBehaviour>> pool = new Dictionary<string, Queue<PooledBehaviour>>();
    private Dictionary<string, PooledBehaviour> prefabRegistry = new Dictionary<string, PooledBehaviour>();

    protected override void CustomAwake()
    {
        base.CustomAwake();

        if (poolConfig == null)
        {
            poolConfig = Resources.Load<PoolConfig>(Define.CONFIG_POOL);
        }

        foreach (var cfg in poolConfig.Configs)
        {
            if (cfg.prefab == null) continue;

            string key = string.IsNullOrEmpty(cfg.key) ? cfg.prefab.name : cfg.key;

            EnsurePrefabRegistered(key, cfg.prefab);
            EnsureQueueForKey(key);

            if (cfg.size > 0)
            {
                //Preloading objects into pool
                for (int i = 0; i < cfg.size; i++)
                {
                    PreloadingObject(cfg.prefab, key);
                }
            }
        }
    }

    private void EnsurePrefabRegistered(string key, PooledBehaviour prefab)
    {
        // Register prefab if not already
        if (!prefabRegistry.ContainsKey(key))
        {
            prefab.gameObject.SetActive(false);
            prefabRegistry[key] = prefab;
        }
    }

    private void EnsureQueueForKey(string key)
    {
        // Ensure queue exists
        if (!pool.ContainsKey(key))
        {
            pool[key] = new Queue<PooledBehaviour>();
        }
    }

    /// <summary>
    /// Spawn object from pool by key
    /// </summary>
    public PooledBehaviour Spawn(string key, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!prefabRegistry.ContainsKey(key))
        {
            Debug.LogError($"[Pool] No prefab registered with key: {key}");
            return null;
        }

        EnsureQueueForKey(key);

        PooledBehaviour obj;

        // Reuse or create new
        if (pool[key].Count > 0)
        {
            obj = pool[key].Dequeue();
        }
        else
        {
            obj = Instantiate(prefabRegistry[key]);
            obj.SetOwner(this, key);
        }

        // Setup object
        obj.transform.SetParent(parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);
        obj.OnSpawned();

        return obj;
    }

    /// <summary>
    /// Spawn an object of type T (must inherit PooledBehaviour).
    /// If pool has inactive objects -> reuse.
    /// If pool is empty -> Instantiate new.
    /// </summary>
    public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : PooledBehaviour
    {
        string key = prefab.name;

        EnsurePrefabRegistered(key, prefab);

        PooledBehaviour obj = Spawn(key, position, rotation, parent);

        return (T)obj;
    }

    /// <summary>
    /// Return object back to its pool and deactivate it.
    /// </summary>
    public void Despawn(PooledBehaviour obj, string key)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool[key].Enqueue(obj);
        obj.OnDespawned();
    }

    /// <summary>
    /// Preloading object into pool.
    /// </summary>
    public void PreloadingObject(PooledBehaviour prefab, string key)
    {
        PooledBehaviour obj = Instantiate(prefabRegistry[key]);
        obj.SetOwner(this, key);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool[key].Enqueue(obj);
    }
}