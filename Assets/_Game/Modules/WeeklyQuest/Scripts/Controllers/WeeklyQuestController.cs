using Cysharp.Threading.Tasks;
using MainMenuBar;
using NUnit.Framework;
using Storage;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Storage.LocalDb;


namespace WeeklyQuest
{
    public class WeeklyQuestController : MonoBehaviour
    {
        [SerializeField] private RectTransform giftHolder;
        [SerializeField] private RectTransform questHolder;
        [SerializeField] private List<ItemGift> lstItemGift;
        [SerializeField] private List<ItemQuest> lstItemQuest;

        [SerializeField] private Transform tfmTargetPoint;
        [SerializeField] private Transform tfmTargetGift;


        [SerializeField] private TotalQuestProcess totalQuestProcess;
        [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
        [SerializeField] private bool isWaitingCheckGiftOpen = false;
        [SerializeField] private bool inited = false;

        public bool IsWaitingCheckGiftOpen { get => isWaitingCheckGiftOpen; }

        private async UniTask Start()
        {
            Init();
        }
        public async UniTask Init()
        {
            await UniTask.WaitUntil(() => TimeHelperController.Instance.IsInitedTime);
            inited = false;
            Debug.Log("WeeklyQuestController Start");
            await CheckAndInitDataInDB();
            InitGifts();
            InitQuests();
            InitProcessBar();

            totalQuestProcess.SetAction(OnChangeProcessPoint);
            inited = true;
        }
        public async UniTask CheckAndInitDataInDB()
        {
            await UniTask.WaitUntil(() => Db.storage != null);
            await UniTask.WaitUntil(() => Db.storage.Inited);
            await UniTask.WaitUntil(() => WeeklyQuestManager.Instance.WeeklyDataHelper != null);
            var data = WeeklyQuestManager.Instance.WeeklyDataHelper.GetWeeklyData();
            data = (WeeklyData)data.Clone();
            Debug.Log("WeeklyQuestDataDB isLoading....");
            Db.storage.LoadWeeklyQuestData(data);


            Debug.Log($"{Db.storage.WeeklyQuestData.ToString()}");

            if (Db.storage.WeeklyQuestData != null)
            {
                //Check Gift data
                int currentPoint = Db.storage.WeeklyQuestData.currentPoint;
                for (int i = 0; i < Db.storage.WeeklyQuestData.gifts.Count; i++)
                {
                    var gift = Db.storage.WeeklyQuestData.gifts[i];
                    Debug.Log($"Checking gift {i} with require points: {gift.requirePoints}/{currentPoint}, isReceived: {gift.isReceived}");
                    if (!gift.isReceived && gift.requirePoints <= currentPoint)
                    {
                        Debug.Log($"Gift {i} with require points {gift.requirePoints} is available for reward. Current points: {currentPoint}");
                        // Get Reward
                        gift.isReceived = true;
                    }
                }


                // Check Quest Data
                //  UpdateQuestNoti();
                var quests = Db.storage.WeeklyQuestData.quests;
                // SortTask
                bool isAllQuestsComplete = true;
                for (int i = 0; i < quests.Count; i++)
                {
                    if (quests[i].isComplete == false)
                    {
                        isAllQuestsComplete = false;
                        break;
                    }
                }
                if (!isAllQuestsComplete)
                {
                    Debug.Log("Sorting quests by progress percentage (currentValue / targetValue) in descending order.");
                    Db.storage.WeeklyQuestData.quests.Sort((a, b) => (b.currentValue / (float)b.targetValue).CompareTo((a.currentValue / (float)a.targetValue)));
                    var lstQuestComplete = Db.storage.WeeklyQuestData.quests.FindAll(q => q.isComplete == true);
                    quests.RemoveAll(q => q.isComplete == true);
                    quests.AddRange(lstQuestComplete);
                }



                Db.storage.WeeklyQuestData.Save(); // Save the updated data
            }

        }

        public void UpdateQuestNoti()
        {
            if (Db.storage.WeeklyQuestData.isFirstTimeInWeek)
            {
                MainMenuBarController.Instance.ShowNotiWeeklyQuest();
                return;
            }
            var quests = Db.storage.WeeklyQuestData.quests;

            for (int i = 0; i < quests.Count; i++)
            {
                var quest = quests[i];
                Debug.Log($"Checking quest {i} with type: {quest.questType}, current value: {quest.currentValue}, target value: {quest.targetValue}");
                if (quest.isComplete == false && quest.currentValue >= quest.targetValue)
                {
                    Debug.Log($"Checking quest true {i} with type: {quest.questType}, current value: {quest.currentValue}, target value: {quest.targetValue}");

                    MainMenuBarController.Instance.ShowNotiWeeklyQuest();
                    return;
                }
            }
        }
        public async UniTask<QuestDataDB> GetClosestToCompletionQuestAsync()
        {
            await UniTask.WaitUntil(() => inited);
            var quests = Db.storage.WeeklyQuestData.quests;
            return quests[0];
        }
        public void InitGifts()
        {
            var weeklyQuestDataDB = Db.storage.WeeklyQuestData;
            if (weeklyQuestDataDB == null)
            {
                Debug.LogError("WeeklyQuestDataDB is null. Please initialize it first.");
                return;
            }
            lstItemGift = new List<ItemGift>();
            var offset = -50;
            for (int i = 0; i < weeklyQuestDataDB.gifts.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        offset = -50;
                        break;
                    case 1:
                        offset = 0;
                        break;
                    case 2:
                        offset = -25;
                        break;
                    default:
                        offset = 0;
                        break;
                }
                var gift = weeklyQuestDataDB.gifts[i];
                {
                    Debug.Log($"Initializing gift {i} with require points: {gift.requirePoints}");
                    var imageGift = WeeklyQuestManager.Instance.WeeklyDataHelper.GetImageGift(i);

                    var itemGift = WeeklyQuestManager.Instance.ObjectPoolWeekly.GetItemGift();
                    itemGift.SetData(imageGift, gift, tfmTargetGift);

                    itemGift.SetParent(giftHolder, weeklyQuestDataDB.maxPoint, offset);
                    lstItemGift.Add(itemGift);
                }
            }
        }
        public void InitProcessBar()
        {
            var weeklyQuestDataDB = Db.storage.WeeklyQuestData;
            if (weeklyQuestDataDB == null)
            {
                Debug.LogError("WeeklyQuestDataDB is null. Please initialize it first.");
                return;
            }
            totalQuestProcess.InitBar(weeklyQuestDataDB.currentPoint, weeklyQuestDataDB.maxPoint);
        }
        public void InitQuests()
        {
            var weeklyQuestDataDB = Db.storage.WeeklyQuestData;
            if (weeklyQuestDataDB == null)
            {
                Debug.LogError("WeeklyQuestDataDB is null. Please initialize it first.");
                return;
            }
            lstItemQuest = new List<ItemQuest>();
            for (int i = 0; i < weeklyQuestDataDB.quests.Count; i++)
            {
                var quest = weeklyQuestDataDB.quests[i];
                {
                    Debug.Log($"Initializing gift {i} with require points: {quest.questType}");

                    var itemQuest = WeeklyQuestManager.Instance.ObjectPoolWeekly.GetItemQuest();
                    itemQuest.SetData(quest);
                    itemQuest.SetParrent(questHolder);
                    itemQuest.SetTargetTransfrom(tfmTargetPoint);
                    lstItemQuest.Add(itemQuest);
                }
            }
        }
        public async UniTask OnShow()
        {

            Debug.Log("Check to show weekly quest tutorial 3");
            MainMenuBarController.Instance.AddBlockLayer();
            for (int i = 0; i < lstItemQuest.Count; i++)
            {
                var itemQuest = lstItemQuest[i];
                if (itemQuest != null && !itemQuest.IsCompleted && itemQuest.HasUpdate())
                {
                    await itemQuest.SetDataAsync(() =>
                     {
                         totalQuestProcess.Addcount(itemQuest.Point);
                     });
                    await UniTask.Delay(100);
                    await UniTask.WaitUntil(() => isWaitingCheckGiftOpen == false);
                }

            }
            MainMenuBarController.Instance.RemoveBlockLayer();
            Debug.Log("Check to show weekly quest tutorial 4");

        }
        public void OnClickTestShow()
        {
            Debug.Log("OnClickTestShow");
            OnShow();
        }
        private void OnChangeProcessPoint(int point)
        {
            OnChangeProcessPointAsync(point).Forget();
        }
        public async UniTask OnChangeProcessPointAsync(int point)
        {
            await UniTask.WaitUntil(() => isWaitingCheckGiftOpen == false);
            isWaitingCheckGiftOpen = true;
            for (int i = 0; i < lstItemGift.Count; i++)
            {

                await lstItemGift[i].CheckOpenChest(point);

            }
            isWaitingCheckGiftOpen = false;
        }
        public async UniTask CheckToShowTutorial(UnityAction actionAfterShow)
        {
            Debug.Log("Check to show weekly quest tutorial");

            if (Db.storage.WeeklyQuestData.isFirstTimeInWeek)
            {
                Debug.Log("Check to show weekly quest tutorial 2");

                await WeeklyQuestManager.Instance.PopupWeeklyTutorial.Show();
                var weeklyQuestData = Db.storage.WeeklyQuestData;
                weeklyQuestData.isFirstTimeInWeek = false; // Set the flag to false after showing the tutorial
                weeklyQuestData.Save(); // Save the updated data
                await WeeklyQuestManager.Instance.PopupWeeklyTutorial.WaitToClose();

                actionAfterShow?.Invoke();
            }
            else
            {
                Debug.Log("Weekly quest tutorial already shown this week, skipping.");
                actionAfterShow?.Invoke();
            }
        }
        public void DestroyOldItem()
        {
            for (int i = 0; i < lstItemGift.Count; i++)
            {
                Destroy(lstItemGift[i].gameObject);
            }
            lstItemGift.Clear();
            for (int i = 0; i < lstItemQuest.Count; i++)
            {
               Destroy(lstItemQuest[i].gameObject);
            }
            lstItemQuest.Clear();


        }
    }
}