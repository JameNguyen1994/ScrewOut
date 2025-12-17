using Cysharp.Threading.Tasks;
using PS.Ad;
using PS.IAP;
using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using UnityEngine;
using TMPro;
using Storage;
using ps.modules.journey;
using WeeklyQuest;
using UnityEngine.SocialPlatforms.Impl;
using ps.modules.leaderboard;

namespace MainMenuBar
{
    public class MainMenuBarController : Singleton<MainMenuBarController>
    {
        DBMainMenuBarController dBMainMenuBarController;

        [SerializeField] private List<ItemBar> lstItemBar;
        [SerializeField] private ItemBar prefabItembar;
        [SerializeField] private ItemBar currentItembar;
        [SerializeField] private MainMenuBarDataSO mainMenuBarDataSO;

        [SerializeField] private RectTransform rtfmHolder;
        [SerializeField] private int intBlockLayer;
        private float selectedScale = 1.3f;
        private float timeAnimation = 1.3f;
        private Vector2 normalIconSize;
        private Vector2 selectedIconSize;
        private int level = 1;
        private int defaultIndexItembar = 2;
        private int lastIndex = 1;

        [SerializeField] private TabController tabController;
        [SerializeField] private GameObject iconNoAds;

        [SerializeField] private ResourceDataSO shopNoAdsData1;
        [SerializeField] private ResourceDataSO shopNoAdsData2;

        public bool IsBlock { get => intBlockLayer == 0; }
        public int CurrentIndex { get => currentItembar.Index; }
        public TabController TabController { get => tabController; }

        public GameObject buttonPlayNormal;
        public GameObject buttonPlayHard;

        public TextMeshProUGUI buttonPlayNormalText;
        public TextMeshProUGUI buttonPlayHardText;

        public void AddBlockLayer()
        {
            intBlockLayer++;
        }
        public void RemoveBlockLayer()
        {
            intBlockLayer--;
        }
        private async void Start()
        {
            SetNoAds();

            //AudioController.Instance.ChangeMusic(SoundName.Music);
            AdsController.Instance.OnGoHome();
            UITopController.Instance.OnShowMainMenu();
            GetReference();
            Application.targetFrameRate = 120;
            GeneratDataFromSO();
            CalculatorSize();
            Init();

            currentItembar = lstItemBar[defaultIndexItembar];
            currentItembar.Select(0);
            lastIndex = 0;
            tabController.Init(defaultIndexItembar);
            IngameData.SHOP_PLACEMENT = shop_placement.Home;

            int level = Storage.Db.storage.USER_INFO.level;
            LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(level);

            buttonPlayNormal.SetActive(levelDifficulty != LevelDifficulty.Hard);
            buttonPlayHard.SetActive(levelDifficulty == LevelDifficulty.Hard);

            string btnPlay = "LEVEL " + level;
            buttonPlayNormalText.text = btnPlay;
            buttonPlayHardText.text = btnPlay;
        }

        public async UniTask SetNoAds()
        {
            iconNoAds.SetActive(false);
            CheckNoAds.Instance.CheckIsNoAds();
            await UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);
            var indexKey = GameAnalyticController.Instance.Remote().CbsNoAd;

            Debug.Log($"Check set noads {indexKey}");

