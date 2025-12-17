using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TextToTMPWindow : EditorWindow
{
    private TMP_FontAsset newTMPFont;
    private GameObject searchRoot;

    private SerializedObject so;
    private SerializedProperty spLegacyTexts;
    private Vector2 mainScroll;

    // Options
    private bool optAlignCenter = true;
    private bool optAutoSize = true;
    private int fontSizeMin = 10;
    private int fontSizeMax = 100;

    [SerializeField]
    private List<Text> legacyTexts = new();

    [MenuItem("Tools/Text to TMP Converter")]
    public static void ShowWindow()
    {
        GetWindow<TextToTMPWindow>("Text to TMP");
    }

    private void OnEnable()
    {
        so = new SerializedObject(this);
        spLegacyTexts = so.FindProperty("legacyTexts");
    }

    private void OnGUI()
    {
        so.Update();

        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        GUILayout.Label("🛠 Chuyển Text (Legacy) sang TextMeshPro", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // TMP Font
        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font", newTMPFont, typeof(TMP_FontAsset), false);

        // Options
        EditorGUILayout.Space();
        GUILayout.Label("⚙️ Tuỳ chọn chuyển đổi", EditorStyles.boldLabel);
        optAlignCenter = EditorGUILayout.Toggle("Căn giữa (Center)", optAlignCenter);
        optAutoSize = EditorGUILayout.Toggle("Bật Auto Size", optAutoSize);
        if (optAutoSize)
        {
            EditorGUI.indentLevel++;
            fontSizeMin = EditorGUILayout.IntField("Font Size Min", fontSizeMin);
            fontSizeMax = EditorGUILayout.IntField("Font Size Max", fontSizeMax);
            EditorGUI.indentLevel--;
        }

        // Find all Texts in scene
        EditorGUILayout.Space();
        if (GUILayout.Button("🔍 Tìm tất cả Text (Legacy) trong scene"))
        {
            legacyTexts = new List<Text>(FindObjectsOfType<Text>(true));
            so = new SerializedObject(this);
            spLegacyTexts = so.FindProperty("legacyTexts");
        }

        // Find in specific object
        EditorGUILayout.Space();
        GUILayout.Label("🔎 Tìm trong 1 GameObject cụ thể:", EditorStyles.boldLabel);
        searchRoot = (GameObject)EditorGUILayout.ObjectField("Root Object", searchRoot, typeof(GameObject), true);
        if (searchRoot != null && GUILayout.Button("➕ Tìm Text (Legacy) trong đối tượng này"))
        {
            var foundTexts = searchRoot.GetComponentsInChildren<Text>(true);
            int addedCount = 0;

            foreach (var txt in foundTexts)
            {
                if (!legacyTexts.Contains(txt))
                {
                    legacyTexts.Add(txt);
                    addedCount++;
                }
            }

            Debug.Log($"✅ Đã thêm {addedCount} Text từ {searchRoot.name}");
            so = new SerializedObject(this);
            spLegacyTexts = so.FindProperty("legacyTexts");
        }

        // Display list like Inspector
        EditorGUILayout.Space();
        GUILayout.Label("📋 Danh sách Text Legacy:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(spLegacyTexts, new GUIContent("Legacy Texts"), true);

        EditorGUILayout.EndScrollView(); // End of scroll

        so.ApplyModifiedProperties();

        EditorGUILayout.Space();

        // Convert button
        GUI.enabled = newTMPFont != null && legacyTexts.Count > 0;
        if (GUILayout.Button("🚀 Chuyển tất cả sang TMP", GUILayout.Height(40)))
        {
            ConvertToTMP();
        }
        GUI.enabled = true;
    }

    private void ConvertToTMP()
    {
        int count = 0;

        foreach (var legacyText in legacyTexts)
        {
            if (legacyText == null || legacyText.gameObject == null) continue;
            if (legacyText.GetComponent<TextMeshProUGUI>() != null) continue;

            GameObject go = legacyText.gameObject;

            string oldText = legacyText.text;
            Color oldColor = legacyText.color;

            Undo.RecordObject(go, "Convert Text to TMP");

            // ❌ Xóa tất cả Outline/Shadow (có thể nhiều)
            foreach (var effect in go.GetComponents<BaseMeshEffect>())
            {
                if (effect is Outline || effect is Shadow)
                {
                    Undo.DestroyObjectImmediate(effect);
                }
            }

            // ❌ Xoá Text legacy
            DestroyImmediate(legacyText, true);

            // ✅ Thêm TMP
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = oldText;
            tmp.font = newTMPFont;
            tmp.color = oldColor;

            if (optAlignCenter)
                tmp.alignment = TextAlignmentOptions.Center;

            if (optAutoSize)
            {
                tmp.enableAutoSizing = true;
                tmp.fontSizeMin = fontSizeMin;
                tmp.fontSizeMax = fontSizeMax;
            }

            count++;
        }

        Debug.Log($"✅ Đã chuyển {count} Text sang TMP (xoá hết Outline/Shadow nếu có).");
        legacyTexts.Clear();
        so.Update();
    }
}
