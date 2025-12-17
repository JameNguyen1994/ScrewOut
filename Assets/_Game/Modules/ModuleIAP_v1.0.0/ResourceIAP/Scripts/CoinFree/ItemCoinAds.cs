using Cysharp.Threading.Tasks;
using PS.Analytic;
using PS.Analytic.RemoteConfig;
using Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.UI;
using ResourceIAP;

public class ItemCoinAds : MonoBehaviour
{
    [SerializeField] private Text txtCoinAmount;
    [SerializeField] private Image imgCoin;
    [SerializeField] private Text txtAdsAmount;
    [SerializeField] private Text txtTime;
    [SerializeField] private Image imgAds;
    [SerializeField] private Button btnGet;
    [SerializeField] private GameObject gobjNoti;

    public Image buttonImage;
    public Sprite rewardOn;
    public Sprite rewardOff;

    Coroutine countDownTime;
    private void Start()
    {

        GetRemote();
        txtCoinAmount.text = $"{GameAnalyticController.Instance.Remote().RewardFreeCoin.CoinAmount}";
        // txtAdsAmount.text = $"{Db.storage.ADS_COIN_AMOUNT}";

        CountDownFreeCoin();
        Debug.Log($"Db.storage.ADS_COIN_MARK{Db.storage.ADS_COIN_MARK}");

        UpdateUI();

    }
    private void GetRemote()
    {
        var remote = GameAnalyticController.Instance.Remote();
        // gameObject.SetActive(Db.storage.REWARD_SHOP_COIN_IAP_COUNT < remote.RewardControl.rewardShopCoinAmountPerDay);
    }
    public void NextDay()
    {
        Db.storage.ADS_COIN_AMOUNT = GameAnalyticController.Instance.Remote().RewardControl.rewardShopCoinAmountPerDay;
        Db.storage.FREE_COIN_MARK = true;
        Db.storage.ADS_COIN_MARK = TimeGetter.Instance.CurrentTime;
        Db.storage.REWARD_SHOP_COIN_IAP_COUNT = 0;
        txtCoinAmount.text = $"{GameConfig.SHOP_COIN_ADS}";
        txtAdsAmount.text = $"{Db.storage.ADS_COIN_AMOUNT}";

        // CountDownFreeCoin();
    }
    [SerializeField] bool isStart = true;
    async UniTask CountDownFreeCoin()
    {
        if (IsFull())
        {
            UpdateUI();
            return;
        }

        btnGet.interactable = true;
        long timeLeftt = TimeLeft(Db.storage.ADS_COIN_MARK);
        Debug.Log($"timeLeftt{timeLeftt}");
        if (timeLeftt <= 0)
        {
            isStart = false;
            txtTime.text = "";
            imgAds.gameObject.SetActive(true);
            gobjNoti.gameObject.SetActive(true);
            //itemCoinAds.gameObject.SetActive(false);
        }
        else
        {
            isStart = true;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeLeftt);
            var detail = timeSpan.TotalHours > 0 ? timeSpan.ToString(@"hh\:mm\:ss") : timeSpan.ToString(@"mm\:ss");
            txtTime.text = detail;
            imgAds.gameObject.SetActive(false);
            gobjNoti.gameObject.SetActive(false);
        }

        if (buttonImage != null)
        {
            buttonImage.sprite = timeLeftt <= 0 ? rewardOn : rewardOff;
        }

