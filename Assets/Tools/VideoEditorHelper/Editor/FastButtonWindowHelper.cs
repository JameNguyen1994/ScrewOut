using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEditor.Events;
using System.Reflection;

namespace VideoEditorHelper
{
    public class FastButtonWindowHelper : EditorWindow
    {
        private string pathButtonsData = "Assets/Tools/VideoEditorHelper/Datas/FastButtonData.json";
        private List<ButtonData> buttonDatas = new List<ButtonData>();
        private List<string> lstDebug = new List<string>();
        private GameObject selectedObject;
        private int selectedComponentIndex = 0;
        private int oldSelectedComponentIndex = -1;
        private int selectedMethodIndex = 0;

        private List<MonoBehaviour> componentList = new List<MonoBehaviour>();
        private List<MethodInfo> methodList = new List<MethodInfo>();
        private int editingKeyIndex = -1;

        private List<FileHotkeyData> fileHotkeys = new List<FileHotkeyData>();
        private int editingFileHotkeyIndex = -1;  // để edit key giống ButtonData


        private Vector2 scroll;

        [MenuItem("Tam's Window/FastButtonWindowHelper")]
        public static void ShowWindow()
        {
            GetWindow<FastButtonWindowHelper>("FastButton Window Helper");
        }

        private void OnGUI()
        {
            lstDebug = new List<string>();
            GUILayout.Label("Fast Button Window Helper", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button("Load Data")) LoadData();
            if (GUILayout.Button("Save Data")) SaveData();

            GUILayout.Space(10);
            GUILayout.Label("Add Method From Object", EditorStyles.boldLabel);

            var newSelected = (GameObject)EditorGUILayout.ObjectField("Target GameObject", selectedObject, typeof(GameObject), true);
            if (newSelected != selectedObject)
            {
                selectedObject = newSelected;
                RefreshComponentList();
                oldSelectedComponentIndex = -1;
            }

            if (componentList.Count > 0)
            {
                string[] componentNames = componentList.ConvertAll(c => c.GetType().Name).ToArray();
                selectedComponentIndex = EditorGUILayout.Popup("Component", selectedComponentIndex, componentNames);
                if (oldSelectedComponentIndex != selectedComponentIndex)
                {
                    oldSelectedComponentIndex = selectedComponentIndex;
                    RefreshMethodList();

                }
            }

            if (methodList.Count > 0)
            {
                string[] methodNames = methodList.ConvertAll(m => m.Name).ToArray();
                selectedMethodIndex = EditorGUILayout.Popup("Method", selectedMethodIndex, methodNames);
            }

            GUI.enabled = selectedObject != null && componentList.Count > 0 && methodList.Count > 0;
            if (GUILayout.Button("Add to List"))
            {
                string scriptName = componentList[selectedComponentIndex].GetType().Name;
                string methodName = methodList[selectedMethodIndex].Name;
                buttonDatas.Add(new ButtonData(selectedObject.gameObject.name, scriptName, methodName));
            }
            GUI.enabled = true;

            GUILayout.Space(10);
            GUILayout.Label("Saved Button Data", EditorStyles.boldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(250));
            for (int i = 0; i < buttonDatas.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("", buttonDatas[i].gameObjectName, GUILayout.Width(200));
                EditorGUILayout.LabelField("", buttonDatas[i].scriptName, GUILayout.Width(200));
                EditorGUILayout.LabelField("", buttonDatas[i].methodName, GUILayout.Width(220));

                if (editingKeyIndex == i)
                {
                    EditorGUILayout.LabelField("Press a key...", GUILayout.Width(100));
                }
                else
                {
                    EditorGUILayout.LabelField($"Key: {buttonDatas[i].shortcutKey}", GUILayout.Width(100));
                }

                if (GUILayout.Button("Edit", GUILayout.Width(50)))
                {
                    editingKeyIndex = i;
                    GUI.FocusControl(null);
                }

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    buttonDatas.RemoveAt(i);
                    i--;
                    continue;
                }
                if (GUILayout.Button("GoTo", GUILayout.Width(80)))
                {
                    string goName = buttonDatas[i].gameObjectName;
                    string scriptName = buttonDatas[i].scriptName;

                    if (string.IsNullOrEmpty(goName))
                    {
                        Debug.LogWarning("No GameObjectName specified.");
                        //  return;
                    }
                    else
                    {


                        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                        List<GameObject> matchesByName = new List<GameObject>();

                        foreach (var obj in allObjects)
                        {
                            if (obj.name == goName)
                                matchesByName.Add(obj);
                        }

                        if (matchesByName.Count == 0)
                        {
                            Debug.LogWarning($"No GameObject found with name '{goName}'.");
                            // return;
                        }
                        else
                        {

                            bool found = false;
                            foreach (var go in matchesByName)
                            {
                                if (found)
                                    break;
                                foreach (var mono in go.GetComponents<MonoBehaviour>())
                                {

                                    if (mono != null && mono.GetType().Name == scriptName)
                                    {
                                        Selection.activeGameObject = go;
                                        //EditorGUIUtility.PingObject(go);
                                        Debug.Log($"GoTo: Selected '{go.name}' with component '{scriptName}'.");
                                        found = true;
                                        break;
                                    }
                                }
                            }
                            if (!found)
                                Debug.LogWarning($"No GameObject named '{goName}' contains script '{scriptName}'.");
                        }

                    }

                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();

            if (editingKeyIndex >= 0 && Event.current.type == EventType.KeyDown)
            {
                buttonDatas[editingKeyIndex].shortcutKey = Event.current.keyCode;
                Debug.Log($"Set shortcut: {buttonDatas[editingKeyIndex].shortcutKey}");
                editingKeyIndex = -1;
                Event.current.Use();
                Repaint();
            }

            GUILayout.Space(20);
            GUILayout.Label("Image / Video Hotkeys", EditorStyles.boldLabel);

            DrawFileHotkeySection();
            HandleFileDragAndDrop();   // xử lý kéo thả file

            ReCheckMissingGameobject();
            DebugList();
        }

        private void RefreshComponentList()
        {
            componentList.Clear();
            methodList.Clear();
            selectedComponentIndex = 0;
            selectedMethodIndex = 0;

            if (selectedObject == null) return;

            foreach (var mono in selectedObject.GetComponents<MonoBehaviour>())
            {
                if (mono != null)
                    componentList.Add(mono);
            }
            RefreshMethodList();
        }

        private void RefreshMethodList()
        {
            methodList.Clear();
            selectedMethodIndex = 0;

            if (componentList.Count == 0) return;

            var component = componentList[selectedComponentIndex];
            var methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                if (method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    methodList.Add(method);
                }
            }
        }

