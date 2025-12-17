using DG.Tweening;
using Life;
using PS.Analytic;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeeklyQuest;

public class TabWeeklyQuest : MainMenuTabBase
{
    [SerializeField] private Image imgBGPanel;

    public override void Init(int index)
    {
        base.Init(index);
        // GoToThisTab();

    }
    public override void GoToThisTab()
    {
        imgBGPanel.DOKill();
        imgBGPanel.gameObject.SetActive(true);

        Debug.Log("Go to Weekly Quest Tab");
        base.GoToThisTab();
        UITopController.Instance.OnShowWeeklyTask();

        imgBGPanel.DOFade(0f, 0.5f).OnComplete(() =>
        {
            imgBGPanel.gameObject.SetActive(true);
            WeeklyQuestManager.Instance.WeeklyQuestController.CheckToShowTutorial(() =>
            {
                WeeklyQuestManager.Instance.WeeklyQuestController.OnShow();
            });

        }).From(0);
        WeeklyQuestManager.Instance.WeeklyTabNavigation.ShowAtHome(base.width);

    }

    public override void ExitThisTab()
    {
        imgBGPanel.DOKill();
        imgBGPanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            imgBGPanel.gameObject.SetActive(false);
        });
        Debug.Log("Exit Weekly Quest Tab");
        WeeklyQuestManager.Instance.WeeklyTabNavigation.ExitTab(base.width);
    }

    public void OnClickPlay()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        if (DBLifeController.Instance.LIFE_INFO.lifeAmount > 0 || DBLifeController.Instance.LIFE_INFO.timeInfinity > 0)
        {
            var scene = GameAnalyticController.Instance.Remote().ModeGamePlayControl == 2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
            TrackingController.Instance.TrackingStartSession();
            UITopController.Instance.OnStartGameplay();
            SceneController.Instance.ChangeScene(scene);
        }
        else
        {
            LifeController.Instance.ShowPopupLife();
        }
    }
}
