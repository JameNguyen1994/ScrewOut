using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Storage.LocalDb;

namespace WeeklyQuest
{
    public class ItemQuest : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform rtfmItem;
        [SerializeField] private RectTransform rtfmPoint;
        [SerializeField] private RectTransform holderAnimationQuest;
        [SerializeField] private Transform tfmDone;
        [SerializeField] private Image iconQuest;
        [SerializeField] private TextMeshProUGUI questDescription;
        [SerializeField] private TextMeshProUGUI txtPoint;
        [SerializeField] private TextMeshProUGUI txtProcess;
        [SerializeField] private Image fillBar;
        [SerializeField] private Image fillBarParent;
        [SerializeField] private int offsetSize;
        [SerializeField] private QuestDataDB questDataDB;
        [SerializeField] private RectTransform rtfmParent;
        [SerializeField] private ParticleSystem parPoint;
        [SerializeField] private RectTransform rtfmFill;
        [SerializeField] private Vector2Int size;


        [SerializeField] private Transform tfmTargetPoint;
        public int Point => questDataDB?.point ?? 0;

        public bool IsCompleted { get => questDataDB == null ? true : questDataDB.isComplete; }

        private void Start()
        {
            var rect = fillBar.rectTransform;

            // Căn trái giữa theo chiều dọc
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(offsetSize/2,0);

            size = new Vector2Int((int)fillBarParent.rectTransform.rect.width- offsetSize, (int)rect.rect.height);
        }

