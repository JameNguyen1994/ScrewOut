using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using UnityEngine;

public class CoreRetentionMainMenuElementController : MonoBehaviour
{
    [SerializeField] private RectTransform mainMenuBar;
    [SerializeField] private RectTransform middleLeft;
    [SerializeField] private RectTransform middleRight;
    [SerializeField] private RectTransform btnPlay;
    [SerializeField] private RectTransform btnSetting;
    [SerializeField] private RectTransform btnShowElement;

    [SerializeField] private RectTransform coreRetentionContent;
    [SerializeField] private RectTransform coreRetentionReward;
    [SerializeField] private RectTransform BG;

    [SerializeField] private RectTransform wrenchCollectionControllerProgressBar;
    [SerializeField] private List<RatioData<float>> coreRetentionRewardTarget;
    [SerializeField] private List<RatioData<float>> coreRetentionContentScale;
    [SerializeField] private List<RatioData<float>> coreRetentionContentTarget;

    [SerializeField] private List<RatioData<Vector2>> coreRetentionRewardPosition;

    private Dictionary<string, Vector2> originsPosistion = new Dictionary<string, Vector2>();
    private float playTimeDuration = 0.5f;

    public bool IsHideElement;

    private void Awake()
    {
        btnShowElement.gameObject.SetActive(false);
    }

    public void Init()
    {
        if (originsPosistion.Count > 0) return;

        originsPosistion.Add(nameof(mainMenuBar), mainMenuBar.anchoredPosition);
        originsPosistion.Add(nameof(middleLeft), middleLeft.anchoredPosition);
        originsPosistion.Add(nameof(middleRight), middleRight.anchoredPosition);
        originsPosistion.Add(nameof(btnPlay), btnPlay.anchoredPosition);
        originsPosistion.Add(nameof(wrenchCollectionControllerProgressBar), wrenchCollectionControllerProgressBar.anchoredPosition);
        originsPosistion.Add(nameof(btnSetting), btnSetting.anchoredPosition);
        originsPosistion.Add(nameof(coreRetentionReward), RatioService.GetValue(coreRetentionRewardPosition, coreRetentionReward.anchoredPosition));
        originsPosistion.Add(nameof(BG), BG.anchoredPosition);
        originsPosistion.Add(nameof(coreRetentionContent), coreRetentionContent.anchoredPosition);
    }

    [Button]
    public async UniTask ShowElement()
    {
        if (!IsHideElement) return;

        BlockController.Instance.AddBlockLayer();
        UITopController.Instance.OnFocusObject(false, playTimeDuration);

        coreRetentionContent.DOScale(Vector3.one, playTimeDuration);
        BG.DOScale(Vector3.one, playTimeDuration);

        mainMenuBar.DOAnchorPos(originsPosistion[nameof(mainMenuBar)], playTimeDuration);
        middleLeft.DOAnchorPos(originsPosistion[nameof(middleLeft)], playTimeDuration);
        middleRight.DOAnchorPos(originsPosistion[nameof(middleRight)], playTimeDuration);
        wrenchCollectionControllerProgressBar.DOAnchorPos(originsPosistion[nameof(wrenchCollectionControllerProgressBar)], playTimeDuration);
        btnSetting.DOAnchorPos(originsPosistion[nameof(btnSetting)], playTimeDuration);
        btnPlay.DOAnchorPos(originsPosistion[nameof(btnPlay)], playTimeDuration);
        coreRetentionReward.DOAnchorPos(originsPosistion[nameof(coreRetentionReward)], playTimeDuration);
        BG.DOAnchorPos(originsPosistion[nameof(BG)], playTimeDuration);
        coreRetentionContent.DOAnchorPos(originsPosistion[nameof(coreRetentionContent)], playTimeDuration);

        btnShowElement.gameObject.SetActive(false);

        await UniTask.Delay((int)(playTimeDuration * 1000));

        BlockController.Instance.RemoveBlockLayer();
        IsHideElement = false;
    }

