using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneChange
{
    [MenuItem("Scene/Loading")]
    static void Loading()   =>      LoadScene("Assets/_Game/Scenes/Loading.unity");

    [MenuItem("Scene/Game Play")]
    static void GamePlay()  =>      LoadScene("Assets/_Game/Scenes/GamePlayNewControl.unity");

    [MenuItem("Scene/Main Menu")]
    static void MainMenu()  =>      LoadScene("Assets/_Game/Scenes/MainMenu.unity");

    [MenuItem("Scene/Level Design")]
    static void LevelDesign() =>    LoadScene("Assets/_Game/Scenes/LevelDesign.unity");

    static void LoadScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
    }
}