using DG.Tweening;
using MainMenuBar;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCoin : MonoBehaviour
{
    [SerializeField] private Text txtCoin;
    [SerializeField] private Image imgCoin;
    [SerializeField] private Image imgAdd;
    [SerializeField] private RectTransform rtfmCoin;

    [SerializeField] private CoinFlyAnim coinFlyAnim;
    private int currentCoin = 0;
    public Image ImgCoin { get => imgCoin; }

    Tween tweenCoin;

    private void Start()
    {
        currentCoin = Db.storage.USER_INFO.coin;
        EventDispatcher.Register(EventId.UpdateCoinUI, OnCoinUpdate);
        EventDispatcher.Register(EventId.MakeCoinFly, OnCoinFlying);
        UpdateCoinUI();
    }
    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.UpdateCoinUI, UpdateCoinUI);
        EventDispatcher.RemoveCallback(EventId.MakeCoinFly, OnCoinFlying);

    }
    public void UpdateCoinUI(object data = null)
    {
        txtCoin.text = $"{currentCoin}";
    }

    void OnCoinUpdate(object data)
    {
        if (data == null)
        {
            var coin = Db.storage.USER_INFO.coin;
            txtCoin.text = $"{coin}";
            currentCoin = coin;
            return;
        }

        if (data is not UpdateCoinData)
        {
            data = new UpdateCoinData()
            {
                coin = (int)data,
                coinMode = CoinMode.Plus
            };
        }

        UpdateCoinData coinData = (UpdateCoinData)data;

        if (coinData.coinMode == CoinMode.Immediate)
        {
            currentCoin = Db.storage.USER_INFO.coin;
            UpdateCoinUI();
        }
        else if (coinData.coinMode == CoinMode.Plus)
        {
            currentCoin = Db.storage.USER_INFO.coin;
            UpdateCoinUI();
        }
        else
        {
            if (tweenCoin != null)
            {
                tweenCoin.Kill();
            }

            int updateCoin = Db.storage.USER_INFO.coin;
            tweenCoin = DOVirtual.Int(currentCoin, updateCoin, 0.5f, (value) =>
            {
                currentCoin = value;
                UpdateCoinUI(value);
            });
        }
    }
    public void OnCoinFlying(object data)
    {
        if (data == null) return;
        Vector3 fromPos = (Vector3)data;
        coinFlyAnim.PlayCoinFlyWithPos(0.3f, fromPos, isHaveSound: true).Forget();
    }
    public void ToggleAddIcon(bool active)
    {
        imgAdd.gameObject.SetActive(active);
    }
    public void OnClickAddCoin()
    {

        switch (SceneController.Instance.CurrentScene)
        {
            case SceneType.MainMenu:
                ShopIAPController.Instance.ShowAbbreviated();
                ShopIAPController.Instance.OnClickShowPopup(() => PopupController.Instance.PopupCount--, true);
                UITopController.Instance.OnShowTabShop();
                break;
            case SceneType.GamePlayNewControl:
            case SceneType.Gameplay:
                if (PopupController.Instance != null && PopupController.Instance.PopupWinNew.IsShowing)
                    return;
                if (ShopIAPController.Instance.Showing) break;
                PopupController.Instance.PopupCount++;
                ShopIAPController.Instance.ShowAbbreviated();
                ShopIAPController.Instance.OnClickShowPopup(() => PopupController.Instance.PopupCount--, true);
                ShopIAPController.Instance.ExpandBottom(false);
                break;
        }
        ToggleAddIcon(false);
    }
    public void OnClickCoinBuyHeart()
    {
        switch (SceneController.Instance.CurrentScene)
        {
            case SceneType.MainMenu:
                ShopIAPController.Instance.ShowAbbreviatedHeart();
                ShopIAPController.Instance.OnClickShowPopup(() => PopupController.Instance.PopupCount--, true);
                break;
            case SceneType.GamePlayNewControl:
            case SceneType.Gameplay:
                //IngameData.SHOP_PLACEMENT = shop_placement.icon_coin;

                ShopIAPController.Instance.ShowAbbreviatedHeart();

                PopupController.Instance.PopupCount++;
                ShopIAPController.Instance.OnClickShowPopup(() => PopupController.Instance.PopupCount--, true);
                break;
        }
    }

    [SerializeField] private float activeValue = -50f;
    [SerializeField] private float deActiveValue = 350f;

    public void TogglePanel(bool active, float time = 1)
    {
        //rtfmCoin.DOKill();
        //if (active)
        //{
        //    rtfmCoin.DOAnchorPosX(100f, time / 2).SetEase(Ease.OutQuad);
        //}
        //else
        //{
        //    rtfmCoin.DOAnchorPosX(-300f, time / 2).SetEase(Ease.InQuad);
        //}

        rtfmCoin.DOKill();
        if (active)
        {
            rtfmCoin.DOAnchorPosY(activeValue, time / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            rtfmCoin.DOAnchorPosY(deActiveValue, time / 2).SetEase(Ease.InQuad);
        }
    }

    public void UpdateCoinUIForReward()
    {
        currentCoin = Db.storage.USER_INFO.coin;
        UpdateCoinUI();
    }
}
public enum CoinMode
{
    Immediate = 0,
    Smooth = 1,
    Plus = 2
}

public struct UpdateCoinData
{
    public int coin;
    public CoinMode coinMode;
}