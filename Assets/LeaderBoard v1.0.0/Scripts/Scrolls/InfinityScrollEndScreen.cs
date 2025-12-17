using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    public class InfinityScrollEndScreen : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform root;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private UserItem itemPrefab;
        [SerializeField] private UserItem itemPlayer;

        [SerializeField] private UserItem userItemFly;
        [SerializeField] private Transform tfmHolderUserItemFly;

        [Header("Data")]
        [SerializeField] private UserData playerData;
        [SerializeField] private List<UserData> lstUserData;
        private List<UserData> displayList = new();

        [Header("Settings")]
        [SerializeField] private int visibleCount = 10;
        [SerializeField] private int totalCount = 100;
        [SerializeField][Min(1)] private int initialTopId = 1;

        [Header("Bound Settings")]
        [SerializeField] private int minIndex = 0;
        [SerializeField] private int startIndex = 0;
        [SerializeField] private int maxIndex = 99;
        [SerializeField] private int offsetShow = 1;



        private float itemHeight;
        [SerializeField] private int topIndex;
        private LinkedList<UserItem> items = new();
        private CancellationTokenSource cts;
        [SerializeField] private int playerIndex = -1;
        public ScrollRect ScrollRect => scrollRect;
        public RectTransform Content => content;
        public int TotalCount => totalCount;

        #region ===== PUBLIC =====

        public void Show()
        {
            root.gameObject.SetActive(true);
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }

        public void Init(List<UserData> userDatas, UserData player, int startIndex, int total, int maxShow, int playerIndex)
        {
            lstUserData = userDatas ?? new List<UserData>();
            playerData = player;
            this.playerIndex = playerIndex;
            // clone danh sách gốc
            displayList = new List<UserData>(lstUserData);
            //  playerIndex = InsertPlayerTemp(displayList, player);

            totalCount = displayList.Count;
            visibleCount = Mathf.Clamp(visibleCount, 1, totalCount);
            minIndex = 0;
            this.startIndex = startIndex;
            maxIndex = Mathf.Clamp(startIndex + maxShow - 1, 0, totalCount - 1);

            itemHeight = ((RectTransform)itemPrefab.transform).sizeDelta.y;
            CreateInitialItems();

            int topId = Mathf.Clamp(initialTopId, minIndex + 1, maxIndex + 1);
            InitAtTopId(topId);

            scrollRect.onValueChanged.RemoveAllListeners();
            scrollRect.onValueChanged.AddListener(OnScroll);

            Debug.Log($"[InfinityScroll] Init done. Player #{playerIndex + 1}, Total {totalCount}");
        }
        public async UniTask GoToPlayerAsync()
        {
            int targerIndex = playerIndex;
            /*            if (playerIndex < 4)
                            return;*/
            if (playerIndex >= totalCount)
                targerIndex = totalCount - 1;
            Debug.Log($"Go to player at id: {targerIndex}");
            await GotoUserAsync(targerIndex, 1f, true);
        }
        public async UniTask GoToPlayer()
        {
            int targerIndex = playerIndex;
            /*            if (playerIndex < 4)
                            return;*/
            if (playerIndex >= totalCount)
                targerIndex = totalCount - 1;
            Debug.Log($"Go to player at id: {targerIndex}");
            SetTopIndexImmediate(targerIndex - offsetShow);
        }
        [Button]
        public async UniTask GotoUserAsync(int index, float duration = 1f, bool center = false)
        {
            if (center)
                index -= offsetShow;

            index = Mathf.Clamp(index, 0, totalCount - 1);

            cts?.Cancel();
            cts = new CancellationTokenSource();

            float targetY = index * itemHeight;

            float maxY = Mathf.Max(0, content.sizeDelta.y - scrollRect.viewport.rect.height);
            targetY = Mathf.Clamp(targetY, 0, maxY);

            Vector2 startPos = content.anchoredPosition;
            Vector2 endPos = new(startPos.x, targetY);

            float t = 0;
            bool oldInertia = scrollRect.inertia;
            //scrollRect.inertia = false; 

            try
            {
                while (t < 1f)
                {
                    t += Time.deltaTime / duration;
                    var oldAnchoredPos = content.anchoredPosition;
                    content.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
                    var offset = content.anchoredPosition - oldAnchoredPos;
                    //OnScroll(offset);
                    await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
                }
                content.anchoredPosition = endPos;
            }
            catch (System.OperationCanceledException) { }
            finally
            {
                scrollRect.inertia = oldInertia;
                //SetTopIndexImmediate(id);
            }
        }
        public UserItem GetPlayerItem()
        {
            return itemPlayer;
        }
        #endregion

        #region ===== CORE =====

        private int InsertPlayerTemp(List<UserData> list, UserData player, bool insert = true)
        {
            if (player == null || list == null || list.Count == 0)
            {
                Debug.LogError("InsertPlayerTemp: Invalid player or list");
                return -1;
            }

            int insertIndex = list.FindIndex(u => u.points < player.points);

            if (insert)
            {

                if (insertIndex < 0)
                {
                    list.Add(player);
                    insertIndex = list.Count - 1;
                }
                else
                {
                    list.Insert(insertIndex, player);
                }
            } else
            {
                if (insertIndex < 0)
                {
                    insertIndex = list.Count;
                }
            }
                return insertIndex;
        }

        private void CreateInitialItems()
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalCount * itemHeight);
            items.Clear();

            for (int i = 0; i < visibleCount + 4; i++)
            {
                var item = Instantiate(itemPrefab, content);
                items.AddLast(item);

                var rt = (RectTransform)item.transform;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 1f);
                rt.anchoredPosition = new Vector2(0, -i * itemHeight);
            }
        }

        public void InitAtTopId(int idOneBased)
        {
            SetTopIndexImmediate(idOneBased - 1);
        }

        private void SetTopIndexImmediate(int newTopIndex)
        {
            newTopIndex = Mathf.Clamp(newTopIndex, minIndex, Mathf.Max(0, totalCount - 2));
            float targetY = newTopIndex * itemHeight;
            float maxY = Mathf.Max(0, content.sizeDelta.y - scrollRect.viewport.rect.height);
            targetY = Mathf.Clamp(targetY, 0, maxY);
            content.anchoredPosition = new Vector2(content.anchoredPosition.x, targetY);

            int i = 0;
            foreach (var item in items)
            {
                int dataIndex = newTopIndex + i;
                var rt = (RectTransform)item.transform;
                rt.anchoredPosition = new Vector2(0, -dataIndex * itemHeight);

                if (dataIndex < totalCount)
                {
                    if (!item.gameObject.activeSelf) item.gameObject.SetActive(true);
                    UpdateItem(item, dataIndex);
                }
                // else item.gameObject.SetActive(false);

                i++;
            }

            topIndex = newTopIndex;
            OnScroll(Vector2.zero);
        }

        private void OnScroll(Vector2 _)
        {
            float scrollY = content.anchoredPosition.y;
            int newTopIndex = Mathf.FloorToInt(scrollY / itemHeight);
            newTopIndex = Mathf.Clamp(newTopIndex, minIndex, maxIndex - visibleCount);

            if (newTopIndex == topIndex) return;

            int diff = newTopIndex - topIndex;
            if (Mathf.Abs(diff) >= items.Count)
            {
                SetTopIndexImmediate(newTopIndex);
                return;
            }

            if (diff > 0)
            {
                for (int i = 0; i < diff; i++)
                {
                    var first = items.First.Value;
                    items.RemoveFirst();
                    items.AddLast(first);

                    int newDataIndex = topIndex + visibleCount + i + 1;
                    var rt = (RectTransform)first.transform;
                    rt.anchoredPosition = new Vector2(0, -newDataIndex * itemHeight);

                    if (newDataIndex <= maxIndex)
                        UpdateItem(first, newDataIndex);
                    // else first.gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < -diff; i++)
                {
                    var last = items.Last.Value;
                    items.RemoveLast();
                    items.AddFirst(last);

                    int newDataIndex = newTopIndex + i;
                    var rt = (RectTransform)last.transform;
                    rt.anchoredPosition = new Vector2(0, -newDataIndex * itemHeight);

                    if (newDataIndex >= minIndex)
                        UpdateItem(last, newDataIndex);
                    // else last.gameObject.SetActive(false);
                }
            }

            topIndex = newTopIndex;
        }

        private void UpdateItem(UserItem item, int index)
        {
            if (index < 0 || index >= displayList.Count) return;

            var data = displayList[index];
            bool isPlayer = playerData != null && data == playerData;
            item.SetData(index + startIndex, data, isPlayer);
            if (isPlayer)
                itemPlayer = item;
        }

        #endregion

        [Button]
        private void ClonePlayerItem()
        {
            userItemFly = Instantiate(itemPlayer, tfmHolderUserItemFly, true);
            userItemFly.GetRectTransform().localScale = Vector3.one;
        }
        [SerializeField] private int offset = 0;
        [Button]
        public void SetTop()
        {
            SetTopIndexImmediate(playerIndex - offset);
        }
        public async UniTask CloneAndScaleUp()
        {
            if (userItemFly == null)
                ClonePlayerItem();
            else
            {
                Destroy(userItemFly.gameObject);
                ClonePlayerItem();
            }
            userItemFly.gameObject.SetActive(true);
            var rect = userItemFly.GetRectTransform();
            await rect.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.Linear);
        }
        [Button]
        public async UniTask FlyToTop(float duration = 1, int point = 3)
        {
            playerData.points += point;
            int newIndex = InsertPlayerTemp(displayList, playerData, false);
            Debug.Log($"Fly to top: player new id {newIndex}, old id {playerIndex}. {displayList.Count}");
            if (newIndex == playerIndex + 1)
            {
                Debug.Log($"Player id not change; new Point {playerData.points}");
                var timeAdd = 0.5f;
                var sequence = DOTween.Sequence();

                itemPlayer.UI.DOPlayPoint(playerIndex, playerData.points, 0);


                await userItemFly.UI.DOPlayPoint(playerIndex, playerData.points, timeAdd / 2);
                await userItemFly.GetRectTransform().DOScale(Vector3.one * 1f, timeAdd / 2).SetEase(Ease.OutBack);

                return;
            }

            var offset = newIndex - playerIndex - 1;
            if (offset < 5)
            {
                duration /= 2;
            }
            var rect = userItemFly.GetRectTransform();
            displayList.Remove(playerData);


            var timeScale = 0.2f;
            var scaleDownTask = itemPlayer.GetRectTransform().DOScaleY(0, timeScale).SetEase(Ease.InBack);
            await MoveUpItemBellowPlayer(playerIndex, timeScale);
            await scaleDownTask;
            itemPlayer.GetRectTransform().DOScaleY(1, 0).SetEase(Ease.InBack);

            // return;
            SetTopIndexImmediate(playerIndex - offsetShow);
            //return;
            // ResetItemBellowPlayer(playerIndex);
            playerIndex = newIndex;
            userItemFly.UI.DOPlayPoint(newIndex, playerData.points, duration);
            await GotoUserAsync(newIndex, duration, true);
            //SetTopIndexImmediate(playerIndex - offsetShow);
            rect.DOScale(Vector3.one, 0.3f).SetEase(Ease.Linear);
            await MoveDownItemBellowPlayer(newIndex, timeScale);

            InsertPlayerTemp(displayList, playerData, true);
            await UniTask.Delay(100);
            SetTopIndexImmediate(playerIndex - offsetShow);

            var userItem = GetUserWithIndex(newIndex);
            userItem.gameObject.SetActive(false);
            Debug.Log($"User item at new id {newIndex}: {userItem}, pos {userItem.GetRectTransform().position} - fly pos {userItemFly.GetRectTransform().position}");
           // return;
          /*  if (userItem.GetRectTransform().position + new Vector3(0,itemHeight,0) != userItemFly.GetRectTransform().position)
            {
                Debug.Log($"Fix position {newIndex}) {userItem.GetRectTransform().position} - {userItemFly.GetRectTransform().position} {itemHeight}");
                await userItemFly.GetRectTransform().DOMoveY(userItem.GetRectTransform().position.y+ itemHeight , 0.3f);
            }*/
            if (userItem.GetRectTransform().position  != userItemFly.GetRectTransform().position)
            {
                Debug.Log($"Fix position {newIndex}) {userItem.GetRectTransform().position} - {userItemFly.GetRectTransform().position} {itemHeight}");
                await userItemFly.GetRectTransform().DOMoveY(userItem.GetRectTransform().position.y, 0.3f);
            }
            userItem.gameObject.SetActive(true);

             SetTopIndexImmediate(playerIndex - offsetShow);

            userItemFly.gameObject.SetActive(false);

        }
        private UserItem GetUserWithIndex(int index)
        {
            Debug.Log($"Get user with id: {index}");
            foreach (var item in items)
            {
                if (item.Id == index)
                    return item;
            }
            return null;
        }
        private async UniTask MoveUpItemBellowPlayer(int index, float time)
        {
            var lstTask = new List<UniTask>();
            var rtPlayer = itemPlayer.GetRectTransform();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items.ElementAt(i);
                var rt = item.GetRectTransform();

                if (items.ElementAt(i).Id > index && rt.anchoredPosition.y > rtPlayer.anchoredPosition.y)
                {
                    item.UI.DOPlayNewIndex(item.Id - 1);
                    lstTask.Add(rt.DOAnchorPosY(rt.anchoredPosition.y + itemHeight, time).SetEase(Ease.OutFlash).ToUniTask());
                }
            }
            await UniTask.WhenAll(lstTask);

        }
        private async UniTask ResetItemBellowPlayer(int index)
        {
            var lstTask = new List<UniTask>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items.ElementAt(i).Id > index)
                {
                    var item = items.ElementAt(i);
                    var rt = item.GetRectTransform();
                    lstTask.Add(rt.DOAnchorPosY(0, 0).ToUniTask());
                }
            }
            await UniTask.WhenAll(lstTask);

        }
        private async UniTask MoveDownItemBellowPlayer(int index, float time)
        {
            var lstTask = new List<UniTask>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items.ElementAt(i).Id >= index)
                {
                    var item = items.ElementAt(i);
                    var rt = item.GetRectTransform();
                    item.UI.DOPlayNewIndex(item.Id);
                    lstTask.Add(rt.DOAnchorPosY(rt.anchoredPosition.y - itemHeight, time).SetEase(Ease.OutFlash).ToUniTask());
                }
            }
            await UniTask.WhenAll(lstTask);

        }
    }
}
