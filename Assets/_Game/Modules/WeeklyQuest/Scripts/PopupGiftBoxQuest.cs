using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace WeeklyQuest
{
    public class PopupGiftBoxQuest : MonoBehaviour
    {
        [SerializeField] private Image imgFade;
        [SerializeField] private Image imgContent;
        [SerializeField] private Image imgGiftBox;
        [SerializeField] private Image imgGiftLid;
        [SerializeField] private Transform tfmTarget;
        [SerializeField] private float fade = 0.9f;
        [SerializeField] private float time = 0.9f;
        [SerializeField] private bool isOpen = false;
        [SerializeField] private Ease ease;

        [SerializeField] private Button btnClose;
        [SerializeField] private Transform tfmTextContinue;

        [SerializeField] private List<ItemResourcePopup> lstItemResourceTop;
        [SerializeField] private List<ItemResourcePopup> lstItemResourceBottom;
        [SerializeField] private List<ItemResourcePopup> lstItemResourceShow;



        private void Awake()
        {
            imgFade.gameObject.SetActive(false);
            imgContent.gameObject.SetActive(false);
            btnClose.onClick.AddListener(OnClickClose);
            btnClose.interactable = false;
            tfmTextContinue.localScale = Vector3.zero;
            tfmTextContinue.gameObject.SetActive(false);
        }
        public async UniTask Show()
        {
            imgFade.gameObject.SetActive(true);
            imgFade.DOFade(fade, time).SetEase(ease);
            imgContent.gameObject.SetActive(true);
            imgContent.transform.localScale = Vector3.zero;
            await imgContent.transform.DOScale(Vector3.one, time).SetEase(ease).ToUniTask();

            tfmTextContinue.localScale = Vector3.zero;
            btnClose.gameObject.SetActive(true);
            tfmTextContinue.gameObject.SetActive(true);

            await tfmTextContinue.DOScale(Vector3.one, time).SetEase(ease).SetDelay(time);
            btnClose.interactable = true;

        }
        public async UniTask Hide()
        {
            // await imgContent.transform.DOScale(Vector3.zero, time).SetEase(easeOpen).ToUniTask();

            imgFade.DOFade(0, time).SetEase(ease);
            imgGiftBox.DOFade(0, time).SetEase(ease);
            imgGiftLid.DOFade(0, time).SetEase(ease);
            tfmTextContinue.localScale = Vector3.zero;
            tfmTextContinue.gameObject.SetActive(false);
            btnClose.interactable = false;
            var lstTask = new List<UniTask>();
            for (int i = 0; i < lstItemResourceShow.Count; i++)
            {
                lstTask.Add(lstItemResourceShow[i].MoveTo(tfmTarget.position));
                await UniTask.Delay(100);
            }
            await UniTask.WhenAll(lstTask);
            imgContent.gameObject.SetActive(false);
            isOpen = false;
            imgFade.gameObject.SetActive(false);

        }

        public void OnClickClose()
        {
            btnClose.interactable = false;
            Hide().Forget();


        }
        public async UniTask OnShowGift(Sprite sprBox, Sprite sprLid, List<ResourceValue> listResou)
        {
            AudioController.Instance.PlaySound(SoundName.OpenGift);

            isOpen = true;
            imgGiftBox.sprite = sprBox;
            imgGiftLid.sprite = sprLid;
            imgGiftBox.DOFade(1, time).SetEase(ease);
            imgGiftLid.DOFade(1, time).SetEase(ease);
            int count = listResou.Count;
            SetListItem(count);
            for (int i = 0; i < lstItemResourceShow.Count; i++)
            {
                lstItemResourceShow[i].SetData(listResou[i]);
                lstItemResourceShow[i].gameObject.SetActive(true);
                lstItemResourceShow[i].transform.localPosition = new Vector3(0, 0, 0);
                lstItemResourceShow[i].transform.localScale = new Vector3(0, 0, 0);
            }
            for (int i = 0; i < lstItemResourceShow.Count; i++)
            {
                lstItemResourceShow[i].Show();
                await UniTask.Delay(100);
            }
            Show();
            await UniTask.WaitUntil(() => isOpen == false);
        }
        private void SetListItem(int count)
        {
            lstItemResourceShow = new List<ItemResourcePopup>();

            if (count <= 3)
            {
                for (int i = 0; i < count && i < lstItemResourceTop.Count; i++)
                {
                    lstItemResourceShow.Add(lstItemResourceTop[i]);
                }
            }
            else
            {
                int bottomCount = count / 2 + (count % 2); // lẻ thì ưu tiên bottom
                int topCount = count / 2;

                for (int i = 0; i < bottomCount && i < lstItemResourceBottom.Count; i++)
                {
                    lstItemResourceShow.Add(lstItemResourceBottom[i]);
                }
                for (int i = 0; i < topCount && i < lstItemResourceTop.Count; i++)
                {
                    lstItemResourceShow.Add(lstItemResourceTop[i]);
                }
            }
        }

    }
}
