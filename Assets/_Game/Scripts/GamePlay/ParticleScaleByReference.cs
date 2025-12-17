using UnityEngine;

public enum ScaleMode
{
    MaxAxis,
    Average,
    Volume,
    YAxisOnly
}

public class ParticleScaleByReference : Singleton<ParticleScaleByReference>
{
    [Header("References")]
    [SerializeField] private Transform referenceModel;

    [Header("Settings")]
    [SerializeField] private ScaleMode scaleMode = ScaleMode.Average;
    [SerializeField] private bool includeChildren = false;

    private void Start()
    {
    }

    [ContextMenu("Spawn Scaled Particle")]
    public void SpawnScaledParticle(Transform targetObject, Transform particleSystem, int scaleForce = 1)
    {
        if (referenceModel == null || targetObject == null )
        {
            Debug.LogWarning("⚠️ Chưa gán referenceModel / targetModel / particlePrefab");
            return;
        }

        Bounds refBounds = GetBounds(referenceModel);
        Bounds targetBounds = GetBounds(targetObject);

        if (refBounds.size == Vector3.zero)
        {
            Debug.LogWarning("⚠️ referenceModel không có Renderer hợp lệ");
            return;
        }

        float scaleRatio = GetScaleRatio(refBounds, targetBounds);
        particleSystem.transform.localScale = Vector3.one * scaleRatio*scaleForce;
    }

    private Bounds GetBounds(Transform obj)
    {
        Renderer[] renderers = includeChildren ? obj.GetComponentsInChildren<Renderer>() : new[] { obj.GetComponent<Renderer>() };
        if (renderers.Length == 0)
            return new Bounds();

        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            b.Encapsulate(renderers[i].bounds);
        return b;
    }

    private float GetScaleRatio(Bounds refB, Bounds targetB)
    {
        switch (scaleMode)
        {
            case ScaleMode.Average:
                return ((targetB.size.x + targetB.size.y + targetB.size.z) /
                        (refB.size.x + refB.size.y + refB.size.z));
            case ScaleMode.Volume:
                float refV = refB.size.x * refB.size.y * refB.size.z;
                float tarV = targetB.size.x * targetB.size.y * targetB.size.z;
                return Mathf.Pow(tarV / refV, 1f / 3f);
            case ScaleMode.YAxisOnly:
                return targetB.size.y / refB.size.y;
            case ScaleMode.MaxAxis:
            default:
                return Mathf.Max(targetB.size.x, targetB.size.y, targetB.size.z) /
                       Mathf.Max(refB.size.x, refB.size.y, refB.size.z);
        }
    }
}
