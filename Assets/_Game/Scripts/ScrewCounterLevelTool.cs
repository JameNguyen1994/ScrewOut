using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewCounterLevelTool : MonoBehaviour
{


    [ContextMenu("Log screw count")]
    void CountScrewsPerLayer()
    {
        Transform root = transform; // Gắn script này vào GameObject gốc (ví dụ: Level_65)

        foreach (Transform layer in root)
        {
            int screwCount = 0;

            // Đệ quy hoặc duyệt toàn bộ con trong Layer
            screwCount = CountScrewsInTransform(layer);

            Debug.Log($"Layer '{layer.name}' có {screwCount} screw.");
        }
    }

    int CountScrewsInTransform(Transform parent)
    {
        int count = 0;

        foreach (Transform child in parent)
        {
            if (child.name.ToLower().Contains("screw"))
            {
                count++;
            }

            // Kiểm tra tiếp các con bên trong (nếu có)
            count += CountScrewsInTransform(child);
        }

        return count;
    }
}
