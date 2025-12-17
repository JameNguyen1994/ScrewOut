using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using MainMenuBar;
using ps.modules.leaderboard;
using ps.modules.journey;
using PS.Analytic;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeeklyQuest;

public class TabJourney : MainMenuTabBase
{
    [SerializeField] private Image imgBGPanel;

    private void Start()
    {
        // JourneyTabNavigation.Instance.ExitTab(base.width, 0);
    }
    public override void Init(int index)
    {
        base.Init(index);
     //   ExitThisTab();

    }
    public override void GoToThisTab()
    {
        imgBGPanel.DOKill();
        imgBGPanel.gameObject.SetActive(true);

        Debug.Log("Go to Weekly Quest Tab");
        base.GoToThisTab();
        UITopController.Instance.OnShowWeeklyTask();

        imgBGPanel.DOFade(0.9f, 0.5f).OnComplete(() =>
        {
            imgBGPanel.gameObject.SetActive(true);

        }).From(0);
        //WeeklyQuestManager.Instance.WeeklyTabNavigation.ShowAtHome(base.width);
        int lastTabIndex = MainMenuBarController.Instance.GetBeforeTab();
        JourneyTabNavigation.Instance.ShowAtHome(base.width, lastTabIndex);
    }

    public override void ExitThisTab()
    {
        imgBGPanel.DOKill();
        imgBGPanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            imgBGPanel.gameObject.SetActive(false);

        });
        Debug.Log("Exit Weekly Quest Tab");


        int nextTabIndex = MainMenuBarController.Instance.TabController.NextTab.Index;

        JourneyTabNavigation.Instance.ExitTab(base.width, nextTabIndex);

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
