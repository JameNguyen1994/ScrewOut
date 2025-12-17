using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Level level = (Level)target;

        if (GUILayout.Button("Convert Level To Level MapData"))
        {
            level.ConvertLevelToLevelMapData();
        }
 	    if (GUILayout.Button("Convert Cube name With Level"))
        {
            level.ConvertCubeNameWithLevel();
        }
    }
}
