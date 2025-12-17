using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [System.Serializable]
    public class UserData
    {
        public string name;
        public int points;
        public Sprite avatar;
        public Sprite border;
    }

    [CreateAssetMenu(fileName = "LeaderboardDataSO", menuName = "Scriptable Objects/LeaderboardDataSO")]
    public class LeaderboardDataSO : ScriptableObject
    {
        [Header("Info")]
        public List<UserData> users = new();
        /// <summary>
        /// Tạo mock data cho leaderboard (từ danh sách tên/ảnh)
        /// </summary>
        
        public void Generate(
            int count,
            int minPoint,
            int maxPoint,
            bool sortDescending,
            List<string> nameList,
            List<Sprite> avatarList,
            List<Sprite> borderList)
        {
            users.Clear();

            if (count <= 0)
                return;
            
            for (int i = 0; i < count; i++)
            {
                bool willBePointZero = Random.Range(0f, 1f) < 0.1f; // 4% cơ hội có điểm là 0
                bool willBeNamePlayer = Random.Range(0f, 1f) < 0.3f; // 10% cơ hội có tên là Player_x
                var name = (nameList != null && nameList.Count > 0&&!willBeNamePlayer)
                    ? nameList[Random.Range(0, nameList.Count)]
                    : $"Player_{i + 1}";

                var avatar = (avatarList != null && avatarList.Count > 0)
                    ? avatarList[Random.Range(0, avatarList.Count)]
                    : null;

                var border = (borderList != null && borderList.Count > 0)
                    ? borderList[Random.Range(0, borderList.Count)]
                    : null;

                users.Add(new UserData
                {
                    name = name,
                    points = willBePointZero?0: Random.Range(minPoint, maxPoint + 1),
                    avatar = avatar,
                    border = border
                });
            }

            // sắp xếp
            if (sortDescending)
                users.Sort((a, b) => b.points.CompareTo(a.points));
            else
                users.Sort((a, b) => a.points.CompareTo(b.points));

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}
