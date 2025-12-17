using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BoosterDataSO", fileName = "BoosterDataSO")]
public class BoosterDataSO : ScriptableObject
{
    public List<BoosterData> data;
}
[System.Serializable]
public class BoosterData
{
    public BoosterType boosterType;
    public Sprite sprBooster;
    public Sprite sprTitle;
    public string title;
    public string content;
    public int levelUnlock;
}
public enum BoosterType
{
    AddHole = 0,
    Hammer = 1,
    Clears = 2,
    Magnet = 3,
    UnlockBox = 4,
    None = 5,
    FreeRevive = 6,
}
