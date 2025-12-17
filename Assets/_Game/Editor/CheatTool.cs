using Life;
using Spin;
using Storage;
using Storage.Model;
using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

public class CheatTool : EditorWindow
{
    private int screwAmount;
    private int ads;
    private int wrenchAmount;
    private int value;

    [MenuItem("Tools/Cheat Tool")]
    public static void ShowWindow()
    {
        CheatTool CheatTool = GetWindow<CheatTool>("Cheat Tool");
    }

    private void OnGUI()
    {
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
        GUILayout.Label("🚀 Cheat Tool", headerStyle);
        GUILayout.Space(10);

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Luscky Spin", EditorStyles.boldLabel);

        screwAmount = EditorGUILayout.IntField("Screw Amount", screwAmount);

        if (GUILayout.Button("🔄 Add Screw", bigButtonStyle))
        {
            SpinService.CollectScrew(screwAmount);
        }

        ads = EditorGUILayout.IntField("ADS", ads);

        if (GUILayout.Button("▶ Add ADS", bigButtonStyle))
        {
            Db.storage.LuckySpinData.dailySpinByADS = ads;
        }

        if (GUILayout.Button("⚙️ Show LuckSpin", bigButtonStyle))
        {
            SpinController spin = FindFirstObjectByType<SpinController>();
            spin.DOShow();
        }

        if (GUILayout.Button("🚀 Reset LuckSpin", bigButtonStyle))
        {
            Db.storage.LuckySpinData = new LuckySpinData();
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Wrench Collection", EditorStyles.boldLabel);

        wrenchAmount = EditorGUILayout.IntField("Wrench Amount", wrenchAmount);

        if (GUILayout.Button("▶ Add Wrench", bigButtonStyle))
        {
            WrenchCollectionService.CollectWrench(wrenchAmount);
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
            Debug.Log("Add Wrench");
        }

        if (GUILayout.Button("⚙️ Start", bigButtonStyle))
        {
            WrenchCollectionData data = new WrenchCollectionData();

            DateTime dateTime = TimeGetter.Instance.Now;
            dateTime = dateTime.AddDays(3);

            data.isActive = true;

            data.endYear = dateTime.Year;
            data.endMonth = dateTime.Month;
            data.endDay = dateTime.Day;
            data.endHour = dateTime.Hour;
            data.endMinute = dateTime.Minute;
            data.rewardGroup = UnityEngine.Random.Range(0, 3);

            Db.storage.WrenchCollectionData = data;
            Debug.Log("Start Wrench");
        }

        if (GUILayout.Button("🔄 End", bigButtonStyle))
        {
            WrenchCollectionData data = new WrenchCollectionData();

            data.isComplete = true;

            DateTime dateTime = DateTime.Now;

            data.endYear = dateTime.Year;
            data.endMonth = dateTime.Month;
            data.endDay = dateTime.Day;
            data.endHour = dateTime.Hour;
            data.endMinute = dateTime.Minute;

            Db.storage.WrenchCollectionData = data;
            Debug.Log("End Wrench");
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Core Retention", EditorStyles.boldLabel);

        if (GUILayout.Button("🥰 Active Event", bigButtonStyle))
        {
            CoreRetentionService.ActiveWinLevelEvent();
            Debug.Log("Active Event");
        }

        if (GUILayout.Button("🔑 Reload Scene", bigButtonStyle))
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);
            Debug.Log("Reload Scene");
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Reward", EditorStyles.boldLabel);

        value = EditorGUILayout.IntField("Value", value);

        if (GUILayout.Button("🌞 Add Gold", bigButtonStyle))
        {
            var rewardData = Db.storage.RewardData.DeepClone();
            rewardData.AddCoinValue(value);
            Db.storage.RewardData = rewardData;
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }

        if (GUILayout.Button("🥵 Add Time", bigButtonStyle))
        {
            var rewardData = Db.storage.RewardData.DeepClone();
            rewardData.AddHeartTimeValue(value * 1000 * 60);
            Db.storage.RewardData = rewardData;
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }

        if (GUILayout.Button("🔑 Add Item", bigButtonStyle))
        {
            var rewardData = Db.storage.RewardData.DeepClone();
            rewardData.itemAmount += value;
            Db.storage.RewardData = rewardData;
            MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }

        if (GUILayout.Button("⏱️ Clear Heart", bigButtonStyle))
        {
            LifeInfo lifeInfo = DBLifeController.Instance.LIFE_INFO;

            lifeInfo.timeInfinity = 0;
            lifeInfo.timeRegen = 0;
            lifeInfo.markTick = 0;
            lifeInfo.lifeAmount = 0;

            DBLifeController.Instance.LIFE_INFO = lifeInfo;
        }

        GUILayout.EndVertical();

        if (GUILayout.Button("⏱️ Check Glass Material", bigButtonStyle))
        {
            Check();
        }
    }

    private static void Check()
    {
        UnityEngine.Object[] selected = Selection.objects;
        string data = "";
        var settings = AddressableAssetSettingsDefaultObject.Settings;

        if (selected.Length == 0)
        {
            Debug.Log("No objects selected.");
            return;
        }

        foreach (var obj in selected)
        {
            GameObject go = obj as GameObject;
            if (go == null)
                continue;

            Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);

            bool isHaveThis = false;

            foreach (var renderer in renderers)
            {
                if (isHaveThis)
                {
                    break;
                }

                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null) continue;

                    Shader shader = mat.shader;

                    // Check URP/Lit
                    if (shader != null &&
                        shader.name == "Universal Render Pipeline/Lit")
                    {
                        // Check Transparent mode
                        bool isTransparent = false;

                        if (mat.HasProperty("_Surface"))
                        {
                            // URP 12–16: 0=Opaque, 1=Transparent
                            isTransparent = mat.GetFloat("_Surface") == 1;
                        }

                        if (isTransparent)
                        {
                            isHaveThis = true;
                        }
                    }
                }
            }

            if (isHaveThis)
            {
                string path = AssetDatabase.GetAssetPath(go);

                var entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), settings.DefaultGroup);
                entry.address = path;

                Debug.Log($"[TRANSPARENT] {go.name}");
                data += go.name + ",";
            }
        }

        AssetDatabase.SaveAssets();

        Debug.Log($"[TRANSPARENT] " + data);
    }
}
