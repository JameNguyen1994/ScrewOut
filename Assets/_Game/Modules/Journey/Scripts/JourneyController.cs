using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using Storage;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace ps.modules.journey
{
    public class JourneyController : Singleton<JourneyController>

    {
        [Header("References")]
        [SerializeField] private JourneyStory prfStory;
        [SerializeField] private Transform tfmStoryParent;
        [SerializeField] private List<JourneyStory> lstStory = new();
        [SerializeField] private Transform tfmCloud;
        [SerializeField] private JourneyDataSO journeyDataSO;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject gobjContent;
        [SerializeField] private GameObject gobjBtnTop;
        [SerializeField] private GameObject gobjBtnBottom;
        [SerializeField] private PopupJourneyTutorial popupJourneyTutorial;

        [Header("Performance Settings")]
        [SerializeField, Tooltip("Khoảng đệm hiển thị thêm (theo pixel) trên/dưới vùng nhìn thấy")]
        private float buffer = 300f;

        [SerializeField, Tooltip("Thời gian giữa các lần check hiển thị (giây)")]
        private float checkInterval = 0.1f;

        private RectTransform viewportRT;
        private RectTransform contentRT;

        [System.Serializable]
        private class StoryBounds
        {
            public JourneyStory story;
            public float top;
            public float bottom;
        }

        [SerializeField] private List<StoryBounds> storyBounds = new();
        [SerializeField] private bool needUpdate = false;
        private enum ScrollState { Bottom, Player, Top }
        private ScrollState currentState = ScrollState.Player;

        private float lastInteractTime;
        private const float autoHideDelay = 2f;
        private async UniTask Start()
        {
            gobjContent.gameObject
    .SetActive(true);
            await Init();
            GoToPlayer(0f).Forget();
            await UniTask.Delay(100);
            gobjContent.gameObject
                .SetActive(false);
        }

        private async UniTask Init()
        {
            int countStory = journeyDataSO.lstJourneyData.Count;
            for (int i = 0; i < countStory; i++)
            {
                JourneyStory storyInstance = Instantiate(prfStory, tfmStoryParent);
                lstStory.Add(storyInstance);
                storyInstance.transform.SetAsFirstSibling();
                await storyInstance.Init(i, journeyDataSO.lstJourneyData[i], Db.storage.USER_INFO.level);
            }

            // Setup cloud và tail
            lstStory[0].EnableTail();
            tfmCloud.transform.SetAsFirstSibling();

            viewportRT = scrollRect.viewport != null
                ? scrollRect.viewport
                : scrollRect.GetComponent<RectTransform>();
            contentRT = scrollRect.content;

            // Sự kiện scroll
            scrollRect.onValueChanged.AddListener(_ => needUpdate = true);

            // Đợi layout xong rồi tắt ContentSizeFitter
            await UniTask.DelayFrame(1);
            for (int i = 0; i < countStory; i++)
                lstStory[i].TurnOffContenSizeFitter();

            await UniTask.DelayFrame(2);

            // Force rebuild layout để lấy đúng anchoredPosition
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);

            // Cache vị trí top/bottom
            await CacheStoryBounds();

            // Lần đầu cập nhật
            UpdateStoryVisibility();

            // Vòng lặp kiểm tra định kỳ
            ThrottleUpdateLoop().Forget();
            AutoHideButtonsLoop().Forget();
        }
        // ==========================
        // SCROLL CONTROL
        // ==========================

        [Button]
        public void OnClickTop()
        {
            RegisterInteraction();
            if (currentState == ScrollState.Bottom)
                GoToPlayer();
            else
                GoToTop();
        }

        [Button]
        public void OnClickBottom()
        {
            RegisterInteraction();
            if (currentState == ScrollState.Top)
                GoToPlayer();
            else
                GoToBottom();
        }

        [Button]
        public void OnClickGoToPlayer()
        {
            RegisterInteraction();
            GoToPlayer();
        }

        private async UniTask GoToTop(float duration = 0.5f)
        {
            await scrollRect.DOVerticalNormalizedPos(1f, duration).ToUniTask();
            UpdateStoryVisibility();
            currentState = ScrollState.Top;
        }

        private async UniTask GoToBottom(float duration = 0.5f)
        {
            await scrollRect.DOVerticalNormalizedPos(0f, duration).ToUniTask();
            UpdateStoryVisibility();
            currentState = ScrollState.Bottom;
        }

        public async UniTask GoToPlayer(float duration = 0.5f)
        {
            var currentStory = GetCurrentStory();
            if (currentStory == null)
            {
                await GoToBottom(duration);
                return;
            }

            var index = currentStory.ID;
            var dataBound = storyBounds[index];
            var posYPlayer = dataBound.bottom + currentStory.GetCurrentSizeFill();
            // posYPlayer *= -1;
            var fill = posYPlayer / (contentRT.rect.height);
            Debug.Log($"GoToPlayer - Story ID: {index} - PosY: {posYPlayer}/{contentRT.rect.height} - Fill: {fill}");
            await scrollRect.DOVerticalNormalizedPos(fill, duration).ToUniTask();
            currentState = ScrollState.Player;
        }

        // ==========================
        // BUTTON VISIBILITY LOGIC
        // ==========================

        private async UniTaskVoid AutoHideButtonsLoop()
        {
            UpdateButtonVisibility(false);
          /*  while (this != null)
            {
                if (Time.time - lastInteractTime > autoHideDelay)
                {
                    UpdateButtonVisibility(false);
                }
                await UniTask.Delay(250);
            }*/
        }

        private void RegisterInteraction()
        {
            lastInteractTime = Time.time;
            UpdateButtonVisibility(true);
        }

        public void UpdateButtonVisibility(bool forceShow)
        {
            if (gobjBtnTop == null || gobjBtnBottom == null) return;

            // Nếu forceShow = false → tức là AutoHide đang muốn ẩn tất cả
            if (!forceShow)
            {
                gobjBtnTop.SetActive(false);
                gobjBtnBottom.SetActive(false);
                return;
            }

            float normalized = scrollRect.verticalNormalizedPosition;

            // 1. Ở Top → ẩn nút Top
            bool isAtTop = normalized >= 0.999f;

            // 2. Ở Bottom → ẩn nút Bottom
            bool isAtBottom = normalized <= 0.001f;

            gobjBtnTop.SetActive(!isAtTop);       // ẩn khi đang ở đầu danh sách
            gobjBtnBottom.SetActive(!isAtBottom); // ẩn khi đang ở cuối danh sách
        }

        private async UniTask CacheStoryBounds()
        {
            storyBounds.Clear();

            foreach (var story in lstStory)
            {
                if (story == null) continue;

                RectTransform rt = story.GetRectTransform();
                float height = rt.rect.height;

                // Y của item trong content (local space)
                float top = rt.anchoredPosition.y + contentRT.rect.height + height;
                float bottom = top - height;

                storyBounds.Add(new StoryBounds
                {
                    story = story,
                    top = top,
                    bottom = bottom
                });
            }

            await UniTask.CompletedTask;
        }

        private async UniTaskVoid ThrottleUpdateLoop()
        {
            while (this != null)
            {
                if (needUpdate)
                {
                    needUpdate = false;
                    UpdateStoryVisibility();
                }
                await UniTask.Delay((int)(checkInterval * 1000));
            }
        }

        private void UpdateStoryVisibility()
        {
            if (viewportRT == null || contentRT == null) return;

            float contentY = contentRT.anchoredPosition.y;
            float viewHeight = viewportRT.rect.height;

            // Vùng hiển thị (local space)
            float viewBot = contentY;
            float viewTop = contentY - viewHeight;

            viewBot *= -1;
            viewTop *= -1;

            viewTop += buffer;
            viewBot -= buffer;

            foreach (var sb in storyBounds)
            {
                bool isVisibleBot = sb.bottom <= viewBot && sb.top >= viewBot;
                bool isVisibleTop = sb.bottom <= viewTop && sb.top >= viewTop;
                bool isIn = sb.top >= viewBot && sb.bottom <= viewTop;
                bool isOut = sb.top >= viewTop && sb.bottom <= viewBot;
                bool isVisible = isVisibleBot || isVisibleTop || isIn || isOut;
                if (isVisible)
                    sb.story.ShowStory();
                else
                    sb.story.HideStory();
                //  Debug.Log($"View Bot: {viewBot} {sb.bottom}, View Top: {viewTop} {sb.top} {isVisible}");
            }
            UpdateButtonVisibility(true);
        }


        public async UniTask OnGoTop(float duration = 0.4f)
        {
            await scrollRect.DOVerticalNormalizedPos(1f, duration).ToUniTask();
            UpdateStoryVisibility();
        }

        public async UniTask OnGoBottom(float duration = 0.4f)
        {
            await scrollRect.DOVerticalNormalizedPos(0f, duration).ToUniTask();
            UpdateStoryVisibility();
        }

        public async UniTask OnGoPlayer(float duration = 0.4f)
        {
            var currentStory = GetCurrentStory();
            var index = currentStory != null ? currentStory.ID : 0;

            var dataBound = storyBounds[index];
            var posYPlayer = dataBound.bottom + currentStory.GetCurrentSizeFill();
            posYPlayer *= -1;
            var fill = posYPlayer / (contentRT.rect.height);
            await scrollRect.DOVerticalNormalizedPos(fill, duration).ToUniTask();
            Debug.Log($"OnGoPlayer: ID {index} - PosYPlayer: {posYPlayer} ");
            //UpdateStoryVisibility();
        }
        public void OnChangeLevel()
        {
            foreach (var sb in lstStory)
            {
                sb.OnChangeLevel(Db.storage.USER_INFO.level);
            }
        }

        private JourneyStory GetCurrentStory()
        {
            var level = Db.storage.USER_INFO.level;
            for (int i = 0; i < lstStory.Count; i++)
            {
                var data = lstStory[i].JourneyData;
                if (level >= data.levelStart &&
                    level <= data.levelEnd)
                {
                    return lstStory[i];
                }
            }
            return null;
        }
        public List<ResourceValueJourney> GetRewardByLevel(int level)
        {
            for (int i = 0; i < journeyDataSO.lstJourneyData.Count; i++)
            {
                var journeyData = journeyDataSO.lstJourneyData[i];
                for (int j = 0; j < journeyData.lstMarkLevel.Count; j++)
                {
                    var levelData = journeyData.lstMarkLevel[j];
                    if (levelData.level == level)
                    {
                        return levelData.lstReward;
                    }
                }
            }
            return null;
        }
        public JourneyData GetBackgroundByLevel(int level)
        {
            for (int i = 0; i < journeyDataSO.lstJourneyData.Count; i++)
            {
                var journeyData = journeyDataSO.lstJourneyData[i];
                if (level == journeyData.levelStart)
                {
                    return journeyData;
                }
            }
            return null;
        }
        public async UniTask ShowTutorial()
        {
            await popupJourneyTutorial.Show();
            await popupJourneyTutorial.WaitToClose();
        }
    }
}
