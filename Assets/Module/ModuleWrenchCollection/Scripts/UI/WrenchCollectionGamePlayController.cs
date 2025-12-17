using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Coffee.UIExtensions;

public class WrenchCollectionGamePlayController : Singleton<WrenchCollectionGamePlayController>
{
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI textWrenchAmount;
    [SerializeField] private Transform target;
    [SerializeField] private Transform targetForce;
    [SerializeField] private Transform imgIcon;
    [SerializeField] private UIParticle particle;

    public PathType PathType = PathType.CatmullRom;
    private int wrenchAmount;

    public void Init()
    {
        if (WrenchCollectionService.IsShowInMain() && !WrenchCollectionService.IsMaxLevel())
        {
            content.SetActive(true);
        }
        else
        {
            content.SetActive(false);
        }

        wrenchAmount = 0;
        UpdateUI();
    }
    public void HideUI()
    {
        content.SetActive(false);
    }
    public void Reload(int wrenchAmount)
    {
        this.wrenchAmount = wrenchAmount;
        UpdateUI();
    }

    public void UpdateUI()
    {
        DOText(textWrenchAmount, wrenchAmount.ToString(), 0.15f);
    }

    public async void CollectWrench(Wrench wrench)
    {
        AudioController.Instance.PlaySound(SoundName.ScrewDown);

        await wrench.transform.DOScale(2f, 0.05f).SetEase(Ease.OutBack);
        await wrench.transform.DOScale(1f, 0.05f).SetEase(Ease.OutQuad);

        AudioController.Instance.PlaySound(SoundName.Fly);
        wrench.transform.parent = null;
        wrench.Animator.enabled = true;
        target.parent = null;

        wrench.transform.rotation = Quaternion.identity;
        Vector3 start = wrench.transform.position;
        Vector3 end = target.position;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(start);
        Vector3 mid = new Vector3(viewportPos.x < 0.5f ? (start.x + Random.Range(5, 10)) : (start.x + Random.Range(-10, -5)), start.y + 10, start.z - 20);

        wrench.UpdateParentRootTween();
        await wrench.transform.DOPath(new Vector3[] { start, (start + mid) / 2f + Vector3.up * -5, mid }, 0.85f, PathType);
        await wrench.transform.DOPath(new Vector3[] { mid, (mid + end) / 2f + Vector3.up * 2, end }, 0.35f, PathType)
           .SetEase(Ease.InOutFlash)
           .OnUpdate(() =>
           {
               wrench.UpdateParentRoot();
           })
           .OnComplete(() =>
           {
               wrench.gameObject.SetActive(false);
           });

        AudioController.Instance.PlaySound(SoundName.CollectExp);

        await imgIcon.DOScale(2f, 0.1f).SetEase(Ease.OutBack);
        imgIcon.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);

        particle.Play();

        wrenchAmount++;
        UpdateUI();
    }

    public void OnWinGame()
    {
        WrenchCollectionService.CollectWrench(wrenchAmount);
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