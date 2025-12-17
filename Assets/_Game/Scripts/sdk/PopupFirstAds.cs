using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using PS.Ad;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupFirstAds : MonoBehaviour
{
    [SerializeField] private Image imgFade;
    [SerializeField] private RectTransform rtfmContent;
    [SerializeField] private RectTransform rtfmWatchADs, rtfmNoAds;
    [SerializeField] private RectTransform rtfmBtnWatchAds, rtfmBtnNoAds;
    [SerializeField] private RectTransform rtfmCharacter;
    [SerializeField] private bool isShowing = false;
    private UnityAction actionAfterShow;

    [Button]
    private void Reset()
    {
        imgFade.rectTransform.localScale = Vector3.zero;
        rtfmContent.localScale = Vector3.zero;
        rtfmWatchADs.localScale = Vector3.zero;
        rtfmNoAds.localScale = Vector3.zero;
        rtfmBtnNoAds.localScale = Vector3.zero;
        rtfmBtnWatchAds.localScale = Vector3.zero;
        rtfmCharacter.localScale = Vector3.zero;

        imgFade.gameObject.SetActive(false);
        imgFade.color = new Color(0, 0, 0, 0);
        rtfmContent.gameObject.SetActive(false);
        rtfmWatchADs.gameObject.SetActive(false);
        rtfmNoAds.gameObject.SetActive(false);
        rtfmBtnWatchAds.gameObject.SetActive(false);
        rtfmBtnNoAds.gameObject.SetActive(false);
        rtfmCharacter.gameObject.SetActive(false);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif 
    }
    [Button]
    public void ShowInEditor()
    {
        imgFade.gameObject.SetActive(true);
        rtfmContent.gameObject.SetActive(true);
        rtfmWatchADs.gameObject.SetActive(true);
        rtfmNoAds.gameObject.SetActive(true);
        rtfmBtnWatchAds.gameObject.SetActive(true);
        rtfmBtnNoAds.gameObject.SetActive(true);
        rtfmCharacter.gameObject.SetActive(true);

        imgFade.color = new Color(0, 0, 0, 0.9f);
        imgFade.rectTransform.localScale = Vector3.one;
        rtfmContent.localScale = Vector3.one;
        rtfmWatchADs.localScale = Vector3.one;
        rtfmNoAds.localScale = Vector3.one;
        rtfmBtnWatchAds.localScale = Vector3.one;
        rtfmBtnNoAds.localScale = Vector3.one;
        rtfmCharacter.localScale = Vector3.one;

#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif 
    }
    [Button]
    public async UniTask Show(UnityAction unityAction)
    {
        isShowing = true;
        actionAfterShow = unityAction;
        imgFade.gameObject.SetActive(true);
        imgFade.rectTransform.localScale = Vector3.one;
        imgFade.DOFade(0.9f, 0.3f).ToUniTask();

        rtfmContent.gameObject.SetActive(true);
        await rtfmContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();

        rtfmCharacter.gameObject.SetActive(true);
        await rtfmCharacter.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();

        rtfmWatchADs.gameObject.SetActive(true);
        rtfmNoAds.gameObject.SetActive(true);
        await UniTask.WhenAll(
            rtfmWatchADs.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask(),
            rtfmNoAds.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask()
        );

        rtfmBtnWatchAds.gameObject.SetActive(true);
        rtfmBtnNoAds.gameObject.SetActive(true);
        await UniTask.WhenAll(
            rtfmBtnWatchAds.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask(),
            rtfmBtnNoAds.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask()
        );
    }

    [Button]
    private async UniTask Hide()
    {
        // Ẩn nút trước
        await UniTask.WhenAll(
            rtfmBtnWatchAds.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).ToUniTask(),
            rtfmBtnNoAds.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).ToUniTask()
        );

        rtfmBtnWatchAds.gameObject.SetActive(false);
        rtfmBtnNoAds.gameObject.SetActive(false);

        // Ẩn nhân vật
        await rtfmCharacter.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask();
        rtfmCharacter.gameObject.SetActive(false);

        // Ẩn phần nội dung chính
        await UniTask.WhenAll(
            rtfmWatchADs.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask(),
            rtfmNoAds.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask()
        );

        rtfmWatchADs.gameObject.SetActive(false);
        rtfmNoAds.gameObject.SetActive(false);

        await rtfmContent.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).ToUniTask();
        rtfmContent.gameObject.SetActive(false);

        // Làm mờ dần nền fade
        await imgFade.DOFade(0, 0.3f).SetEase(Ease.Linear).ToUniTask();
        imgFade.gameObject.SetActive(false);

        // Reset lại trạng thái để lần sau Show() chạy đúng
        Reset();
        isShowing = false;
    }

    public void OnClickWatchAds()
    {
        Hide();
        actionAfterShow?.Invoke();
    }
    public void OnBuyNoAds()
    {
        if (isShowing == false) return;
        UITopController.Instance.OnStartGameplay();
        Hide();
        actionAfterShow?.Invoke();
    }
    public void OnClickNoAds()
    {
        ShopIAPController.Instance.ShowNoAds();
        IngameData.SHOP_PLACEMENT = shop_placement.PopupFirstAds;

    }
}
