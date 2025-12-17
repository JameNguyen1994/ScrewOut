using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIPreBoosterGameplay : Singleton<UIPreBoosterGameplay>
{
    [SerializeField] private Image imgGlass;
    [SerializeField] private Image imgRocket;

    public async UniTask ShowGlass()
    {
        imgGlass.transform.localScale = Vector3.zero;
        imgGlass.gameObject.SetActive(true);
        await imgGlass.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();

    }
    public void HideGlass()
    {
        imgGlass.gameObject.SetActive(false);
    }
    public async UniTask ShowRocket()
    {
        imgRocket.transform.localScale = Vector3.zero;
        imgRocket.gameObject.SetActive(true);
        await imgRocket.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();
    }
    public void HideRocket()
    {
        imgRocket.gameObject.SetActive(false);
    }
}
