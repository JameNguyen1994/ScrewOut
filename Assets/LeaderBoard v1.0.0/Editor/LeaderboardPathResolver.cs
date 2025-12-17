// Editor/LeaderboardPathResolver.cs
using UnityEditor;
using UnityEngine;
using System.IO;
using ps.modules.leaderboard;

public static class LeaderboardPathResolver
{
    /// <summary>
    /// Lấy thư mục gốc đặt các YearData dựa trên vị trí của AllYearData.asset
    /// </summary>
    public static string GetBaseDir(AllYearData allYearData)
    {
        if (allYearData == null) return null;
        var allPath = AssetDatabase.GetAssetPath(allYearData);
        if (string.IsNullOrEmpty(allPath)) return null;
        return Path.GetDirectoryName(allPath)?.Replace("\\", "/");
    }

    /// <summary>
    /// Lấy thư mục chứa YearData dựa trên chính YearData
    /// </summary>
    public static string GetYearDir(YearDataSO yearData)
    {
        if (yearData == null) return null;
        var path = AssetDatabase.GetAssetPath(yearData);
        if (string.IsNullOrEmpty(path)) return null;
        return Path.GetDirectoryName(path)?.Replace("\\", "/");
    }

    /// <summary>
    /// Đảm bảo thư mục tồn tại, trả về path dạng forward-slash
    /// </summary>
    public static string EnsureFolder(string parent, string child)
    {
        var combined = Path.Combine(parent, child).Replace("\\", "/");
        if (!Directory.Exists(combined))
            Directory.CreateDirectory(combined);
        return combined;
    }

    /// <summary>
    /// Ghép path và chuẩn hóa slash
    /// </summary>
    public static string Combine(params string[] parts)
    {
        return Path.Combine(parts).Replace("\\", "/");
    }
}
