using UnityEngine;
using UnityEditor;

public static class GameTools
{
    [MenuItem("Tools/Game/Clear All Data In Game Play")]
    public static void ClearAllDataInGamePlay()
    {
        EditorLogger.Log(">>>> Clear All Data In Game Play!");
        SerializationManager.ClearAllDataInGamePlay();
    }
}