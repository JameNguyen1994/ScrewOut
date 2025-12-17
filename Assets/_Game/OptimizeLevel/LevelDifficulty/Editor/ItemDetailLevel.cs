using Cysharp.Threading.Tasks;
using NUnit.Framework;
using ScrewCraze3D;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace OptimizeLevel.LevelDifficulty.Editor
{
    public class ItemDetailLevel
    {
        private LevelScrewBlockedData data;
        private Vector2 scrollPosDefaultData;
        private Vector2 scrollPosRealTime;
        private bool isShowHighlight = true;
        private bool showDefaltData = true;
        private bool showRealTimeData = true;

        private List<Screw> lstTotalScrewAvailable = new List<Screw>();

        private System.Action onBack;


        private List<ScrewBlockedData> lstScrewBlockedDataRealTime = new List<ScrewBlockedData>();
        public ItemDetailLevel(LevelScrewBlockedData data, System.Action onBack)
        {
            this.data = data;
            this.onBack = onBack;
        }

        public void Draw()
        {

            if (GUILayout.Button("← Back", GUILayout.Width(80)))
            {
                onBack?.Invoke();
                return;
            }


            string note = "";
            if (Application.isPlayer)
            {
                var levelMap = LevelController.Instance?.Level;
                if (levelMap != null && levelMap.LevelId == data.level)
                {
                    note = " (Loaded)";
                }
                else
                {
                    note = " (Click to Load)";
                }
            }
            else
            {
                note = " (Editor Mode)";
            }
            GUILayout.Label($"Detail Level Data {note}", EditorStyles.boldLabel);
            if (GUILayout.Button("Load Level", GUILayout.Width(100)))
            {
                LoadLevel().Forget();
            }
            if (GUILayout.Button("Reset Target", GUILayout.Width(100)))
            {
                var levelMap = LevelController.Instance?.Level;
                if (levelMap != null)
                    levelMap.ResetTarget();
            }

            isShowHighlight = GUILayout.Toggle(isShowHighlight, "Highlight Selected Screw", GUILayout.Width(200));
            EditorGUILayout.LabelField($"Level {data.level}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Screw: {data.totalScrew}");
            GUILayout.BeginHorizontal();
            showDefaltData = GUILayout.Toggle(showDefaltData, "", GUILayout.Width(50));
            GUILayout.Label("-------------------------------------------Default Data---------------------------------------------------------------------");

            GUILayout.EndHorizontal();

            if (showDefaltData)
            {


                int blockedCount = data.lstScrewBlockedData.Count(d => d.lstIndexShapeBlock != null && d.lstIndexShapeBlock.Count > 0);
                int coveredCount = data.lstScrewBlockedData.Count(d => d.lstIndexShapeCover != null && d.lstIndexShapeCover.Count > 0);

                EditorGUILayout.LabelField($"Blocked: {blockedCount}");
                EditorGUILayout.LabelField($"Covered: {coveredCount}");

                EditorGUILayout.Space();

                // 🔥 bắt đầu scroll riêng cho danh sách Screw
                scrollPosDefaultData = EditorGUILayout.BeginScrollView(scrollPosDefaultData, GUILayout.Height(300));
                int itemAmountAt1Row = 10;
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();

                for (int i = 0; i < data.lstScrewBlockedData.Count; i++)
                {
                    if (i > 0 && i % itemAmountAt1Row == 0)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                    var screwData = data.lstScrewBlockedData[i];
                    string blockedList = screwData.lstIndexShapeBlock != null && screwData.lstIndexShapeBlock.Count > 0
                        ? string.Join(",", screwData.lstIndexShapeBlock)
                        : "(none)";
                    string coveredList = screwData.lstIndexShapeCover != null && screwData.lstIndexShapeCover.Count > 0
                        ? string.Join(",", screwData.lstIndexShapeCover)
                        : "(none)";

                    var levelMap = LevelController.Instance?.Level;
                    var screw = (levelMap != null && levelMap.LevelId == data.level) ? levelMap?.LstScrew[screwData.index] : null;

                    if (screw != null && screw.State == ScrewState.OnShape)
                        GUI.backgroundColor = GetColor(screw.ScrewColor); // xanh lá nhạt
                    else
                        GUI.backgroundColor = Color.white;


                    // 🔥 Tạo rect cho button
                    string btnText = $"{i + 1}. ScrewID: {screwData.index}\nBlocked by: {blockedList}\nCovered by: {coveredList}";
                    Rect rect = GUILayoutUtility.GetRect(new GUIContent(btnText), GUI.skin.button, GUILayout.Height(60));

                    // Vẽ button
                    if (GUI.Button(rect, btnText))
                    {
                        Debug.Log($"Clicked Screw {screwData.index} in Level {data.level}");
                        if (screw != null)
                        {
                            EditorGUIUtility.PingObject(screw.gameObject);
                            Selection.activeGameObject = screw.gameObject;
                            if (isShowHighlight)
                                levelMap.ShowScrewAndShapeTarget(screw);
                        }
                    }

                    // 🔥 Highlight viền nếu screw này chính là screw đang chọn
                    var selectedScrew = ScrewSelectionHelper.GetSelectedScrew();
                    if (selectedScrew != null && screw != null && selectedScrew == screw)
                    {
                        Color borderColor = Color.green;
                        EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, 2), borderColor); // top
                        EditorGUI.DrawRect(new Rect(rect.x - 2, rect.yMax, rect.width + 4, 2), borderColor); // bottom
                        EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y - 2, 2, rect.height + 4), borderColor); // left
                        EditorGUI.DrawRect(new Rect(rect.xMax, rect.y - 2, 2, rect.height + 4), borderColor); // right
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                EditorGUILayout.EndScrollView();
                //LevelScrewBlockedDataWindow.Instance.EndScroll();
                GUILayout.Label("-------------------------------------------Default Data---------------------------------------------------------------------");
            }


            GUILayout.BeginHorizontal();
            showRealTimeData = GUILayout.Toggle(showRealTimeData, "", GUILayout.Width(50));

            GUILayout.Label("-------------------------------------------Real Time Data---------------------------------------------------------------------");
            GUILayout.EndHorizontal();
            if (showRealTimeData)
            {

                var levelMap = LevelController.Instance?.Level;
                if (levelMap != null)
                {
                    scrollPosRealTime = EditorGUILayout.BeginScrollView(scrollPosRealTime, GUILayout.Height(300));
                    int itemAmountAt1Row = 10;


                    int blockedCount = levelMap.LstScrew.Count(d => d.LstShapeBlocked != null && d.LstShapeBlocked.Count > 0);
                    int coveredCount = levelMap.LstScrew.Count(d => d.LstShapeCovered != null && d.LstShapeCovered.Count > 0);

                    EditorGUILayout.LabelField($"Blocked: {blockedCount}");
                    EditorGUILayout.LabelField($"Covered: {coveredCount}");
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    var screws = levelMap.LstScrew;
                    for (int i = 0; i < screws.Count; i++)
                    {
                        if (i > 0 && i % itemAmountAt1Row == 0)
                        {
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal();
                        }
                        var screw = screws[i];
                        if (screw != null && (screw.State == ScrewState.OnShape || screw.State == ScrewState.OnTray))
                            GUI.backgroundColor = GetColor(screw.ScrewColor); // xanh lá nhạt
                        else
                            GUI.backgroundColor = Color.white;

                        string blockedList = screw.LstShapeBlocked != null && screw.LstShapeBlocked.Count > 0
                       ? string.Join(",", screw.LstShapeBlocked)
                       : "(none)";
                        string coveredList = screw.LstShapeCovered != null && screw.LstShapeCovered.Count > 0
                            ? string.Join(",", screw.LstShapeCovered)
                            : "(none)";
                        // 🔥 Tạo rect cho button
                        if (screw == null || screw.gameObject == null)
                            continue;
                        string btnText = $"{i + 1}. {screw.gameObject.name}\nBlocked by: {blockedList}\nCovered by: {coveredList}";
                        Rect rect = GUILayoutUtility.GetRect(new GUIContent(btnText), GUI.skin.button, GUILayout.Height(60));

                        // Vẽ button
                        if (GUI.Button(rect, btnText))
                        {
                            Debug.Log($"Clicked Screw {i} in Level {data.level}");
                            if (screw != null)
                            {
                                EditorGUIUtility.PingObject(screw.gameObject);
                                Selection.activeGameObject = screw.gameObject;
                                if (isShowHighlight)
                                    levelMap.ShowScrewAndShapeTarget(screw);
                            }
                        }

                        // 🔥 Highlight viền nếu screw này chính là screw đang chọn
                        var selectedScrew = ScrewSelectionHelper.GetSelectedScrew();
                        if (selectedScrew != null && screw != null && selectedScrew == screw)
                        {
                            Color borderColor = Color.green;
                            EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y - 2, rect.width + 4, 2), borderColor); // top
                            EditorGUI.DrawRect(new Rect(rect.x - 2, rect.yMax, rect.width + 4, 2), borderColor); // bottom
                            EditorGUI.DrawRect(new Rect(rect.x - 2, rect.y - 2, 2, rect.height + 4), borderColor); // left
                            EditorGUI.DrawRect(new Rect(rect.xMax, rect.y - 2, 2, rect.height + 4), borderColor); // right
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                }
            }

            GUILayout.Label("-------------------------------------------Real Time Data Color---------------------------------------------------------------------");
            if (ScrewBlockedRealTimeController.Instance != null)
            {
                GUILayout.Label($"Total Screw {ScrewBlockedRealTimeController.Instance.DicTotalScrew.Keys.Count}");
                GUILayout.BeginHorizontal();
                var totalScrews = ScrewBlockedRealTimeController.Instance.DicTotalScrew;
                foreach (var kvp in totalScrews)
                {
                    var color = kvp.Key;
                    var count = kvp.Value;
                    GUI.backgroundColor = GetColor(color);
                    if (GUILayout.Button($"{color}: {count}", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        lstTotalScrewAvailable.Clear();
                        var levelMap = LevelController.Instance?.Level;
                        var screws = levelMap.LstScrew;
                        for (int i = 0; i < screws.Count; i++)
                        {
                            var screw = screws[i];
                            if (screw.ScrewColor == color && screw.State == ScrewState.OnShape)

                            {
                                if (!lstTotalScrewAvailable.Contains(screw))
                                    lstTotalScrewAvailable.Add(screw);
                            }
                        }

                        levelMap.ShowLevelMapScrews(lstTotalScrewAvailable, isShowHighlight);
                        ScrewUtility.PingAll(lstTotalScrewAvailable);

                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Label($"Current Screw {ScrewBlockedRealTimeController.Instance.DicCurrScrew.Keys.Count}");
                var currBlockedScrewsResolved = ScrewBlockedRealTimeController.Instance.DicCurrScrewResolved;
                var totalResolve = 0;
                var totalScrew = 0;
                GUILayout.BeginHorizontal();
                var currScrews = ScrewBlockedRealTimeController.Instance.DicCurrScrew;
                foreach (var kvp in currScrews)
                {
                    var color = kvp.Key;
                    var count = kvp.Value;
                    var countResolved = currBlockedScrewsResolved.ContainsKey(color) ? currBlockedScrewsResolved[color] : 0;
                    totalResolve += countResolved;
                    totalScrew+= count;
                    GUI.backgroundColor = GetColor(color);
                    if (GUILayout.Button($"{color}: {countResolved}/{count}", GUILayout.Width(100), GUILayout.Height(30)))
                    {

                    }
                }
                GUILayout.Label($"Total: {totalResolve}/{totalScrew}");
                GUILayout.EndHorizontal();

                GUILayout.Label($"Total Screw Available {ScrewBlockedRealTimeController.Instance.DicBlockTotalBlockedScrew.Keys.Count}");

                GUILayout.BeginHorizontal();
                var blockedScrews = ScrewBlockedRealTimeController.Instance.DicBlockTotalBlockedScrew;
                foreach (var kvp in blockedScrews)
                {
                    var color = kvp.Key;
                    var count = kvp.Value;
                    GUI.backgroundColor = GetColor(color);
                    if (GUILayout.Button($"{color}: {count}", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                        lstTotalScrewAvailable.Clear();
                        var levelMap = LevelController.Instance?.Level;
                        var screws = levelMap.LstScrewAvailable;
                        for (int i = 0; i < screws.Count; i++)
                        {
                            var screw = screws[i];
                            if (screw.ScrewColor == color && screw.State == ScrewState.OnShape)
                            {
                                if (!lstTotalScrewAvailable.Contains(screw))
                                    lstTotalScrewAvailable.Add(screw);
                            }
                        }

                        levelMap.ShowLevelMapScrews(lstTotalScrewAvailable, isShowHighlight);
                        ScrewUtility.PingAll(lstTotalScrewAvailable);

                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Label($"Current Screw Available {ScrewBlockedRealTimeController.Instance.DicCurrBlockedScrew.Keys.Count}");

                GUILayout.BeginHorizontal();
                var totalCurrBlockedScrews = 0;
                var currBlockedScrews = ScrewBlockedRealTimeController.Instance.DicCurrBlockedScrew;
                foreach (var kvp in currBlockedScrews)
                {
                    var color = kvp.Key;
                    var count = kvp.Value;
                    GUI.backgroundColor = GetColor(color);
                    totalCurrBlockedScrews += count;
                    if (GUILayout.Button($"{color}:{count}", GUILayout.Width(100), GUILayout.Height(30)))
                    {
                    }
                }
                int totalAvailableOnLevel = LevelController.Instance.Level.LstScrewAvailable.Count;
                GUILayout.Label($"Total: {totalCurrBlockedScrews} / {totalAvailableOnLevel}");
                GUILayout.EndHorizontal();
            }
        }

        private async UniTask LoadLevel()
        {
            await LevelController.Instance.ForceLoadLevel(data.level);

            var levelMap = LevelController.Instance.Level;
        }
        private Color GetColor(ScrewColor screwColor)
        {
            switch (screwColor)
            {
                case ScrewColor.Blue:
                    return new Color(0.0f, 0.45f, 0.85f);        // xanh dương tươi
                case ScrewColor.Orange:
                    return new Color(1f, 0.55f, 0.0f);           // cam tươi
                case ScrewColor.Brown:
                    return new Color(0.55f, 0.27f, 0.07f);       // nâu gỗ
                case ScrewColor.Purple:
                    return new Color(0.6f, 0.2f, 0.8f);          // tím đậm
                case ScrewColor.Be:
                    return new Color(0.96f, 0.87f, 0.70f);       // be sáng
                case ScrewColor.Sky:
                    return new Color(0.53f, 0.81f, 0.98f);       // xanh da trời
                case ScrewColor.Pink:
                    return new Color(1f, 0.41f, 0.71f);          // hồng pastel
                case ScrewColor.Green:
                    return new Color(0.13f, 0.55f, 0.13f);       // xanh lá (forest green)
                case ScrewColor.Yellow:
                    return new Color(1f, 0.92f, 0.23f);          // vàng tươi
                case ScrewColor.Red:
                    return new Color(0.9f, 0.1f, 0.1f);          // đỏ đậm

                case ScrewColor.None:
                default:
                    return Color.white;
            }
        }


    }


}
