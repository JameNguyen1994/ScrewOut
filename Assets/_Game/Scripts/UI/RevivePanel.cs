using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevivePanel : MonoBehaviour
{
    [SerializeField] private Image imgFade;
    [SerializeField] private Image imgLose;

    public async UniTask ShowLose()
    {
      //  AudioController.Instance.PlaySound(SoundName.Lose);
        gameObject.SetActive(true);
        await imgFade.DOFade(1f,0.5f).From(0);
        imgLose.gameObject.SetActive(true);

        await UniTask.Delay(2000);
        imgLose.gameObject.SetActive(false);

        imgFade.DOFade(0, 1f).OnComplete(()=> {
            gameObject.SetActive(false);

        });


    }
}
