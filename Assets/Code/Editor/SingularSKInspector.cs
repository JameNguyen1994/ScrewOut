using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SingularSDK))]
public class SingularSKInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
#if STAGING_SERVER
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Switch To Release Server"))
        {
            RemoveScriptingDefineSymbol("STAGING_SERVER");
        }
        EditorGUILayout.Space(10);
        //EditorGUILayout.HelpBox("You are on the STAGING SERVER, please switch to the release server before building the AAB file!", MessageType.Warning);
#else
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Switch To Staging Server"))
        {
            AddScriptingDefineSymbol("STAGING_SERVER");
        }
        EditorGUILayout.Space(10);
        //EditorGUILayout.HelpBox("Please thoroughly check the Release server information before building the AAB file!", MessageType.Warning);
#endif
    }
    
    public static void AddScriptingDefineSymbol(string symbol)
    {
        var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        
        if (!currentSymbols.Contains(symbol))
        {
            currentSymbols += ";" + symbol;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentSymbols);
            Debug.Log($"Added scripting define symbol: {symbol}");
        }
        else
        {
            Debug.LogWarning($"Symbol {symbol} already exists.");
        }
    }
    
    public static void RemoveScriptingDefineSymbol(string symbol)
    {
        var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        
        if (currentSymbols.Contains(symbol))
        {
            var symbolsList = new System.Collections.Generic.List<string>(currentSymbols.Split(';'));
            symbolsList.Remove(symbol);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbolsList.ToArray()));
            Debug.Log($"Removed scripting define symbol: {symbol}");
        }
        else
        {
            Debug.LogWarning($"Symbol {symbol} does not exist.");
        }
    }
}
