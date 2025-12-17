using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PopupLevelBonus : MonoBehaviour
{
    [SerializeField] private Image imgFade;
    [SerializeField] private RectTransform popupBonusLevel;
    [SerializeField] private RectTransform rtfmTag;       // TAG BONUS
    [SerializeField] private TMP_Text txtTime;       // TAG BONUS
    [SerializeField] private Image imgCharacter;
    [SerializeField] private Image imgReward;
    [SerializeField] private Button btnGO;

    private bool isShowPopup = false;

    // ============================================================
    // SHOW POPUP
    // ============================================================
    [EasyButtons.Button]
    public async UniTask ShowBonusLevel()
    {
        isShowPopup = true;
        var remote = GameAnalyticController.Instance.Remote();
        txtTime.text = $"{remote.BonusTime}s";
        // Reset state
        popupBonusLevel.localScale = Vector3.zero;
        imgFade.color = new Color(0, 0, 0, 0);
        //imgCharacter.color = new Color(1, 1, 1, 0);
        imgReward.color = new Color(1, 1, 1, 0);
        btnGO.gameObject.SetActive(false);
        rtfmTag.localScale = Vector3.zero;
        // Reset TAG


        popupBonusLevel.gameObject.SetActive(true);
        imgFade.gameObject.SetActive(true);

        // --------------------------------------------------
        // 1. Fade background
        // --------------------------------------------------
        await imgFade.DOFade(0.6f, 0.25f).ToUniTask();

        // --------------------------------------------------
        // 2. Popup scale in
        // --------------------------------------------------
        if (rtfmTag != null)
        {
            rtfmTag.DOScale(1f, 1).From(Vector3.zero)
                   .SetEase(Ease.OutBack); 
            float angle = 15f;
            float duration = 0.15f;
            var seq = DOTween.Sequence();
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, angle), duration).SetEase(Ease.OutQuad));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, -angle), duration).SetEase(Ease.OutQuad));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, angle * 0.5f), duration * 0.8f));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, -angle), duration).SetEase(Ease.OutQuad));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, angle), duration).SetEase(Ease.OutQuad));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, -angle), duration).SetEase(Ease.OutQuad));
            seq.Append(rtfmTag.DOLocalRotate(new Vector3(0, 0, angle * 0.5f), duration * 0.8f));
            seq.Append((rtfmTag.DOLocalRotate(Vector3.zero, duration * 0.8f)));


        }
        await popupBonusLevel
            .DOScale(1f, 0.45f)
            .SetEase(Ease.OutBack)
            .ToUniTask();


        // --------------------------------------------------
        // 4. Character xuất hiện
        // --------------------------------------------------
        imgCharacter.transform.localScale = Vector3.one * 0.6f;
        imgCharacter.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        imgCharacter.DOFade(1f, 0.25f);

        // --------------------------------------------------
        // 5. Reward xuất hiện
        // --------------------------------------------------
        imgReward.transform.localScale = Vector3.zero;
        imgReward.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        imgReward.DOFade(1f, 0.15f);

        AudioController.Instance.PlaySound(SoundName.COIN_LEVEL_BONUS);
        // --------------------------------------------------
        // 6. GO button xuất hiện
        // --------------------------------------------------
        await UniTask.Delay(200);
        btnGO.gameObject.SetActive(true);
        btnGO.transform.localScale = Vector3.zero;
        btnGO.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        // đợi đến khi đóng popup
        await UniTask.WaitUntil(() => isShowPopup == false);

    }

    // Called from GO button
    [EasyButtons.Button]
    public void OnClickStartLevelBonus()
    {
        HideAsync().Forget();
    }

    // ============================================================
    // HIDE POPUP
    // ============================================================
    private async UniTask HideAsync()
    {
        isShowPopup = false;

        btnGO.gameObject.SetActive(false);

        // Fade out character
        // imgCharacter.DOFade(0f, 0.2f);
        //.transform.DOScale(0.8f, 0.2f);

        // // Fade out reward
        // imgReward.DOFade(0f, 0.2f);
        // imgReward.transform.DOScale(0.8f, 0.2f);

        // Tag fade out
        if (rtfmTag != null)
        {
            /*CanvasGroup cg = rtfmTag.GetOrAddComponent<CanvasGroup>();
            // cg.DOFade(0f, 0.2f);
            // rtfmTag.DOScale(0.8f, 0.2f);*/
        }

        await UniTask.Delay(200);

        // Popup scale out
        await popupBonusLevel
            .DOScale(0f, 0.3f)
            .SetEase(Ease.InBack)
            .ToUniTask();

        popupBonusLevel.gameObject.SetActive(false);

        // Fade OUT background
        await imgFade.DOFade(0f, 0.25f).ToUniTask();
        imgFade.gameObject.SetActive(false);
    }

    // ============================================================
    // EDITOR TOOLS
    // ============================================================
#if UNITY_EDITOR
    [EasyButtons.Button("Show In Editor")]
    private void ShowInEditor()
    {
        imgFade.color = new Color(0, 0, 0, 0.6f);

        popupBonusLevel.gameObject.SetActive(true);
        popupBonusLevel.localScale = Vector3.one;

        imgCharacter.color = Color.white;
        imgReward.color = Color.white;

        if (rtfmTag != null)
        {
            var cg = rtfmTag.GetOrAddComponent<CanvasGroup>();
            cg.alpha = 1;
            rtfmTag.localScale = Vector3.one;
            //rtfmTag.anchoredPosition = Vector2.zero;
        }

        btnGO.gameObject.SetActive(true);

        EditorUtility.SetDirty(this.gameObject);
    }

    [EasyButtons.Button("Hide In Editor")]
    private void HideInEditor()
    {
        popupBonusLevel.gameObject.SetActive(false);
        imgFade.color = new Color(0, 0, 0, 0);
        btnGO.gameObject.SetActive(false);

        imgCharacter.color = new Color(1, 1, 1, 0);
        imgReward.color = new Color(1, 1, 1, 0);

        if (rtfmTag != null)
        {
            rtfmTag.localScale = Vector3.one * 0;
        }

        imgFade.gameObject.SetActive(false);

        EditorUtility.SetDirty(this.gameObject);
    }
#endif
}


// ============================================================
// Helper Extension
// ============================================================
public static class ComponentExtensions
{
    public static T GetOrAddComponent<T>(this Component c) where T : Component
    {
        var comp = c.GetComponent<T>();
        if (comp == null) comp = c.gameObject.AddComponent<T>();
        return comp;
    }
}
