using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VideoEditorHelper
{
    [System.Serializable]
    public class SaveWrapper
    {
        public List<ButtonData> buttonDatas;
        public List<FileHotkeyData> fileHotkeys;
    }
    public class ActiveButtonHelper : MonoBehaviour
    {
        public static ActiveButtonHelper Instance { get; private set; }

        [SerializeField] private List<ButtonData> buttonDatas = new();
        [SerializeField] private List<FileHotkeyData> fileHotkeys = new();

        private string pathWrapperJson;


        // ============================================================
        //  INIT
        // ============================================================
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            pathWrapperJson = Path.Combine(
                Application.dataPath,
                "Tools/VideoEditorHelper/Datas/FastButtonData.json"
            );

            LoadAllData();
        }


        // ============================================================
        //  UPDATE
        // ============================================================
        private void Update()
        {
            if (Input.anyKeyDown)
            {
                Debug.Log($"[Shortcut] Key pressed: {Input.anyKeyDown}");
            }
            HandleScriptHotkeys();
            HandleFileHotkeys();
        }


        // ============================================================
        //  LOAD JSON (SaveWrapper)
        // ============================================================
        private void LoadAllData()
        {
            if (!File.Exists(pathWrapperJson))
            {
                Debug.LogWarning("[ActiveButtonHelper] JSON not found: " + pathWrapperJson);
                return;
            }

            string json = File.ReadAllText(pathWrapperJson);
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);

            if (wrapper == null)
            {
                Debug.LogError("[ActiveButtonHelper] JSON parse failed!");
                return;
            }

            if (wrapper.buttonDatas != null)
                buttonDatas = wrapper.buttonDatas;

            if (wrapper.fileHotkeys != null)
                fileHotkeys = wrapper.fileHotkeys;

            Debug.Log($"[ActiveButtonHelper] Loaded {buttonDatas.Count} script hotkeys, {fileHotkeys.Count} file hotkeys.");
        }


        // ============================================================
        //  OLD HOTKEY FEATURE (SCRIPT METHOD)
        // ============================================================
        private void HandleScriptHotkeys()
        {
            //Debug.Log($"[ActiveButtonHelper] Checking script hotkeys... {buttonDatas.Count}");
            foreach (var data in buttonDatas)
            {

                if (data.shortcutKey != KeyCode.None &&
                    Input.GetKeyDown(data.shortcutKey))
                {
                    Debug.Log($"[Shortcut] Key pressed: {data.shortcutKey}");
                    GameObject targetGO = FindTargetObject(data);
                    if (targetGO != null)
                    {
                        InvokeMethod(targetGO, data.scriptName, data.methodName);
                        Debug.Log($"[Shortcut] {data.shortcutKey} → {data.scriptName}.{data.methodName}()");
                    }
                }
            }
        }


        private GameObject FindTargetObject(ButtonData data)
        {
            // first try by name (direct)
            if (!string.IsNullOrEmpty(data.gameObjectName))
            {
                GameObject go = GameObject.Find(data.gameObjectName);
                if (go != null && FindScriptOnObject(go, data.scriptName) != null)
                    return go;
            }

            // fallback: find any object with script
            return FindGameObjectWithScript(data.scriptName);
        }

        private MonoBehaviour FindScriptOnObject(GameObject obj, string scriptName)
        {
            MonoBehaviour[] list = obj.GetComponents<MonoBehaviour>();
            foreach (var mb in list)
            {
                if (mb != null && mb.GetType().Name == scriptName)
                    return mb;
            }
            return null;
        }

        private GameObject FindGameObjectWithScript(string scriptName)
        {
            MonoBehaviour[] all = GameObject.FindObjectsOfType<MonoBehaviour>();
            foreach (var mb in all)
            {
                if (mb != null && mb.GetType().Name == scriptName)
                    return mb.gameObject;
            }
            return null;
        }


        public void InvokeMethod(GameObject target, string scriptName, string methodName)
        {
            MonoBehaviour script = FindScriptOnObject(target, scriptName);
            if (script == null)
            {
                Debug.LogWarning($"Script '{scriptName}' not found on {target.name}");
                return;
            }

            MethodInfo mInfo = script.GetType().GetMethod(
                methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            if (mInfo == null)
            {
                Debug.LogWarning($"Method '{methodName}' not found in '{scriptName}'.");
                return;
            }

            if (mInfo.GetParameters().Length > 0)
            {
                Debug.LogWarning($"{methodName} must have no parameters.");
                return;
            }

            mInfo.Invoke(script, null);
        }


        // ============================================================
        //  NEW FEATURE (IMAGE / VIDEO HOTKEY)
        // ============================================================
        private void HandleFileHotkeys()
        {
            foreach (var f in fileHotkeys)
            {
                if (f.hotkey != KeyCode.None && Input.GetKeyDown(f.hotkey))
                {
                    Debug.Log($"[FileHotkey] {f.hotkey} → Open file: {f.filePath}");
                    if (f.isVideo)
                        OpenVideo(f.filePath);
                    else
                        OpenImage(f.filePath);
                }
            }
        }


        public void OpenImage(string assetPath)
        {
#if UNITY_EDITOR
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (tex == null)
            {
                Debug.LogWarning("[FileHotkey] Image NOT found: " + assetPath);
                return;
            }

            Debug.Log("[FileHotkey] Show Image: " + assetPath);

           // ImagePreviewWindow.Show(tex);
            PreviewInScene.Instance.ShowImage(tex);
#endif
        }


        public void OpenVideo(string assetPath)
        {
#if UNITY_EDITOR
            VideoClip clip = AssetDatabase.LoadAssetAtPath<VideoClip>(assetPath);

            if (clip == null)
            {
                Debug.LogWarning("[FileHotkey] Video NOT found: " + assetPath);
                return;
            }

            Debug.Log("[FileHotkey] Play Video: " + assetPath);

            PreviewInScene.Instance.ShowVideo(clip);

           // VideoPreviewWindow.Show(clip);
#endif
        }
    }


    // ============================================================
    //  OLD DATA (KEEP ORIGINAL)
    // ============================================================
    [Serializable]
    public class ButtonData
    {
        public string scriptName;
        public string gameObjectName;
        public string methodName;
        public KeyCode shortcutKey;

        public ButtonData(string go, string script, string method)
        {
            scriptName = script;
            gameObjectName = go;
            methodName = method;
            shortcutKey = KeyCode.None;
        }

        public ButtonData() { }
    }


    // ============================================================
    //  NEW FILE HOTKEY DATA
    // ============================================================
    [Serializable]
    public class FileHotkeyData
    {
        public string filePath;
        public bool isVideo;
        public KeyCode hotkey;
    }


