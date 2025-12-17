using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BoxDifficultyData
{
    public string name = "Name";
    public int easePercent = 0;
    public int normalPercent = 0;
    public int hardPercent = 0;
}

[System.Serializable]
public class LevelDifficultyData
{
    public int level;
    public List<ProcessData> lstProcessData = new List<ProcessData>();
}

[System.Serializable]
public class ProcessData
{
    public int process;
    public BoxDifficultyData boxDifficulty = new BoxDifficultyData();
}

