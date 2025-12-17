using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupListOfferWall : MonoBehaviour
{
    [SerializeField] private Transform main;
    [SerializeField] private Image imgFade;
    
    
    public async UniTask Show()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.9f, 0.5f).From(0);
        gameObject.SetActive(true);
        await main.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        DoHide().Forget();
    }

    async UniTask DoHide()
    {
        imgFade.DOFade(0f, 0.5f);
        await main.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
        imgFade.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}