using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OptimizeLevel.LevelDifficulty.Editor
{
    public class ItemLevel
    {
        private LevelScrewBlockedData data;
        private System.Action<LevelScrewBlockedData> onClick;

        public ItemLevel(LevelScrewBlockedData data, System.Action<LevelScrewBlockedData> onClick)
        {
            this.data = data;
            this.onClick = onClick;
        }

        public void Draw(int width = 180)
        {
            int blockedCount = data.lstScrewBlockedData.Count(d => d.lstIndexShapeBlock != null && d.lstIndexShapeBlock.Count > 0);
            int coveredCount = data.lstScrewBlockedData.Count(d => d.lstIndexShapeCover != null && d.lstIndexShapeCover.Count > 0);

            EditorGUILayout.BeginVertical("box", GUILayout.Width(width));

            if (GUILayout.Button($"Level {data.level}", EditorStyles.boldLabel))
            {
                onClick?.Invoke(data); // chuyển sang detail mode
            }

            EditorGUILayout.LabelField($"Total: {data.totalScrew}");
            EditorGUILayout.LabelField($"Blocked: {blockedCount}");
            EditorGUILayout.LabelField($"Covered: {coveredCount}");

            EditorGUILayout.EndVertical();
        }
    }
}
