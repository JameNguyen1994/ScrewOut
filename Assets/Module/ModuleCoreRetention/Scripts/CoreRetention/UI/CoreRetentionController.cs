using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using Storage.Model;
using UnityEngine;

public class CoreRetentionController : Singleton<CoreRetentionController>
{
    //Ray
    [SerializeField] private RectTransform rayConveyorParent;
    [SerializeField] private RectTransform rayConveyor0;
    [SerializeField] private RectTransform rayConveyor1;
    [SerializeField] private RectTransform rayConveyor2;

    //Scroll
    [SerializeField] private RectTransform scrollContent;

    //Cap
    [SerializeField] private CoreRetentionCap cap0;
    [SerializeField] private CoreRetentionCap cap1;
    [SerializeField] private CoreRetentionCap cap2;
    [SerializeField] private CoreRetentionCap capTemp;

    //Switch
    [SerializeField] private CoreRetentionSwitch retentionSwitch;

    //Reward
    [SerializeField] private CoreRetentionReward coreRetentionReward;
    [SerializeField] private CoreRetentionMainMenuElementController menuElementController;

    public CoreRetentionMainMenuElementController MenuElementController => menuElementController;

    //Effect Flare
    [SerializeField] private GameObject[] flareEffects;

    private int currentLevel = 0;
    private float moveDuration = 0.35f;
    private int height = 140;
    private CoreRetentionCap temp;
    private UserInfo userInfo;

    protected override void CustomAwake()
    {
        base.CustomAwake();
        userInfo = Db.storage.USER_INFO;

        if (CoreRetentionService.IsWinLevelEventActive())
        {
            LevelUpEventSetup();
        }
        else
        {
            InitData();
        }
    }

    private void EndMove()
    {
        rayConveyor0.parent = rayConveyorParent.parent;
        rayConveyor1.parent = rayConveyorParent.parent;
        rayConveyor2.parent = rayConveyorParent.parent;

        rayConveyorParent.anchoredPosition = new Vector2(0, height);

        rayConveyor0.parent = rayConveyorParent;
        rayConveyor1.parent = rayConveyorParent;
        rayConveyor2.parent = rayConveyorParent;

        rayConveyor0.anchoredPosition = new Vector2(-1080, 0);
        rayConveyor1.anchoredPosition = new Vector2(0, 0);
        rayConveyor2.anchoredPosition = new Vector2(1080, 0);

        //AudioController.Instance.PlaySound(SoundName.Coin);

        UpdateUI();

        BlockController.Instance.RemoveBlockLayer();
    }

    public void InitData()
    {
        currentLevel = userInfo.level;
        UpdateUI();
    }

    private void UpdateUI()
    {
        cap0.UpdateData(currentLevel - 1, userInfo.level);
        cap1.UpdateData(currentLevel, userInfo.level);
        cap2.UpdateData(currentLevel + 1, userInfo.level);

        coreRetentionReward.UpdateUI(currentLevel, userInfo.level);

        AutoPlayIdle();
    }

    [EasyButtons.Button]
    public async UniTask MoveLeft()
    {
        BlockController.Instance.AddBlockLayer();

        CancelAutoPlayIdle();

        AudioController.Instance.PlaySound(SoundName.CAP_SWIPE);

        if (cap2.IsBorderCap)
        {
            await ScrollBack();
            return;
        }

        capTemp.UpdateData(currentLevel + 2, userInfo.level);
        capTemp.RectTransform.anchoredPosition = new Vector2(1360, height);

        cap0.RectTransform.DOAnchorPosX(-1360, moveDuration);
        cap1.RectTransform.DOAnchorPosX(-680, moveDuration);
        cap2.RectTransform.DOAnchorPosX(0, moveDuration);
        capTemp.RectTransform.DOAnchorPosX(680, moveDuration);

        scrollContent.DOAnchorPosX(0, moveDuration);

        await rayConveyorParent.DOAnchorPosX(rayConveyorParent.anchoredPosition.x - 1080, moveDuration);

        cap0.RectTransform.anchoredPosition = new Vector2(-1360, height);
        cap1.RectTransform.anchoredPosition = new Vector2(-680, height);
        cap2.RectTransform.anchoredPosition = new Vector2(0, height);
        capTemp.RectTransform.anchoredPosition = new Vector2(680, height);

        temp = capTemp;

        capTemp = cap0;
        cap0 = cap1;
        cap1 = cap2;
        cap2 = temp;

        currentLevel++;
        currentLevel = currentLevel == userInfo.level + 6 ? 1 : currentLevel;

        EndMove();
    }

