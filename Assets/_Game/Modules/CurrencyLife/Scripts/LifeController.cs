using Cysharp.Threading.Tasks;
using PS.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GameAnalyticsSDK;
using Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Life
{

    public class LifeController : Singleton<LifeController>
    {
        [SerializeField] private LifeUI lifeUI;
        [SerializeField] private int lifeAmount;
        [SerializeField] private PopupBuyLife popupBuyLife;
        [SerializeField] private bool inited = false;

        Coroutine countDownTimeRegen;
        Coroutine downTimeInfinityLife;

        public bool Inited { get => inited; }

        /*        private void Start()
                {
                    InitUI();
                }*/
        public async UniTask Init()
        {
            GetReference();
            await ResetFirstTimeOnline();
            UpdateInfo();
        }
        public void AddInfinityTime(long time)
        {
            Debug.Log($"AddInfinityTime: {time}");
            if (time <= 0) return;
            DBLifeController.Instance.LIFE_INFO.AddTimeInfinity(time);
            UpdateInfo();
        }
        private async UniTask ResetFirstTimeOnline()
        {
            bool isFireEventLifeInventory = false;
            await UniTask.WaitUntil(() => TimeGetter.Instance.IsGettedTime);
            var lifeInfor = DBLifeController.Instance.LIFE_INFO;
            if (DBLifeController.Instance.LIFE_INFO.isQuitGameWhenLose)
            {
                lifeInfor.isQuitGameWhenLose = false;
                if (lifeInfor.lifeAmount > 0 && !SerializationManager.HaveDataToReloadGamePlay())
                {
                    if (lifeInfor.lifeAmount == LifeConfig.MAX_LIFE)
                    {
                        DBLifeController.Instance.LIFE_INFO.MarkTime(TimeGetter.Instance.CurrentTime);
                        DBLifeController.Instance.LIFE_INFO.ResetTimeRegen();
                    }

                    isFireEventLifeInventory = true;
                    lifeInfor.lifeAmount -= 1;
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, "Life", 1, "life", "QuitToHome");
                    EditorLogger.Log("[ResetFirstTimeOnline] lifeInfor amount " + lifeInfor.lifeAmount);
                }
            }

            if (lifeInfor.markTick == 0)
            {
                DBLifeController.Instance.LIFE_INFO.MarkTime(TimeGetter.Instance.CurrentTime);
            }
            var timeOffline = (TimeGetter.Instance.CurrentTime - lifeInfor.markTick);
            lifeInfor.AddTimeInfinity(-timeOffline);

            if (timeOffline >= lifeInfor.timeRegen)
            {
                isFireEventLifeInventory = true;
                lifeInfor.AddLifeAmount(1);
                timeOffline -= lifeInfor.timeRegen;

                int heartRegen = (int)(timeOffline / LifeConfig.TIME_REGENT);
                timeOffline -= heartRegen * LifeConfig.TIME_REGENT;
                lifeInfor.AddLifeAmount(heartRegen);

                GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, "Life", heartRegen, "life", "Regent");

                var timeRegenLeft = lifeInfor.lifeAmount == LifeConfig.MAX_LIFE ? 0 : LifeConfig.TIME_REGENT - timeOffline;
                lifeInfor.timeRegen = timeRegenLeft;
            }
            DBLifeController.Instance.LIFE_INFO = lifeInfor;

            if (isFireEventLifeInventory)
            {
                int level = 0;
                float percentage = 0;
                if (SceneManager.GetActiveScene().name == "GamePlayNewControl")
                {
                    level = Db.storage.USER_INFO.level;
                    percentage = IngameData.TRACKING_UN_SCREW_COUNT * 1.0f / LevelController.Instance.Level.LstScrew.Count;
                }

                TrackingController.Instance.TrackingInventory(level, percentage);

            }

            inited = true;
        }
        public void UpdateInfo()
        {
            if (countDownTimeRegen != null)
            {
                StopCoroutine(countDownTimeRegen);
            }
            if (downTimeInfinityLife != null)
            {
                StopCoroutine(downTimeInfinityLife);
            }


            var lifeInfor = DBLifeController.Instance.LIFE_INFO;
            lifeAmount = lifeInfor.lifeAmount;
            bool isInfinity = lifeInfor.timeInfinity > 0;

            if (isInfinity)
            {
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(lifeInfor.timeInfinity);
                lifeUI.Init(isInfinity, false, lifeAmount, timeSpan);
                downTimeInfinityLife = StartCoroutine(CountDownTimeInfinity());
                return;
            }
            bool isFull = lifeAmount >= LifeConfig.MAX_LIFE;
            if (isFull)
            {
                lifeUI.Init(false, isFull, lifeAmount, new TimeSpan());
            }
            else
            {
                if (lifeInfor.timeRegen == 0)
                {
                    lifeInfor.ResetTimeRegen();
                }
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(lifeInfor.timeRegen);
                lifeUI.Init(isInfinity, false, lifeAmount, timeSpan);

                countDownTimeRegen = StartCoroutine(CountDownTimeRegen());
            }
        }
        private void GetReference()
        {
           // DBLifeController.Instance = DBLifeController.Instance;
        }
        IEnumerator CountDownTimeInfinity()
        {
            bool isStart = true;
            var lifeInfor = DBLifeController.Instance.LIFE_INFO;
            lifeUI.UpdateTextDetail(TimeSpan.FromMilliseconds(lifeInfor.timeInfinity));
            while (isStart)
            {
                yield return new WaitForSeconds(1);
                lifeInfor = DBLifeController.Instance.LIFE_INFO;

                long timeover = TimeOver(lifeInfor.markTick);
                lifeInfor.AddTimeInfinity(-timeover);
                //  Debug.Log($"CountDownTimeInfinity Time over {timeover}");
                lifeUI.UpdateTextDetail(TimeSpan.FromMilliseconds(lifeInfor.timeInfinity));
                lifeInfor.MarkTime(TimeGetter.Instance.CurrentTime);
                if (lifeInfor.timeInfinity <= 0)
                {
                    isStart = false;
                    lifeInfor.timeInfinity = 0;
                    bool isFull = lifeAmount >= LifeConfig.MAX_LIFE;
                    if (isFull)
                    {
                        lifeUI.Init(false, isFull, lifeAmount, new TimeSpan());
                    }
                    else
                    {
                        lifeInfor.ResetTimeRegen();
                        TimeSpan timeSpan = TimeSpan.FromMilliseconds(lifeInfor.timeRegen);
                        lifeUI.Init(false, isFull, lifeAmount, timeSpan);
                        countDownTimeRegen = StartCoroutine(CountDownTimeRegen());
                    }
                }

            }

        }
        IEnumerator CountDownTimeRegen()
        {
            bool isStart = true;
            var lifeInfor = DBLifeController.Instance.LIFE_INFO;
            lifeUI.UpdateTextDetail(TimeSpan.FromMilliseconds(lifeInfor.timeRegen));
            while (isStart)
            {
                yield return new WaitForSeconds(1);

                lifeInfor = DBLifeController.Instance.LIFE_INFO;
                var timeover = TimeOver(lifeInfor.markTick);
                Debug.Log($"CountDownTimeRegen Time over {timeover}");
                //Debug.Log($"CountDownTimeRegen Time over {timeover}");
                lifeInfor.AddTimeRegen(-timeover);
                TimeSpan timeSpan = TimeSpan.FromMilliseconds(lifeInfor.timeRegen);
                lifeUI.UpdateTextDetail(timeSpan);
                lifeInfor.MarkTime(TimeGetter.Instance.CurrentTime);
                if (lifeInfor.timeRegen <= 0)
                {
                    isStart = false;
                    if (lifeInfor.lifeAmount < LifeConfig.MAX_LIFE)
                    {
                        lifeInfor.AddLifeAmount(1);
                    }
                    UpdateInfo();
                }
            }
        }
        [ContextMenu("Test1MInfinity")]
        public void Test1M()
        {
            DBLifeController.Instance.LIFE_INFO.AddTimeInfinity(60 * 1000);
            UpdateInfo();
        }
        [ContextMenu("AddLife")]
        public void AddLife()
        {
            DBLifeController.Instance.LIFE_INFO.AddLifeAmount(1);
            UpdateInfo();
        }
        [ContextMenu("UseLife")]
        public void UseLife()
        {
            DBLifeController.Instance.LIFE_INFO.SetQuitGameWhenLose(false);
            if (DBLifeController.Instance.LIFE_INFO.timeInfinity <= 0)
            {
                if (DBLifeController.Instance.LIFE_INFO.lifeAmount > 0)
                    DBLifeController.Instance.LIFE_INFO.AddLifeAmount(-1);
                DBLifeController.Instance.LIFE_INFO.MarkTime(TimeGetter.Instance.CurrentTime);
                if (DBLifeController.Instance.LIFE_INFO.lifeAmount == LifeConfig.MAX_LIFE-1)
                {
                    DBLifeController.Instance.LIFE_INFO.ResetTimeRegen();
                }
                UpdateInfo();
            }
        }
        public void ShowPopupLife()
        {
            if (ShopIAPController.Instance.Showing) return;
            if (DBLifeController.Instance.LIFE_INFO.lifeAmount < LifeConfig.MAX_LIFE && DBLifeController.Instance.LIFE_INFO.timeInfinity <= 0)
            {
                popupBuyLife.Show();
            }
        }
        public long TimeOver(long markTime)
        {
            var currentTime = TimeGetter.Instance.CurrentTime;
            // Calculate time difference in ticks
            long elapsedTicks = currentTime - markTime;

            return elapsedTicks;
        }
        public async UniTask HidePopup()
        {
            popupBuyLife.Hide();
        }

        public void AddInfinityTimeWihoutUpdateUI(long time)
        {
            Debug.Log($"AddInfinityTime: {time}");
            if (time <= 0) return;
            DBLifeController.Instance.LIFE_INFO.AddTimeInfinity(time);
        }

        public void UpdateUI()
        {
            UpdateInfo();
        }

        public void StopCountDown()
        {
            if (countDownTimeRegen != null)
            {
                StopCoroutine(countDownTimeRegen);
            }

            if (downTimeInfinityLife != null)
            {
                StopCoroutine(downTimeInfinityLife);
            }
        }
    }
}
