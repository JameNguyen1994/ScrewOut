using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

public class PreBoosterRocket : MonoBehaviour
{
    [SerializeField] private float delaySeconds = 0.3f;
    [SerializeField] private TestPreBoosterRocket3D_PathList testPreBoosterRocket3D_PathList;

    [EasyButtons.Button]
    public async UniTask StartAction()
    {
        var lstTask = new List<UniTask>();
        var lstShape = LevelController.Instance.Level.GetListShapeNearestCamera(5);
        for (int i = 0; i < lstShape.Count; i++)
        {
            var shape = lstShape[i];
            lstTask.Add(testPreBoosterRocket3D_PathList.StartRocket(shape.transform, i, () => { OnCompleteShape(shape); }));
            await UniTask.WaitForSeconds(delaySeconds);
        }
        await UniTask.WhenAll(lstTask);
        await UniTask.Delay(1000);
    }
    public void OnCompleteShape(Shape shape)
    {
        shape.RemoveAllScrewDontDestroy();
        LevelController.Instance.CheckAllBox();
    }
}
