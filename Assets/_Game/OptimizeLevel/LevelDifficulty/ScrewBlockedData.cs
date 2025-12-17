using System.Collections.Generic;

[System.Serializable]
public class ScrewBlockedData
{
    public int index;
    public List<int> lstIndexShapeBlock;
    public List<int> lstIndexShapeCover;
    public List<int> lstIndexObstacle;
    public List<int> lstIndexObstacleCover;
    public ScrewBlockedData()
    {
        index = -1;
        lstIndexShapeBlock = new List<int>();
        lstIndexShapeCover = new List<int>();
        lstIndexObstacleCover = new List<int>();
        lstIndexObstacle = new List<int>();
    }
}
[System.Serializable]
public class LevelScrewBlockedData
{
    public int level;
    public int totalScrew;
    public List<ScrewBlockedData> lstScrewBlockedData = new List<ScrewBlockedData>();
}
