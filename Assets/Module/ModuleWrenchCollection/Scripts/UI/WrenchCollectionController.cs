using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Storage;
using System.Collections.Generic;

public class WrenchCollectionController : PopupBase
{
    [SerializeField] private Transform content;
    [SerializeField] private Transform buttonCancel;
    [SerializeField] private Transform buttonTutorial;
    [SerializeField] WrenchCollectionMainMenuProgressBar menuProgressBar;
    [SerializeField] WrenchCollectionTutorial tutorial;

    public static WrenchCollectionController Instance { get; private set; }

    [SerializeField] private WrenchCollectionItem collectionItem;
    [SerializeField] private Transform root;
    [SerializeField] private ScrollRect scrollRect;

    private List<WrenchCollectionItem> collectionItems = new List<WrenchCollectionItem>();

    public WrenchCollectionMainMenuProgressBar ProgressBar => menuProgressBar;
    public bool IsHaveReward;
    public WrenchCollectionTutorial Tutorial => tutorial;

    public bool IsShow;

    private void Awake()
    {
        Instance = this;

        if (WrenchCollectionService.IsShowInMain())
        {
            LoadAllWrenchCollectionItems();
        }
    }

    private void LoadAllWrenchCollectionItems()
    {
        if (WrenchCollectionService.IsShowInMain() && collectionItems.Count == 0)
        {
            WrenchCollectionData data = Db.storage.WrenchCollectionData;
            List<WrenchCollectionRewardData> rewardDatas = WrenchCollectionManager.Instance.Config.GetConfigsByGroup(data.rewardGroup);
            collectionItem.gameObject.SetActive(false);

            for (int i = 0; i < rewardDatas.Count; i++)
            {
                WrenchCollectionItem item = Instantiate(collectionItem, root);
                item.transform.SetAsFirstSibling();
                WrenchCollectionRewardData reward = WrenchCollectionManager.Instance.Config.GetConfigByIndex(i, data.rewardGroup);
                item.gameObject.SetActive(true);
                item.UpdateData(i, reward, i != 0);
                collectionItems.Add(item);
            }
        }
        else
        {
            int index = 0;

            for (int i = 0; i < collectionItems.Count; i++)
            {
                collectionItems[i].UpdateUI();

                if (WrenchCollectionService.IsClaimed(collectionItems[i].Level))
                {
                    index = i;
                }
            }

            MoveParentToChildBottom(scrollRect.content, collectionItems[index].GetComponent<RectTransform>(), scrollRect.viewport, 120);
        }
    }

    public override async UniTask Show()
    {
        Setup();
        DOShow().Forget();
    }

    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        content.localScale = Vector3.zero;
        buttonCancel.localScale = Vector3.zero;
        buttonTutorial.localScale = Vector3.zero;
    }

    async UniTask DOShow()
    {
        IsShow = true;
        scrollRect.enabled = false;

        UITopController.Instance.OnShowWeeklyTask();

        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        content.gameObject.SetActive(true);
        await content.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        buttonCancel.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        buttonTutorial.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.8f);
        LoadAllWrenchCollectionItems();

        scrollRect.enabled = true;

        buttonTutorial.GetComponent<Button>().interactable = !WrenchCollectionService.IsMaxLevel();
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    async UniTask DOHide()
    {
        buttonCancel.DOScale(0, 0.3f).SetEase(Ease.InBack);
        buttonTutorial.DOScale(0, 0.3f).SetEase(Ease.InBack);
        content.DOScale(0, 0.3f).SetEase(Ease.InBack);
        await imgFade.DOFade(0, 0.5f);
        imgFade.gameObject.SetActive(false);

        UITopController.Instance.OnShowMainMenu();
        await UniTask.Delay(500);

        if (IsHaveReward)
        {
            await MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }

        IsShow = false;
    }

    public void OnClickCancel()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Hide();
    }

    public void OnClick()
    {
        if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        Show().Forget();
    }

    public void OnClickTutorial()
    {
        if (WrenchCollectionService.IsMaxLevel())
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        tutorial.Show().Forget();
    }

    public static void MoveParentToMatchChildImmediate(
           RectTransform parent,         // e.g. content
           RectTransform target,         // child that must align
           RectTransform viewport,       // scrollRect.viewport or any reference RectTransform
           Vector2? desiredPoint = null, // desired local point in viewport (default = center (0,0))
           Camera uiCamera = null        // pass canvas.worldCamera if renderMode == ScreenSpaceCamera, otherwise null
       )
    {
        if (parent == null || target == null || viewport == null)
        {
            Debug.LogWarning("MoveParentToMatchChildImmediate: null param");
            return;
        }

        // Make sure layout info is up-to-date
        Canvas.ForceUpdateCanvases();

        // Default desired point = center of viewport in local coords (0,0)
        Vector2 desiredLocal = desiredPoint ?? Vector2.zero;

        // 1) Get world position of the target's rect center
        Vector3 targetWorldPos = target.TransformPoint(target.rect.center);

        // 2) Convert that world point to screen point (in pixels)
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, targetWorldPos);

        // 3) Convert screen point to local point in viewport's local space
        //    The 'uiCamera' parameter should be null for ScreenSpace-Overlay, or canvas.worldCamera otherwise.
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, screenPoint, uiCamera, out Vector2 targetLocalInViewport);

        // 4) Compute required offset: how far targetLocalInViewport is from desiredLocal
        Vector2 delta = targetLocalInViewport - desiredLocal;

        // 5) Apply inverse to parent.anchoredPosition so the child moves from targetLocal->desiredLocal
        //    Note: subtracting delta (parent.anchoredPosition -= delta) is the correct sign.
        parent.anchoredPosition -= delta;
    }

    public static void MoveParentToChildBottom(
       RectTransform parent,
       RectTransform target,
       RectTransform viewport,
       float bottomOffset = 20f, // khoảng cách lên khỏi mép dưới
       Camera uiCamera = null
   )
    {
        if (parent == null || target == null || viewport == null)
        {
            Debug.LogWarning("MoveParentToChildBottom: missing references");
            return;
        }

        // Update layout
        Canvas.ForceUpdateCanvases();

        // Lấy vị trí world của item con (trung tâm rect)
        Vector3 targetWorldPos = target.TransformPoint(target.rect.center);

        // Convert sang local trong viewport
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            viewport,
            RectTransformUtility.WorldToScreenPoint(uiCamera, targetWorldPos),
            uiCamera,
            out Vector2 targetLocalInViewport
        );

        // Điểm mong muốn: cách mép dưới viewport một khoảng bottomOffset
        Vector2 desiredLocal = new Vector2(0, -viewport.rect.height / 2 + bottomOffset);

        // Khoảng lệch
        Vector2 delta = targetLocalInViewport - desiredLocal;

        // Dịch parent ngược lại
        parent.anchoredPosition -= delta;
    }
}