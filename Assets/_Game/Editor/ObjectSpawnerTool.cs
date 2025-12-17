using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using VHACD.Unity;

public class ObjectSpawnerTool : EditorWindow
{
    private GameObject prefab;
    private GameObject parentObject;
    private bool isSpawning = false;
    private bool isUsingWorldScale = true;
    private float offset = 0;
    private LevelDifficulty levelDifficulty;
    private LevelMapSize levelMapSize;
    private bool isTransparentLevel = false;
    private Vector2 valueMinMaxZoom = new Vector2(0.5f, 2f);

    [Range(0.000001f, 10.0f)]
    private float spawnSize = 1.0f;

    private int _levelMapId = 1;
    private int amountColor = 1;
    private static int quality = 1;
    private static int levelMapId = 1;
    private Vector2 scrollPos;
    private LevelMapEditorData levelMapEditorData;
    private string levelName;

    private static readonly string[] QualityNames = { "Low", "Medium", "High", "Insane" };

    [MenuItem("Tools/Object Spawner Tool")]
    public static void ShowWindow()
    {
        ObjectSpawnerTool spawnerTool = GetWindow<ObjectSpawnerTool>("Object Spawner");
        spawnerTool.ValidateData();
    }

    private void ValidateData()
    {
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/level_new_control/screw.prefab");

        if (Selection.objects != null && Selection.objects.Length > 0 && Selection.objects[0] is GameObject selectedGameObject)
        {
            parentObject = selectedGameObject;
        }

        if (levelMapEditorData == null)
        {
            levelMapEditorData = new LevelMapEditorData();
        }

        if (parentObject != null)
        {
            levelMapEditorData.Serialize(parentObject.name);

            spawnSize = levelMapEditorData.spawnSize;
            _levelMapId = levelMapEditorData.levelMapId;
            amountColor = levelMapEditorData.amountColor;
            offset = levelMapEditorData.offset;
            levelDifficulty = levelMapEditorData.levelDifficulty;
            levelMapSize = levelMapEditorData.levelMapSize;
            isTransparentLevel = levelMapEditorData.isTransparentLevel;
            valueMinMaxZoom = levelMapEditorData.valueMinMaxZoom;
            levelName = levelMapEditorData.levelName;
        }
    }

    private int CountScrew()
    {
        if (parentObject != null)
        {
            Screw[] screws = parentObject.GetComponentsInChildren<Screw>();

            return screws != null ? screws.Length : 0;
        }

        return 0;
    }

    private void OnGUI()
    {

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(position.width), GUILayout.Height(position.height));

        // Background
        var rect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(rect, new Color(0.13f, 0.13f, 0.13f));

