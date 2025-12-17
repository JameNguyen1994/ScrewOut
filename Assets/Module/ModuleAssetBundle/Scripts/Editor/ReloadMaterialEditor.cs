using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ReloadMaterialEditor : EditorWindow
{
    private GameObject prefab;

    [MenuItem("Tools/Reload Material AA")]
    public static void ShowWindow()
    {
        ReloadMaterialEditor reloadMaterialEditor = GetWindow<ReloadMaterialEditor>("Reload Material Editor");
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
        GUILayout.Label("🚀 Reload Material Editor", headerStyle);
        GUILayout.Space(10);

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Check Level", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Level Map", prefab, typeof(GameObject), true);

        if (GUILayout.Button("⏱️ Check Level Glass Material", bigButtonStyle))
        {
            CheckLevel();
        }

        GUILayout.EndVertical();
    }

    private void CheckLevel()
    {
        string name = "";
        string mats = "";
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);

        DestroyImmediate(prefab.GetComponent<ReloadTextureContain>());
        ReloadMaterial[] reloadMaterials = prefab.GetComponentsInChildren<ReloadMaterial>(true);

        if (reloadMaterials != null)
        {
            foreach (var reload in reloadMaterials)
            {
                DestroyImmediate(reload);
            }
        }

        bool isHaveThis = false;

        ReloadTextureContain reloadTexture = prefab.GetComponent<ReloadTextureContain>();

        if (reloadTexture == null)
        {
            reloadTexture = prefab.AddComponent<ReloadTextureContain>();
        }

        reloadTexture.textures = new System.Collections.Generic.List<Texture>();

        foreach (var renderer in renderers)
        {
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
                        CloneMaterialToFolder(mat, "Assets/Module/ModuleAssetBundle/Materials", mat.name);

                        name += "\n" + renderer.gameObject.name;
                        isHaveThis = true;

                        ReloadMaterial reload = renderer.GetComponent<ReloadMaterial>();

                        if (reload == null)
                        {
                            reload = renderer.gameObject.AddComponentAtTop<ReloadMaterial>();
                        }

                        reload.reloadTexture = reloadTexture;
                        reload.meshRenderer = (MeshRenderer)renderer;
                        reload.materialName = mat.name;

                        if (mat.HasProperty("_BaseMap"))
                        {
                            var tex = mat.GetTexture("_BaseMap");
                            if (tex != null)
                            {
                                if (!reloadTexture.textures.Contains(tex))
                                {
                                    reloadTexture.textures.Add(tex);
                                }

                                reload.baseMap = tex.name;
                            }
                        }

                        if (mat.HasProperty("_BumpMap"))
                        {
                            var tex = mat.GetTexture("_BumpMap");
                            if (tex != null)
                            {
                                if (!reloadTexture.textures.Contains(tex))
                                {
                                    reloadTexture.textures.Add(tex);
                                }

                                reload.normalMap = tex.name;
                            }
                        }

                        if (mat.HasProperty("_DetailNormalMap"))
                        {
                            var tex = mat.GetTexture("_DetailNormalMap");
                            if (tex != null)
                            {
                                if (!reloadTexture.textures.Contains(tex))
                                    reloadTexture.textures.Add(tex);

                                reload.detailNormal = tex.name;
                            }
                        }

                        if (!mats.Contains(reload.materialName))
                        {
                            mats += "\n" + reload.materialName;
                        }
                    }
                }
            }
        }

        if (isHaveThis)
        {
            Debug.Log($"[TRANSPARENT] {prefab.name}");
            Debug.Log($"[LIST NAME] {name}");
            Debug.Log($"[LIST MATS] {mats}");
        }

        AssetDatabase.SaveAssets();
    }

    public static Material CloneMaterialToFolder(Material source, string folderPath, string newName)
    {
        // đảm bảo folder tồn tại
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            AssetDatabase.Refresh();
        }

        string assetPath = $"{folderPath}/{newName}.mat";

        // ✅ Nếu material đã tồn tại -> load và return luôn
        Material existingMat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (existingMat != null)
        {
            return existingMat;
        }

        // ✅ Chưa có -> tạo mới
        Material newMat = new Material(source);

        AssetDatabase.CreateAsset(newMat, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return newMat;
    }
}

public static class ComponentHelper
{
    public static T AddComponentAtTop<T>(this GameObject go) where T : Component
    {
        // Add component bình thường
        T comp = go.AddComponent<T>();

#if UNITY_EDITOR
        // Move lên trên cùng
        while (ComponentUtility.MoveComponentUp(comp)) { }
#endif

        return comp;
    }
}
