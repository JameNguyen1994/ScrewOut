using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ConvertData : EditorWindow
{
    [SerializeField] private List<GameObject> lstObjects = new();
    [SerializeField] private List<TextAsset> lstTexts = new();

    private SerializedObject so;
    private SerializedProperty propObjects;
    private SerializedProperty propTexts;

    // ======================
    // SHOW WINDOW
    // ======================
    [MenuItem("Tools/Convert Data")]
    public static void ShowWindow()
    {
        GetWindow<ConvertData>("Convert Data");
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        propObjects = so.FindProperty("lstObjects");
        propTexts = so.FindProperty("lstTexts");
    }

    private void OnGUI()
    {
        so.Update();

        GUILayout.Label("=== LIST GAMEOBJECT ===", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(propObjects, true);

        GUILayout.Space(10);

        GUILayout.Label("=== LIST TEXTASSET ===", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(propTexts, true);

        GUILayout.Space(20);

        if (GUILayout.Button("Convert Data", GUILayout.Height(30)))
        {
            Convert();
        }

        so.ApplyModifiedProperties();
    }

    // ======================
    // CUSTOM FUNCTION
    // ======================
    private void Convert()
    {
        Debug.Log("=== Running Convert() ===");
        if (lstObjects.Count != lstTexts.Count)
        {
            ShowNotification(new GUIContent("Error: Lists must have the same number of elements."));
        }

        Debug.Log("---- Objects ----");
        bool isCompareList = true;
        for (int i = 0; i < lstObjects.Count; i++)
        {
            var level = lstObjects[i].GetComponent<LevelMap>();
            int indexText = GetIntFromText(lstTexts[i]);

            int indexLevel = level.LevelId;
            if (indexLevel != indexText)
            {
                isCompareList = false;
                Debug.LogError($"[ConvertData] LevelId mismatch at index {i}: LevelId = {indexLevel}, TextIndex = {indexText}");
            }
        }
        if (isCompareList)
        {
            for (int i = 0; i < lstObjects.Count; i++)
            {
                var level = lstObjects[i].GetComponent<LevelMap>();
                level.LevelMapDataJson = lstTexts[i];
                EditorUtility.SetDirty(level); // Fixed: Replaced 'EditorUltilitis' with 'EditorUtility'
            }
        }
        else
        {
            Debug.LogError("Lists are NOT consistent.");
        }
    }

    private int GetIntFromText(TextAsset textAsset)  // "LevelData_11" => 11
    {
        if (textAsset == null)
            return -1;

        string name = textAsset.name;   // Lấy tên file, không phải nội dung text

        int underscoreIndex = name.LastIndexOf('_');
        if (underscoreIndex < 0)
            return -1;

        string numberPart = name.Substring(underscoreIndex + 1);

        // Parse ra số
        if (int.TryParse(numberPart, out int result))
            return result;

        return -1;
    }
}
