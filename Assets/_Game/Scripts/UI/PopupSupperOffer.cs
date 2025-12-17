using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using TMPro;
using UnityEngine;

public class PopupSupperOffer : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonConfirm;

    public static PopupSupperOffer Instance { get; private set; }

    public ResourceDataSO SupperOfferData;
    public ItemIAPBundleSupperOffer ItemIAPBundleSupperOffer;
    public bool IsShow;
    public TextMeshProUGUI txtCountDown;
    public GameObject CountDown;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckShowButtonHappyShop()
    {
        CountDown.SetActive(SupperOfferService.IsActive());
        MainMenuEventManager.Instance.ButtonHappyShop.SetActive(SupperOfferService.IsActive());
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        if (!SupperOfferService.IsActive())
        {
            MainMenuEventManager.Instance.ButtonHappyShop.SetActive(false);
            return;
        }

        SupperOfferData data = Db.storage.SupperOfferData;
        string countDown = Utility.CountDownTimeToString(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);
        MainMenuEventManager.Instance.txtCountDown.text = countDown;
        txtCountDown.text = Utility.ClockDownTimeToString(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);

        if (string.IsNullOrEmpty(countDown))
        {
            MainMenuEventManager.Instance.ButtonHappyShop.SetActive(false);
            CancelInvoke(nameof(UpdateCountdown));
        }
        else
        {
            Invoke(nameof(UpdateCountdown), 1);
        }
    }

    public void OnClickShow()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Show().Forget();
    }

    public override async UniTask Show()
    {
        IsShow = true;
        var buySupperOfferHandler = new SupperOfferBuyBundleHandler();
        ItemIAPBundleSupperOffer.Init(SupperOfferData.data[0], buySupperOfferHandler).Forget();
        Setup();
        DOShow().Forget();
    }

    private void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        content.localScale = Vector3.zero;
        buttonCancel.localScale = Vector3.zero;
        buttonConfirm.localScale = Vector3.zero;
    }

    private async UniTask DOShow()
    {
        CountDown.SetActive(SupperOfferService.IsActive());
        UpdateCountdown();
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(200);
        content.gameObject.SetActive(true);
        content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(200);
        buttonCancel.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        buttonConfirm.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    private async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await buttonConfirm.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        imgFade.DOFade(0, 0.5f);
        await UniTask.Delay(500);
        imgFade.gameObject.SetActive(false);
        IsShow = false;
    }

    public void OnClickCancel()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        if (!MainMenuEventManager.Instance.ButtonHappyShop.activeSelf)
        {
            ShowButtonHandler();
        }
        else
        {
            Hide();
        }
    }

    private async void ShowButtonHandler()
    {
        await DOHide();

        if (SupperOfferService.IsActive())
        {
            MainMenuEventManager.Instance.ButtonHappyShop.SetActive(true);
            MainMenuEventManager.Instance.particle.Play();
        }
        else
        {
            MainMenuEventManager.Instance.ButtonHappyShop.SetActive(false);
        }

        UpdateCountdown();
    }

    public void OnClickConfirm()
    {
        BuyHandler();
    }

    private async void BuyHandler()
    {
        ItemIAPBundleSupperOffer.OnItemClick();
    }
}