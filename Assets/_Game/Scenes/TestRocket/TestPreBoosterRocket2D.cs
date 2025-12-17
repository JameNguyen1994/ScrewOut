using UnityEngine;
using Cysharp.Threading.Tasks;
using EasyButtons;
using System.Collections.Generic;

public class TestPreBoosterRocket2D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PreBoosterRocket2D preBooster;
    [SerializeField] private PreBoosterRocket2D_PathList preBoosterPathList;
    [SerializeField] private Transform targetStart;
    [SerializeField] private Transform centerA;
    [SerializeField] private Transform targetB;
    [SerializeField] private List<Transform> pathList;

    [Header("Test Settings")]
    [SerializeField] private float delayBetween = 2f;
    [SerializeField] private bool loop = true;

    [Button]
    public async UniTask Test()
    {
        if (preBooster == null || centerA == null || targetB == null)
        {
            Debug.LogError("⚠️ Missing reference for PreBoosterRocket2D test!");
            return;
        }

        do
        {
            await preBooster.Show(targetStart,centerA, targetB);

            await UniTask.Delay((int)(delayBetween * 1000));
        }
        while (loop);
    }

    [Button]
    public async UniTask Test_PathList()
    {
        if (preBooster == null || targetStart == null || targetB == null)
        {
            Debug.LogError("⚠️ Missing reference for PreBoosterRocket2D test!");
            return;
        }
        do
        {
            await preBoosterPathList.Show(targetStart, pathList, targetB);
            await UniTask.Delay((int)(delayBetween * 1000));
        }
        while (loop);
    }
}
