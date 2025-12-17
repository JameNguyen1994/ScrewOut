using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Storage;
using Coffee.UIExtensions;

public class WrenchCollectionMainMenuProgressBar : MonoBehaviour
{
    [SerializeField] private Transform contentInMainMenu;
    [SerializeField] private Transform contentInMainMenuLock;
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtTimeCountDown;
    [SerializeField] private TextMeshProUGUI txtReward;
    [SerializeField] private TextMeshProUGUI txtProgress;
    [SerializeField] private Image imgReward;
    [SerializeField] private RoundedProgressBar imgFill;
    [SerializeField] private UIParticle fillEffect;
    [SerializeField] private Transform target;

    //Popup
    [SerializeField] private TextMeshProUGUI txtTimeCountDownPopup;
    [SerializeField] private TextMeshProUGUI txtRewardPopup;
    [SerializeField] private TextMeshProUGUI txtProgressPopup;
    [SerializeField] private Image imgRewardPopup;
    [SerializeField] private RoundedProgressBar imgFillPopup;

    [SerializeField] private TextMeshProUGUI txtFinalRewardPopup;
    [SerializeField] private Image imgFinalRewardPopup;

    //Popup Tutorial
    [SerializeField] private TextMeshProUGUI txtTimeCountDownPopupTutorial;
    [SerializeField] private TextMeshProUGUI txtRewardPopupTutorial;
    [SerializeField] private TextMeshProUGUI txtProgressPopupTutorial;
    [SerializeField] private Image imgRewardPopupTutorial;
    [SerializeField] private RoundedProgressBar imgFillPopupTutorial;

    [SerializeField] private TextMeshProUGUI txtFinalTutorialRewardPopup;
    [SerializeField] private Image imgFinalTutorialRewardPopup;

    //For Get WrechEffect
    [SerializeField] private Transform contentLevel;
    [SerializeField] private UIParticle particle;
    [SerializeField] private RewardNotification rewardNotification;

    [SerializeField] private CanvasGroup contentInMainMenuCanvasGroup;
    [SerializeField] private UIFlyItemToTarget uiFlyItemToTarget;

    private void Awake()
    {
        WrenchCollectionService.ActiveEvent();
        contentInMainMenuLock.gameObject.SetActive(false);

        if (WrenchCollectionService.IsShowInMain())
        {
            contentInMainMenu.gameObject.SetActive(true);
            UpdateUIMainMenu(false);
        }
        else
        {
            contentInMainMenu.gameObject.SetActive(false);

            int userLevel = Db.storage.USER_INFO.level;

            if (!WrenchCollectionService.IsUnlock(userLevel))
            {
                contentInMainMenuLock.gameObject.SetActive(true);
            }
        }
    }

    public Vector3 GetPosTarget()
    {
        return target.position;
    }

    public async UniTask UpdateUIMainMenu(bool isPlayEffect)
    {
        contentInMainMenuCanvasGroup.alpha = 1;

        CancelInvoke(nameof(UpdateCountdown));

        WrenchCollectionData data = Db.storage.WrenchCollectionData;
        WrenchCollectionRewardData reward;

        if (WrenchCollectionService.IsMaxLevel())
        {
            txtLevel.text = "MAX";
            reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level - 1, data.rewardGroup);

            if (isPlayEffect)
            {
                fillEffect.Stop();
                fillEffect.Play();
                await imgFill.SetProgress(1, 0.25f);
            }
            else
            {
                imgFill.SetProgress(1);
            }
        }
        else
        {
            txtLevel.text = (data.level + 1).ToString();
            reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

            if (isPlayEffect)
            {
                fillEffect.Stop();
                fillEffect.Play();
                await imgFill.SetProgress((float)data.collectedWrench / reward.WrenchAmount, 0.25f);
            }
            else
            {
                imgFill.SetProgress((float)data.collectedWrench / reward.WrenchAmount);
            }
        }

        imgReward.sprite = reward.GetIcon();
        imgRewardPopup.sprite = reward.GetIcon();
        imgRewardPopupTutorial.sprite = reward.GetIcon();

        txtReward.text = reward.RewardAmoutString();
        txtRewardPopup.text = reward.RewardAmoutString();
        txtRewardPopupTutorial.text = reward.RewardAmoutString();

        txtProgress.text = $"{data.collectedWrench}/{reward.WrenchAmount}";
        txtProgressPopup.text = $"{data.collectedWrench}/{reward.WrenchAmount}";
        txtProgressPopupTutorial.text = $"{data.collectedWrench}/{reward.WrenchAmount}";

        imgFillPopup.SetProgress((float)data.collectedWrench / reward.WrenchAmount);
        imgFillPopupTutorial.SetProgress((float)data.collectedWrench / reward.WrenchAmount);

