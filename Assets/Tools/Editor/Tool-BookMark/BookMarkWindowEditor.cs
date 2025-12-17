using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR

public class BookMarkWindowEditor : EditorWindow
{
    static private List<Object> bookmarkedObjects = new List<Object>();
    static private List<string> bookmarkedFolders = new List<string>();
    static public Dictionary<string, List<string>> objectTags = new Dictionary<string, List<string>>();
    static private bool autoRefresh = true;
    private ShowType showType = ShowType.Short;
    static private string jsonFilePathBook = "Assets/Tools/Tool-BookMark/bookmarks.json";
    static private string jsonFilePathFixedTag = "Assets/Tools/Tool-BookMark/fixedTags.json";
    static private List<string> fixedTags = new List<string>();// { "Important", "ToDo", "Reviewed", "Pending" }; // Fixed list of tags
    static private List<bool> showFixedTags = new List<bool>();// { "Important", "ToDo", "Reviewed", "Pending" }; // Fixed list of tags
    static private bool isShowFull = true;

    private Vector2 scrollPosition = Vector2.zero;
    [MenuItem("Tam's Window/Bookmarks")]
    public static void ShowWindow()
    {
        LoadBookmarks();
        EditorWindow.GetWindow<BookMarkWindowEditor>("Bookmarks");
    }

    public void AutoRefresh()
    {
        // SaveTags();
        LoadBookmarks();
        LoadFixedTags();
    }

