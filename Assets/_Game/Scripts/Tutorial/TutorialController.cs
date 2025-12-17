using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Utils;
using UnityEngine;

public class TutorialController : Singleton<TutorialController>
{
    [SerializeField] private List<IStep> lstCommand;
    [SerializeField] private List<IStep> lstCommand2;
    [SerializeField] private int indexProcess = 0;


    public async UniTask DoTutorial()
    {
        SliderZoom.Instance.OnChangePos(true);
        
      await Process(lstCommand);
        SliderZoom.Instance.OnChangePos(false);
    }

    public async UniTask DoTutorialSwitchColor()
    {
        SliderZoom.Instance.OnChangePos(true);

        await Process(lstCommand2);
        SliderZoom.Instance.OnChangePos(false);
    }

    async UniTask Process(List<IStep> lstStep)
    {
        while (indexProcess < lstStep.Count)
        {
            var step = lstStep[indexProcess];
            if (step.IsWaitingComplete)
            {
                await step.Execute();
            }
            else
            {
                step.Execute().Forget();
            }

            indexProcess++;
        }
    }
}
