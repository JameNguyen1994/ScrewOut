#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public static class LeaderboardEditorUtils
{
    public static void OpenDataFolder()
    {
        string path = Path.Combine(Application.persistentDataPath, "Leaderboard");

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        EditorUtility.RevealInFinder(path);
    }
}
#endif
