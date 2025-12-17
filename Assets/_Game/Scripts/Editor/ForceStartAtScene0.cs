#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class ForceStartAtScene0
{
    private const string TempSceneKey = "LastOpenedScene"; // KeyUnlockBox để lưu Scene hiện tại

    static ForceStartAtScene0()
    {
        // Đăng ký sự kiện trước khi Play
        //EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Lưu Scene hiện tại trước khi chuyển sang Play Mode
            EditorPrefs.SetString(TempSceneKey, EditorSceneManager.GetActiveScene().path);

            // Mở Scene đầu tiên (Scene 0 trong Build Settings)
            string firstScenePath = UnityEditor.EditorBuildSettings.scenes[0].path;

            if (EditorSceneManager.GetActiveScene().path != firstScenePath)
            {
                EditorSceneManager.OpenScene(firstScenePath);
            }
        }
        else if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // Scene 0 đang chạy, không cần làm gì thêm
        }
        else if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Quay lại Scene trước khi Play
            string lastScenePath = EditorPrefs.GetString(TempSceneKey, string.Empty);
            if (!string.IsNullOrEmpty(lastScenePath) && lastScenePath != EditorSceneManager.GetActiveScene().path)
            {
                EditorSceneManager.OpenScene(lastScenePath);
            }
        }
    }
}
#endif