        WrenchCollectionRewardData finalReward = WrenchCollectionManager.Instance.Config.GetFinalReward(data.rewardGroup);

        txtFinalRewardPopup.text = finalReward.RewardAmoutString();
        txtFinalTutorialRewardPopup.text = finalReward.RewardAmoutString();

        imgFinalRewardPopup.sprite = finalReward.GetIcon();
        imgFinalTutorialRewardPopup.sprite = finalReward.GetIcon();

        if (!IsInvoking(nameof(UpdateCountdown)))
        {
            InvokeRepeating(nameof(UpdateCountdown), 0f, 1f);
        }

        await UniTask.DelayFrame(1);
    }

    private void UpdateCountdown()
    {
        WrenchCollectionData data = Db.storage.WrenchCollectionData;

        if (data.endYear == 0)
        {
            txtTimeCountDown.text = "END";
            txtTimeCountDownPopup.text = "END";
            txtTimeCountDownPopupTutorial.text = "END";
            CancelInvoke(nameof(UpdateCountdown));
            return;
        }

        string countDown = Utility.CountDownTimeToString(data.endYear, data.endMonth, data.endDay, data.endHour, data.endMinute, 0);

        if (string.IsNullOrEmpty(countDown))
        {
            txtTimeCountDown.text = "END";
            txtTimeCountDownPopup.text = "END";
            txtTimeCountDownPopupTutorial.text = "END";
            CancelInvoke(nameof(UpdateCountdown));
            return;
        }

        txtTimeCountDown.text = countDown;
        txtTimeCountDownPopup.text = countDown;
        txtTimeCountDownPopupTutorial.text = countDown;
    }

    public async UniTask UpgradeLevel(WrenchCollectionData data)
    {
        AudioController.Instance.PlaySound(SoundName.CollectExp);
        uiFlyItemToTarget.SetIconAndValue(null, data.collectedInGamplayWrench.ToString());
        await uiFlyItemToTarget.Play(() => { particle.Play(); });
        AudioController.Instance.PlaySound(SoundName.CollectBooster);
        await contentLevel.transform.DOScale(1.25f, 0.1f).SetEase(Ease.OutBack);
        await contentLevel.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);
        await UpgradeLevelHandler(data);
    }

    private async UniTask UpgradeLevelHandler(WrenchCollectionData data)
    {
        WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(data.level, data.rewardGroup);

        if (reward == null)
        {
            WrenchCollectionService.UpgradeLevel();
            await UpdateUIMainMenu(true);
            return;
        }

        bool isUpgrade = false;
        int currentValue = data.collectedWrench;
        int targetValue = 0;

        if (reward != null)
        {
            targetValue = reward.WrenchAmount;

            if (data.collectedWrench + data.collectedInGamplayWrench < reward.WrenchAmount)
            {
                data.collectedWrench = data.collectedWrench + data.collectedInGamplayWrench;
                data.collectedInGamplayWrench = 0;
            }
            else if (data.collectedWrench + data.collectedInGamplayWrench >= reward.WrenchAmount)
            {
                data.collectedInGamplayWrench = data.collectedWrench + data.collectedInGamplayWrench - reward.WrenchAmount;
                data.collectedWrench = 0;
                data.level++;
                isUpgrade = true;
            }
        }

        if (isUpgrade)
        {
            fillEffect.Stop();
            fillEffect.Play();
            imgFill.SetProgress(0);
            txtProgress.DoTextProgress(currentValue, targetValue, 0.5f);
            await imgFill.SetProgress(1, 0.25f);
            await imgReward.transform.DOScale(1.25f, 0.1f).SetEase(Ease.OutBack);

            AudioController.Instance.PlaySound(SoundName.CollectBooster);
            rewardNotification.ShowMyText(txtReward.text, imgReward.sprite, 1f);
            imgReward.sprite = reward.GetIcon();
            txtReward.text = reward.RewardAmoutString();

            await imgReward.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);

            await UniTask.Delay(500);
            await UpgradeLevelHandler(data);
        }
        else
        {
            await UpdateUIMainMenu(true);
        }
    }

    public async UniTask ShowContent(float duration)
    {
        if (WrenchCollectionService.IsShowInMain())
        {
            await contentInMainMenuCanvasGroup.DOFade(1, duration).OnComplete(() => contentInMainMenuCanvasGroup.interactable = true);
        }
    }

    public async UniTask HideContent(float duration)
    {
        if (WrenchCollectionService.IsShowInMain())
        {
            await contentInMainMenuCanvasGroup.DOFade(0, duration).OnComplete(() => contentInMainMenuCanvasGroup.interactable = false);
        }
    }
}