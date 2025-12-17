using EasyButtons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetBundleConfig", menuName = "AssetBundle/Config", order = 0)]
public class AssetBundleConfig : ScriptableObject
{
    public string Host;
    public int Retry = 4;
    public List<AssetBundleData> BundleDatas = new List<AssetBundleData>();
    public List<AssetBundleData> BundleBonusDatas = new List<AssetBundleData>();

    public List<Material> Materials;

    public string GetFullURL(string url)
    {
        return string.Format(Host, url);
    }

#if UNITY_EDITOR

    [Button]
    public void SetData()
    {
        BundleDatas = new List<AssetBundleData>();

        string data = "Level_1,Level_10,Level_11,Level_12,Level_13,Level_14,Level_15,Level_16,Level_17,Level_18,Level_19,Level_2,Level_20,Level_21,Level_22,Level_23,Level_24,Level_25,Level_26,Level_27,Level_28,Level_29,Level_3,Level_30,Level_31,Level_32,Level_33,Level_34,Level_35,Level_4,Level_5,Level_6,Level_7,Level_8,Level_80,Level_9";
        string dataVer2 = "Level_12,Level_21,Level_23,Level_25,Level_35,Level_44,Level_45,Level_50,Level_53,Level_54,Level_65,Level_67,Level_69,Level_70,Level_71,Level_75,Level_78,Level_81,Level_82,Level_86,Level_92,Level_93,Level_94,Level_96,Level_97,Level_103,Level_107,Level_108,Level_109,Level_114,Level_115,Level_117,Level_121,Level_122,Level_125,Level_130,Level_132,Level_135,Level_142,Level_143,Level_146,Level_148,Level_152,Level_153,Level_154";
        List<string> datas = data.Split(',').ToList();
        List<string> dataVer2s = dataVer2.Split(',').ToList();

        Debug.Log(datas.Count);

        for (int i = 1; i < 156; i++)
        {
            string id = $"Level_{i}";

            if (!datas.Contains(id))
            {
                if (dataVer2s.Contains(id))
                {
                    BundleDatas.Add(new AssetBundleData()
                    {
                        Id = id,
                        PrefabName = id,
                        BundleName = id.ToLower() + "_2",
                    });
                }
                else
                {
                    BundleDatas.Add(new AssetBundleData()
                    {
                        Id = id,
                        PrefabName = id,
                        BundleName = id.ToLower(),
                    });
                }
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    [Button]
    public void SetDataBonus()
    {
        BundleBonusDatas = new List<AssetBundleData>();

        for (int i = 1; i <= 10; i++)
        {
            string id = $"Level_Bonus_{i}";

            BundleBonusDatas.Add(new AssetBundleData()
            {
                Id = id,
                PrefabName = id,
                BundleName = id.ToLower(),
            });
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    public string folderPath;

    [Button]
    public void LoadMaterialsFromFolder()
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Material", new[] { folderPath });

        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Material mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (mat != null && !Materials.Contains(mat))
            {
                Materials.Add(mat);
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();

        EditorLogger.Log("[LoadMaterialsFromFolder] DONE");
    }

#endif
}