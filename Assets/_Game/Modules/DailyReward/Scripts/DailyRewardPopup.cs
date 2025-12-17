using Cysharp.Threading.Tasks;
using DailyReward;
using DG.Tweening;
using EasyButtons;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace DailyReward
{
    public class DailyRewardPopup : MonoBehaviour
    {
        [SerializeField] private Image imgFade;
        [SerializeField] private Image imgContent;
        [SerializeField] private float fade = 0.9f;
        [SerializeField] private float time = 0.9f;
        [SerializeField] private Ease easeShow;
        [SerializeField] private Ease easeHide;

        [SerializeField] private Button btnClose;
        [SerializeField] private List<DailyRewardItem> lstTransfromDay1_3;
        [SerializeField] private List<DailyRewardItem> lstTransfromDay4_6;
        [SerializeField] private DailyRewardItem tfm7;
        [SerializeField] private bool isShow = false;


        private void Awake()
        {
            imgFade.gameObject.SetActive(false);
            imgContent.gameObject.SetActive(false);
            btnClose.onClick.AddListener(OnClickClose);
            //btnClose.interactable = false;
            btnClose.transform.localScale = Vector3.zero;
        }
        public async UniTask Show()
        {
            isShow = true;
            for (int i = 0; i < lstTransfromDay1_3.Count; i++)
            {
                lstTransfromDay1_3[i].Hide();
            }
            for (int i = 0; i < lstTransfromDay4_6.Count; i++)
            {
                lstTransfromDay4_6[i].Hide();
            }
            tfm7.Hide();



            imgFade.gameObject.SetActive(true);
            imgFade.DOFade(fade, time).SetEase(Ease.OutQuad);
            imgContent.gameObject.SetActive(true);
            imgContent.transform.localScale = Vector3.zero;
            ShowListItemAsync();

            await imgContent.transform.DOScale(Vector3.one, time).SetEase(easeShow).ToUniTask();

            btnClose.transform.localScale = Vector3.zero;
            await btnClose.transform.DOScale(Vector3.one, 0.1f).SetEase(easeShow);//.SetDelay(time);
            btnClose.interactable = true;
        }
        public async UniTask Hide(bool canGetReward = false)
        {
            await imgContent.transform.DOScale(Vector3.zero, time).SetEase(easeHide).ToUniTask();
            imgFade.DOFade(0, time).SetEase(Ease.OutQuad);
            await UniTask.Delay((int)(time * 1000));
            imgFade.gameObject.SetActive(false);
            imgContent.gameObject.SetActive(false);
            btnClose.transform.localScale = Vector3.zero;

            if (canGetReward)
            {
                await MainMenuRecieveRewardsHelper.Instance.OnGetReward();

            }
            isShow = false;
        }
        public async UniTask WaitToClose()
        {
            await UniTask.WaitUntil(() => !isShow);
        }
        public void OnClickClose()
        {
            AudioController.Instance.PlaySound(SoundName.Click);

            //btnClose.interactable = false;
            Hide().Forget();
        }
        public void OnClickShow()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            AudioController.Instance.PlaySound(SoundName.Click);

            Show().Forget();
        }

        private async UniTask ShowListItemAsync(int timeDelay = 140, float timeShow = 0.2f)
        {
            await UniTask.WaitForSeconds(time / 2);
            for (int i = 0; i < lstTransfromDay1_3.Count; i++)
            {
                lstTransfromDay1_3[i].ShowAsync(timeShow).Forget();
            }
            await UniTask.Delay(timeDelay);
            for (int i = 0; i < lstTransfromDay4_6.Count; i++)
            {
                lstTransfromDay4_6[i].ShowAsync(timeShow).Forget();
            }
            await UniTask.Delay(timeDelay);
            await tfm7.ShowAsync(timeShow);
        }
        [Button("Test Show")]
        public void TestShow(int timeDelay = 140, float timeShow = 0.2f)
        {
            for (int i = 0; i < lstTransfromDay1_3.Count; i++)
            {
                lstTransfromDay1_3[i].Hide();
            }
            for (int i = 0; i < lstTransfromDay4_6.Count; i++)
            {
                lstTransfromDay4_6[i].Hide();
            }
            tfm7.Hide();

            ShowListItemAsync(timeDelay, timeShow).Forget();
        }
    }

}