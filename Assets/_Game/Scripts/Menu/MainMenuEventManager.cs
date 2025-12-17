using Coffee.UIExtensions;
using Spin;
using TMPro;
using UnityEngine;

public class MainMenuEventManager : Singleton<MainMenuEventManager>
{
    public GameObject ButtonLuckySpin;
    public GameObject ButtonLuckySpinLock;

    public GameObject ButtonDailyGift;
    public GameObject ButtonDailyGiftLock;

    public GameObject ButtonBeginner;
    public UIParticle particleBeginner;

    public RectTransform ButtonNoAds;

    public GameObject ButtonHappyShop;
    public UIParticle particle;
    public TextMeshProUGUI txtCountDown;
    public TextMeshProUGUI txtCountDownBeginer;

    public void CheckUnlockEvent()
    {
        ButtonLuckySpin.SetActive(SpinService.IsUnlock());
        ButtonLuckySpinLock.SetActive(!SpinService.IsUnlock());

        ButtonDailyGift.SetActive(MainMenuService.IsUnlockDailyGift());
        ButtonDailyGiftLock.SetActive(!MainMenuService.IsUnlockDailyGift());

        bool checkShowBeginner = BeginerPackService.IsActive();
        ButtonBeginner.SetActive(checkShowBeginner);

        PopupSupperOffer.Instance.CheckShowButtonHappyShop();
        ShopIAPController.Instance.CheckShowBundleBeginerPack();
    }

    public void OnClickShowSupperOffer()
    {
        PopupSupperOffer.Instance.OnClickShow();
    }
}