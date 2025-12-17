using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class CanvasManagerWindow : EditorWindow
{
    private Vector2 scrollPosition = Vector2.zero;
    private List<Canvas> sortedCanvases = new List<Canvas>();
    private bool isHighToLow = true;
    private string searchKeyword = "";

    [MenuItem("Tam's Window/Canvas Manager")]
    public static void ShowWindow()
    {
        GetWindow<CanvasManagerWindow>("Canvas Manager");
    }

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += SceneOpenedCallback;
        EditorApplication.playModeStateChanged += OnPlayModeChange;
        FilterCanvasList();
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= SceneOpenedCallback;
        EditorApplication.playModeStateChanged -= OnPlayModeChange;

    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("List of Canvases" + (isHighToLow ? " Higher to Lower" : " Lower to Higher"), EditorStyles.boldLabel);
        if (GUILayout.Button("Reverse Sort Order"))
        {
            isHighToLow = !isHighToLow;
            SortCanvasList();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        searchKeyword = EditorGUILayout.TextField("Search: ", searchKeyword);
        if (GUILayout.Button("Search"))
        {
            FilterCanvasList();
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Export CSV"))
        {
            ExportCanvasListToCSV();
        }
        // Tạo ScrollView
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Hiển thị danh sách các canvas đã được sắp xếp
        for (int i = 0; i < sortedCanvases.Count; i++)
        {
            var canvas = sortedCanvases[i];
            if (canvas == null)
                continue;
            GUILayout.BeginHorizontal();
            GUILayout.Label($"({i + 1})", EditorStyles.boldLabel, GUILayout.Width(50));
            EditorGUILayout.ObjectField(canvas, typeof(Canvas), true);
            GUILayout.EndHorizontal();

            // Hiển thị thông tin về chế độ, order in layer và plane distance của canvas
            EditorGUILayout.LabelField("Render Mode: ", canvas.renderMode.ToString());
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
                    canvas.planeDistance = EditorGUILayout.IntField("Plane Distance: ", (int)canvas.planeDistance);
                canvas.sortingOrder = EditorGUILayout.IntField("Order in Layer: ", canvas.sortingOrder);
            }
            GUILayout.Space(20);
        }

        // Kết thúc ScrollView
        EditorGUILayout.EndScrollView();
    }

    void SortCanvasList()
    {
        sortedCanvases.Sort((a, b) =>
        {
            if (a.renderMode == RenderMode.ScreenSpaceOverlay && b.renderMode != RenderMode.ScreenSpaceOverlay)
                return -1;
            else if (a.renderMode != RenderMode.ScreenSpaceOverlay && b.renderMode == RenderMode.ScreenSpaceOverlay)
                return 1;
            else if (a.renderMode == RenderMode.ScreenSpaceCamera && b.renderMode == RenderMode.ScreenSpaceCamera)
            {
                int planeDistanceComparison = a.planeDistance.CompareTo(b.planeDistance);
                if (planeDistanceComparison != 0)
                    return planeDistanceComparison;
                else
                    return b.sortingOrder.CompareTo(a.sortingOrder);
            }
            else
                return b.sortingOrder.CompareTo(a.sortingOrder);
        });

        if (!isHighToLow)
            sortedCanvases.Reverse();
    }

    void FilterCanvasList()
    {
        if (string.IsNullOrEmpty(searchKeyword))
        {
            sortedCanvases = GameObject.FindObjectsOfType<Canvas>().ToList();
            SortCanvasList();
            return;
        }

        sortedCanvases = GameObject.FindObjectsOfType<Canvas>().Where(canvas => canvas.name.Contains(searchKeyword)).ToList();
        SortCanvasList();
    }

    void SceneOpenedCallback(Scene scene, OpenSceneMode mode)
    {
        searchKeyword = "";
        Reset();
        FilterCanvasList();
    }
    void OnPlayModeChange(PlayModeStateChange state)
    {
        searchKeyword = "";
        Reset();
        FilterCanvasList();
    }

    void Reset()
    {
        sortedCanvases = new List<Canvas>();
    }


    void ExportCanvasListToCSV()
    {
        // Chuỗi CSV bắt đầu với tiêu đề
        string csvContent = "Index,Canvas Name,Render Mode,Order in Layer,Plane Distance\n";

        // Duyệt qua danh sách các Canvas
        for (int i = 0; i < sortedCanvases.Count; i++)
        {
            var canvas = sortedCanvases[i];
            if (canvas == null)
                continue;

            // Tạo một dòng cho mỗi Canvas
            string sortingLayer = $",{canvas.sortingOrder}";
            string planceDistance = $",{canvas.planeDistance}";
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                planceDistance = "";
            string canvasData = $"{i + 1},{canvas.name},{canvas.renderMode}{sortingLayer}{planceDistance}\n";
            csvContent += canvasData;
        }

        // Lưu chuỗi CSV vào tệp tin
        string filePath = EditorUtility.SaveFilePanel("Save Canvas List", "", "CanvasList", "csv");
        if (!string.IsNullOrEmpty(filePath))
        {
            System.IO.File.WriteAllText(filePath, csvContent);
            Debug.Log("Canvas list exported to: " + filePath);
        }
    }
}