        public void SetData(QuestDataDB questDataDB)
        {
            this.questDataDB = questDataDB;
            if (questDataDB == null)
            {
                Debug.LogError("QuestDataDB is null. Cannot set data.");
                return;
            }
            var questImage = WeeklyQuestManager.Instance.WeeklyDataHelper.GetQuestResourceIcon(questDataDB.questType);
            iconQuest.sprite = questImage;

            txtPoint.text = $"{questDataDB.point}";
            
            float process = (float)questDataDB.oldValue / questDataDB.targetValue * 100f;
           // fillBar.fillAmount = (float)questDataDB.oldValue / questDataDB.targetValue;
            //Debug.Log($"SetData Quest {questDataDB.questType} progress: {questDataDB.oldValue}/{questDataDB.targetValue} ({process}%)fillBar.sizeDelta  {fillBar.sizeDelta}");
            rtfmFill.sizeDelta = new Vector2(process / 100f * size.x, size.y);

            txtProcess.text = $"{questDataDB.oldValue}/{questDataDB.targetValue}";
            questDescription.text = WeeklyQuestManager.Instance.WeeklyDataHelper.GetQuestDescription(questDataDB.questType, questDataDB.targetValue);
            if (questDataDB.isComplete)
            {
                tfmDone.gameObject.SetActive(true);
                rtfmPoint.gameObject.SetActive(false);
                txtPoint.gameObject.SetActive(false);
            }
            else
            {
                tfmDone.gameObject.SetActive(false);
                rtfmPoint.gameObject.SetActive(true);
                txtPoint.gameObject.SetActive(true);
            }
        }
        public async UniTask OnlyShowCurrentPoint()
        {
                rtfmFill.DOSizeDelta(new Vector2(((float)questDataDB.currentValue / questDataDB.targetValue) * size.x, size.y), 0.3f);
        }
        public bool HasUpdate()
        {
            if (questDataDB.oldValue != questDataDB.currentValue)
            {
                Debug.Log($"Quest {questDataDB.questType} has update: Old Value: {questDataDB.oldValue}, Current Value: {questDataDB.currentValue}");
                return true;
            }
            return false;
        }
        public async UniTask SetDataAsync(UnityAction actionOnComplete)
        {
            Debug.Log($"Setting data for quest: {questDataDB.questType}");
            float process = (float)questDataDB.currentValue / questDataDB.targetValue * 100f;
            Debug.Log($"Quest {questDataDB.questType} progress: {questDataDB.currentValue}/{questDataDB.targetValue} ({process}%)");
            parPoint.Play();
            DOVirtual.Int(questDataDB.oldValue, questDataDB.currentValue, 0.5f, (value) =>
            {
                txtProcess.text = $"{value}/{questDataDB.targetValue}";
            });
            AudioController.Instance.PlaySound(SoundName.WEEKLY_PROCESS);
            var neValue = (float)questDataDB.currentValue / questDataDB.targetValue;
            //await fillBar.DOFillAmount(neValue, 0.5f).SetEase(Ease.OutCubic);
          //  rtfmFill.sizeDelta = new Vector2(process / 100f * size.x, size.y);
            await rtfmFill.DOSizeDelta(new Vector2(neValue*size.x, size.y),0.5f).SetEase(Ease.OutCubic);

            if (questDataDB.currentValue >= questDataDB.targetValue)
            {
                await OnQuestComplete();
                actionOnComplete?.Invoke();
            }
            else
            {
                questDataDB.oldValue = questDataDB.currentValue;
            }
        }
        public void SetParrent(RectTransform parent)
        {
            if (parent != null)
            {
                rtfmItem.SetParent(parent, false);
                rtfmItem.gameObject.SetActive(true);
                rtfmParent = parent;
            }
            else
            {
                Debug.LogError("Parent transform is null. Cannot set parent for ItemGiftJourney.");
            }
        }
        public void SetTargetTransfrom(Transform tfmTarget)
        {
            tfmTargetPoint = tfmTarget;
        }
        private async UniTask OnQuestComplete()
        { // Tính điểm cộng thêm từ giá trị hiện tại trừ đi giá trị cũ

            /*            if (questDataDB.currentValue >= questDataDB.targetValue)
                        {
                            return;
                        }*/

            questDataDB.oldValue = questDataDB.currentValue;

            if (questDataDB.currentValue >= questDataDB.targetValue)
            {
                questDataDB.isComplete = true;
                var weeklyQuestData = Db.storage.WeeklyQuestData;
                weeklyQuestData.currentPoint += questDataDB.point; // Cộng điểm vào tổng điểm hiện tại
            }
            Db.storage.WeeklyQuestData.Save(); // Lưu dữ liệu sau khi hoàn thành nhiệm vụ
            Transform obj = rtfmPoint;
            obj.SetParent(tfmTargetPoint); // Đặt lại vị trí của điểm số

            Vector3 start = obj.position;
            Vector3 end = tfmTargetPoint.position;
            txtPoint.gameObject.SetActive(false); // Ẩn điểm số khi hoàn thành
                                                  // Đường vòng cung: tạo điểm giữa cao hơn (trung điểm + offset Y)
            Vector3 mid = new Vector3(start.x + 1, (end.y - 1));

            // Tạo đường vòng cung qua 3 điểm: start → mid → end
            float duration = 1.5f;
            IconCompleted(0.4f);
            AudioController.Instance.PlaySound(SoundName.QuestStar);
            var pathTween = obj.DOPath(new Vector3[] { start, mid, end }, duration, PathType.CatmullRom)
                               .SetEase(Ease.OutQuad).ToUniTask();

            // Scale: nhỏ → to → nhỏ (xong thì ẩn luôn)
            var scaleTween = obj.DOScale(Vector3.one * 1.2f, duration / 2)
                                .SetEase(Ease.OutQuad)
                                .OnComplete(() =>
                                {
                                    obj.DOScale(Vector3.zero, duration / 2).SetEase(Ease.InQuad);
                                }).ToUniTask();
            // Tween xoay: quay quanh Z
            var rotateTween = obj.DORotate(new Vector3(0, 0, 360f), duration, RotateMode.FastBeyond360)
                                 .SetEase(Ease.Linear)
                                 .ToUniTask();
            // Chạy song song cả hai tween
            await UniTask.WhenAll(pathTween, scaleTween, rotateTween);

            obj.gameObject.SetActive(false);

            int currentX = (int)holderAnimationQuest.anchoredPosition.x;
            var currentSizeDelta = rtfmItem.sizeDelta;

            bool isLastItem = transform.GetSiblingIndex() == rtfmParent.childCount - 1;
            if (!isLastItem)
            {

                await holderAnimationQuest.DOAnchorPosX(currentSizeDelta.x, 0.3f).SetEase(Ease.OutQuad);
                holderAnimationQuest.gameObject.SetActive(false);
                await rtfmItem.DOSizeDelta(new Vector2(currentSizeDelta.x, 0), 0.1f).SetEase(Ease.Linear).OnUpdate(() =>
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rtfmParent);

                });
                rtfmItem.gameObject.SetActive(false);
                rtfmItem.SetAsLastSibling(); // Đặt lại vị trí cuối cùng trong danh sách
                await rtfmItem.DOSizeDelta(currentSizeDelta, 0).SetEase(Ease.OutBack);
                rtfmItem.gameObject.SetActive(true);
                holderAnimationQuest.gameObject.SetActive(true);
                holderAnimationQuest.DOAnchorPosX(currentX, 0.5f).SetEase(Ease.OutBack);
            }
        }
        private async UniTask IconCompleted(float delay)
        {
            await UniTask.WaitForSeconds(delay);
            tfmDone.localScale = Vector3.zero; // Đặt lại kích thước ban đầu
            tfmDone.gameObject.SetActive(true);
            await tfmDone.DOScale(Vector3.one * 1.2f, 0.5f)
                .SetEase(Ease.OutBack);
            await tfmDone.DOScale(Vector3.one, 0.5f).SetEase(Ease.InBack);
        }
    }

}
