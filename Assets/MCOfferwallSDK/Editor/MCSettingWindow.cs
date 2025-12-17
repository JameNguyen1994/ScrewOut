using UnityEngine;
using UnityEditor;

public class MCSettingsWindow : EditorWindow
{
    private string apiKey = "apiKey";

    // Add menu named "SDK Settings" to the Window menu
    [MenuItem("Window/MyChips Settings")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MCSettingsWindow window = (MCSettingsWindow)EditorWindow.GetWindow(typeof(MCSettingsWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("MyChips Settings", EditorStyles.boldLabel);

        //// Settings fields
        //apiKey = EditorGUILayout.TextField("ApiKey", apiKey);

        //// Save settings button
        //if (GUILayout.Button("Save Settings"))
        //{
        //    SaveSettings();
        //}

        GUILayout.Label("Automatically add GameObject", EditorStyles.boldLabel);
        // Add prefab to scene button
        if (GUILayout.Button("Add Prefab to Scene"))
        {
            AddPrefabToScene();
        }
    }

    void SaveSettings()
    {
        // Here you would save your settings, for example, using EditorPrefs
        EditorPrefs.SetString("io.mychips.settings", apiKey);

        Debug.Log("Settings Saved");
    }

    void AddPrefabToScene()
    {

#if UNITY_2022_1_OR_NEWER
        // For Unity 2021.x versions and newer, where FindFirstObjectByType doesn't work
        if (FindFirstObjectByType<MCOfferwallObject>() != null)
        {
            Debug.Log("Prefab already exist");
            return;
        }
#else

        if (FindObjectOfType<MCOfferwallObject>() != null)
        {
            Debug.Log("Prefab already exist");
            return;
        }
#endif
        // Path to your prefab
        string prefabPath = "Assets/MCOfferwallSDK/Scripts/MCOfferwallSDK/Prefabs/MCOfferwallObject.prefab";

        // Load the prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab != null)
        {
            // Instantiate the prefab in the scene
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            MCOfferwallObject mcOfferwallObject = instance.GetComponent<MCOfferwallObject>();
            mcOfferwallObject.InitializeComponents();
            Debug.Log("Prefab added to scene");
        }
        else
        {
            Debug.LogError("Could not find prefab at path: " + prefabPath);
        }

    }
}
