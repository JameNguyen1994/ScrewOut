using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    public class InfinityScroll : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform root;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private UserItem itemPrefab;

        [Header("Data")]
        [SerializeField] private UserData playerData;
        [SerializeField] private List<UserData> lstUserData;
        private List<UserData> displayList = new();

        [Header("Buttons")]
        [SerializeField] private Button btnGoTop;
        [SerializeField] private Button btnGoBottom;
        [SerializeField] private Image imgBtnGoTop;
        [SerializeField] private Image imgBtnGoBottom;

        [Header("Settings")]
        [SerializeField] private int visibleCount = 10;
        [SerializeField] private int totalCount = 100;
        [SerializeField][Min(1)] private int initialTopId = 1;

        [Header("Bound Settings")]
        [SerializeField] private int minIndex = 0;
        [SerializeField] private int startIndex = 0;
        [SerializeField] private int maxIndex = 99;

        // --- Thêm 2 biến trong phần khai báo ---
        [SerializeField] private float playerTopY;     // vị trí pixel top của player trong content
        [SerializeField] private float playerBottomY;  // vị trí pixel bottom của player trong content


        [Header("Events")]
        public UnityEvent OnReachMin;
        public UnityEvent OnLeaveMin;
        public UnityEvent OnReachMax;
        public UnityEvent OnLeaveMax;

        // ✅ New events
        [Header("Player Visibility Events")]
        public UnityEvent OnShowPlayerData;
        public UnityEvent<bool> OnHidePlayerData;

        private bool isAtMin = false;
        private bool isAtMax = false;
        [SerializeField] private bool playerVisible = false;

        private float itemHeight;
        private int topIndex;
        private LinkedList<UserItem> items = new();
        private CancellationTokenSource cts;
        [SerializeField] private int playerIndex = -1;
        [SerializeField] private int countShow = 0;
        [SerializeField] private bool isLocked = false;

        private Coroutine corActiveButton;
        private int MaxTopIndex => Mathf.Max(0, totalCount - visibleCount);

        public ScrollRect ScrollRect => scrollRect;
        public RectTransform Content => content;
        public int TotalCount => totalCount;

        #region ===== PUBLIC =====

        public void Show()
        {
            root.gameObject.SetActive(true);
            btnGoTop?.gameObject.SetActive(!isAtMin);
            btnGoBottom?.gameObject.SetActive(true);
            OnActiveButton();
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }

        public void ResetEvents()
        {
            OnReachMin.RemoveAllListeners();
            OnLeaveMin.RemoveAllListeners();
            OnReachMax.RemoveAllListeners();
            OnLeaveMax.RemoveAllListeners();
            OnShowPlayerData.RemoveAllListeners();
            OnHidePlayerData.RemoveAllListeners();
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

            CalculatorPlayerRect();

            playerVisible = false;


            Debug.Log($"[InfinityScroll] Init done. Player #{playerIndex + 1}, Total {totalCount}");

            CheckBoundaryState();
            CheckPlayerVisibility();
        }
        private void CalculatorPlayerRect()
        {
            playerBottomY = (playerIndex - startIndex) * itemHeight;
            playerTopY = playerBottomY - scrollRect.viewport.rect.height + itemHeight;
        }
        public async UniTask GoToPlayer()
        {
            if (isLocked) return;
            int targerIndex = playerIndex;
            /*            if (playerIndex < 4)
                            return;*/
            if (playerIndex >= totalCount)
                targerIndex = totalCount - 1;
            Debug.Log($"Go to player at id: {targerIndex}");
            await GotoUserAsync(targerIndex, 1f, true);
        }
        public async UniTask GotoUserAsync(int index, float duration = 1f, bool center = false)
        {
            isLocked = true;
            scrollRect.vertical = false;
            if (center)
                index -= 4;

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
                    content.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t));
                    await UniTask.Yield(PlayerLoopTiming.Update, cts.Token);
                }
                content.anchoredPosition = endPos;
            }
            catch (System.OperationCanceledException) { }
            finally
            {
                scrollRect.inertia = oldInertia;
                isLocked = false;

            }
            await UniTask.WaitUntil(() => !isLocked);
            scrollRect.vertical = true;

        }

        #endregion

        #region ===== CORE =====

        private int InsertPlayerTemp(List<UserData> list, UserData player)
        {
            if (player == null || list == null || list.Count == 0)
                return -1;

            int insertIndex = list.FindIndex(u => u.points < player.points);
            if (insertIndex < 0) insertIndex = list.Count;
            list.Insert(insertIndex, player);
            return insertIndex;
        }

        private void CreateInitialItems()
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, totalCount * itemHeight);
            //items.Clear();

            for (int i = 0; i < visibleCount + 4; i++)
            {

                var item = (items.Count - 1 >= i) ? items.ElementAt(i) : null;
                if (item == null)
                {

                    item = Instantiate(itemPrefab, content);
                    items.AddLast(item);

                }


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
            CheckBoundaryState();
            CheckPlayerVisibility();
        }

        private void OnScroll(Vector2 _)
        {
            CancelShow();
            CheckPlayerVisibility();
            OnActiveButton();
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
                  //  else last.gameObject.SetActive(false);
                }
            }

            topIndex = newTopIndex;
            CheckBoundaryState();
        }

        private void UpdateItem(UserItem item, int index)
        {
            if (index < 0 || index >= displayList.Count) return;

            var data = displayList[index];
            bool isPlayer = playerData != null && data == playerData;
            item.SetData(index + startIndex, data, isPlayer);
        }

        #endregion

        #region ===== Player Visibility =====

        public void CheckPlayerVisibility(bool force =false)
        {
            if (playerIndex < 0 || playerIndex >= totalCount) return;

            // --- vùng hiển thị trong toạ độ content ---
            float viewTopY = content.anchoredPosition.y;

            // --- player item toạ độ ---
            float itemTop = playerTopY;
            float itemBottom = playerBottomY;

            // --- kiểm tra giao nhau (visible) ---
            bool isVisible = itemBottom >= viewTopY && itemTop <= viewTopY;
            //Debug.Log($"CheckPlayerVisibility: itemTop={itemTop}, itemBottom={itemBottom}, viewTopY={viewTopY}, isVisible={isVisible}");
            if (isVisible != playerVisible || force)
            {
                playerVisible = isVisible;


                if (isVisible)
                {
                    OnShowPlayerData?.Invoke();
                }
                else
                {
                    bool isAbove = itemTop <= viewTopY;
                    OnHidePlayerData?.Invoke(isAbove); // true nếu player nằm trên, false nếu dưới
                }
            }
        }


        #endregion

        #region ===== Boundary & Buttons =====

        private void CheckBoundaryState()
        {
            bool atMin = topIndex <= minIndex;
            bool atMax = topIndex + visibleCount >= maxIndex;

            if (atMin && !isAtMin)
            {
                CancelShow();
                isAtMin = true;
                btnGoTop?.gameObject.SetActive(false);
                OnReachMin?.Invoke();
            }
            else if (!atMin && isAtMin)
            {
                isAtMin = false;
                btnGoTop?.gameObject.SetActive(true);
                OnLeaveMin?.Invoke();
            }

            if (atMax && !isAtMax)
            {
                CancelShow();
                isAtMax = true;
                btnGoBottom?.gameObject.SetActive(false);
                OnReachMax?.Invoke();
            }
            else if (!atMax && isAtMax)
            {
                isAtMax = false;
                btnGoBottom?.gameObject.SetActive(true);
                OnLeaveMax?.Invoke();
            }
        }

        private void OnActiveButton()
        {
            if (corActiveButton != null)
                StopCoroutine(corActiveButton);
            imgBtnGoBottom.DOFade(1f, 0f);
            imgBtnGoTop.DOFade(1f, 0f);
            if (gameObject.activeInHierarchy)
                corActiveButton = StartCoroutine(CorActiveButton());
        }

        private void CancelShow()
        {
            if (corActiveButton != null)
            {
                StopCoroutine(corActiveButton);
                corActiveButton = null;
            }
        }
        private IEnumerator CorActiveButton()
        {
            //  Debug.Log("CorActiveButton");
            yield return new WaitForSeconds(2f);
            imgBtnGoTop.DOFade(0f, 0.5f);
            imgBtnGoBottom.DOFade(0f, 0.5f);
            /*            btnGoTop?.gameObject.SetActive(false);
                        btnGoBottom?.gameObject.SetActive(false);*/
            corActiveButton = null;
        }

        #endregion

        private void Update()
        {
            if (itemHeight == 0)
                return;
            var rectView = scrollRect.viewport.rect;
            visibleCount = (int)rectView.height / (int)itemHeight + (rectView.height / itemHeight > 0 ? 1 : 0);
        }
    }
}
