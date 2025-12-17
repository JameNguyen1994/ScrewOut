using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeeklyQuest
{
    public class ItemTutorialWeekly : MonoBehaviour
    {
        [SerializeField] private Image imgIcon;
        //[SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private Text txtNameLegacy;
        [SerializeField] private Image imgArrow;
        [SerializeField] private string sName;

        public void Reset()
        {
            sName = txtNameLegacy.text;
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
    }
}
