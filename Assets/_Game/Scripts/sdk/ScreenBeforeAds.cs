using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBeforeAds : MonoBehaviour
{
    [SerializeField] private Image imgFade;
    [SerializeField] private RectTransform rtfmContent;
    [SerializeField] private RectTransform rtfmCoin;
    [SerializeField] private Text txtCoin;

    [Button]
    private void Reset()
    {
        imgFade.rectTransform.localScale = Vector3.zero;
        rtfmContent.localScale = Vector3.zero;
        if (rtfmCoin != null)
            rtfmCoin.localScale = Vector3.zero;

        imgFade.gameObject.SetActive(false);
        imgFade.color = new Color(0, 0, 0, 0);
        rtfmContent.gameObject.SetActive(false);
        if (rtfmCoin != null)
            rtfmCoin.gameObject.SetActive(false);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    [Button]
    public void ShowInEditor()
    {
        imgFade.gameObject.SetActive(true);
        rtfmContent.gameObject.SetActive(true);
        if (rtfmCoin != null)
            rtfmCoin.gameObject.SetActive(true);

        imgFade.color = new Color(0, 0, 0, 0.9f);
        imgFade.rectTransform.localScale = Vector3.one;
        rtfmContent.localScale = Vector3.one;
        if (rtfmCoin != null)
            rtfmCoin.localScale = Vector3.one;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }

    [Button]
    public async UniTask Show()
    {
        imgFade.gameObject.SetActive(true);
        imgFade.rectTransform.localScale = Vector3.one;
        imgFade.DOFade(0.9f, 0.3f).ToUniTask();

        rtfmContent.gameObject.SetActive(true);
        await rtfmContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();

        var remote = GameAnalyticController.Instance.Remote();
        var coin = remote.LevelStartAd.inter_rw;
        if (rtfmCoin != null && coin > 0)
        {
            txtCoin.text = $"+{coin}";
            rtfmCoin.gameObject.SetActive(true);
            await rtfmCoin.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            var startPos = rtfmCoin.anchoredPosition;
            await rtfmCoin.DOAnchorPosY(startPos.y + 50, 0.25f)
              .SetEase(Ease.OutQuad)
              .OnComplete(() =>
              {
                  rtfmCoin.DOAnchorPosY(startPos.y + 10, 0.25f).SetEase(Ease.InQuad);
              }).ToUniTask();

            var user = Db.storage.USER_INFO;
            user.coin += coin;
            Db.storage.USER_INFO = user;
            EventDispatcher.Push(EventId.UpdateCoinUI);

        }

        await UniTask.Delay(1500);
    }

    [Button]
    public async UniTask Hide()
    {
        if (rtfmCoin != null && rtfmCoin.gameObject.activeSelf)
        {
            await rtfmCoin.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask();
            rtfmCoin.gameObject.SetActive(false);
        }

        await rtfmContent.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask();
        rtfmContent.gameObject.SetActive(false);

        await imgFade.DOFade(0, 0.3f).SetEase(Ease.Linear).ToUniTask();
        imgFade.gameObject.SetActive(false);

        Reset();
    }
}
