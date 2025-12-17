using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spin
{
    /// <summary>
    /// Handles spin wheel logic, UI updates, and reward claiming.
    /// </summary>
    public class SpinController : Singleton<SpinController>
    {
        [Header("Data References")]
        [SerializeField] private SpinRewardData dataRewardSpin;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Image imgFade;
        [SerializeField] private Transform content;

        [Header("UI Components")]
        [SerializeField] private TMPro.TextMeshProUGUI txtSpinByScrew;
        [SerializeField] private TMPro.TextMeshProUGUI txtSpinByADS;

        [SerializeField] private Button btnSpinByADS;
        [SerializeField] private Button btnSpinByScrew;
        [SerializeField] private Button btnExit;
        [SerializeField] private Button btnHint;
        [SerializeField] private List<GameObject> dotLights;
        [SerializeField] private Image imgIconAds, imgIconScrew;
        [SerializeField] private RewardNotification rewardNotification;

        [Header("UI Dialog")]
        [SerializeField] private ScreenGetRW screenGetRW;
        [SerializeField] private SpinTutorial spinTutorial;

        private int segment = 8;
        private int targetSegment;
        private float spinDuration = 6f;
        private SpinReward reward;
        private bool isSpinOnAir = false;
        private bool isSpinComplete = false;
        private System.DateTime nextResetTime;

        protected override void CustomAwake()
        {
            base.CustomAwake();
            DontDestroyOnLoad(this);
            Setup();
        }

        private void Setup()
        {
            var imgCoverColor = imgFade.color;
            imgCoverColor.a = 0;
            imgFade.color = imgCoverColor;

            imgFade.gameObject.SetActive(false);
            content.gameObject.SetActive(false);

            btnSpinByADS.transform.localScale = Vector3.zero;
            btnSpinByScrew.transform.localScale = Vector3.zero;
            btnExit.transform.localScale = Vector3.zero;
            btnHint.transform.localScale = Vector3.zero;
        }

        [Button]
        public async void DOShow()
        {
            UITopController.Instance.OnShowWeeklyTask();

            UpdateUI(false);
            imgFade.gameObject.SetActive(true);
            imgFade.DOFade(0.98f, 0.5f);
            await UniTask.Delay(200);
            content.gameObject.SetActive(true);
            await content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            btnSpinByADS.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
            btnSpinByScrew.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
            btnExit.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
            btnHint.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);

            if (SpinService.HaveTutorial())
            {
                spinTutorial.Show();
                SpinService.TutorialDone();
            }
        }

        private void UpdateCountdown()
        {
            if (SpinService.CanSpinByADS())
            {
                CancelInvoke(nameof(UpdateCountdown));
                txtSpinByADS.text = string.Format(SpinDefine.SPIN_WITH_ADS, Db.storage.LuckySpinData.dailySpinByADS, SpinDefine.DAILY_SPIN);
                return;
            }

            System.DateTime Now = TimeGetter.Instance.Now;
            System.TimeSpan remaining = nextResetTime - Now;

            if (remaining.TotalSeconds <= 0)
            {
                nextResetTime = Now.Date.AddDays(1);
                remaining = nextResetTime - TimeGetter.Instance.Now;
            }

            txtSpinByADS.text = string.Format(SpinDefine.SPIN_WITH_ADS_COUNT_DOWN, $"{remaining.Hours:D2}:{remaining.Minutes:D2}:{remaining.Seconds:D2}");
        }

        public async void DOHide()
        {
            btnSpinByADS.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
            btnSpinByScrew.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
            btnExit.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);
            btnHint.transform.DOScale(0, 0.3f).SetEase(Ease.InBack);

            await content.DOScale(0, 0.3f).SetEase(Ease.InBack);
            imgFade.DOFade(0, 0.5f);
            await UniTask.Delay(500);
            imgFade.gameObject.SetActive(false);
            Setup();

            CancelInvoke(nameof(UpdateCountdown));

            UITopController.Instance.OnShowMainMenu();
        }

        [ContextMenu("Spin")]
        public void Spin()
        {
            // --- Spin Logic ---
            targetSegment = SpinService.GetTargetSegment(dataRewardSpin);
            Debug.Log($"[Spin] Target Segment: {targetSegment}");
            reward = dataRewardSpin.GetRewardByIndex(targetSegment);

            // Consume energy and apply reward
            SpinService.ClaimReward(reward);

            // --- Start wheel rotation ---
            Spin(targetSegment, reward);
        }

        public void Spin(int rewardIndex, SpinReward reward)
        {
            // ---- 1. Calculate target angle ----
            float anglePerSegment = 360f / segment;
            float baseAngle = rewardIndex * anglePerSegment;

            // Random offset: +/- a few degrees to "near miss"
            //float offset = Random.Range(-SpinDefine.RANDOM_OFFSET_RANGE, SpinDefine.RANDOM_OFFSET_RANGE);
            float targetAngle = baseAngle;

            // Random number of spins
            float fullSpins = Random.Range(SpinDefine.MIN_ROUNDS, SpinDefine.MAX_ROUNDS + 1);
            float finalAngle = -(360f * fullSpins + targetAngle); // clockwise spin

            // Reset rotation
            wheelTransform.localRotation = Quaternion.identity;
            isSpinOnAir = true;

            StartCoroutine(PlayAnimDot());

            // ---- 2. Main spin ----
            wheelTransform
                .DORotate(new Vector3(0, 0, finalAngle), spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    moveTime = 0.15f;

                    // ---- 3. Overshoot + bounce (absolute rotation) ----
                    //Sequence settle = DOTween.Sequence();

                    //float overshoot = finalAngle - 8f;
                    //float rebound = finalAngle + 3f;

                    //settle.Append(wheelTransform
                    //    .DOLocalRotate(new Vector3(0, 0, overshoot), 0.25f)
                    //    .SetEase(Ease.OutQuad)
                    //    .SetRelative(false));

                    //settle.Append(wheelTransform
                    //    .DOLocalRotate(new Vector3(0, 0, rebound), 0.2f)
                    //    .SetEase(Ease.InOutSine)
                    //    .SetRelative(false));

                    //settle.Append(wheelTransform
                    //    .DOLocalRotate(new Vector3(0, 0, finalAngle), 0.25f)
                    //    .SetEase(Ease.OutSine)
                    //    .SetRelative(false)
                    //    .OnComplete(() =>
                    //    {
                    //        isSpinOnAir = false;
                    //        isSpinComplete = true;
                    //        StartCoroutine(OnSpinComplete(reward));
                    //    }));

                    isSpinOnAir = false;
                    isSpinComplete = true;
                    StartCoroutine(OnSpinComplete(reward));
                });
        }

        public void OnSpinByScrewClick()
        {
            if (SpinService.CanSpinByScrew())
            {
                AudioController.Instance.PlaySound(SoundName.Click);
                UpdateUI(true);
                SpinService.SpinByScrewConsumeHandler();
                Spin();
            }
        }

        public void OnSpinByADSClick()
        {
            if (SpinService.CanSpinByADS())
            {
                AudioController.Instance.PlaySound(SoundName.Click);
                AdsController.Instance.ShowRewardAds(RewardAdsPos.lucky_spin, SpiByADSHandler, null, null, "spin_free_by_ads");
            }
        }

        private void SpiByADSHandler()
        {
            UpdateUI(true);
            SpinService.SpinByADSConsumeHandler();
            Spin();
        }

        public void OnClickClose()
        {
            AudioController.Instance.PlaySound(SoundName.Click);
            DOHide();
        }

        /// <summary>
        /// Handles actions after the spin animation completes.
        /// </summary>
        /// <param name="reward">The reward obtained from the spin.</param>
        private IEnumerator OnSpinComplete(SpinReward reward)
        {
            yield return new WaitForSeconds(1f);
            isSpinComplete = false;

            string value = string.Empty;

            if (reward.RewardType is ResourceIAP.ResourceType.InfiniteLives
                                  or ResourceIAP.ResourceType.InfiniteRocket
                                  or ResourceIAP.ResourceType.InfiniteGlass)
            {
                value = reward.RewardAmoutString();
            }
            else
            {
                value = reward.GetValue().ToString();
            }

            screenGetRW.OnShowScreen(value, reward.GetIcon());
        }

        public async void OnClaimRewardComplete(bool isDouble)
        {
            TrackingController.Instance.TrackingSpin(reward.RewardType, reward.Value * (isDouble ? 2 : 1));
            if (isDouble)
            {
                Debug.Log($"[Spin] Double Reward");
                SpinService.ClaimReward(reward);
            }

            await UniTask.Delay(500);

            int value = !isDouble ? reward.GetValue() : reward.GetValue() * 2;

            switch (reward.RewardType)
            {
                case ResourceIAP.ResourceType.Coin:
                    MainMenuRecieveRewardsHelper.Instance.ShowGetCoinEffect(value);
                    break;
                case ResourceIAP.ResourceType.InfiniteLives:
                    MainMenuRecieveRewardsHelper.Instance.ShowGetHeartEffect(isDouble ? reward.GetValue() * 2 : reward.GetValue());
                    break;

                case ResourceIAP.ResourceType.BoosterAddHold:
                case ResourceIAP.ResourceType.BoosterHammer:
                case ResourceIAP.ResourceType.BoosterBloom:
                case ResourceIAP.ResourceType.BoosterUnlockBox:
                    AudioController.Instance.PlaySound(SoundName.CollectBooster);
                    rewardNotification.Show(reward.RewardType != ResourceIAP.ResourceType.InfiniteLives ? value.ToString() : string.Empty, reward.GetIcon());
                    break;
            }

            UpdateUI(false);
            MainMenuController.Instance.UpdateUILuckySpin();
        }

        /// <summary>
        /// Updates the UI.
        /// </summary>
        public void UpdateUI(bool isSpinning)
        {
            txtSpinByScrew.text = string.Format(SpinDefine.SPIN_WITH_SCREW, Storage.Db.storage.LuckySpinData.collectedScrew, SpinDefine.REQURIED_SCREW);

            CountdownSpinByADS();

            btnExit.interactable = !isSpinning;
            btnHint.interactable = !isSpinning;

            if (!SpinService.CanSpinByScrew())
            {
                btnSpinByScrew.interactable = false;
                txtSpinByScrew.color = Color.gray;
                imgIconScrew.color = Color.gray;
            }
            else
            {
                btnSpinByScrew.interactable = !isSpinning;
                txtSpinByScrew.color = isSpinning ? Color.gray : Color.white;
                imgIconScrew.color = isSpinning ? Color.gray : Color.white;
            }

            if (!SpinService.CanSpinByADS())
            {
                btnSpinByADS.interactable = false;
                txtSpinByADS.color = Color.gray;
                imgIconAds.color = Color.gray;
            }
            else
            {
                btnSpinByADS.interactable = !isSpinning;
                txtSpinByADS.color = isSpinning ? Color.gray : Color.white;
                imgIconAds.color = isSpinning ? Color.gray : Color.white;
            }

            ResetDotLight();
        }

        private void ResetDotLight()
        {
            for (int i = 0; i < dotLights.Count; i++)
            {
                dotLights[i].SetActive(false);
            }
        }

        private float moveTime = 0.125f;

        private IEnumerator PlayAnimDot()
        {
            ResetDotLight();
            int index = dotLights.Count - 1;
            moveTime = 0.125f;

            while (isSpinOnAir)
            {
                yield return new WaitForSeconds(moveTime);

                if (index < dotLights.Count - 1)
                {
                    dotLights[index + 1].SetActive(false);
                }
                else
                {
                    dotLights[0].SetActive(false);
                }

                dotLights[index--].SetActive(true);

                if (index < 0)
                {
                    index = dotLights.Count - 1;
                }
            }

            ResetDotLight();

            while (isSpinComplete)
            {
                yield return new WaitForSeconds(0.15f);

                for (int i = 0; i < dotLights.Count; i++)
                {
                    dotLights[i].SetActive(!dotLights[i].activeSelf);
                }
            }

            ResetDotLight();
        }

        public void OnClickHint()
        {
            AudioController.Instance.PlaySound(SoundName.Click);
            spinTutorial.Show();
        }

        public bool IsCountdownRunning()
        {
            return IsInvoking(nameof(UpdateCountdown));
        }

        public void CountdownSpinByADS()
        {
            if (SpinService.CanSpinByADS())
            {
                txtSpinByADS.text = string.Format(SpinDefine.SPIN_WITH_ADS, Db.storage.LuckySpinData.dailySpinByADS, SpinDefine.DAILY_SPIN);
            }
            else if (!SpinService.CanSpinByADS() && !IsCountdownRunning())
            {
                System.DateTime Now = TimeGetter.Instance.Now;
                nextResetTime = Now.Date.AddDays(1);
                InvokeRepeating(nameof(UpdateCountdown), 0f, 1f);
            }
        }
    }
}