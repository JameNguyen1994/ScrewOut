using Cysharp.Threading.Tasks;
using EasyButtons;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class TestPreBoosterRocket3D_PathList : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PreBoosterRocket3D_PathList rocketController;

    [Header("Path Setup")]
    [SerializeField] private Transform startPoint;         // Điểm bắt đầu (A)
    [SerializeField] private List<Transform> pathPoints1;   // Danh sách các điểm trung gian (B, C, D, ...)
    [SerializeField] private List<Transform> pathPoints2;   // Danh sách các điểm trung gian (B, C, D, ...)
    [SerializeField] private Transform targetPoint;        // Mục tiêu cuối cùng (Z)

    [Header("Loop Settings")]
    [SerializeField] private bool loop = false;
    [SerializeField] private float delayBetween = 2f;      // Thời gian chờ giữa các lần test
    [SerializeField] private int index = 0;

    [Button]
    public async void Test()
    {
        index++;

        var path = index% 2 == 0 ? pathPoints1 : pathPoints2;
        await rocketController.Show(startPoint, path, targetPoint);
    }
     
    public async UniTask StartRocket(Transform target, int index, UnityAction actionOnCompleFly)
    {
        var path = index % 2 == 0 ? pathPoints1 : pathPoints2;
        await rocketController.Show(startPoint, path, target);
        actionOnCompleFly?.Invoke();
    }
    private void OnDrawGizmos()
    {
        if (startPoint == null || targetPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(startPoint.position, 0.1f);
        Gizmos.DrawSphere(targetPoint.position, 0.1f);
        Gizmos.DrawLine(startPoint.position, targetPoint.position);

        if (pathPoints1 != null && pathPoints1.Count > 0)
        {
            Gizmos.color = Color.cyan;
            Vector3 prev = startPoint.position;
            foreach (var p in pathPoints1)
            {
                if (p == null) continue;
                Gizmos.DrawSphere(p.position, 0.08f);
                Gizmos.DrawLine(prev, p.position);
                prev = p.position;
            }
            Gizmos.DrawLine(prev, targetPoint.position);
        }
        if (pathPoints2 != null && pathPoints2.Count > 0)
        {
            Gizmos.color = Color.red;
            Vector3 prev = startPoint.position;
            foreach (var p in pathPoints2)
            {
                if (p == null) continue;
                Gizmos.DrawSphere(p.position, 0.08f);
                Gizmos.DrawLine(prev, p.position);
                prev = p.position;
            }
            Gizmos.DrawLine(prev, targetPoint.position);
        }
    }
}
