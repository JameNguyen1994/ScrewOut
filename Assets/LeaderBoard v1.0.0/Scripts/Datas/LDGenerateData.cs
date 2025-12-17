using EasyButtons;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "LDGenerateData", menuName = "Scriptable Objects/LDGenerateData")]
    public class LDGenerateData : ScriptableObject
    {
        [SerializeField] private TextAsset textAsset;

        [Header("Mock Generation Settings")]
        [SerializeField] private List<Sprite> avatarList;
        [SerializeField] private List<Sprite> borderList;
        [SerializeField] private int mockCount = 100;
        [SerializeField] private bool sortDescending = true;

        [Header("Point Range")]
        [SerializeField] private int minPoint = 1000;
        [SerializeField] private int maxPoint = 99999;

        [System.Serializable]
        public class NameListWrapper { public List<string> names = new(); }

        [SerializeField] private List<string> cachedNames;

        [Button]
        private void EnsureNamesLoaded()
        {
            cachedNames = new List<string>();

            cachedNames = new List<string>();
            if (textAsset == null) return;
            var wrapper = JsonUtility.FromJson<NameListWrapper>(textAsset.text);
            if (wrapper == null)
            {
                Debug.Log("Failed to parse names from TextAsset.");
                return;
            }
            if (wrapper != null && wrapper.names != null)
                cachedNames = wrapper.names;

            Debug.Log($"Generating leaderboard data with {cachedNames.Count} entries.");

        }

        public void ApplyTo(LeaderboardDataSO leaderboard)
        {
            if (leaderboard == null) return;

            EnsureNamesLoaded();
            Debug.Log($"Generating leaderboard data with {cachedNames.Count} entries.");
            leaderboard.Generate(
                mockCount,
                Mathf.Min(minPoint, maxPoint),
                Mathf.Max(minPoint, maxPoint),
                sortDescending,
                cachedNames,
                avatarList,
                borderList
            );
        }
        public void GenerateForJson(M monthData)
        {
            if (monthData == null)
            {
                Debug.LogError("MonthData is null!");
                return;
            }

            EnsureNamesLoaded();

            monthData.u.Clear();

            for (int i = 0; i < mockCount; i++)
            {
                bool zeroPoint = Random.value < 0.1f;
                bool randomName = Random.value < 0.3f;

                string name = (!randomName && cachedNames.Count > 0)
                    ? cachedNames[Random.Range(0, cachedNames.Count)]
                    : $"Player_{i + 1}";

                int avatarIndex = avatarList != null && avatarList.Count > 0
                    ? Random.Range(0, avatarList.Count)
                    : -1;

                int borderIndex = borderList != null && borderList.Count > 0
                    ? Random.Range(0, borderList.Count)
                    : -1;

                monthData.u.Add(new U
                {
                    n = name,
                    p = zeroPoint ? 0 : Random.Range(minPoint, maxPoint + 1),
                    a = avatarIndex,
                    b = borderIndex
                });
            }

            if (sortDescending)
                monthData.u.Sort((a, b) => b.p.CompareTo(a.p));
            else
                monthData.u.Sort((a, b) => a.p.CompareTo(b.p));

            Debug.Log($"Generated mock JSON month data: {monthData.m}, Count = {monthData.u.Count}");
        }
        public static M GenerateDailyData(LDGenerateData generator)
        {
            var data = D30Service.Load();

            int day = D30Logic.RandomDay();

            M dailyLeaderboard = new M
            {
                m = day,
                u = new List<U>()
            };

          /*  generator.GenerateForJson(dailyLeaderboard);

            data.m.Add(dailyLeaderboard);

            D30Service.Save(data);

            Debug.Log($"Generated data for day {day}. Total days stored: {data.d.Count}");*/

            return dailyLeaderboard;
        }

    }

}
