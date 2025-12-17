using UnityEngine;

namespace ps.modules.leaderboard
{
    /// <summary>
    /// Cắt ngắn tên người chơi nếu vượt quá độ dài tối đa.
    /// </summary>
    public static class FormatName
    {
        public static string Format(string name, int maxLength = 10)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            if (name.Length <= maxLength)
                return name;
            return name.Substring(0, maxLength-2) + "..";
        }
    }
}