    [Button]
    public async UniTask HideElement()
    {
        Init();

        if (IsHideElement) return;

        IsHideElement = true;
        BlockController.Instance.AddBlockLayer();
        UITopController.Instance.OnFocusObject(true, playTimeDuration);

        coreRetentionContent.DOScale(Vector3.one * RatioService.GetValue(coreRetentionContentScale, 1.45f), playTimeDuration);
        BG.DOScale(Vector3.one * RatioService.GetValue(coreRetentionContentScale, 1.45f), playTimeDuration);

        mainMenuBar.DOAnchorPos(originsPosistion[nameof(mainMenuBar)] + new Vector2(0, -500), playTimeDuration);
        middleLeft.DOAnchorPos(originsPosistion[nameof(middleLeft)] + new Vector2(-300, 0), playTimeDuration);
        middleRight.DOAnchorPos(originsPosistion[nameof(middleRight)] + new Vector2(300, 0), playTimeDuration);
        wrenchCollectionControllerProgressBar.DOAnchorPos(originsPosistion[nameof(wrenchCollectionControllerProgressBar)] + new Vector2(0, 550), playTimeDuration);
        btnSetting.DOAnchorPos(originsPosistion[nameof(btnSetting)] + new Vector2(0, 550), playTimeDuration);
        btnPlay.DOAnchorPos(originsPosistion[nameof(btnPlay)] + new Vector2(0, -800), playTimeDuration);
        coreRetentionReward.DOAnchorPos(originsPosistion[nameof(coreRetentionReward)] + new Vector2(0, RatioService.GetValue(coreRetentionRewardTarget, -350f)), playTimeDuration);

        BG.DOAnchorPos(originsPosistion[nameof(BG)] + new Vector2(0, RatioService.GetValue(coreRetentionContentTarget, -100)), playTimeDuration);
        coreRetentionContent.DOAnchorPos(originsPosistion[nameof(coreRetentionContent)] + new Vector2(0, RatioService.GetValue(coreRetentionContentTarget, -100)), playTimeDuration);

        btnShowElement.gameObject.SetActive(true);
        btnShowElement.transform.DOScale(1.25f, 0.1f).SetEase(Ease.OutBack);
        btnShowElement.transform.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);

        await UniTask.Delay((int)(playTimeDuration * 1000));

        BlockController.Instance.RemoveBlockLayer();
    }

    public void LevelUpEventSetup()
    {
        Init();

        IsHideElement = true;
        UITopController.Instance.OnFocusObject(true, 0);

        coreRetentionContent.localScale = Vector3.one * RatioService.GetValue(coreRetentionContentScale, 1.45f);
        BG.localScale = Vector3.one * RatioService.GetValue(coreRetentionContentScale, 1.45f);

        mainMenuBar.anchoredPosition = originsPosistion[nameof(mainMenuBar)] + new Vector2(0, -500);
        middleLeft.anchoredPosition = originsPosistion[nameof(middleLeft)] + new Vector2(-300, 0);
        middleRight.anchoredPosition = originsPosistion[nameof(middleRight)] + new Vector2(300, 0);
        wrenchCollectionControllerProgressBar.anchoredPosition = originsPosistion[nameof(wrenchCollectionControllerProgressBar)] + new Vector2(0, 550);
        btnSetting.anchoredPosition = originsPosistion[nameof(btnSetting)] + new Vector2(0, 550);
        btnPlay.anchoredPosition = originsPosistion[nameof(btnPlay)] + new Vector2(0, -800);
        coreRetentionReward.anchoredPosition = originsPosistion[nameof(coreRetentionReward)] + new Vector2(0, RatioService.GetValue(coreRetentionRewardTarget, -350f));

        BG.anchoredPosition = originsPosistion[nameof(BG)] + new Vector2(0, RatioService.GetValue(coreRetentionContentTarget, -100));
        coreRetentionContent.anchoredPosition = originsPosistion[nameof(coreRetentionContent)] + new Vector2(0, RatioService.GetValue(coreRetentionContentTarget, -100));
    }
}