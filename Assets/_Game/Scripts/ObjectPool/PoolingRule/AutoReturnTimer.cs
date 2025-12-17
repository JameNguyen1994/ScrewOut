using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically returns particle system GameObject to pool by time.
/// Attach to prefab.
/// </summary>
public class AutoReturnTimer : PooledBehaviour
{
    [SerializeField] private float lifeTime = 2f;
    private Coroutine routine;

    public override void OnSpawned()
    {
        base.OnSpawned();

        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ReturnAfterTime());
    }

    private IEnumerator ReturnAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Despawn();
    }
}