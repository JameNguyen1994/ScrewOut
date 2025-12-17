using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.journey
{
    public class ItemTutorialJourney : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        //[SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private Text txtNameLegacy;
        [SerializeField] private Image imgArrow;
        [SerializeField] private string sName;

        public void Reset()
        {
           // sName = txtNameLegacy.text;
            txtNameLegacy.text = string.Empty;

            imgIcon.transform.localScale = Vector3.zero;
            if (imgArrow != null)
                imgArrow.transform.localScale = Vector3.zero;

        }
        public async UniTask Show()
        {
            var timeImageArrowScale = 0.2f;
            var timeImageIconScale = 0.3f;
            var timeTextName = 0.0f;
            if (imgArrow != null)
                await imgArrow.transform.DOScale(Vector3.one, timeImageArrowScale).SetEase(Ease.OutQuad);
            await imgIcon.transform.DOScale(Vector3.one, timeImageIconScale).SetEase(Ease.OutQuad).ToUniTask();
            await txtNameLegacy.DOText(sName, timeTextName).SetEase(Ease.Linear).ToUniTask();
        }
        public void ForceShow()
        {
            if (imgArrow != null)
                
                imgArrow.transform.localScale = Vector3.one;
            txtNameLegacy.text = sName;
            imgIcon.transform.localScale = Vector3.one;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
