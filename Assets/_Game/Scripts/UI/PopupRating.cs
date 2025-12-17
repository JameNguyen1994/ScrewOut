using PS.Utils;
using Storage;
using System.Collections.Generic;
using PS.Analytic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PopupRating : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<Button> lstStar;
    [SerializeField] private TMP_InputField ratingField;
    [SerializeField] private Button btnSubmit;
    [SerializeField] private Sprite starEnable;
    [SerializeField] private Sprite starDisable;

    [Header("Popup")]
    [SerializeField] private GameObject gobjContent;   // parent scale object
    [SerializeField] private GameObject gobjFade;
    [SerializeField] private Image imgFade;
    [SerializeField] private RectTransform imgContent;

    private bool isShowing = false;
    public bool IsShowing => isShowing;

    private Tween tweenFade;
    private Tween tweenScale;
    private int star = 0;

    private void ResetTween()
    {
        tweenFade?.Kill();
        tweenScale?.Kill();
    }

    // ==================================================
    // SHOW POPUP
    // ==================================================
    [EasyButtons.Button]
    public void ShowPopup()
    {
        ResetTween();

        gobjFade.SetActive(true);
        gobjContent.SetActive(true);

        // reset trạng thái
        imgFade.color = new Color(0, 0, 0, 0);
        imgContent.localScale = Vector3.zero;
        btnSubmit.interactable = false;
        ratingField.text = "";
        star = 0;

        // fade background
        tweenFade = imgFade.DOFade(0.7f, 0.3f)
            .SetEase(Ease.OutQuad);

        // popup scale
        tweenScale = imgContent.DOScale(1f, 0.35f)
            .SetEase(Ease.OutBack);

       // Db.storage.IS_SHOW_RATING = tr;
        isShowing = true;
    }

    // ==================================================
    // HIDE POPUP
    // ==================================================
    public void HidePopup()
    {
        ResetTween();

        // fade out
        tweenFade = imgFade.DOFade(0f, 0.25f)
            .SetEase(Ease.InQuad);

        // scale nhỏ lại
        tweenScale = imgContent.DOScale(0f, 0.25f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gobjContent.SetActive(false);
                gobjFade.SetActive(false);
            });

        Db.storage.REJECT_REVIEW_COUNT++;
        isShowing = false;
    }

    // ==================================================
    // STARS
    // ==================================================
    public void OnStarClick(int star)
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        btnSubmit.interactable = true;
        this.star = star;

        for (int i = 0; i < lstStar.Count; i++)
            lstStar[i].image.sprite = i < star ? starEnable : starDisable;
    }

    // ==================================================
    // SUBMIT REVIEW
    // ==================================================
    public void OnSubmit()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        Debug.Log("OnSubmit: " + star + " - " + ratingField.text);
        Db.storage.IS_SHOW_RATING = false;

        if (star > 3)
            InAppReviewController.Instance.ReviewRequest();

        HidePopup();
    }

    public void OnClickHide()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        HidePopup();
    }
}