        public void LoadData()
        {
            if (!File.Exists(pathButtonsData)) return;

            string json = File.ReadAllText(pathButtonsData);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);

            if (wrapper != null)
            {
                buttonDatas = wrapper.buttonDatas ?? new List<ButtonData>();
                fileHotkeys = wrapper.fileHotkeys ?? new List<FileHotkeyData>();
            }
        }



        public void SaveData()
        {
            SaveWrapper wrapper = new SaveWrapper();
            wrapper.buttonDatas = buttonDatas;
            wrapper.fileHotkeys = fileHotkeys;

            File.WriteAllText(pathButtonsData, JsonUtility.ToJson(wrapper, true));
            AssetDatabase.Refresh();
        }

        public void ReCheckMissingGameobject()
        {
            if (Application.isPlaying)
            {
                var activeButtonHelper = FindAnyObjectByType<ActiveButtonHelper>();
                if (activeButtonHelper == null)
                {
                    lstDebug.Add("ActiveButtonHelper instance not found.Please add it to First Scene in your project");
                }
                for (int i = 0; i < buttonDatas.Count; i++)
                {
                    var goName = buttonDatas[i].gameObjectName;
                    var scriptName = buttonDatas[i].scriptName;

                    GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
                    List<GameObject> matchesByName = new List<GameObject>();

                    foreach (var obj in allObjects)
                    {
                        if (obj.name == goName)
                            matchesByName.Add(obj);
                    }

                    if (matchesByName.Count == 0)
                    {
                        lstDebug.Add($"No GameObject found with name '{goName}'.");
                        //Debug.LogWarning($"No GameObject found with name '{goName}'.");
                        // return;
                    }
                    else
                    {

                        bool found = false;
                        foreach (var go in matchesByName)
                        {
                            if (found)
                                break;
                            foreach (var mono in go.GetComponents<MonoBehaviour>())
                            {

                                if (mono != null && mono.GetType().Name == scriptName)
                                {
                                 //   Selection.activeGameObject = go;
                               //     EditorGUIUtility.PingObject(go);
                                  //  Debug.Log($"GoTo: Selected '{go.name}' with component '{scriptName}'.");
                                    lstDebug.Add($"Found GameObject '{go.name}' with script '{scriptName}'.");
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found)
                            lstDebug.Add($"No GameObject named '{goName}' contains script '{scriptName}'.");
                       // Debug.LogWarning($"No GameObject named '{goName}' contains script '{scriptName}'.");
                    }

                }
            }

        }
        private void DebugList()
        {

            if (Application.isPlaying)
            {
                GUILayout.Label($"Console");

                for (int i = 0; i < lstDebug.Count; i++)
                {
                    //  Debug.Log($"Debug {i}: {lstDebug[i]}");
                    GUILayout.Label( $"Debug {i}: {lstDebug[i]}");
                }
            }
        }
        private void DrawFileHotkeySection()
        {
            for (int i = 0; i < fileHotkeys.Count; i++)
            {
                var data = fileHotkeys[i];

                EditorGUILayout.BeginHorizontal("box");

                GUILayout.Label(Path.GetFileName(data.filePath), GUILayout.Width(200));
                GUILayout.Label(data.isVideo ? "Video" : "Image", GUILayout.Width(80));
                GUILayout.Label("Key: " + data.hotkey, GUILayout.Width(80));

                if (GUILayout.Button("Edit Key", GUILayout.Width(80)))
                {
                    editingFileHotkeyIndex = i;
                    GUI.FocusControl(null);
                }

                if (GUILayout.Button("X", GUILayout.Width(30)))
                {
                    fileHotkeys.RemoveAt(i);
                    i--;
                    continue;
                }
                Texture2D thumb = GetSmallPreview(data.filePath);

                if (thumb != null)
                {
                    GUILayout.Label(thumb, GUILayout.Width(60), GUILayout.Height(60));
                }
                else
                {
                    GUILayout.Box(data.isVideo ? "VIDEO" : "IMAGE", GUILayout.Width(60), GUILayout.Height(60));
                }

                EditorGUILayout.EndHorizontal();
            }

            // Khi user nhấn phím để đổi key
            if (editingFileHotkeyIndex >= 0 && Event.current.type == EventType.KeyDown)
            {
                fileHotkeys[editingFileHotkeyIndex].hotkey = Event.current.keyCode;
                editingFileHotkeyIndex = -1;
                Event.current.Use();
                Repaint();
            }

        }
        private void HandleFileDragAndDrop()
        {
            Event evt = Event.current;

            Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Drag Image/Video Here", EditorStyles.helpBox);

            if (!dropArea.Contains(evt.mousePosition))
                return;

            if (evt.type == EventType.DragUpdated)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            }
            else if (evt.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                foreach (var path in DragAndDrop.paths)
                {
                    string ext = Path.GetExtension(path).ToLower();

                    bool isImage =
                        ext == ".png" ||
                        ext == ".jpg" ||
                        ext == ".jpeg" ||
                        ext == ".tga";

                    bool isVideo =
                        ext == ".mp4" ||
                        ext == ".mov" ||
                        ext == ".avi" ||
                        ext == ".mkv";

                    if (!isImage && !isVideo)
                    {
                        Debug.LogWarning("Unsupported file: " + path);
                        continue;
                    }

                    fileHotkeys.Add(new FileHotkeyData()
                    {
                        filePath = path,
                        isVideo = isVideo,
                        hotkey = KeyCode.None
                    });

                    Debug.Log("Added file hotkey: " + path);
                }

                evt.Use();
            }
        }
        private Texture2D GetSmallPreview(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset == null)
                return null;

            Texture2D preview = AssetPreview.GetMiniThumbnail(asset);
            if (preview == null)
                preview = AssetPreview.GetAssetPreview(asset);

            if (preview == null)
                Repaint();   // yêu cầu Unity render lại thumbnail

            return preview;
        }



    }


}
