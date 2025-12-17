using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITopController : Singleton<UITopController>
{
    [SerializeField] private PanelCoin panelCoin;
    [SerializeField] private PanelStar panelStar;
    [SerializeField] private PanelHeart panelHeart;
    [SerializeField] private PanelAvatar panelAvatar;
    [SerializeField] private Transform fxParent;

    public Transform FxParent => fxParent;

    public Vector3 GetStarPos()
    {
        return panelStar.ImgStar.transform.position;
    }
    public Transform GetHeartTrans()
    {
        return panelHeart.RtfmHeart;
    }
    public Vector3 GetCoinPos()
    {
        return panelCoin.ImgCoin.transform.position;
    }

    public Transform GetExpUserPos()
    {
        return panelAvatar.TfmTargetBar;
    }

    public void UpdateStartUI()
    {
        panelStar.UpdateStartUI();
    }
    public void OnShowMainMenu()
    {
        panelCoin.TogglePanel(false, 0);
        panelStar.TogglePanel(false, 0);
        panelHeart.TogglePanel(false, 0);
        panelAvatar.TogglePanel(false, 0);

        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(true);
        panelHeart.TogglePanel(true);
        panelAvatar.TogglePanel(true);
    }
    public void OnShowTabTask()
    {
        panelCoin.TogglePanel(false);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(false);
    }
    public void OnClickCoin()
    {
        if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        IngameData.SHOP_PLACEMENT = shop_placement.IconCoin;
        panelCoin.OnClickAddCoin();
    }

    public void ShowShop()
    {
        panelCoin.OnClickAddCoin();
    }

    public void OnClickCoinBuyHeart()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        panelCoin.OnClickCoinBuyHeart();
    }
    public void OnStartGameplay()
    {
        panelCoin.TogglePanel(false);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(false);
    }
    public void OnShowWinGame()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(true);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(true);
    }
    public void OnPauseGame()
    {
        panelHeart.TogglePanel(true);
    }
    public void OnShowLose()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(true);
    }
    public void OnShowRevive()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(true);
    }
    public void OnShowBuyLife()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(true);
    }
    public void OnShowPopupBooster()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(false);
    }
    public void ShowPopupBeginner()
    {
        panelCoin.TogglePanel(true);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(false);
    }
    public void OnClosePopupShop()
    {
        panelCoin.ToggleAddIcon(true);

    }
    public void OnShowTabShop()
    {
        Debug.Log("OnShowTabShop");
        panelHeart.TogglePanel(false);
        panelStar.TogglePanel(false);
        panelAvatar.TogglePanel(false);
        panelCoin.ToggleAddIcon(false);
        panelCoin.TogglePanel(false);

    }

    public void OnShowTabHome()
    {
        Debug.Log("OnShowTabHome");
        panelHeart.EnableAddLife(true);
        panelStar.TogglePanel(true);
        panelHeart.TogglePanel(true);
        panelAvatar.TogglePanel(true);
        panelCoin.ToggleAddIcon(true);
        panelCoin.TogglePanel(true);

    }
    public void OnShowWeeklyTask()
    {
        Debug.Log("OnShowWeeklyTask");
        panelHeart.EnableAddLife(false);
        panelStar.TogglePanel(false);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(false);
        panelCoin.TogglePanel(false);
        panelCoin.ToggleAddIcon(false);
    }


    public void OnShowCoinAndHeart()
    {
        panelCoin.TogglePanel(true);
        panelHeart.TogglePanel(true);
    }
    public void OnHideCoinAndHeart()
    {
        panelCoin.TogglePanel(false);
        panelHeart.TogglePanel(false);
    }


    public void OnHideCoin()
    {
        panelCoin.TogglePanel(false);
    }
    public void OnShowPopupNoAds()
    {
        panelCoin.TogglePanel(false);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(false);
    }
    public void OnShowSetting()
    {
        panelCoin.TogglePanel(false);
        panelHeart.TogglePanel(false);
        panelAvatar.TogglePanel(false);
    }

    public void OnFocusObject(bool isForcus, float playTimeDuration)
    {
        panelCoin.TogglePanel(!isForcus, playTimeDuration);
        panelStar.TogglePanel(!isForcus, playTimeDuration);
        panelHeart.TogglePanel(!isForcus, playTimeDuration);
        panelAvatar.TogglePanel(!isForcus, playTimeDuration);
    }

    public void PlayAvatarEffect()
    {
        panelAvatar.PlayEffect();
    }

    public void UpdateCoin()
    {
        panelCoin.UpdateCoinUIForReward();
    }


    public void ShowExpLevelGameplayOnly()
    {
        panelAvatar.TogglePanel(true);
    }
    public void HideExpLevelGameplayOnly()
    {
        panelAvatar.TogglePanel(false);
    }
}
