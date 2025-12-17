using MainMenuBar;
using ps.modules.journey;
using ps.modules.leaderboard;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabShop : MainMenuTabBase
{
    public override void Init(int index)
    {
        base.Init(index);
       // ExitThisTab();
    }
    public override void GoToThisTab()
    {
        base.GoToThisTab();
        if (ShopIAPController.Instance.Showing == false)
        {
            ShopIAPController.Instance.OnClickShowPopup(null, false);
            ShopIAPController.Instance.ShowAtHome(base.width);
        } else
        {
            Debug.Log("Showing");
        }
        int lastTabIndex = MainMenuBarController.Instance.GetBeforeTab();
        ShopTabNavigation.Instance.ShowAtHome(base.width, lastTabIndex);
    }
    public override void ExitThisTab()
    {
        ShopIAPController.Instance.OnClickCloseAtHome(base.width).Forget();
        //ShopIAPController.Instance.OnClickClose();
        int nextTabIndex = MainMenuBarController.Instance.TabController.NextTab.Index;

        ShopTabNavigation.Instance.ExitTab(base.width, nextTabIndex);
    }

    public override void DoKill()
    {
        base.DoKill();
       // ShopIAPController.Instance.DoKillNavigationAtHome();
    }
}
