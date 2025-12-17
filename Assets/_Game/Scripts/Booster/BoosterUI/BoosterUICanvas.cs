using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptsEffect;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoosterUICanvas : BoosterUIBase
{
    [SerializeField] private Image imgBooster;
    [SerializeField] private Image imgBoosterHolder;
    [SerializeField] private Text txtAmount;
    [SerializeField] private GameObject gobjAdd;
    [SerializeField] private GameObject gobjCount;
    [SerializeField] private GameObject gobjHolder;
    [SerializeField] private GameObject gobjFX;
    [SerializeField] private BoosterData boosterData;


    [InspectorName("Tutorial")]
    [SerializeField] private GameObject gobjBannerTutorial;
    [SerializeField] private GameObject gobjFadeTutorial;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtContent;
    [SerializeField] private SimpleAnimationCanvas imgHand;


    [InspectorName("Lock")]
    [SerializeField] private Text txtLevelUnlock;
    [SerializeField] private GameObject gobjLock;
    [SerializeField] private Animator animIdle;
    const string PARAM_IDLE = "Idle";
    const string PARAM_HIGHLIGHT = "Run";

    public override void SetUI(BoosterData boosterData, int amount)
    {
        this.boosterData = boosterData;

        if (imgBooster.sprite == null)
        {
            imgBooster.sprite = boosterData.sprBooster;
        }

        txtAmount.text = $"{amount}";
        txtLevelUnlock.text = $"Level {boosterData.levelUnlock}";

        bool isUnlocked = Db.storage.USER_INFO.level >= boosterData.levelUnlock;
        if (isUnlocked)
        {
            gobjAdd.SetActive(amount == 0);
            gobjCount.SetActive(amount > 0);
            gobjLock.SetActive(false);
            imgBooster.gameObject.SetActive(true);
            imgBoosterHolder.gameObject.SetActive(true);
        }
        else
        {
            gobjAdd.SetActive(false);
            gobjCount.SetActive(false);
            gobjLock.SetActive(true);
            imgBooster.gameObject.SetActive(false);
            imgBoosterHolder.gameObject.SetActive(false);


        }
        ShowBannerTutorial(false);
    }
    public override void HighLightBooster(bool moving = true)
    {
        float time = 0.25f;
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        if (moving)
        {


            sequence.Append(imgBooster.rectTransform.DOScaleY(0.8f, time));

            sequence.Append(imgBoosterHolder.rectTransform.DOAnchorPosY(60, time));
            sequence.Join(imgBooster.rectTransform.DOScaleY(1f, time));

            sequence.Join(imgBoosterHolder.rectTransform.DOLocalRotate(new Vector3(0, 0, -360), time * 2, RotateMode.FastBeyond360));
            sequence.Append(imgBoosterHolder.rectTransform.DOAnchorPosY(0, time).OnComplete(() => gobjFX.SetActive(true)));


            sequence.Append(imgBooster.rectTransform.DOScaleY(0.8f, time));
            sequence.Append(imgBooster.rectTransform.DOScaleY(1f, time));
            sequence.OnComplete(() =>
            gobjFX.SetActive(false)
            );
        }
        else
        {
            gobjFX.SetActive(true);
            DOVirtual.DelayedCall(1f, () => gobjFX.SetActive(false));
        }
        /*        sequence.Append(imgBooster.rectTransform.DOAnchorPosY(80, time));
                sequence.Join(imgBooster.rectTransform.DOLocalRotate(new Vector3(0, 0, -360), time*2, RotateMode.FastBeyond360));

                sequence.Append(imgBooster.rectTransform.DOAnchorPosY(0, 0.2f));*/
        /*  sequence.Append(imgBooster.rectTransform.DOAnchorPosX(50, 0.2f));
          sequence.Join(imgBooster.transform.DOScale(Vector3.one * 1.2f, 0.2f));
          sequence.Append(imgBooster.transform.DOScale(Vector3.one * 0.9f, 0.2f));
          sequence.Join(imgBooster.rectTransform.DOAnchorPosY(50, 0.2f));

          sequence.Append(imgBooster.transform.DOScale(Vector3.one * 1.1f, 0.2f));
          sequence.Join(imgBooster.rectTransform.DOAnchorPosY(70, 0.2f));

          sequence.Append(imgBooster.transform.DOScale(Vector3.one * 1.0f, 0.2f));
          sequence.Join(imgBooster.rectTransform.DOAnchorPosY(0, 0.2f));*/

    }
    public override void ToggleUI(bool active, bool resetUI = false)
    {
        base.ToggleUI(active, resetUI);
        if (!active)
            gobjHolder.transform.localScale = Vector3.zero;
        else
            if (resetUI)
            gobjHolder.transform.localScale = Vector3.one;

        gobjHolder.SetActive(active);
    }

    public override void ShowBannerTutorial(bool active)
    {
        base.ToggleUI(active, true);

        if (active)
        {
            imgHand.StartAnimation(-1, 0.3f);
            //  imgHand.gameObject.SetActive(true);

            if (boosterData != null && txtTitle != null && txtContent != null)
            {
                txtTitle.text = $"{boosterData.title}";
                txtContent.text = $"{boosterData.content}";
            }

            gobjBannerTutorial.SetActive(true);
            gobjFadeTutorial.gameObject.SetActive(true);
            /*  txtTitle.gameObject.SetActive(true);
              txtContent.gameObject.SetActive(true);*/
        }
        else
        {
            imgHand.gameObject.SetActive(false);
            gobjBannerTutorial.SetActive(false);
            gobjFadeTutorial.gameObject.SetActive(false);
            /*   txtTitle.gameObject.SetActive(false);
               txtContent.gameObject.SetActive(false);*/

        }

    }
    public override async UniTask ShowAnimation()
    {
        await gobjHolder.transform.DOScale(Vector3.one * 1.0f, 0.3f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
    public override async UniTask ShowHighLightIdle()
    {
        animIdle.Play(PARAM_HIGHLIGHT);
    }
}
