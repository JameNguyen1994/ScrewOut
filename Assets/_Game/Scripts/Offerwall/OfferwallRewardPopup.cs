using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferwallRewardPopup : MonoBehaviour
{
    [SerializeField] private Transform tfmMain;
    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Transform coinPos;
    [SerializeField] private Button btnClaim;
    [SerializeField] private TextMeshProUGUI txtCoin;
    
    
    int coinToClaim = 0;

    public void Show()
    {
        var vcoin = Db.storage.OW_VC_DATA;
        coinToClaim = (int)vcoin;

        if (coinToClaim < 1)
        {
            return;
        }
        
        CancelInvoke(nameof(DoShow));
        Invoke(nameof(DoShow), 2);
    }

    void DoShow()
    {
        txtCoin.text = $"+{coinToClaim}";
        ShowAsync().Forget();
    }
    
    async UniTask ShowAsync()
    {
        fadePanel.SetActive(true);
        btnClaim.interactable = true;
        tfmMain.localScale = Vector3.zero;
        await tfmMain.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        UITopController.Instance.OnShowCoinAndHeart();
    }

    public void OnClaimBtnClick()
    {
        btnClaim.interactable = false;
        Db.storage.OW_VC_DATA -= coinToClaim;
        TrackingController.Instance.TrackingOfferwallRewardReceived(Db.storage.USER_INFO.level, coinToClaim);
        DoClaimCoin().Forget();
    }

    async UniTask DoClaimCoin()
    {
        await FlyEffectController.Instance.DOFlyCoin(coinToClaim, coinPos.position, UITopController.Instance.GetCoinPos());
        await UniTask.Delay(300);
        await HideAsync();
    }
    
    async UniTask HideAsync()
    {
        await tfmMain.DOScale(0f, 0.3f).SetEase(Ease.InBack);

        if (!ShopIAPController.Instance.Showing)
        {
            UITopController.Instance.OnHideCoinAndHeart();
        }
        
        fadePanel.SetActive(false);
    }
}