    public async UniTask ScrollBack()
    {
        cap0.RectTransform.DOAnchorPosX(-680, moveDuration);
        cap1.RectTransform.DOAnchorPosX(0, moveDuration);
        cap2.RectTransform.DOAnchorPosX(680, moveDuration);

        scrollContent.DOAnchorPosX(0, moveDuration);

        await rayConveyorParent.DOAnchorPosX(0, moveDuration);

        EndMove();
    }


    [EasyButtons.Button]
    public async UniTask MoveRight()
    {
        BlockController.Instance.AddBlockLayer();

        CancelAutoPlayIdle();

        AudioController.Instance.PlaySound(SoundName.CAP_SWIPE);

        if (cap0.IsBorderCap)
        {
            await ScrollBack();
            return;
        }

        capTemp.UpdateData(currentLevel - 2, userInfo.level);
        capTemp.RectTransform.anchoredPosition = new Vector2(-1360, height);

        cap0.RectTransform.DOAnchorPosX(0, moveDuration);
        cap1.RectTransform.DOAnchorPosX(680, moveDuration);
        cap2.RectTransform.DOAnchorPosX(1360, moveDuration);
        capTemp.RectTransform.DOAnchorPosX(-680, moveDuration);

        scrollContent.DOAnchorPosX(0, moveDuration);

        await rayConveyorParent.DOAnchorPosX(rayConveyorParent.anchoredPosition.x + 1080, moveDuration);

        capTemp.RectTransform.anchoredPosition = new Vector2(-680, height);
        cap0.RectTransform.anchoredPosition = new Vector2(0, height);
        cap1.RectTransform.anchoredPosition = new Vector2(680, height);
        cap2.RectTransform.anchoredPosition = new Vector2(1360, height);

        temp = capTemp;

        capTemp = cap2;
        cap2 = cap1;
        cap1 = cap0;
        cap0 = temp;

        currentLevel--;
        currentLevel = currentLevel == 0 ? userInfo.level + 5 : currentLevel;

        EndMove();
    }

    public void LevelUpEventSetup()
    {
        currentLevel = userInfo.level - 1;

        cap0.UpdateData(currentLevel - 1, userInfo.level);
        cap1.UpdateData(currentLevel, currentLevel);
        cap2.UpdateData(currentLevel + 1, userInfo.level);

        coreRetentionReward.UpdateUI(currentLevel, userInfo.level);
        coreRetentionReward.gameObject.SetActive(false);
    }

    public async UniTask LevelUpEvent()
    {
        CancelAutoPlayIdle();

        //Play Anim
        await UniTask.Delay(500);
        menuElementController.LevelUpEventSetup();
        await UniTask.Delay(500);
        await LoadingFade.Instance.HideLoadingFade();

        SpeedGameManager.Instance.Active(true);

        AudioController.Instance.PlaySound(SoundName.Anim_Box_Open);
        await PlayCapAnim(cap1, CoreRetentionDefine.END_GAME, 2);
        //AudioController.Instance.PlaySound(SoundName.Anim_Box_Move);
        await UniTask.Delay(1000);
        AudioController.Instance.PlaySound(SoundName.CAP_SWIPE);
        coreRetentionReward.gameObject.SetActive(true);
        await menuElementController.ShowElement();
        await UniTask.Delay(500);
        await retentionSwitch.MoveToNewLevel();
        await UniTask.Delay(500);

        //Update Done Event
        CoreRetentionService.DoneWinLevelEvent();
    }

    private async UniTask PlayCapAnim(CoreRetentionCap cap, string anim, int delay)
    {
        EditorLogger.Log("[CoreRetentionCap] PlayCapAnim: " + anim + "  delay: " + delay);
        cap.PlayAnim(anim);
        await UniTask.Delay(delay * 1000);
        cap.EndAnim();
    }

    private void AutoPlayIdle()
    {
        CancelAutoPlayIdle();
        int delay = Random.Range(5, 11);
        EditorLogger.Log("[CoreRetentionCap] AutoPlayIdle  delay: " + delay);
        Invoke(nameof(PlayIdle), delay);
    }

