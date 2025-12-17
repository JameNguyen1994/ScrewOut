using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Storage.LocalDb;

namespace WeeklyQuest
{
    public enum ItemGiftState
    {
        Available, // The gift is available to be claimed
        Received, // The gift has been received
    }

    public class ItemGift : MonoBehaviour
    {
        const string OPEN_GIFT_ANIMATION = "Anim_Gift";
        const string IDLE_GIFT_ANIMATION = "Anim_Gift_Idle";
        const string CLOSE_GIFT_ANIMATION = "Anim_Gift_Open";
        [SerializeField] private RectTransform rtfmItem;
        [SerializeField] private RectTransform rtfmGiftBox;
        [SerializeField] private TextMeshProUGUI txtPointRequire;
        [SerializeField] private Transform tfmPreviewGift;
        [SerializeField] private Transform tfmGiftBox;

        [SerializeField] private Transform tfmGiftFly;
        [SerializeField] private Transform tfmTargetGift;

        [SerializeField] private Image imgGiftBox;
        [SerializeField] private Image imgGiftLid;

        [SerializeField] private int requirePoints;

        [SerializeField] private bool isPreview;
        [SerializeField] private bool isAnimationOpen;
        [SerializeField] private ItemGiftState itemGiftState;

        [SerializeField] private Animator animator;
        [SerializeField] private GiftDataDB giftDataDB;
        [SerializeField] private List<ResourceValue> lstResourceValue;

        public async UniTask CheckOpenChest(int currentPoint)
        {
            if (itemGiftState == ItemGiftState.Available && currentPoint >= requirePoints)
            {
                PlayAnimationOpen();
                itemGiftState = ItemGiftState.Received;
                SetStateGift();
            }
            await UniTask.WaitUntil(() => isAnimationOpen == false);
        }
        public void PlayAnimationOpen()
        {
            isAnimationOpen = true;
            animator.Play(OPEN_GIFT_ANIMATION);
        }
        public async UniTask OnDoneAnimOpenGift()
        {
            /*            var time = 1.5f;
                        tfmGiftFly.SetParent(tfmTargetGift);
                        tfmGiftFly.gameObject.SetActive(true);
                        var startPostion = tfmGiftFly.position;
                        var endPosition = tfmTargetGift.position;
                        var middlePosition = tfmGiftFly.position + new Vector3(-20, 20, 0);

                        var flyTask = tfmGiftFly.DOPath(new Vector3[] { startPostion, middlePosition, endPosition }, time, PathType.CatmullRom)
                            .SetEase(Ease.OutQuad).ToUniTask();
                        var scaleTask = tfmGiftFly.DOScale(Vector3.one * 1.5f, time / 2).ToUniTask();
                        var scaleTaskSmall = tfmGiftFly.DOScale(Vector3.one * 1, time / 2).ToUniTask();
                        await UniTask.WhenAll(flyTask, scaleTask, scaleTaskSmall);
                        tfmGiftFly.gameObject.SetActive(false);*/

            await WeeklyQuestManager.Instance.PopupGiftBoxQuest.OnShowGift(imgGiftBox.sprite, imgGiftLid.sprite, lstResourceValue);
            isAnimationOpen = false;
        }
        public async UniTask TurnPreviewGift(bool on)
        {
            if (on)
            {
                isPreview = true;
                tfmPreviewGift.DOKill();
                tfmPreviewGift.gameObject.SetActive(true);
                await tfmPreviewGift.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
            else
            {
                isPreview = false;
                tfmPreviewGift.DOKill();
                await tfmPreviewGift.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
                tfmPreviewGift.gameObject.SetActive(false);
            }
        }
        public void SetStateGift()
        {
            /*   if (itemGiftState == ItemGiftState.Available)
               {
                   imgGiftBox.gameObject.SetActive(true);
                   imgGiftLid.gameObject.SetActive(false);
               }
               else if (itemGiftState == ItemGiftState.Received)
               {
                   imgGiftBox.gameObject.SetActive(false);
                   imgGiftLid.gameObject.SetActive(true);
               }*/
        }
        public void Reset()
        {
            isPreview = false;
            tfmPreviewGift.gameObject.SetActive(false);
            tfmPreviewGift.transform.localScale = Vector3.zero;
        }
        public void SetData(ImageGiftData imageGift, GiftDataDB gift, Transform tfmTargetGift)
        {
            Reset();
            this.tfmTargetGift = tfmTargetGift;

            imgGiftBox.sprite = imageGift.sprBox;
            imgGiftLid.sprite = imageGift.sprLid;
            tfmGiftBox.localScale = Vector3.one * imageGift.scale;

            giftDataDB = gift;


            if (txtPointRequire != null)
            {
                requirePoints = gift.requirePoints;
                txtPointRequire.text = $"{gift.requirePoints}";
            }
            if (tfmPreviewGift != null)
            {
                var lstResource = gift.lstResourceValue;
                for (int i = 0; i < lstResource.Count; i++)
                {
                    lstResourceValue.Add(lstResource[i]);
                    var itemResource = WeeklyQuestManager.Instance.ObjectPoolWeekly.GetItemResource();
                    itemResource.InitResource(lstResource[i]);
                    itemResource.transform.SetParent(tfmPreviewGift, false);
                    itemResource.gameObject.SetActive(true);
                }
            }
            rtfmItem.gameObject.SetActive(true);
            if (gift.isReceived)
            {
                Debug.Log($"Gift is already received with required points: {gift.requirePoints}");
                itemGiftState = ItemGiftState.Received;
                animator.Play(CLOSE_GIFT_ANIMATION);
            }
            else
            {
                Debug.Log($"Gift is available with required points: {gift.requirePoints}");
                itemGiftState = ItemGiftState.Available;
                animator.Play(IDLE_GIFT_ANIMATION);
            }
            SetStateGift();
        }
        private void OnEnable()
        {
            ReCheckAnimAfterShow();
        }
        public void ReCheckAnimAfterShow()
        {
            if (giftDataDB.isReceived)
            {
                Debug.Log($"Gift is already received with required points: {giftDataDB.requirePoints}");
                itemGiftState = ItemGiftState.Received;
                animator.Play(CLOSE_GIFT_ANIMATION);
            }
            else
            {
                Debug.Log($"Gift is available with required points: {giftDataDB.requirePoints}");
                itemGiftState = ItemGiftState.Available;
                animator.Play(IDLE_GIFT_ANIMATION);
            }
        }
        public void SetParent(RectTransform parent, int maxPoint, int offset)
        {
            if (parent != null)
            {
                rtfmItem.SetParent(parent, false);
                rtfmItem.gameObject.SetActive(true);
                float width = parent.sizeDelta.x;
                float sizeOfOnePercent = width / maxPoint;
                var pos = new Vector2Int((int)(sizeOfOnePercent * requirePoints), 0);
                rtfmItem.anchoredPosition = pos;
                rtfmGiftBox.anchoredPosition += new Vector2(offset, 0);
            }
            else
            {
                Debug.LogError("Parent transform is null. Cannot set parent for ItemGiftJourney.");
            }
        }
        public void OnClickItem()
        {
            TurnPreviewGift(!isPreview).Forget();
        }
        private void OnDisable()
        {
            TurnPreviewGift(false);
        }
    }
}