            if (indexKey == 0)
            {
                iconNoAds.SetActive(false);

            }
            else
            {
                if (MainMenuService.IsUnlockNoADS())
                {
                    iconNoAds.SetActive(true);
                }
            }

        }
        private void GetReference()
        {
            dBMainMenuBarController = DBMainMenuBarController.Instance;
        }
        private void GeneratDataFromSO()
        {
            for (int i = 0; i < mainMenuBarDataSO.data.Count; i++)
            {
                var item = GameObject.Instantiate(prefabItembar, rtfmHolder);
                lstItemBar.Add(item);
            }

            this.selectedScale = mainMenuBarDataSO.selectedScale;
            this.timeAnimation = mainMenuBarDataSO.timeAnimation;
            this.normalIconSize = mainMenuBarDataSO.normalIconSize;
            this.selectedIconSize = mainMenuBarDataSO.selectedIconSize;
            this.defaultIndexItembar = mainMenuBarDataSO.defaultIndexItembar;
        }
        private void CalculatorSize()
        {
            float height = rtfmHolder.rect.height * 1.3f;
            float width = rtfmHolder.rect.width;
            Debug.Log($"height{height}");
            Debug.Log($"width{width}");
            float totalScale = lstItemBar.Count - 1 + selectedScale;
            float normalItemWidth = width / totalScale;
            float selectedItemWidth = width / totalScale * selectedScale;

            var normalItemSize = new Vector2(normalItemWidth, height);
            var selectedItemSize = new Vector2(selectedItemWidth, height);
            Debug.Log($"normalItemSize{normalItemSize}");
            Debug.Log($"selectedItemSize{selectedItemSize}");

            var normalIconPos = new Vector2(0, 0);
            var selectedIconPos = new Vector2(0, height / 5);




            for (int i = 0; i < lstItemBar.Count; i++)
            {
                lstItemBar[i].SetDefaultSize(timeAnimation, normalItemSize, selectedItemSize, normalIconPos, selectedIconPos, normalIconSize, selectedIconSize);
            }

        }
        public void Init()
        {
            DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.CheckUnlock(Db.storage.USER_INFO.level);
            for (int i = 0; i < lstItemBar.Count; i++)
            {
                var data = mainMenuBarDataSO.data[i];
                var dataDB = dBMainMenuBarController.DB_MAIN_MENU_ITEMS.lstDBBarItem[i];
                bool isLock = !dataDB.isUnlock;
                bool isNew = dataDB.isNew;
                lstItemBar[i].Init(i, isNew, isLock, data.name, dataDB.levelUnlock, data.spriteNormal, data.spriteSelected, data.spriteLock, ActionOnClickItem, OnFinishAnimation);
            }
        }
        public async UniTask CheckTutorial()
        {
            var lstIndexTutorial = DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.GetListIndexTutorial(Db.storage.USER_INFO.level);

            Debug.Log($"lstIndexTutorial.Count {lstIndexTutorial.Count}");
            for (int i = 0; i < lstIndexTutorial.Count; i++)
            {
                int index = lstIndexTutorial[i];
                if (index == 0 || index == 2)
                    continue;
                await TutorialUnlockTab.Instance.StartTutorial(lstItemBar[index].GetButton(), ImageHighLightType.Circle);
                await UniTask.Delay(200);

                switch (index)
                {
                    case 0:
                        //
                        break;
                    case 1:
                        // Journey
                        await JourneyController.Instance.ShowTutorial();
                        break;
                    case 2:
                        //TutorialUnlockTab.Instance.TestShowHoleEffect();
                        break;
                    case 3:
                        var manager = LeaderboardManager.Instance;
                        var popupTutorial = manager.GetController<PopupTutorialLeaderBoard>();
                        await popupTutorial.Show();
                        await popupTutorial.WaitToClose();
                        break;
                    case 4:
                        await WeeklyQuestManager.Instance.PopupWeeklyTutorial.Show();
                        await WeeklyQuestManager.Instance.PopupWeeklyTutorial.WaitToClose();

                        break;
                }
                dBMainMenuBarController.DB_MAIN_MENU_ITEMS.SetIndexCompleted(index);
                await UniTask.Delay(200);

            }
        }
        public void ActionOnClickItem(ItemBar itemBar)
        {
            if (intBlockLayer > 0)
            {
                Debug.Log("Block Layer");
                return;
            }
            if (currentItembar == itemBar)
                return;
            if (itemBar.IsLock)
            {
                Debug.Log("Lock");
                ToastMessage.Instance.ShowToast($"Unlock at level {itemBar.LevelUnlock}");
            }
            else
            {
                lastIndex = currentItembar.Index;
                int nextIndex = itemBar.Index;
                ItemBar.ChangeTab(nextIndex);
                tabController.OnGoToTab(nextIndex);
                currentItembar.UnSelect(timeAnimation);
                itemBar.Select(timeAnimation);
                currentItembar = itemBar;
                if (itemBar.IsNew)
                {
                    dBMainMenuBarController.DB_MAIN_MENU_ITEMS.lstDBBarItem[itemBar.Index].SetNew(false);
                }
                switch (nextIndex)
                {
                    case 0:
                        //UITopController.Instance.OnShowMainMenu();
                        break;
                    case 1:
                        //   UITopController.Instance.OnShowMainMenu();
                        break;
                    case 2:
                        //  UITopController.Instance.OnShowTabTask();
                        //  lstItemBar[2].SetNoti(false);

                        break;
                }
            }
        }
        public void OnFinishAnimation(ItemBar itemBar)
        {

        }
        public void GoToTabShop()
        {
            IngameData.SHOP_PLACEMENT = shop_placement.Home;
            if (ShopIAPController.Instance.Showing == false)
            {
                ActionOnClickItem(lstItemBar[0]);
            }
        }
        public void ShowPopupBeginner()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            AudioController.Instance.PlaySound(SoundName.Click);
            ShopIAPController.Instance.ShowBeginner();
        }
        public void ShowPopupNoAds()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            AudioController.Instance.PlaySound(SoundName.Click);
            ShopIAPController.Instance.ShowNoAds();
        }
        public async UniTask GoToTabHome()
        {

            // await ShopIAPController.Instance.Hide();

            await UniTask.WaitUntil(() => lstItemBar != null && lstItemBar.Count == 5);
            ActionOnClickItem(lstItemBar[2]);
        }
        public void ShowNotiWeeklyQuest()
        {
            lstItemBar[4].SetNoti(true);
        }

        public void OnClickClickySpin()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            AudioController.Instance.PlaySound(SoundName.Click);
            Spin.SpinController.Instance.DOShow();
        }
        public int GetBeforeTab()
        {
            return lastIndex;
        }
    }
}
