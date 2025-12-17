using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PreBoosterRocket3D_PathList : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ModelRocket rocketPrefab;
    [SerializeField] private Transform rocketsHolder;

    [Header("Path Settings")]
    [SerializeField] private float flyDurationPerSegment = 0.6f; // thời gian cho mỗi đoạn
    [SerializeField] private Ease pathEase = Ease.OutSine;
    [SerializeField] private float lookAtSmooth = 0.15f;
    [SerializeField] private bool orientToPath = true; // tự xoay theo hướng bay

    [Header("Visuals")]
    [SerializeField] private Vector3 spawnScaleFrom = Vector3.zero;
    [SerializeField] private Vector3 spawnScaleTo = Vector3.one;
    [SerializeField] private float spawnScaleDuration = 0.2f;

    private void OnDestroy() => DOTween.Kill(this);

    /// <summary>
    /// Bay qua tất cả các điểm trong pathList (3D)
    /// </summary>
    /// 
    public async UniTask Show(Transform start, List<Transform> pathList, Transform target)
    {
        if (rocketPrefab == null || start == null || target == null)
        {
            Debug.LogError("❌ Missing reference!");
            return;
        }

        // Spawn rocket tại start
        var inst = Instantiate(rocketPrefab, rocketsHolder ? rocketsHolder : transform);
        inst.transform.position = start.position;
        inst.transform.localScale = spawnScaleFrom;
        inst.transform.DOScale(spawnScaleTo, spawnScaleDuration).SetEase(Ease.OutBack);
        inst.PlayParTrail();
        // Tạo path gồm start → các điểm trung gian → target
        var pts = new List<Vector3> { start.position };
        foreach (var p in pathList)
        {
            if (p != null)
                pts.Add(p.position);
        }
        pts.Add(target.position);

        float totalDuration = (pts.Count - 1) * flyDurationPerSegment;

        // Bay 3D — PathType.CatmullRom + PathMode.Full3D
        var tween = inst.transform.DOPath(
                pts.ToArray(),
                totalDuration,
                PathType.CatmullRom,
                orientToPath ? PathMode.Full3D : PathMode.Ignore)
            .SetEase(pathEase)
            .SetId(this);

        if (orientToPath)
            tween.SetLookAt(lookAtSmooth);

        await tween.AsyncWaitForCompletion();

        // Khi đến target
       // inst.transform.SetParent(target, true);
        inst.PlayParDestroy();
        //inst.transform.localPosition = Vector3.zero;
        //inst.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 1f);
    }
}