    private void CancelAutoPlayIdle()
    {
        EditorLogger.Log("[CoreRetentionCap] CancelAutoPlayIdle");
        CancelInvoke(nameof(PlayIdle));
    }

    [EasyButtons.Button]
    private void PlayIdle()
    {
        PlayCapAnim(cap1, CoreRetentionDefine.IDLE, 2);
        AutoPlayIdle();
    }

    public void UpdateWrenchReward()
    {
        coreRetentionReward.UpdateEXPReward();
    }

    public void OnShowUI()
    {
        for (int i = 0; i < flareEffects.Length; i++)
        {
            flareEffects[i].SetActive(true);
        }
    }

    public void OnHideUI()
    {
        for (int i = 0; i < flareEffects.Length; i++)
        {
            flareEffects[i].SetActive(false);
        }
    }

    public async UniTask MoveToCurentLevel(System.Action oncomplete)
    {
        if (userInfo.level > currentLevel)
        {
            await MoveToLeft(userInfo.level, oncomplete);
        }
        else
        {
            await MoveToRight(userInfo.level, oncomplete);
        }
    }

    [EasyButtons.Button]
    public async UniTask MoveToRight(int index, System.Action oncomplete)
    {
        if (index == currentLevel)
        {
            return;
        }

        BlockController.Instance.AddBlockLayer();

        CancelAutoPlayIdle();

        AudioController.Instance.PlaySound(SoundName.CAP_SWIPE);

        cap0.UpdateData(index, userInfo.level);
        capTemp.UpdateData(index - 1, userInfo.level);
        capTemp.RectTransform.anchoredPosition = new Vector2(-1360, height);

        cap0.RectTransform.DOAnchorPosX(0, moveDuration);
        cap1.RectTransform.DOAnchorPosX(680, moveDuration);
        cap2.RectTransform.DOAnchorPosX(1360, moveDuration);
        capTemp.RectTransform.DOAnchorPosX(-680, moveDuration);

        scrollContent.DOAnchorPosX(0, moveDuration);

        await rayConveyorParent.DOAnchorPosX(rayConveyorParent.anchoredPosition.x + 1080, moveDuration);

        capTemp.RectTransform.anchoredPosition = new Vector2(-680, height);
        cap0.RectTransform.anchoredPosition = new Vector2(0, height);
        cap1.RectTransform.anchoredPosition = new Vector2(680, height);
        cap2.RectTransform.anchoredPosition = new Vector2(1360, height);

        temp = capTemp;

        capTemp = cap2;
        cap2 = cap1;
        cap1 = cap0;
        cap0 = temp;

        currentLevel = index;

        await UniTask.Delay(250);

        EndMove();

        oncomplete?.Invoke();
    }

    public async UniTask MoveToLeft(int index, System.Action oncomplete)
    {
        if (index == currentLevel)
        {
            return;
        }

        BlockController.Instance.AddBlockLayer();

        CancelAutoPlayIdle();

        AudioController.Instance.PlaySound(SoundName.CAP_SWIPE);

        cap2.UpdateData(index, userInfo.level);
        capTemp.UpdateData(index + 1, userInfo.level);
        capTemp.RectTransform.anchoredPosition = new Vector2(1360, height);

        cap0.RectTransform.DOAnchorPosX(-1360, moveDuration);
        cap1.RectTransform.DOAnchorPosX(-680, moveDuration);
        cap2.RectTransform.DOAnchorPosX(0, moveDuration);
        capTemp.RectTransform.DOAnchorPosX(680, moveDuration);

        scrollContent.DOAnchorPosX(0, moveDuration);

        await rayConveyorParent.DOAnchorPosX(rayConveyorParent.anchoredPosition.x - 1080, moveDuration);

        cap0.RectTransform.anchoredPosition = new Vector2(-1360, height);
        cap1.RectTransform.anchoredPosition = new Vector2(-680, height);
        cap2.RectTransform.anchoredPosition = new Vector2(0, height);
        capTemp.RectTransform.anchoredPosition = new Vector2(680, height);

        temp = capTemp;

        capTemp = cap0;
        cap0 = cap1;
        cap1 = cap2;
        cap2 = temp;

        currentLevel = index;

        await UniTask.Delay(250);

        EndMove();

        oncomplete?.Invoke();
    }
}