    static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            BookMarkWindowEditor window = GetWindow<BookMarkWindowEditor>("Bookmarks");
            if (window != null)
            {
                window.AutoRefresh();
            }
        }
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

    }

    private void OnDestroy()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    void OnGUI()
    {
        if (GUILayout.Button("Clear", GUILayout.Width(100)))
        {
            bookmarkedObjects.Clear();
            bookmarkedFolders.Clear();
            objectTags.Clear();
            SaveBookmarks();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(100)))
        {
            LoadBookmarks();
        }

        autoRefresh = GUILayout.Toggle(autoRefresh, "");
        if (autoRefresh)
            AutoRefresh();

        GUILayout.EndHorizontal();

        if (GUILayout.Button($"Showtype: {showType}", GUILayout.Width(100)))
        {
            showType = showType == ShowType.Short ? ShowType.Detail : ShowType.Short;
        }
        if (GUILayout.Button($"Edit Tags", GUILayout.Width(100)))
        {
            var editTagWindow = EditorWindow.GetWindow<TagManagementWindow>();
            if (editTagWindow == null)
                editTagWindow = EditorWindow.CreateWindow<TagManagementWindow>();

            TagManagementWindow.InitListTag(fixedTags);
            TagManagementWindow.SetCallBackSave(SaveFixedTags);

        }
        GUILayout.Label("Bookmarks: ", EditorStyles.boldLabel);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        // Display bookmarks under each fixed tag
        for (int i = 0; i < fixedTags.Count; i++)
        {
            var tag = fixedTags[i];
            int count = 0;
            foreach (var obj in bookmarkedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (objectTags.ContainsKey(path) && objectTags[path].Contains(tag))
                {
                    count++;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{tag}({count})", EditorStyles.boldLabel, GUILayout.Width(300));
            showFixedTags[i] = GUILayout.Toggle(showFixedTags[i], "");

            GUILayout.EndHorizontal();
            bool hasBookmarks = false;

            if (showFixedTags[i])
                foreach (var obj in bookmarkedObjects)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    if (objectTags.ContainsKey(path) && objectTags[path].Contains(tag))
                    {
                        hasBookmarks = true;
                        GUILayout.BeginHorizontal();

                        if (showType == ShowType.Detail)
                        {

                            if (GUILayout.Button("X", GUILayout.Width(20)))
                            {
                                bookmarkedObjects.Remove(obj);
                                SaveBookmarks();
                                Repaint();
                                return;
                            }

                            if (GUILayout.Button(":", GUILayout.Width(10)))
                            {
                                TagWindow.ShowWindow(obj, objectTags.ContainsKey(path) ? objectTags[path] : new List<string>(), SaveBookmarks);
                                GetWindow<TagWindow>().InitListTag(fixedTags);
                            }
                            if (GUILayout.Button("Open", GUILayout.Width(50)))
                            {
                                AssetDatabase.OpenAsset(obj);
                            }

                            if (GUILayout.Button("Ping", GUILayout.Width(50)))
                            {
                                EditorGUIUtility.PingObject(obj);
                            }

                            if (GUILayout.Button("Properties", GUILayout.Width(100)))
                            {
                                ShowProperties(obj);
                            }

                            GUILayout.Label(obj.name + ": " + path);

                            string tags = string.Join(", ", objectTags.ContainsKey(path) ? objectTags[path] : new List<string>());
                            GUILayout.Label($"Tags: {tags}");

                        }
                        else
                        {
                            if (GUILayout.Button(":", GUILayout.Width(10)))
                            {
                                Debug.Log("adfasdf");

                                TagWindow.ShowWindow(obj, objectTags.ContainsKey(path) ? objectTags[path] : new List<string>(), SaveBookmarks);
                                GetWindow<TagWindow>().InitListTag(fixedTags);
                            }
                            if (GUILayout.Button($"{obj.name}"))
                            {
                                AssetDatabase.OpenAsset(obj);
                            }
                        }

                        GUILayout.EndHorizontal();
                    }
                }

            if (!hasBookmarks)
            {
                // GUILayout.Label("No bookmarks under this tag.", EditorStyles.label);
            }

            GUILayout.Space(10); // Add some space between different tag groups
        }
        // Display bookmarks without any tags
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Full Bookmarks: ({bookmarkedObjects.Count})", EditorStyles.boldLabel, GUILayout.Width(300));
        isShowFull = GUILayout.Toggle(isShowFull, "");
        GUILayout.EndHorizontal();
        if (isShowFull)
            foreach (var obj in bookmarkedObjects)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                // if (!objectTags.ContainsKey(path) || objectTags[path].Count == 0 || objectTags[path][0] == "")
                {
                    GUILayout.BeginHorizontal();

                    if (showType == ShowType.Detail)
                    {
                        if (GUILayout.Button("X", GUILayout.Width(10)))
                        {
                            bookmarkedObjects.Remove(obj);
                            SaveBookmarks();
                            Repaint();
                            return;
                        }
                        if (GUILayout.Button(":", GUILayout.Width(30)))
                        {
                            TagWindow.ShowWindow(obj, objectTags.ContainsKey(path) ? objectTags[path] : new List<string>(), SaveBookmarks);
                            GetWindow<TagWindow>().InitListTag(fixedTags);
                        }
                        if (GUILayout.Button("Open", GUILayout.Width(50)))
                        {
                            AssetDatabase.OpenAsset(obj);
                        }

                        if (GUILayout.Button("Ping", GUILayout.Width(50)))
                        {
                            EditorGUIUtility.PingObject(obj);
                        }

                        if (GUILayout.Button("Properties", GUILayout.Width(100)))
                        {
                            ShowProperties(obj);
                        }

                        GUILayout.Label(obj.name + ": " + path);

                        string tags = string.Join(", ", objectTags.ContainsKey(path) ? objectTags[path] : new List<string>());
                        GUILayout.Label($"Tags: {tags}");


                    }
                    else
                    {
                        if (GUILayout.Button(":", GUILayout.Width(10)))
                        {

                            TagWindow.ShowWindow(obj, objectTags.ContainsKey(path) ? objectTags[path] : new List<string>(), SaveBookmarks);
                            GetWindow<TagWindow>().InitListTag(fixedTags);
                        }
                        if (GUILayout.Button($"{obj.name}"))
                        {
                            AssetDatabase.OpenAsset(obj);
                        }
                    }

                    GUILayout.EndHorizontal();
                }
            }


        GUILayout.Label("Bookmarked Folders: ", EditorStyles.boldLabel);

        for (int i = 0; i < bookmarkedFolders.Count; i++)
        {
            var folderPath = bookmarkedFolders[i];

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                bookmarkedFolders.RemoveAt(i);
                SaveBookmarks();
                Repaint();
                return;
            }

            if (GUILayout.Button(folderPath, GUILayout.Width(200)))
            {
                PingFolder(folderPath);
            }

            GUILayout.Label(folderPath);
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }


    void PingFolder(string folderPath)
    {
        string absoluteFolderPath = folderPath;
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(absoluteFolderPath));
    }

    [MenuItem("Assets/Bookmark")]
    static void BookmarkAsset(MenuCommand command)
    {
        Object obj = Selection.activeObject;

        if (obj != null)
        {
            if (!bookmarkedObjects.Contains(obj))
                bookmarkedObjects.Add(obj);
            SaveBookmarks();
        }
        EditorWindow.GetWindow<BookMarkWindowEditor>("Bookmarks").Show();
    }

    [MenuItem("Assets/Bookmark Folder")]
    static void BookmarkFolder(MenuCommand command)
    {
        string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (!string.IsNullOrEmpty(folderPath) && AssetDatabase.IsValidFolder(folderPath))
        {
            if (!bookmarkedFolders.Contains(folderPath))
                bookmarkedFolders.Add(folderPath);
            SaveBookmarks();
        }

        EditorWindow.GetWindow<BookMarkWindowEditor>("Bookmarks").Show();
    }

    static void ShowProperties(Object obj)
    {
        EditorWindow propertiesWindow = EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
        if (propertiesWindow != null)
        {
            propertiesWindow.titleContent.text = "Properties - " + obj.name;
            propertiesWindow.Show();
        }
    }

    static void SaveBookmarks()
    {
        BookmarkData data = new BookmarkData();
        data.objects = new string[bookmarkedObjects.Count + bookmarkedFolders.Count];
        int index = 0;

        // Save bookmarked objects
        foreach (var obj in bookmarkedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            data.objects[index] = path;
            if (objectTags.ContainsKey(path))
            {
                data.objects[index] += $"|{string.Join(",", objectTags[path].ToArray())}";
            }
            index++;
        }

        // Save bookmarked folders
        foreach (var folderPath in bookmarkedFolders)
        {
            data.objects[index] = folderPath;
            index++;
        }

        string jsonData = JsonUtility.ToJson(data);
        string directory = Path.GetDirectoryName(jsonFilePathBook);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(jsonFilePathBook, jsonData);
        AssetDatabase.Refresh();
    }

    static void LoadBookmarks()
    {
        if (File.Exists(jsonFilePathBook))
        {
            string jsonData = File.ReadAllText(jsonFilePathBook);
            BookmarkData data = JsonUtility.FromJson<BookmarkData>(jsonData);

            bookmarkedObjects.Clear();
            bookmarkedFolders.Clear();
            objectTags.Clear();

            foreach (string entry in data.objects)
            {
                string[] parts = entry.Split('|');
                string path = parts[0];
                if (AssetDatabase.IsValidFolder(path))
                {
                    bookmarkedFolders.Add(path);
                }
                else
                {
                    Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj != null)
                    {
                        bookmarkedObjects.Add(obj);

                        if (parts.Length > 1)
                        {
                            List<string> tags = new List<string>(parts[1].Split(','));
                            objectTags.Add(path, tags);
                        }
                        else
                        {
                            objectTags.Add(path, new List<string>());
                        }
                    }
                }
            }
        }
    }

    static public void LoadFixedTags()
    {
        if (File.Exists(jsonFilePathFixedTag))
        {
            string jsonData = File.ReadAllText(jsonFilePathFixedTag);
            FixedTagDatas data = JsonUtility.FromJson<FixedTagDatas>(jsonData);

            fixedTags.Clear();
            //objectTags.Clear();

            foreach (var entry in data.fixedTags)
            {
                fixedTags.Add(entry.fixedTag);
                showFixedTags.Add(entry.isShow);
            }
        }
    }
    static public void SaveFixedTags()
    {
        FixedTagDatas data = new FixedTagDatas();
        data.fixedTags = new List<FixedTag>();
        for (int i = 0; i < fixedTags.Count; i++)
        {
            data.fixedTags.Add(new FixedTag(fixedTags[i], showFixedTags[i]));
        }
        string jsonData = JsonUtility.ToJson(data);
        string directory = Path.GetDirectoryName(jsonFilePathFixedTag);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(jsonFilePathFixedTag, jsonData);
        AssetDatabase.Refresh();
    }

}

[InitializeOnLoad]
public class AutoRefresh
{
    public static void RefreshWindow()
    {
        BookMarkWindowEditor window = EditorWindow.GetWindow<BookMarkWindowEditor>("Bookmarks");
        if (window != null)
        {
            window.AutoRefresh();
        }
    }
}


[System.Serializable]
public class BookmarkData
{
    public string[] objects;
    public List<TagData> tags;
}

[System.Serializable]
public class TagData
{
    public string path;
    public List<string> tags;
}

[System.Serializable]
public class FixedTagDatas
{
    public List<FixedTag> fixedTags;
}
[System.Serializable]
public class FixedTag
{
    public string fixedTag;
    public bool isShow;

    public FixedTag(string fixedTag, bool isShow)
    {
        this.fixedTag = fixedTag;
        this.isShow = isShow;
    }
}



public enum ShowType
{
    Short = 0,
    Detail = 1
}
#endif