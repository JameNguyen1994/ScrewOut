using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PreBoosterRocket2D_PathList : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketsHolder;

    [Header("Path Settings")]
    [SerializeField] private float flyDurationPerSegment = 0.6f; // thời gian cho mỗi đoạn
    [SerializeField] private Ease pathEase = Ease.OutSine;
    [SerializeField] private float lookAtSmooth = 0.2f;

    [Header("Visuals")]
    [SerializeField] private Vector3 spawnScaleFrom = Vector3.zero;
    [SerializeField] private Vector3 spawnScaleTo = Vector3.one;
    [SerializeField] private float spawnScaleDuration = 0.2f;

    private void OnDestroy() => DOTween.Kill(this);

    /// <summary>
    /// Bay theo danh sách path: A -> B -> C -> ... -> target
    /// </summary>
    public async UniTask Show(Transform start, List<Transform> pathList, Transform target)
    {
        if (rocketPrefab == null || start == null || target == null)
        {
            Debug.LogError("❌ Missing reference!");
            return;
        }

        if (pathList == null || pathList.Count == 0)
        {
            Debug.LogWarning("⚠️ Path list is empty, flying straight to target.");
        }

        // Spawn rocket tại start
        var inst = Instantiate(rocketPrefab, rocketsHolder ? rocketsHolder : transform);
        inst.transform.position = start.position;
        inst.transform.localScale = spawnScaleFrom;
        inst.transform.DOScale(spawnScaleTo, spawnScaleDuration).SetEase(Ease.OutBack);

        // ✅ Build path
        var pts = new List<Vector3>();
        pts.Add(start.position); // điểm đầu
        foreach (var p in pathList)
        {
            if (p != null)
                pts.Add(p.position);
        }
        pts.Add(target.position); // điểm cuối

        // Tổng thời gian = số đoạn * thời gian mỗi đoạn
        float totalDuration = (pts.Count - 1) * flyDurationPerSegment;

        // Bay mượt qua tất cả điểm
        await inst.transform.DOPath(pts.ToArray(), totalDuration, PathType.CatmullRom, PathMode.TopDown2D)
            .SetEase(pathEase)
            .SetLookAt(lookAtSmooth, Vector3.forward)
            .SetId(this)
            .AsyncWaitForCompletion();

        // Khi tới target
        inst.transform.SetParent(target, true);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 1f);
    }
}
