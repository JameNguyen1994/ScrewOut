using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

/// ----------------------
/// Data classes
/// ----------------------
[System.Serializable]
public class ScrewTransformData
{
    // public string name;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[System.Serializable]
public class ShapeData
{
    //public string shapeName;
    public List<ScrewTransformData> screws = new List<ScrewTransformData>();
}
[System.Serializable]
public class LinkScrew
{
    public int indexShape;
    public int indexScrew;
}
[System.Serializable]
public class LinkData
{
    // public string shapeName;
    public List<ScrewTransformData> holes = new List<ScrewTransformData>();
    public List<LinkScrew> lstLinkScrew = new List<LinkScrew>();
}

[System.Serializable]
public class LevelDataJS
{
    public List<ShapeData> shapes = new List<ShapeData>();
    public List<LinkData> links = new List<LinkData>();
}

public class LevelScrewHelper : MonoBehaviour
{
    [SerializeField] private List<TextAsset> lstLevelData;
    [SerializeField] private Screw prbScrew;
    [SerializeField] private Rigidbody prbHole;


    public async UniTask SetLevelData(LevelMap levelMap)
    {
      /*  if (IngameData.IS_GEN_SCREW_BLOCK)
            return;*/
        int levelId = levelMap.LevelId - 1;
        // levelId = 0;
        LevelDataJS levelData = (LevelDataJS)JsonUtility.FromJson<LevelDataJS>(lstLevelData[levelId].text);
        levelMap.ResetScrews();
        for (int i = 0; i < levelData.shapes.Count; i++)
        {
            var shapeData = levelData.shapes[i];
            var shape = levelMap.LstShape[i];
            if (shape == null)
            {
                //  Debug.LogWarning($"Shape {shapeData.shapeName} not found in LevelMap");
                continue;
            }
            shape.Reset();
            foreach (var screwData in shapeData.screws)
            {
                Screw screw = Instantiate(prbScrew, screwData.position, screwData.rotation, shape.transform);
                //  screw.name = screwData.name;
                screw.transform.localScale = screwData.scale;

                Rigidbody hole = Instantiate(prbHole, screwData.position, screwData.rotation, shape.transform);
                hole.name = "Hole";
                hole.transform.localScale = screwData.scale;

                levelMap.AddScrew(screw);
                shape.AddScrewAndHole(screw, hole);
                screw.SetShape(shape);

                // await UniTask.Yield();
            }
        }

        var links = levelMap.LstLinkObstacle;

        for (int i = 0; i < levelData.links.Count; i++)
        {
            var link = links[i];
            var linkData = levelData.links[i];

            link.Reset();

            foreach (var holeData in linkData.holes)
            {
                Rigidbody hole = Instantiate(prbHole, holeData.position, holeData.rotation, link.transform);
                hole.name = "Hole";
                hole.transform.localScale = holeData.scale;
                link.AddScrewAndHole(null, hole);
                // await UniTask.Yield();
            }
            foreach (var linkScrew in linkData.lstLinkScrew)
            {
                var shape = levelMap.LstShape[linkScrew.indexShape];
                var screw = shape.LstScrew[linkScrew.indexScrew];
                screw.LstLinkObstacle.Add(link);
                link.LstScrew.Add(screw);
            }
        }
    }
}
