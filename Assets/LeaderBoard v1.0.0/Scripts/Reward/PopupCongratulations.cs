using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard.reward
{
    public class PopupCongratulations : LeaderBoardCtrBase
    {
        [Header("References")]
        [SerializeField] private Image imgFade;
        [SerializeField] private RectTransform rtfmContent;
        [SerializeField] private TMP_Text txtContinue;
        [SerializeField] private RectTransform rtfmTitle;
        [SerializeField] private RectTransform rtfmFlagShow;
        [SerializeField] private RectTransform rtfmFlagMove;
        [SerializeField] private RectTransform rtfmShadow;
        [SerializeField] private RectTransform rtfmCrown;

        [SerializeField] private RectTransform rtfmTrumpet1;
        [SerializeField] private RectTransform rtfmTrumpet2;

        [SerializeField] private GameObject gobjParticles;
        [SerializeField] private AudioSource audioFirework;
        [SerializeField] private AudioSource audioTrumpet;
        [SerializeField] private Animator animationTrumpet1;
        [SerializeField] private Animator animationTrumpet2;

        [SerializeField] private RectTransform rtfmGifts;
        [SerializeField] private ItemResource prefabReward;
        [SerializeField] private List<ItemResource> lstItemResouce;



        [Header("Setup")]
        [SerializeField] private Image imgCrown;
        [SerializeField] private Image imgFlag;
        [SerializeField] private Image imgFlagMove;
        [SerializeField] private Image imgAvatar;
        [SerializeField] private Image imgBorder;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtTime;

        [SerializeField] private List<Sprite> lstSprCrown;
        [SerializeField] private List<Sprite> lstSprFlag;
        [SerializeField] private List<Sprite> lstSprFlagMove;

        [Header("Animation Settings")]
        [SerializeField] private Color fadeColor;
        [SerializeField] private float fadeValue;
        [SerializeField] private float durationPopup;
        [SerializeField] private float durationItem;
        [SerializeField] private bool isShowing = false;

        [SerializeField] private Vector2 anchorFlagShow;
        [SerializeField] private Vector2 anchorFlagHide;


        [SerializeField] private Sprite sprTestAvatar;
        [SerializeField] private Sprite sprTestBorder;
        [SerializeField] private bool isShowingPopup;

        public bool IsShowingPopup { get => isShowingPopup;}

        [Button]
        public void TestShow(int top = 1)
        {
            var userData = new UserData
            {
                name = "Test User",
                points = 1000,
                avatar = sprTestAvatar,
                border = sprTestBorder
            };
            var time = new DateTime(2025, 10, 20);
            Show(top, time.ToShortString(), userData).Forget();
        }
        public async UniTask Show(int top , string time, UserData userData = null, GiftData giftData = null)
            
        {
            isShowingPopup = false;
            Reset();
            isShowing = true;
            imgFade.gameObject.SetActive(true);
            rtfmContent.gameObject.SetActive(true);


            //SetUp
            imgCrown.sprite = lstSprCrown[Mathf.Clamp(top - 1, 0, lstSprCrown.Count - 1)];
            imgFlag.sprite = lstSprFlag[Mathf.Clamp(top - 1, 0, lstSprFlag.Count - 1)];
            imgFlagMove.sprite = lstSprFlagMove[Mathf.Clamp(top - 1, 0, lstSprFlagMove.Count - 1)];
            imgAvatar.sprite = userData != null ? userData.avatar : null;
            imgBorder.sprite = userData != null ? userData.border : null;
            txtName.text = userData != null ? userData.name : "Player";
            txtTime.text = time;
            rtfmCrown.gameObject.SetActive(true);
            for (int i = lstItemResouce.Count - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(lstItemResouce[i].gameObject);
            }
            lstItemResouce.Clear();

            // Gift
            for (int i = 0; i < giftData.rewards.Count; i++)
            {
                var reward = giftData.rewards[i];
          
                var itemReward = GameObject.Instantiate(prefabReward, rtfmGifts);
                itemReward.InitResource(reward);
                itemReward.Hide();
                lstItemResouce.Add(itemReward);
            }





            imgFade.DOFade(fadeValue, durationPopup);
            await rtfmContent.DOScale(Vector3.one, durationPopup).SetEase(Ease.OutBack);

            rtfmFlagShow.gameObject.SetActive(true);
            rtfmTitle.gameObject.SetActive(true);
            await rtfmTitle.DOScale(Vector3.one, durationItem).SetEase(Ease.OutBack);
            await rtfmFlagShow.DOScale(Vector3.one, durationItem);
            gobjParticles.gameObject.SetActive(true);
            rtfmFlagMove.gameObject.SetActive(true);

            rtfmShadow.gameObject.SetActive(true);
            rtfmShadow.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack).From(new Vector3(1, 0, 1));
            await rtfmFlagMove.DOAnchorPos(anchorFlagShow, durationItem).SetEase(Ease.OutCubic);


            /*            DOVirtual.DelayedCall(5f, () =>
                        {
                            gobjParticles.gameObject.SetActive(false);
                        });*/

            isShowing = false;

            rtfmCrown.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).From(Vector3.zero);

            AnimationTrumpet();

            await UniTask.Delay(500);
            for (int i = 0; i < lstItemResouce.Count; i++)
            {
                var item = lstItemResouce[i];
                item.Show();
                item.GetGift();
                await UniTask.Delay(150);
            }

            await UniTask.Delay(2000);
            txtContinue.gameObject.SetActive(true);

            await UniTask.WaitUntil(() => !isShowingPopup);

        }
        [Button]
        public async UniTask Hide()
        {
            isShowing = true;
            txtContinue.gameObject.SetActive(false);
            gobjParticles.gameObject.SetActive(false);
            DOVirtual.DelayedCall(durationItem / 5, () =>
            {
                rtfmShadow.DOScaleY(0, 0.1f).SetEase(Ease.InBack);
            });
            for (int i = 0; i < lstItemResouce.Count; i++)
            {
                var item = lstItemResouce[i];
                item.Hide();
                await UniTask.Delay(100);
            }
            await rtfmFlagMove.DOAnchorPos(anchorFlagHide, durationItem).SetEase(Ease.InCubic);
            rtfmFlagShow.DOScale(Vector3.zero, durationItem).SetEase(Ease.InBack);
            rtfmCrown.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
            await rtfmTitle.DOScale(Vector3.zero, durationItem).SetEase(Ease.InBack);



            imgFade.DOFade(0, durationPopup);

            await rtfmContent.DOScale(Vector3.zero, durationPopup).SetEase(Ease.InBack);
            await Reset();
            isShowing = false;

            isShowingPopup = false;

        }
        [Button]
        public async UniTask Reset()
        {
            imgFade.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
            imgFade.gameObject.SetActive(false);
            rtfmContent.localScale = Vector3.zero;
            rtfmContent.gameObject.SetActive(false);
            txtContinue.gameObject.SetActive(false);

            rtfmTitle.gameObject.SetActive(false);
            rtfmFlagShow.gameObject.SetActive(false);
            rtfmFlagMove.gameObject.SetActive(false);
            rtfmShadow.gameObject.SetActive(false);
            rtfmTitle.localScale = Vector3.zero;
            rtfmShadow.localScale = Vector3.zero;
            rtfmFlagShow.localScale = Vector3.zero;
            rtfmFlagMove.anchoredPosition = anchorFlagHide;
            rtfmCrown.gameObject.SetActive(false);
            rtfmCrown.localScale = Vector3.zero;

            rtfmTrumpet1.localScale = Vector3.zero;
            rtfmTrumpet2.localScale = Vector3.zero;


            gobjParticles.gameObject.SetActive(false);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        [Button]
        public void ShowInEditor()
        {
            rtfmContent.localScale = Vector3.one;
            rtfmContent.gameObject.SetActive(true);

            imgFade.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeValue);
            imgFade.gameObject.SetActive(true);
            txtContinue.gameObject.SetActive(true);
            rtfmFlagShow.gameObject.SetActive(true);

            rtfmTitle.gameObject.SetActive(true);
            rtfmShadow.gameObject.SetActive(true);
            rtfmCrown.gameObject.SetActive(true);
            rtfmCrown.localScale = Vector3.one;
            rtfmShadow.localScale = Vector3.one;
            rtfmTitle.localScale = Vector3.one;
            rtfmFlagShow.localScale = Vector3.one;

            rtfmTrumpet1.localScale = Vector3.one;
            rtfmTrumpet2.localScale = Vector3.one;

            rtfmFlagMove.gameObject.SetActive(true);

            rtfmFlagMove.anchoredPosition = anchorFlagShow;
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        public void OnClickContinue()
        {
            if (isShowing)
                return;
            Hide().Forget();


        }


        private async UniTask AnimationTrumpet()
        {
            rtfmTrumpet1.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).From(Vector3.zero);
            await rtfmTrumpet2.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).From(Vector3.zero);
            
            audioTrumpet.Play();
            animationTrumpet1.Play("TrumpetScale");
            animationTrumpet2.Play("TrumpetScale");
        }
    }
}