        /*  if (Db.storage.ADS_COIN_AMOUNT <= 0)
          {
              Debug.Log($"Db.storage.ADS_COIN_AMOUNT : {Db.storage.ADS_COIN_AMOUNT}");
              isStart = false;
              btnGet.interactable = false;
          }*/
        while (isStart)
        {
            //Debug.Log("Start Count down");
            await UniTask.WaitForSeconds(1);
            // Debug.Log("Start Count down 2");

            long timeLeft = TimeLeft(Db.storage.ADS_COIN_MARK);
            //  Debug.Log("Start Count down 3");

            // DBController.Instance.FREE_COIN_MARK = TimeGetter.Instance.CurrentTime;

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeLeft);
            var detail = timeSpan.ToString(@"mm\:ss");
            if (timeSpan.Days > 0)
            {
                detail = timeSpan.ToString(@"dd\:hh\:mm\:ss");
            }
            else if (timeSpan.Hours > 0)
            {
                detail = timeSpan.ToString(@"hh\:mm\:ss");
            }
            //    Debug.Log("Fail" + detail);
            txtTime.text = detail;
            if (timeLeft <= 0)
            {
                //    Debug.Log("Fail");
                isStart = false;
                txtTime.text = "";
                imgAds.gameObject.SetActive(true);
                gobjNoti.gameObject.SetActive(true);
                //  itemCoinAds.gameObject.SetActive(false);
            }
            //  Debug.Log("Start Count down 3");

        }

        if (buttonImage != null)
        {
            buttonImage.sprite = rewardOn;
        }

        //   Debug.Log("End");
    }

    private void UpdateUI()
    {
        if (IsFull() && buttonImage != null)
        {
            buttonImage.sprite = rewardOff;

            txtTime.text = "";
            imgAds.gameObject.SetActive(true);
            gobjNoti.gameObject.SetActive(false);
        }
    }

    private bool IsFull()
    {
        var remote = GameAnalyticController.Instance.Remote();
        Debug.Log($"[Remote] ShopCoin {remote.RewardControl.rewardShopCoinAmountPerDay} - ADS {Db.storage.REWARD_COIN_FREE_COUNT}");
        return remote.RewardControl.rewardShopCoinAmountPerDay <= Db.storage.REWARD_COIN_FREE_COUNT;
    }

    public void OnClickGetFree()
    {
        if (IsFull())
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        long timeLeft = TimeLeft(Db.storage.ADS_COIN_MARK);
        var lstResource = new List<ResourceValue>();

        if (timeLeft <= 0)
        {
            AdsController.Instance.ShowRewardAds(RewardAdsPos.shop, () =>
            {
                var remote = GameAnalyticController.Instance.Remote();
                Db.storage.REWARD_SHOP_COIN_IAP_COUNT += 1;
                // gameObject.SetActive(Db.storage.REWARD_SHOP_COIN_IAP_COUNT < remote.RewardControl.rewardShopCoinAmountPerDay);
                Debug.Log("Getted");
                var coin = GameAnalyticController.Instance.Remote().RewardFreeCoin.CoinAmount;
                lstResource.Add(new ResourceIAP.ResourceValue()
                {
                    type = ResourceIAP.ResourceType.Coin,
                    value = coin
                });
                if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
                {
                    var reward = Db.storage.RewardData.DeepClone();
                    reward.AddCoinValue(coin);
                    Db.storage.RewardData = reward;



                    ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, () =>
                    {
                        MainMenuRecieveRewardsHelper.Instance.OnGetReward();

                    });

                }
                else
                {
                    var user = Db.storage.USER_INFO;
                    user.coin += coin;
                    Db.storage.USER_INFO = user;
                    EventDispatcher.Push(EventId.UpdateCoinUI
                   , coin);
                    ShopIAPController.Instance.ShowCompletedPurchasePopup(lstResource, () =>
                    {
                        EventDispatcher.Push(EventId.UpdateCoinUI
                  , coin);
                    });
                }

                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Coin", coin, "reward_ad", "ShopFreeCoin");

                Db.storage.ADS_COIN_MARK = TimeGetter.Instance.CurrentTime;
                Debug.Log($"Db.storage.ADS_COIN_MARK{Db.storage.ADS_COIN_MARK}");
                gobjNoti.gameObject.SetActive(false);
                txtAdsAmount.text = $"{Db.storage.ADS_COIN_AMOUNT}";
                Db.storage.REWARD_COIN_FREE_COUNT += 1;
                CountDownFreeCoin();
            }, null, null, "coin_free");






        }
        else
        {
            Debug.Log(timeLeft);
        }
    }

    public long TimeLeft(long markTime)
    {
        var currentTime = TimeGetter.Instance.CurrentTime;
        // Calculate time difference in ticks     second to milliseconds
        long elapsedTicks = GameAnalyticController.Instance.Remote().RewardFreeCoin.CoinTime * 1000 - (currentTime - markTime);

        return elapsedTicks;
    }

}
