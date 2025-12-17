using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Process Level Bar Data", fileName = "process_lvl_bar_data")]
public class ProcessLevelBarData : ScriptableObject
{
    public List<LevelDotData> lstData;

    #if UNITY_EDITOR
    [ContextMenu("Auto InitUI Data")]
    void InitDataAuto()
    {
        DoInit();
    }
    
    void DoInit()
    {
        // Đường dẫn thư mục
        string folderPath = Path.Combine(Application.dataPath, "_Game/Art/2D/LevelProcess");

        // Kiểm tra xem thư mục có tồn tại không
        if (!Directory.Exists(folderPath))
        {
            Debug.LogWarning($"Folder not found: {folderPath}");
            return;
        }

        // Lấy tất cả các file trong thư mục
        List<string> files = Directory.GetFiles(folderPath, "icon_preview_*.png", SearchOption.TopDirectoryOnly).ToList();

        // Đếm file có chứa cụm từ "icon_preview" trong tên
        int lvl = 1;
        lstData.Clear();

        while (lvl <= files.Count)
        {
            var filePath = files.Find(x => Path.GetFileName(x).Contains($"icon_preview_{lvl}"));
            string assetPath = "Assets" + filePath.Replace(Application.dataPath, "").Replace("\\", "/");
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite)
            {
                lstData.Add(new LevelDotData()
                {
                    level = lvl,
                    icon = sprite
                });

                lvl++;
            }
        }
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
    [ContextMenu("SetLevel")]
    public void SetLevel()
    {
        for (int i = 65; i < 121; i++)
        {
            lstData[i].level = i+1;
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [ContextMenu("Check Data Missing Or Null")]
    public void CheckDataMissingOrNull()
    {
        int numDataNull = 0;
        for (int i = 0; i < lstData.Count; i++)
        {
            if (!lstData[i].icon)
            {
                numDataNull++;
                Debug.Log($"icon null at id {i} with level {lstData[i].level}");
            }
        }
        
        Debug.Log($"======Overall: missing total {numDataNull} icons=======");
    }
    #endif
}

[Serializable]
public class LevelDotData
{
    public int level;
    public Sprite icon;
}
