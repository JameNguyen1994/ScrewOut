using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinItemTutorial : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private Image imgArrow;

    private string textValue;

    public void ResetUI()
    {
        textValue = txtName.text;
        txtName.text = string.Empty;

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
        {
            await imgArrow.transform.DOScale(Vector3.one, timeImageArrowScale).SetEase(Ease.OutQuad);
        }
        await imgIcon.transform.DOScale(Vector3.one, timeImageIconScale).SetEase(Ease.OutQuad).ToUniTask();
        await DOText(txtName, textValue, timeTextName).SetEase(Ease.Linear).ToUniTask();
    }

    public static TweenerCore<string, string, StringOptions> DOText(TextMeshProUGUI target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
    {
        if (endValue == null)
        {
            if (Debugger.logPriority > 0)
            {
                Debugger.LogWarning("You can't pass a NULL string to DOText: an empty string will be used instead to avoid errors");
            }

            endValue = "";
        }

        TweenerCore<string, string, StringOptions> t = DOTween.To(() => target.text, x => target.text = x, endValue, duration);
        t.SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(target);

        return t;
    }
}