        // Styles
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 20,
            normal = { textColor = Color.cyan }
        };

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(15, 15, 10, 10),
            margin = new RectOffset(10, 10, 10, 10)
        };

        GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 25
        };

        GUILayout.Space(10);
        GUILayout.Label("🚀 Object Spawner Tool", headerStyle);
        GUILayout.Space(10);

        // ================= Prefab Settings =================
        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Prefab Settings", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab to Spawn", prefab, typeof(GameObject), false);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        spawnSize = EditorGUILayout.FloatField("Spawn Size", spawnSize);
        _levelMapId = EditorGUILayout.IntField("Level Map Id", _levelMapId);
        levelMapId = _levelMapId;
        amountColor = EditorGUILayout.IntField("Amount Color", amountColor);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        offset = EditorGUILayout.FloatField("Offset", offset);
        EditorGUILayout.IntField("Screw Number", CountScrew());
        levelDifficulty = (LevelDifficulty)EditorGUILayout.EnumPopup("Level Difficulty", levelDifficulty);
        levelMapSize = (LevelMapSize)EditorGUILayout.EnumPopup("Level Map Size", levelMapSize);

        isTransparentLevel = EditorGUILayout.Toggle("Transparent Level", isTransparentLevel);
        valueMinMaxZoom = EditorGUILayout.Vector2Field("Value Min Max Zoom", valueMinMaxZoom);

        isUsingWorldScale = EditorGUILayout.Toggle("Use World Scale", isUsingWorldScale);

        if (GUILayout.Button("🔄 Load Prefabs", bigButtonStyle))
        {
            ValidateData();
        }
        GUILayout.EndVertical();

        // ================= Game Designer =================
        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🎨 Game Designer", EditorStyles.boldLabel);

        if (GUILayout.Button(isSpawning ? "⏹ Stop Spawning Screw" : "▶ Start Spawning Screw", bigButtonStyle))
        {
            isSpawning = !isSpawning;
            SceneView.duringSceneGui -= OnSceneGUI;
            if (isSpawning) SceneView.duringSceneGui += OnSceneGUI;
        }

        if (isSpawning)
        {
            EditorGUILayout.HelpBox("Press [Space] or [C] in Scene view to spawn screws.", MessageType.Info);
        }

        if (GUILayout.Button("💾 Save Level", bigButtonStyle))
        {
            var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(parentObject);
            if (prefabAsset != null)
            {
                ResetOldLevel();
                PrefabUtility.SaveAsPrefabAssetAndConnect(parentObject, AssetDatabase.GetAssetPath(prefabAsset), InteractionMode.UserAction);
                EditorLogger.Log(">>>> Saved prefab - DONE");
            }

            levelMapEditorData = new LevelMapEditorData();

            levelMapEditorData.spawnSize = spawnSize;
            levelMapEditorData.levelMapId = _levelMapId;
            levelMapEditorData.amountColor = amountColor;
            levelMapEditorData.offset = offset;
            levelMapEditorData.levelDifficulty = levelDifficulty;
            levelMapEditorData.levelMapSize = levelMapSize;
            levelMapEditorData.isTransparentLevel = isTransparentLevel;
            levelMapEditorData.valueMinMaxZoom = valueMinMaxZoom;
            levelMapEditorData.levelName = levelName;

            levelMapEditorData.Save(parentObject.name);
        }

        if (GUILayout.Button("♻️ Revert Level", bigButtonStyle))
        {
            PrefabUtility.RevertPrefabInstance(parentObject, InteractionMode.UserAction);
            EditorLogger.Log(">>>> Revert Origin Level Map - DONE");
        }

        if (GUILayout.Button("🧹 Clean Up All Screw", bigButtonStyle))
        {
            OnResetAllScrew();
            EditorLogger.Log(">>>> Clean Up All Screw - DONE");
        }

        GUILayout.EndVertical();

        // ================= Setup Level Map =================
        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🧩 Setup Level Map And Asset", EditorStyles.boldLabel);

        if (GUILayout.Button("⚡ Setup Level Map", bigButtonStyle))
        {
            SetupLevelMapInternal();
        }

        if (GUILayout.Button("🧪 Test Level", bigButtonStyle))
        {
            LevelMap _levelMap = parentObject.GetComponent<LevelMap>();
            if (_levelMap != null) _levelMap.TestLevel();
        }

        if (GUILayout.Button("🧪 Stop Test Level", bigButtonStyle))
        {
            LevelMap _levelMap = parentObject.GetComponent<LevelMap>();
            if (_levelMap != null) _levelMap.stopTest = true;
        }

        if (GUILayout.Button("💾 Save & Setup (Test Immediately)", bigButtonStyle))
        {
            SaveAsPrefab(parentObject, levelMapId);
            EditorLogger.Log(">>>> Save And Setup Level Can Test Immediately - DONE");
        }

        if (GUILayout.Button("▶ Run Game", bigButtonStyle))
        {
            RunAndLoadScene();
        }

        if (GUILayout.Button("▶ Active All", bigButtonStyle))
        {
            Transform[] transforms = parentObject.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.SetActive(true);
            }
        }

        GUILayout.EndVertical();

        // ================= Setup Model =================
        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🏗️ Setup Model", EditorStyles.boldLabel);

        if (GUILayout.Button("⚡Change And Add Mesh Colliders to Cubes", bigButtonStyle))
        {
            ChangeNameAllCubes();
            AddMeshCollidersToCubes();
            EditorUtility.DisplayDialog("Info", ">>>>> Change And Add Mesh Colliders to Cubes - DONE!!!", "OK");
        }

        if (GUILayout.Button("⚡Setup Link", bigButtonStyle))
        {
            if (Selection.objects != null && Selection.objects.Length > 0)
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    if (Selection.objects[i] != null && Selection.objects[i] is GameObject select)
                    {
                        select.name = "Link";

                        if (select.GetComponent<MeshCollider>() == null)
                        {
                            var mesh = select.AddComponent<MeshCollider>();
                            mesh.convex = true;
                        }
                    }
                }
            }

            EditorUtility.DisplayDialog("Info", ">>>>> Setup Link - DONE!!!", "OK");
        }

        if (GUILayout.Button("🔧 Reset Old Level", bigButtonStyle))
        {
            ResetOldLevel();
            ChangeNameAllCubes();
            AddMeshCollidersToCubes();
            EditorUtility.DisplayDialog("Info", ">>>>> Reset Old Level - DONE!!!", "OK");
        }

        if (GUILayout.Button("⚡Change Name Screw", bigButtonStyle))
        {
            Screw[] screws = parentObject.GetComponentsInChildren<Screw>();

            for (int i = 0; i < screws.Length; i++)
            {
                screws[i].name = screws[i].name + "_" + i;
            }
        }

        if (GUILayout.Button("🧪 Check Collider Missing Mesh", bigButtonStyle))
        {
            MeshCollider[] colliders = parentObject.GetComponentsInChildren<MeshCollider>(true);
            bool isSuccess = true;

            foreach (var collider in colliders)
            {
                if (collider.sharedMesh == null)
                {
                    EditorUtility.DisplayDialog("Info", $"Missing Mesh - {collider.name}", "OK");
                    isSuccess = false;
                    break;
                }
            }

            if (isSuccess)
            {
                EditorUtility.DisplayDialog("Info", ">>>>> Success!!!", "OK");
            }
        }

        GUILayout.EndVertical();

        // ================= Fix Collider =================

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("🛠️ Fix Collider", EditorStyles.boldLabel);

        quality = GUILayout.Toolbar(quality, QualityNames);

        if (GUILayout.Button("🔧 Fix Convex Collider", bigButtonStyle))
        {
            if (Selection.activeGameObject != null)
            {
                RemoveComponentIfExists<ComplexCollider>(Selection.activeGameObject);
                ConvexColliderGenerator.CreateColliders(Selection.activeGameObject, quality, parentObject.name);
            }
        }

        if (GUILayout.Button("❌ Clear Mesh Collider", bigButtonStyle))
        {
            if (Selection.activeGameObject != null)
            {
                ConvexColliderService.GenOrClearColliders(null, Selection.activeGameObject);
            }
        }

        if (GUILayout.Button("🔧 Fix Convex Complicated Collider", bigButtonStyle))
        {
            if (Selection.activeGameObject != null)
            {
                RemoveComponentIfExists<ComplexCollider>(Selection.activeGameObject);
                Selection.activeGameObject.AddComponent<ConvexCollider>();
            }
        }

        if (GUILayout.Button("❌ Clear Complex Collider", bigButtonStyle))
        {
            if (Selection.activeGameObject != null)
            {
                DestroyImmediate(Selection.activeGameObject.GetComponent<ConvexCollider>());
            }
        }

        GUILayout.EndVertical();

        GUILayout.EndScrollView();
    }

    private void SetupLevelMapInternal()
    {
        LevelMap _levelMap = parentObject.GetComponent<LevelMap>();

        if (_levelMap == null)
        {
            _levelMap = parentObject.AddComponent<LevelMap>();

            if (parentObject.GetComponent<SwipeRotation360Degrees>() == null)
            {
                parentObject.AddComponent<SwipeRotation360Degrees>();
            }

            if (parentObject.GetComponent<FixMissingMeshColliders>() == null)
            {
                parentObject.AddComponent<FixMissingMeshColliders>();
            }

            if (parentObject.GetComponent<Swipe360DegreeForMobile>() == null)
            {
                parentObject.AddComponent<Swipe360DegreeForMobile>();
            }
        }

        Screw[] screws = parentObject.GetComponentsInChildren<Screw>();

        if (screws == null || screws.Length % 3 != 0)
        {
            EditorUtility.DisplayDialog("ERROR", $">>>>> Screw count is {screws.Length}, not divisible by 3. Please verify!!!", "OK");
            return;
        }

        _levelMap.LevelId = levelMapId;
        _levelMap.SetLevelDifficulty(levelDifficulty);
        _levelMap.SetLevelMapSize(levelMapSize);
        _levelMap.SetIsTransparentLevel(isTransparentLevel);
        _levelMap.SetZoomSize(valueMinMaxZoom);

        SwipeRotation360Degrees swipeRotation360 = _levelMap.GetComponent<SwipeRotation360Degrees>();
        swipeRotation360.SetDefaultData();

        Swipe360DegreeForMobile swipe360Degree = _levelMap.GetComponent<Swipe360DegreeForMobile>();
        swipe360Degree.SetDefaultData();

        _levelMap.SetAmountColor(amountColor);
        _levelMap.SetLevelName(levelName);
        _levelMap.GetAllScrewOnCustomsMap();
        _levelMap.SetUpDataColorAsync();

        _levelMap.GetScrewOnLinkObstacle();

        EditorLogger.Log(">>>> Setup Level Map - DONE");
        EditorUtility.DisplayDialog("Info", ">>>>> Setup Level Map - DONE!!!", "OK");
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && prefab != null)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (e.keyCode == KeyCode.Space)
                {
                    SpawnObjectAtSurface(hit);
                }
                else if (e.keyCode == KeyCode.C)
                {
                    SpawnObjectAtCenter(hit);
                }
                e.Use();
            }
        }

        SceneView.RepaintAll();
    }

    private void SpawnObjectAtSurface(RaycastHit hit)
    {
        GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        spawned.transform.position = hit.point;
        spawned.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        spawned.transform.SetParent(hit.collider.transform);
        if (isUsingWorldScale)
        {
            SetWorldScale(spawned.transform, new Vector3(spawnSize, spawnSize, spawnSize));
        }
        else
        {
            spawned.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
        }

        if (offset != 0)
        {
            spawned.transform.Translate(Vector3.down * offset);
        }

        Undo.RegisterCreatedObjectUndo(spawned, "Spawn Object on Surface");
    }

    private void SpawnObjectAtCenter(RaycastHit hit)
    {
        GameObject spawned = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Renderer renderer = hit.collider.GetComponent<Renderer>();
        if (renderer != null)
        {
            // Lấy tâm của bounding box
            Vector3 boundsCenter = renderer.bounds.center;

            // Dịch tâm về phía mặt phẳng đang được click
            Vector3 surfaceCenter = boundsCenter;
            surfaceCenter += hit.normal * Vector3.Dot(hit.point - boundsCenter, hit.normal);

            // Đặt object tại center của mặt phẳng
            spawned.transform.position = surfaceCenter;
            spawned.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            spawned.transform.SetParent(hit.collider.transform);

            if (offset != 0)
            {
                spawned.transform.Translate(Vector3.down * offset);
            }

            if (isUsingWorldScale)
            {
                SetWorldScale(spawned.transform, new Vector3(spawnSize, spawnSize, spawnSize));
            }
            else
            {
                spawned.transform.localScale = new Vector3(spawnSize, spawnSize, spawnSize);
            }

            Undo.RegisterCreatedObjectUndo(spawned, "Spawn Object at Center Surface");
        }
    }


    private void AddMeshCollidersToCubes()
    {
        if (parentObject == null) return;
        Transform[] children = parentObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if ((child.name.ToLower().Contains("cube") || child.name.ToLower().Contains("link"))
              && child.GetComponent<MeshCollider>() == null
              && child.GetComponent<ComplexCollider>() == null)
            {
                var mesh = child.gameObject.AddComponent<MeshCollider>();
                mesh.convex = true;
            }
        }
    }

    private void ChangeNameAllCubes()
    {
        if (parentObject == null) return;

        Transform[] children = parentObject.GetComponentsInChildren<Transform>(true);

        int index = 1;

        foreach (Transform child in children)
        {
            if (child.GetComponent<MeshFilter>() != null
             && child.GetComponent<Screw>() == null
             && !child.name.ToLower().Contains("screw"))
            {
                child.name = $"Cube_{index++}";
            }
        }
    }

    private void OnResetAllScrew()
    {
        if (parentObject == null) return;
        Screw[] children = parentObject.GetComponentsInChildren<Screw>(true);
        foreach (var child in children)
        {
            if (child == null || child.gameObject == null) continue;
            DestroyImmediate(child.gameObject);
        }

        Transform[] childrenTransforms = parentObject.GetComponentsInChildren<Transform>(true);
        foreach (var child in childrenTransforms)
        {
            if (child.gameObject.name.ToLower().Contains("hole(Clone)"))
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public static void SetWorldScale(Transform target, Vector3 desiredWorldScale)
    {
        Vector3 currentWorldScale = target.lossyScale;

        if (currentWorldScale.x == 0 || currentWorldScale.y == 0 || currentWorldScale.z == 0)
        {
            Debug.LogWarning("Cannot set world scale because current world scale has zero component.");
            return;
        }

        Vector3 correction = new Vector3(
            desiredWorldScale.x / currentWorldScale.x,
            desiredWorldScale.y / currentWorldScale.y,
            desiredWorldScale.z / currentWorldScale.z
        );

        Vector3 newLocalScale = new Vector3(
            target.localScale.x * correction.x,
            target.localScale.y * correction.y,
            target.localScale.z * correction.z
        );

        target.localScale = newLocalScale;
    }

    private void SaveAsPrefab(GameObject levelMap, int levelId)
    {
        string folderPath = "Assets/_Game/SOF_Lvl/LevelMapRelease/";
        string prefabName = $"Level_{levelId}.prefab";
        string fullPath = Path.Combine(folderPath, prefabName);

        //GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(levelMap);
        //PrefabUtility.UnpackPrefabInstance(clone, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        GameObject clone = Instantiate(levelMap, levelMap.transform.parent);

        clone.transform.SetPositionAndRotation(levelMap.transform.position, levelMap.transform.rotation);
        clone.transform.localScale = levelMap.transform.localScale;

        // Ensure folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        // Confirm overwrite if prefab exists
        if (File.Exists(fullPath))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Prefab Exists",
                $"A prefab named '{prefabName}' already exists. Overwrite it?",
                "Yes", "Cancel");

            if (!overwrite)
                return;
        }

        PrefabUtility.SaveAsPrefabAsset(clone, fullPath);
        DestroyImmediate(clone);
        Debug.Log($"Saved prefab: {fullPath}");
    }

    private void EnsureAddressable(string assetPath, string address)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        var entry = settings.FindAssetEntry(guid);

        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, settings.DefaultGroup);
            entry.address = address;
            Debug.Log($"Addressable assigned: {address}");
        }
    }

    private static void RunAndLoadScene()
    {
        EditorApplication.isPlaying = true;
        SerializationManager.ClearAllDataInGamePlay();
    }

    private void ResetOldLevel()
    {
        // Remove special components from parentObject
        RemoveComponentIfExists<LevelMap>(parentObject);
        RemoveComponentIfExists<SwipeRotation360Degrees>(parentObject);
        RemoveComponentIfExists<FixMissingMeshColliders>(parentObject);
        RemoveComponentIfExists<Swipe360DegreeForMobile>(parentObject);

        // Process all rigidbodies inside the parentObject
        Rigidbody[] rigidbodies = parentObject.GetComponentsInChildren<Rigidbody>();

        foreach (var rb in rigidbodies)
        {
            // Remove HingeJoint if it exists
            RemoveComponentIfExists<HingeJoint>(rb.gameObject);

            // If the object has a Shape component → remove Shape and the Rigidbody itself
            if (rb.GetComponent<Shape>() != null)
            {
                DestroyImmediate(rb.GetComponent<Shape>());
                DestroyImmediate(rb);
            }
            // If the object has a LinkObstacle component → remove LinkObstacle and the Rigidbody itself
            else if (rb.GetComponent<LinkObstacle>() != null)
            {
                DestroyImmediate(rb.GetComponent<LinkObstacle>());
                DestroyImmediate(rb);
            }
            // If the object is named "hole(Clone)" → remove the whole GameObject
            else if (rb.name == "hole(Clone)")
            {
                DestroyImmediate(rb.gameObject);
            }
        }
    }

    /// <summary>
    /// Remove component T from the target if it exists
    /// </summary>
    private static void RemoveComponentIfExists<T>(GameObject target) where T : Component
    {
        var comp = target.GetComponent<T>();
        if (comp != null)
        {
            DestroyImmediate(comp);
        }
    }

    [MenuItem("Tools/Clear User Data")]
    private static void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        EditorLogger.Log("Clear User Data - DONE");
    }
}

