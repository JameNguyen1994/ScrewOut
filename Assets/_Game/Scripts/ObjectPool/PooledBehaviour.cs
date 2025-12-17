using UnityEngine;

/// <summary>
/// Optional interface so pooled objects can react to spawn/despawn.
/// </summary>
public interface IPoolable
{
    void SetOwner(PoolManager owner, string key);
    void OnSpawned();
    void OnDespawned();
    void Despawn();
}

/// <summary>
/// Base component: provides store of pool key and default IPoolable behavior.
/// Derive your effect scripts from this if you want OnSpawned/OnDespawned callbacks.
/// </summary>
[DisallowMultipleComponent]
public class PooledBehaviour : MonoBehaviour, IPoolable
{
    protected PoolManager owner;
    protected string key;

    public virtual void SetOwner(PoolManager owner, string key)
    {
        this.owner = owner;
        this.key = key;
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }

    public virtual void Despawn()
    {
        owner.Despawn(this, key);
    }
}