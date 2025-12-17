using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class CheckFolderShaders : EditorWindow
{
    private DefaultAsset targetFolder;
    private Shader[] shadersFound;
    private bool checkedShaders = false;
    private bool showIncludedState = false;

    [MenuItem("Tools/Check Shaders in Folder")]
    public static void ShowWindow()
    {
        GetWindow<CheckFolderShaders>("Check Folder Shaders");
    }

    void OnGUI()
    {
        GUILayout.Label("🔍 Check Shaders in Folder", EditorStyles.boldLabel);
        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", targetFolder, typeof(DefaultAsset), false);

        if (targetFolder == null)
        {
            EditorGUILayout.HelpBox("Chọn thư mục chứa prefab cần kiểm tra (phải nằm trong Assets).", MessageType.Info);
            return;
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Check All Prefabs in Folder"))
        {
            CheckShadersInFolder(targetFolder);
        }

        if (checkedShaders && shadersFound != null && shadersFound.Length > 0)
        {
            GUILayout.Space(10);
            showIncludedState = GUILayout.Toggle(showIncludedState, "Hiện trạng thái trong Included Shaders");

            var includedShaders = GetAlwaysIncludedShaders();

            foreach (var shader in shadersFound)
            {
                if (shader == null) continue;

                bool isIncluded = includedShaders.Contains(shader);
                string shaderInfo = $"{shader.name}";
                if (showIncludedState)
                    shaderInfo += isIncluded ? " ✅ (Included)" : " ⚠️ (Not Included)";

                GUIStyle style = new GUIStyle(EditorStyles.label);
                style.normal.textColor = isIncluded ? Color.green : Color.yellow;
                GUILayout.Label(shaderInfo, style);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Add All Missing to Always Included"))
            {
                AddMissingShadersToIncluded(shadersFound, includedShaders);
            }
        }
    }

    private void CheckShadersInFolder(DefaultAsset folder)
    {
        string folderPath = AssetDatabase.GetAssetPath(folder);
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("❌ Thư mục không hợp lệ!");
            return;
        }

        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        var shaders = new HashSet<Shader>();
        int missingCount = 0;

        foreach (var guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            foreach (var r in prefab.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var m in r.sharedMaterials)
                {
                    if (m == null)
                    {
                        Debug.LogWarning($"⚠️ Prefab {prefab.name} có material NULL", prefab);
                        continue;
                    }
                    if (m.shader == null)
                    {
                        Debug.LogError($"❌ Prefab {prefab.name} có material '{m.name}' MISSING shader!", prefab);
                        missingCount++;
                        continue;
                    }

                    shaders.Add(m.shader);
                }
            }
        }

        shadersFound = shaders.ToArray();
        checkedShaders = true;

        Debug.Log($"📦 Đã quét {prefabGuids.Length} prefab trong thư mục '{folderPath}'.");
        Debug.Log($"🔎 Tìm thấy {shadersFound.Length} shader duy nhất, {missingCount} material bị thiếu shader.");

        foreach (var shader in shadersFound)
            Debug.Log($"✅ {shader.name}");
    }

    // ✅ Lấy danh sách Always Included Shaders
    private static Shader[] GetAlwaysIncludedShaders()
    {
        var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/GraphicsSettings.asset");
        var serializedObject = new SerializedObject(graphicsSettingsObj);
        var prop = serializedObject.FindProperty("m_AlwaysIncludedShaders");

        Shader[] shaders = new Shader[prop.arraySize];
        for (int i = 0; i < prop.arraySize; i++)
        {
            shaders[i] = prop.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
        }
        return shaders.Where(s => s != null).ToArray();
    }

    // ✅ Thêm các shader chưa có vào Always Included Shaders
    private static void AddMissingShadersToIncluded(Shader[] found, Shader[] included)
    {
        var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/GraphicsSettings.asset");
        var serializedObject = new SerializedObject(graphicsSettingsObj);
        var prop = serializedObject.FindProperty("m_AlwaysIncludedShaders");

        int addedCount = 0;
        foreach (var shader in found)
        {
            if (shader == null || included.Contains(shader))
                continue;

            prop.InsertArrayElementAtIndex(prop.arraySize);
            prop.GetArrayElementAtIndex(prop.arraySize - 1).objectReferenceValue = shader;
            addedCount++;
        }

        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();

        Debug.Log($"✅ Đã thêm {addedCount} shader mới vào Always Included Shaders.");
    }
}