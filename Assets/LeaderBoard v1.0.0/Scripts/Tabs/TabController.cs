using Cysharp.Threading.Tasks;
using EasyButtons;
using NUnit.Framework;
using ps.modules.leaderboard;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

namespace ps.modules.leaderboard
{

    public class TabController : LeaderBoardCtrBase
    {
        [SerializeField] private GameObject holder;
        [SerializeField] private TabBaseLB currentTab;
        [SerializeField] private List<TabBaseLB> lstTabs;
        [SerializeField] private TMP_Text txtTitle;
        [SerializeField] private bool inited = false;

        [SerializeField] private bool isLocked = false;

        public async UniTask Start()
        {
            await Init();
            await UniTask.WaitUntil(() => manager.GetController<LeaderBoardController>() != null && manager.GetController<LeaderBoardController>().InitData);
            ShowTab();
            HideTab();
        }
        [Button]
        public void OnNeedToReset()
        {
            inited = false;
        }
        private void InitItems()
        {
            for (int i = 0; i < lstTabs.Count; i++)
            {
                lstTabs[i].Init();
            }
            inited = true;
        }
        public void LockTab(bool value)
        {
            isLocked = value;
        }
        public void ShowTab()
        {
            if (isLocked) return;
           // if (!inited)
            {
                InitItems();
            }
            int defaultIndex = 0;
            LeaderboardManager.Instance.GetController<AdapterController>().OnShow();
            OnClickTab(lstTabs[defaultIndex]);

            holder.gameObject.SetActive(true);
        }
        public void HideTab()
        {
            if (currentTab != null)
            {
                currentTab.OnExitTab();
                currentTab = null;
            }
            holder.gameObject.SetActive(false);
        }
        public void OnClickTab(TabBaseLB tab)
        {
            if (isLocked) return;
            if (currentTab != null)
            {
                currentTab.OnExitTab();
                currentTab = null;
            }
            foreach (var t in lstTabs)
            {
                if (t == tab)
                {
                    t.GoToTab();
                    currentTab = t;
                }
            }

            switch (tab)
            {
                case TabLegend:
                    txtTitle.text = "";
                    break;
                case TabDaily:
                    txtTitle.text = "*REFRESH EVERY 20 MINUTES. UPDATED AT 7:00 GMT EVERYDAY.";
                    break;
                case TabMonthly:
                    txtTitle.text = "*REFRESH EVERY 20 MINUTES. UPDATED ON THE 1ST DAY OF EACH MONTH";
                    break;
                default:
                    txtTitle.text = "LeaderBoard";
                    break;
            }
        }
        public void ResetTabLegend()
        {
            var tabLegend = lstTabs.Find(t => t is TabLegend) as TabLegend;
            tabLegend?.InitLeaderBoard();
        }
    }
}