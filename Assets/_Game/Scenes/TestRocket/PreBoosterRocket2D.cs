using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PreBoosterRocket2D : MonoBehaviour
{
    [Header("Prefabs / Refs")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform rocketsHolder;

    [Header("Path Settings")]
    [SerializeField] private float orbitRadius = 2f;       // bán kính vòng quanh center
    [SerializeField] private float orbitDegrees = 270f;    // góc cung quanh center
    [SerializeField] private int orbitSamples = 36;        // độ mịn của cung
    [SerializeField] private float curveExtend = 1.5f;     // kéo dài đoạn cong hướng tới target
    [SerializeField] private float duration = 2.5f;
    [SerializeField] private Ease pathEase = Ease.OutSine;

    [Header("Visuals")]
    [SerializeField] private float lookAtSmooth = 0.2f;
    [SerializeField] private float spawnScaleDuration = 0.2f;
    [SerializeField] private Vector3 spawnScaleFrom = Vector3.zero;
    [SerializeField] private Vector3 spawnScaleTo = Vector3.one;

    private void OnDestroy() => DOTween.Kill(this);

    /// <summary>
    /// Bay từ start → vòng quanh center → nối cong tới target, bằng 1 path duy nhất
    /// </summary>
    public async UniTask Show(Transform start, Transform center, Transform target, bool clockwise = true)
    {
        if (rocketPrefab == null || start == null || center == null || target == null)
        {
            Debug.LogError("❌ Missing references!");
            return;
        }

        // Spawn rocket tại vị trí start
        var inst = Instantiate(rocketPrefab, rocketsHolder ? rocketsHolder : transform);
        inst.transform.position = start.position;
        inst.transform.localScale = spawnScaleFrom;
        inst.transform.DOScale(spawnScaleTo, spawnScaleDuration).SetEase(Ease.OutBack);

        // Tạo path duy nhất
        var path = BuildUnifiedOrbitPath(start.position, center.position, target.position, clockwise);

        // Bay theo path
        await inst.transform.DOPath(path, duration, PathType.CatmullRom, PathMode.TopDown2D)
            .SetEase(pathEase)
            .SetLookAt(lookAtSmooth, Vector3.forward)
            .SetId(this)
            .AsyncWaitForCompletion();

        // Khi tới target
        inst.transform.SetParent(target, true);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 1f);
    }

    /// <summary>
    /// Sinh một đường cong duy nhất: start → cung quanh center → mở ra target
    /// </summary>
    private Vector3[] BuildUnifiedOrbitPath(Vector3 start, Vector3 center, Vector3 target, bool clockwise)
    {
        var pts = new List<Vector3>();
        pts.Add(start);

        // hướng từ center tới start
        Vector3 dirStart = (start - center).normalized;
        float startAngle = Mathf.Atan2(dirStart.y, dirStart.x);
        float sign = clockwise ? -1f : 1f;

        // tạo cung tròn quanh center
        int orbitPoints = orbitSamples;
        for (int i = 1; i <= orbitPoints; i++)
        {
            float t = i / (float)orbitPoints;
            float angle = startAngle + sign * (t * orbitDegrees * Mathf.Deg2Rad);
            Vector3 pointOnCircle = center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * orbitRadius;
            pts.Add(pointOnCircle);
        }

        // điểm cuối cung
        Vector3 endOfOrbit = pts[^1];

        // tạo các điểm cong hướng tới target (không thẳng)
        Vector3 dirToTarget = (target - endOfOrbit).normalized;
        Vector3 right = new Vector3(-dirToTarget.y, dirToTarget.x, 0f);

        int curvePoints = 16;
        for (int i = 1; i <= curvePoints; i++)
        {
            float t = i / (float)curvePoints;
            float widen = Mathf.Sin(t * Mathf.PI) * curveExtend; // cong mở
            Vector3 pos = Vector3.Lerp(endOfOrbit, target, t) + right * widen * 0.3f;
            pts.Add(pos);
        }

        pts.Add(target);
        return pts.ToArray();
    }
}
