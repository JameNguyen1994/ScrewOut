using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class TabLegend : TabBaseLB
    {
        [SerializeField] private ItemTabLegend prbTop3;
        [SerializeField] private Transform tfmContent;
        [SerializeField] private GameObject gobjScroll;
        [SerializeField] private List<ItemTabLegend> lstItemTop3;
        [SerializeField] private bool inited = false;
        protected override void Show()
        {
            base.Show();
           // if (!inited)
            {
                InitLeaderBoard();
            }
            gobjScroll.SetActive(true);

        }
        protected override void Hide()
        {
            base.Hide();
            gobjScroll.SetActive(false);
        }
        public void InitLeaderBoard()
        {
            inited = true;
            int maxShow = 5;
            var dataController = LeaderboardManager.Instance.GetController<LeaderboardDataController>();
            var data = dataController.GetYearlyData();
            var time = LeaderboardManager.Instance.GetController<AdapterController>().TimeAdapter.GetCurrentTime();


            var months = data.lstMonthData.FindAll(m => m.month < time.Month);

            maxShow = Mathf.Min(maxShow, months.Count);
            for (int i = 0; i<lstItemTop3.Count;i++)
            {
                if (lstItemTop3[i] != null)
                {
                    Destroy(lstItemTop3[i].gameObject);
                }
            }
            lstItemTop3.Clear();
            for (int i = 0; i < maxShow; i++)
            {
                int index = months.Count - 1 - i;
                var monthData = months[index];

                var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;

                var displayData = Combine(monthData.data.users, playerData.GetPointLegend(data.year, monthData.month));




                var top3 = Instantiate(prbTop3, tfmContent);
                top3.SetData(data.year, monthData.month, displayData.Item1, displayData.Item2);
                top3.gameObject.SetActive(true);
                lstItemTop3.Add(top3);
            }
            if (months.Count < 5)
            {

                var dataBeforeYear = dataController.GetYearlyDataBefore();
                if (dataBeforeYear != null)
                {
                    var monthsBefore = dataBeforeYear.lstMonthData;
                    for (int i = 0; i < 5 - months.Count; i++)
                    {
                        int index = monthsBefore.Count - 1 - i;
                        var monthData = monthsBefore[index];
                        var playerData = LeaderboardManager.Instance.GetController<PlayerDataManager>().CurrentUser;
                        var displayData = Combine(monthData.data.users, playerData.GetPointLegend(dataBeforeYear.year, monthData.month));

                        var top3 = Instantiate(prbTop3, tfmContent);
                        top3.SetData(dataBeforeYear.year, monthData.month, displayData.Item1, displayData.Item2);
                        top3.gameObject.SetActive(true);
                        lstItemTop3.Add(top3);
                    }
                }
            }
        }

        private (List<UserData>, int) Combine(List<UserData> data, UserData playerData)
        {
            var displayData = new List<UserData>(data);
            int index = RankCalculator.GetIndex(data, playerData);
            var playerIndex = index;
            displayData.Insert(index, playerData);

            return (displayData, playerIndex);
        }

    }
}
