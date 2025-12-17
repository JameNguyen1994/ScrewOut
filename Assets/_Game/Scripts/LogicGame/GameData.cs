using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class GameData
{
    public string Id;
}

[Serializable]
public class ScrewData : GameData
{
    public ScrewColor Color;
    public ScrewState State;
}

[Serializable]
public class BoxData : GameData
{
    public List<string> Screws;

    public ScrewColor Color;
    public BoxState State;

    public BoxData(string id)
    {
        Id = id;
        Screws = new List<string>();
    }
}

[Serializable]
public class BaseTrayData : GameData
{
    public List<TrayData> Trays;

    public BaseTrayData(string id)
    {
        Id = id;
        Trays = new List<TrayData>();
    }
}

[Serializable]
public class TrayData : GameData
{
    public string ScrewId;
}

[Serializable]
public class LevelMapData : GameData
{
    public List<ScrewColor> Origins;
    public List<ScrewColor> Progress;
    public List<string> RemovedScrews;
    public List<string> FilledScrews;
    public ScrewDataList screwDataList;
    public ShapeDataList shapeDataList;
    public LinkObstacleDataList linkObstacleDataList;
    public List<string> Wrenchs;
    public List<string> WrenchCollecteds;

    public LevelMapData(string id, List<ScrewColor> origins, List<ScrewColor> progress, List<string> removedScrews, List<string> filledScrews, ScrewDataList screwDataList, ShapeDataList shapeDataList, LinkObstacleDataList linkObstacleDataList, List<string> wrenchs, List<string> wrenchCollecteds)
    {
        Id = id;
        Origins = origins != null ? new List<ScrewColor>(origins) : new List<ScrewColor>();
        Progress = progress != null ? new List<ScrewColor>(progress) : new List<ScrewColor>();

        RemovedScrews = removedScrews != null ? removedScrews : new List<string>();
        FilledScrews = filledScrews != null ? filledScrews : new List<string>();

        this.screwDataList = screwDataList != null ? screwDataList : new ScrewDataList();
        this.shapeDataList = shapeDataList != null ? shapeDataList : new ShapeDataList();
        this.linkObstacleDataList = linkObstacleDataList != null ? linkObstacleDataList : new LinkObstacleDataList();

        Wrenchs = wrenchs != null ? wrenchs : new List<string>();
        WrenchCollecteds = wrenchCollecteds != null ? wrenchCollecteds : new List<string>();
    }
}


[Serializable]
public class ScrewDataList
{
    public List<ScrewDataBlock> lstScrewDataBlock = new List<ScrewDataBlock>();
    public ScrewDataList()
    {
        lstScrewDataBlock = new List<ScrewDataBlock>();
    }
}
[Serializable]
public class ShapeDataList
{
    public List<ShapeDataBlock> lstShapeDataBlock = new List<ShapeDataBlock>();
    public ShapeDataList()
    {
        lstShapeDataBlock = new List<ShapeDataBlock>();
    }
}
[Serializable]
public class LinkObstacleDataList
{
    public List<LinkObstacleDataBlock> lstLinkObstacleDataBlock = new List<LinkObstacleDataBlock>();
    public LinkObstacleDataList()
    {
        lstLinkObstacleDataBlock = new List<LinkObstacleDataBlock>();
    }
}




[Serializable]
public class ScrewDataBlock
{
    public string id;
    public List<string> lstShapeBlocked = new List<string>();
    public List<string> lstLinkBlocked = new List<string>();
    public List<string> lstShapeCover = new List<string>();
    public List<string> lstLinkCover = new List<string>();
}
[Serializable]
public class ShapeDataBlock
{
    public string id;
    public List<string> lstScrewBlock = new List<string>();
    public List<string> lstScrewCover = new List<string>();
}
[Serializable]
public class LinkObstacleDataBlock
{
    public string id;
    public List<string> lstScrewBlock = new List<string>();
    public List<string> lstScrewCover = new List<string>();
}