[System.Serializable]
public class LevelMapEditorData
{
    public float spawnSize;
    public int levelMapId;
    public int amountColor;
    public int wrenchAmount;
    public float offset;
    public LevelDifficulty levelDifficulty;
    public LevelMapSize levelMapSize;
    public bool isTransparentLevel;
    public Vector2 valueMinMaxZoom;
    public string levelName;

    public LevelMapEditorData()
    {
        spawnSize = 1;
        valueMinMaxZoom = new Vector2(0.5f, 2f);
    }

    public void Serialize(string groupName)
    {
        string rootFolder = EnsureFolder("Assets", "LevelDesign");
        string assetPath = $"{rootFolder}/{groupName}.json";

        if (File.Exists(assetPath))
        {
            string json = File.ReadAllText(assetPath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }

    public string Deserialize()
    {
        return JsonUtility.ToJson(this);
    }

    public void Save(string groupName)
    {
        string rootFolder = EnsureFolder("Assets", "LevelDesign");
        string assetPath = $"{rootFolder}/{groupName}.json";

        File.WriteAllText(assetPath, Deserialize());
    }

    public string EnsureFolder(string parent, string folderName)
    {
        string path = $"{parent}/{folderName}";
        if (!AssetDatabase.IsValidFolder(path))
        {
            AssetDatabase.CreateFolder(parent, folderName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        return path;
    }
}