#if UNITY_EDITOR

    // ============================================================
    // IMAGE PREVIEW WINDOW
    // ============================================================
    public class ImagePreviewWindow : EditorWindow
    {
        private Texture2D tex;

        public static void Show(Texture2D t)
        {
            var w = CreateInstance<ImagePreviewWindow>();
            w.tex = t;
            w.titleContent = new GUIContent("Image Preview");
            w.Show();
        }

        private void OnGUI()
        {
            if (tex == null)
            {
                GUILayout.Label("Texture is null");
                return;
            }

            Rect r = GUILayoutUtility.GetRect(position.width, position.height);
            GUI.DrawTexture(r, tex);
        }
    }


    // ============================================================
    // VIDEO PREVIEW WINDOW
    // ============================================================
    public class VideoPreviewWindow : EditorWindow
    {
        private VideoClip clip;
        private Texture2D preview;

        public static void Show(VideoClip c)
        {
            var w = CreateInstance<VideoPreviewWindow>();
            w.clip = c;
            w.titleContent = new GUIContent("Video Preview");
            w.Show();
        }

        private void OnGUI()
        {
            if (clip == null)
            {
                GUILayout.Label("Video is null");
                return;
            }

            preview = AssetPreview.GetAssetPreview(clip);

            GUILayout.Label("Video: " + clip.name);

            Rect r = GUILayoutUtility.GetRect(position.width, position.height - 30);

            if (preview != null)
                GUI.DrawTexture(r, preview);
            else
            {
                GUILayout.Label("Generating preview...");
                Repaint();
            }
        }
    }

#endif

}
