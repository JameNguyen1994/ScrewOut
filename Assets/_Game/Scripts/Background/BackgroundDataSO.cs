using UnityEngine;

[CreateAssetMenu(fileName = "BackgroundDataSO", menuName = "Scriptable Objects/BackgroundDataSO")]
public class BackgroundDataSO : ScriptableObject
{
    public string defaultBackgroundName;
    public BackgroundData[] backgrounds;
    public string GetCurrentBackground(int currentLevel)
    {
        BackgroundData selectedBackground = null;
        foreach (var background in backgrounds)
        {
            if (currentLevel >= background.startLevel)
            {
                selectedBackground = background;
            }
            else
            {
                break;
            }
        }
        return selectedBackground != null ? selectedBackground.backgroundSpriteName : defaultBackgroundName;
    }
}

[System.Serializable]
public class BackgroundData
{
    public string backgroundSpriteName;
    public int startLevel;
}