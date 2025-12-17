using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class IconHoldWarningController : MonoBehaviour
{
    [SerializeField] private List<IconHoldWarning> iconHoldWarnings;
    public async UniTask EnableEffect(bool active)
    {
        foreach (var iconHoldWarning in iconHoldWarnings)
        {
            if (active)
            {
                iconHoldWarning.StartEffect();
               // await UniTask.Delay(50);
            }
            else
                iconHoldWarning.StopEffect();
        }
    }
}
