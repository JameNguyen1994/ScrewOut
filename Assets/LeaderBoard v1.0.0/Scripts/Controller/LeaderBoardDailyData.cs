using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class LeaderBoardDailyData : MonoBehaviour
    {
        [SerializeField] private List<LeaderboardDataSO> lstDailyRandom;
        [SerializeField] private int index;

        private void Start()
        {
           
        }
        public LeaderboardDataSO GetRandomDailyData()
        {
            int randomIndex = Random.Range(0, lstDailyRandom.Count);
            index = PlayerPrefs.GetInt("PLAYER_DAILY_INDEX", -1);
            if (index < 0)
            {
                index = randomIndex;
                PlayerPrefs.SetInt("PLAYER_DAILY_INDEX", randomIndex);
            }
            PlayerPrefs.Save();
            if (lstDailyRandom == null || lstDailyRandom.Count == 0)
            {
                Debug.LogWarning("No daily data available.");
                return null;
            }
            return lstDailyRandom[index];
        }
        public void Reset()
        {
            int randomIndex = Random.Range(0, lstDailyRandom.Count);
            index = randomIndex;
            PlayerPrefs.SetInt("PLAYER_DAILY_INDEX", randomIndex);
            PlayerPrefs.Save();

        }

    }

}