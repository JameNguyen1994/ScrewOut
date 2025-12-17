using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically returns particle system GameObject to pool when finished playing.
/// Attach to prefab root that has a ParticleSystem component (or multiple child particle systems).
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class AutoReturnParticle : PooledBehaviour
{
    [SerializeField]
    private ParticleSystem[] _particles;
    private ParticleSystem[] Particles
    {
        get
        {
            if (_particles == null || _particles.Length == 0)
            {
                GetAllParticleSystem();
            }

            return _particles;
        }
    }

    private Coroutine routine;

    public override void OnSpawned()
    {
        base.OnSpawned();

        for (int i = 0; i < Particles.Length; i++)
        {
            if (Particles[i] != null)
            {
                Particles[i].Play(true);
            }
        }

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ReturnWhenFinished());
    }

    private IEnumerator ReturnWhenFinished()
    {
        yield return new WaitUntil(() => !IsAnyAlive());
        yield return null;
        Despawn();
    }

    private bool IsAnyAlive()
    {
        for (int i = 0; i < Particles.Length; i++)
        {
            if (Particles[i] != null && Particles[i].IsAlive(true))
            {
                return true;
            }
        }

        return false;
    }

    public override void OnDespawned()
    {
        base.OnDespawned();

        for (int i = 0; i < Particles.Length; i++)
        {
            if (Particles[i] != null)
            {
                Particles[i].Clear(true);
            }
        }
    }

    [EasyButtons.Button]
    private void GetAllParticleSystem()
    {
        _particles = GetComponentsInChildren<ParticleSystem>(true);
    }
}