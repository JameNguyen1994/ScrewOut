using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.journey
{

    public class PopupJourneyTutorial : MonoBehaviour
    {
        [SerializeField] private Image imgFade;
        [SerializeField] private Image imgTitle;
        [SerializeField] private Image imgContent;
        [SerializeField] private Image btnOk;
        [SerializeField] private Transform tfmPopup;
        [SerializeField] private Canvas canvasGroup;
        [SerializeField] private List<ItemTutorialJourney> lstItemTutorialWeekly;
        [SerializeField] private bool isOpen;

        private void Start()
        {
            Reset();
        }
        public async UniTask WaitToClose()
        {
            await UniTask.WaitUntil(() => !isOpen);

        }
        [Button("Reset")]
        public void Reset()
        {
            imgFade.gameObject.SetActive(false);
            imgFade.color = new Color(0, 0, 0, 0);
            imgTitle.transform.localScale = Vector3.zero;
            imgContent.gameObject.SetActive(false);
            btnOk.transform.localScale = Vector3.zero;
            tfmPopup.gameObject.SetActive(false);
            foreach (var item in lstItemTutorialWeekly)
            {
                item.Reset();
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        [Button("Show")]
        public async UniTask Show()
        {
            isOpen = true;
            canvasGroup.sortingOrder = 3;
            imgFade.gameObject.SetActive(true);
            await imgFade.DOFade(0.99f, 0.2f).SetEase(Ease.Linear).From(0);
            tfmPopup.gameObject.SetActive(true);
            imgContent.gameObject.SetActive(true);
           //AudioController.Instance.PlaySound(SoundName.Popup);

            await imgTitle.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();
            foreach (var item in lstItemTutorialWeekly)
            {
                //AudioController.Instance.PlaySound(SoundName.Star);

                await item.Show();
                await UniTask.WaitForSeconds(0.05f);
            }
            btnOk.gameObject.SetActive(true);
            await btnOk.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).From(Vector3.zero);
        } 
        public void OnClickOK()
        {
            Reset();
            isOpen = false;
            canvasGroup.sortingOrder = 1;

        }
        public void OnClickShow()
        {
            Show().Forget();
        }
        [Button]
        public void ForceShow()
        {
            imgContent.gameObject.SetActive(true);
            tfmPopup.gameObject.SetActive(true);
            imgFade.gameObject.SetActive(true);
            imgContent.gameObject.SetActive(true);
            btnOk.transform.localScale = Vector3.one;
            imgTitle.transform.localScale = Vector3.one;
            btnOk.gameObject.SetActive(true);
            for (int i = 0; i < lstItemTutorialWeekly.Count; i++)
            {
                var item = lstItemTutorialWeekly[i];
                item.ForceShow();
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

        }
    }

}
