using Cysharp.Threading.Tasks;
using DG.Tweening;
using MainMenuBar;
using Storage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeeklyQuest;

public class PopupMissionStartGame : PopupBase
{
    [SerializeField] private Transform tfmContent;
    [SerializeField] private Transform tfmHard, tfmSuperHard;
    //  [SerializeField] private Transform tfmCharacterNormal, tfmCharacterHard, tfmCharacterSuperHard;
    [SerializeField] private Image imgChaNormal, imgChaHard, imgChaSuperHard;
    [SerializeField] private Transform tfmScrew, tfmWeeklyQuestItem;
    [SerializeField] private ItemQuest itemQuest;
    [SerializeField] private TextMeshProUGUI txtLevel, txtScrew;
    [SerializeField] private Transform flyScrew;
    [SerializeField] private Transform screwFlyTarget;

    [Header("Special UI")]
    [SerializeField] private Image imgPanel;
    [SerializeField] private Sprite sprNormal, sprHard;


    public override async UniTask Show()
    {
        Setup();
        await DOShow();
    }

    void Setup()
    {
        var imgCoverColor = imgFade.color;
        imgCoverColor.a = 0;
        imgFade.color = imgCoverColor;
        imgFade.gameObject.SetActive(false);
        tfmHard.localScale = Vector3.zero;
        tfmSuperHard.localScale = Vector3.zero;
        tfmHard.gameObject.SetActive(false);
        tfmSuperHard.gameObject.SetActive(false);
        GetWeeklyQuest().Forget();
        tfmScrew.localScale = Vector3.zero;
        tfmContent.localScale = Vector3.zero;
        tfmWeeklyQuestItem.localScale = Vector3.zero;
        txtLevel.text = $"LEVEL {Db.storage.USER_INFO.level}";
        txtScrew.text = "0";

        imgChaNormal.gameObject.SetActive(false);
        imgChaHard.gameObject.SetActive(false);
        imgChaSuperHard.gameObject.SetActive(false);

        LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(Db.storage.USER_INFO.level);
        Debug.Log($"Current Level Difficulty: {levelDifficulty}");
        switch (levelDifficulty)
        {
            case LevelDifficulty.Easy:
            case LevelDifficulty.Normal:
                imgPanel.sprite = sprNormal;
                imgChaNormal.gameObject.SetActive(true);
                break;

            case LevelDifficulty.Hard:
                imgPanel.sprite = sprHard;
                imgChaHard.gameObject.SetActive(true);
                break;
                /*            case LevelDifficulty.Hard:
                                imgChaSuperHard.gameObject.SetActive(true);
                                break;*/
        }
    }

    async UniTask DOShow()
    {

        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.98f, 0.5f);
        await UniTask.Delay(400);
        AudioController.Instance.PlaySound(SoundName.StartLevel);




        await tfmContent.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.7f);
        await tfmScrew.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        int screwCount = LevelController.Instance.Level.LstScrew.Count;
        DOVirtual.Int(0, screwCount, 1, value =>
        {
            txtScrew.text = $"{value}";
        });

        if (LevelController.Instance.Level.LevelDifficulty == LevelDifficulty.Hard)
        {
            tfmHard.gameObject.SetActive(true);
            tfmHard.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.7f);
        }
        //else if (LevelController.Instance.Level.LevelDifficulty == LevelDifficulty.Hard)
        //{
        //    tfmSuperHard.gameObject.SetActive(true);
        //    tfmSuperHard.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.7f);
        //}
        if (Db.storage.USER_INFO.level >= DBMainMenuBarController.Instance.DB_MAIN_MENU_ITEMS.lstDBBarItem[4].levelUnlock)
        {

            await tfmWeeklyQuestItem.DOScale(1, 0.3f).SetEase(Ease.OutBack).From(0.7f);
            await itemQuest.OnlyShowCurrentPoint();
        }
        await UniTask.Delay(2000);
        await DOHide();
    }

    async UniTask GetWeeklyQuest()
    {
        var questInfo = await WeeklyQuestManager.Instance.WeeklyQuestController.GetClosestToCompletionQuestAsync();
        itemQuest.SetData(questInfo);
        if (Db.storage.USER_INFO.level == 1 || questInfo == null || questInfo.isComplete)
        {
            Debug.LogWarning("No weekly quest available.");
            itemQuest.gameObject.SetActive(false);
            return;
        }
    }

    public override void Hide()
    {
        DOHide().Forget();
    }

    async UniTask DOHide()
    {
        imgChaNormal.DOFillAmount(0, 0.2f);
        imgChaHard.DOFillAmount(0, 0.2f);
        imgChaSuperHard.DOFillAmount(0, 0.2f);

        tfmWeeklyQuestItem.DOScale(0, 0.2f).SetEase(Ease.InBack);
        await UniTask.Delay(150);
        txtScrew.gameObject.SetActive(false);
        tfmContent.DOScale(0, 0.2f).SetEase(Ease.OutBack);
        flyScrew.gameObject.SetActive(true);
        await UniTask.Delay(200);
        await FlyScrew();

    }

    async UniTask FlyScrew()
    {
        Vector3 middlePoint = (flyScrew.position + screwFlyTarget.position) / 2;
        middlePoint.x -= 100;
        middlePoint.y -= 60;
        Vector3[] path = new[]
        {
            flyScrew.position,
            middlePoint,
            screwFlyTarget.position
        };
        var lstTask = new List<UniTask>();
        lstTask.Add(imgFade.DOFade(0f, 0.3f).ToUniTask());
        lstTask.Add(flyScrew.DOScale(0.5f, 0.5f).ToUniTask());
        AudioController.Instance.PlaySound(SoundName.Fly);
        await flyScrew.DOPath(path, 0.5f, PathType.CatmullRom)
            .SetEase(Ease.Linear);
        var shakeTask = screwFlyTarget.DOShakeScale(0.2f, new Vector3(0.15f, 0.15f, 0.15f));
        lstTask.Add(shakeTask.ToUniTask());
        flyScrew.gameObject.SetActive(false);
        imgFade.gameObject.SetActive(false);

        await UniTask.WhenAll(lstTask);
        AudioController.Instance.PlaySound(SoundName.LevelStart_ScrewDown);
    }
}
