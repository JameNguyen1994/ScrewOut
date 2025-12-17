using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WeeklyQuest;
using TMPro;

namespace Life
{
    public class PopupBuyLife : MonoBehaviour
    {
        [SerializeField] private GameObject gobjContent;
        [SerializeField] private Image imgFade;

        [SerializeField] private GameObject gobjButtonBuyDiamond;
        [SerializeField] private GameObject gobjButtonBuyAds;
        [SerializeField] private Text txtHeartAmount;
        [SerializeField] private TextMeshProUGUI txtCoinNeed;


        public void Show()
        {
            UITopController.Instance.OnShowBuyLife();
            if (PopupController.Instance != null)
                PopupController.Instance.PopupCount++;
            int lifeNeed = 5 - DBLifeController.Instance.LIFE_INFO.lifeAmount;
            lifeNeed = 1;
            txtHeartAmount.text = $"+{lifeNeed}";
            txtCoinNeed.text = $"{lifeNeed * GetCost()}";
            imgFade.gameObject.SetActive(true);
            gobjContent.SetActive(true);
            GetRemoteToShow();
        }
        private int GetCost()
        {
            var remote = GameAnalyticController.Instance.Remote();
            var costInGame = remote.CostInGame;
            return costInGame.coinLife;
        }
        public void Hide()
        {
            AudioController.Instance.PlaySound(SoundName.Click);

            if (PopupController.Instance != null)
                PopupController.Instance.PopupCount--;

            imgFade.gameObject.SetActive(false);
            gobjContent.SetActive(false);

            //if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
                UITopController.Instance.OnShowMainMenu();
        }
        public void GetRemoteToShow()
        {
            bool showButtonDiamond = true;
            gobjButtonBuyDiamond.SetActive(showButtonDiamond);
            var remote = GameAnalyticController.Instance.Remote();
            Debug.Log($"[Remote] Life {remote.RewardControl.rewardLifeAmountPerDay} - ADS {Db.storage.REWARD_LIFE_COUNT}");
            gobjButtonBuyAds.SetActive(remote.RewardControl.rewardLifeAmountPerDay > Db.storage.REWARD_LIFE_COUNT);
        }
        public void OnClickBuyDiamond()
        {
            AudioController.Instance.PlaySound(SoundName.Click);

            int lifeNeed = 5 - DBLifeController.Instance.LIFE_INFO.lifeAmount;
            int coinNeed = 1 * GetCost();
            var user = Db.storage.USER_INFO;
            var dataCoin = user.coin;
            if (dataCoin >= coinNeed)
            {
                DBLifeController.Instance.LIFE_INFO.MarkTime(TimeGetter.Instance.CurrentTime);
                DBLifeController.Instance.LIFE_INFO.AddLifeAmount(lifeNeed);
                user.coin -= coinNeed;
                SingularSDK.Event("COIN_SPEND");

                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.UseCoins, coinNeed);

                Db.storage.USER_INFO = user;
                EventDispatcher.Push(EventId.UpdateCoinUI,coinNeed);
                LifeController.Instance.UpdateInfo();
                /*    if (DBController.Instance.QUESTION_LEVEL_INFO.IS_LOSE)
                    {
                        imgFade.gameObject.SetActive(false);
                        gobjContent.SetActive(false);
                        GameplayController.Instance.Replay();
                    }*/

                AudioController.Instance.PlaySound(SoundName.Buy_Heart);
                
                int level = 0;
                float percentage = 0;
                if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
                {
                    level = Db.storage.USER_INFO.level;
                    percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
                }
            
                TrackingController.Instance.TrackingInventory(level, percentage);
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Life", lifeNeed, "coin", "PopupBuyLife");
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Coin", coinNeed, "life", "PopupBuyLife");
                
                Hide();
            }
            else
            {
                IngameData.SHOP_PLACEMENT = shop_placement.Heart;

                UITopController.Instance.OnClickCoinBuyHeart();
            }
        }
        public void OnClickBuyAds()
        {
            AudioController.Instance.PlaySound(SoundName.Click);

            AdsController.Instance.ShowRewardAds(RewardAdsPos.NONE, () =>
            {
                DBLifeController.Instance.LIFE_INFO.MarkTime(TimeGetter.Instance.CurrentTime);
                DBLifeController.Instance.LIFE_INFO.AddLifeAmount(1);
                LifeController.Instance.UpdateInfo();
                /* if (DBController.Instance.QUESTION_LEVEL_INFO.IS_LOSE)
                 {
                     imgFade.gameObject.SetActive(false);
                     gobjContent.SetActive(false);
                     GameplayController.Instance.Replay();
                 }*/
                AudioController.Instance.PlaySound(SoundName.Buy_Heart);
                
                int level = 0;
                float percentage = 0;
                if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
                {
                    level = Db.storage.USER_INFO.level;
                    percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
                }
            
                TrackingController.Instance.TrackingInventory(level, percentage);
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Life",1, "reward_ad", "PopupBuyLife");
                
                Hide();

                Db.storage.REWARD_LIFE_COUNT += 1;

            },null, null,"buy_life");
        }
    }
}
