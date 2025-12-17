using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spin
{
    public class SpinTutorial : MonoBehaviour
    {
        [SerializeField] private Image imgFade;
        [SerializeField] private Image imgTitle;
        [SerializeField] private Image imgContent;
        [SerializeField] private Image btnOk;
        [SerializeField] private Transform tfmPopup;
        [SerializeField] private List<SpinItemTutorial> lstItemTutorial;

        private void Start()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            imgFade.gameObject.SetActive(false);
            imgFade.color = new Color(0, 0, 0, 0);
            imgTitle.transform.localScale = Vector3.zero;
            imgContent.gameObject.SetActive(false);
            btnOk.transform.localScale = Vector3.zero;
            tfmPopup.gameObject.SetActive(false);
            foreach (var item in lstItemTutorial)
            {
                item.ResetUI();
            }
        }

        [Button("Show")]
        public async UniTask Show()
        {
            //UITopController.Instance.OnShowSetting();

            imgFade.gameObject.SetActive(true);
            await imgFade.DOFade(0.99f, 0.2f).SetEase(Ease.Linear).From(0);
            tfmPopup.gameObject.SetActive(true);
            imgContent.gameObject.SetActive(true);
            AudioController.Instance.PlaySound(SoundName.Popup);

            await imgTitle.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            foreach (var item in lstItemTutorial)
            {
                AudioController.Instance.PlaySound(SoundName.Star);

                await item.Show();
                await UniTask.WaitForSeconds(0.05f);
            }
            btnOk.gameObject.SetActive(true);
            await btnOk.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).From(Vector3.zero);
        }

        public void OnClickOK()
        {
            AudioController.Instance.PlaySound(SoundName.Click);
            ResetUI();
            //UITopController.Instance.OnShowMainMenu();
        }
    }
}