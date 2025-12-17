using PS.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataScrewColor : Singleton<DataScrewColor>
{
    [SerializeField] private List<DataColor> data = new List<DataColor>();

    public Material GetMaterialByColor(ScrewColor screwColor)
    {
        var mat = data.Find(x => x.color == screwColor).material;
        return mat;
    }

    public Material GetMaterialScrewByColor(ScrewColor screwColor)
    {
        var mat = data.Find(x => x.color == screwColor).materialScrew;
        return mat;
    }

    public Mesh GetBoxMeshByColor(ScrewColor screwColor)
    {
        var mesh = data.Find(x => x.color == screwColor).boxMesh;
        return mesh;
    } 
    
    public Mesh GetLidMeshByColor(ScrewColor screwColor)
    {
        var mesh = data.Find(x => x.color == screwColor).lidMesh;
        return mesh;
    }

}

[Serializable]
public class DataColor
{
    public ScrewColor color;
    public Material material;
    public Material materialScrew;
    public Mesh boxMesh;
    public Mesh lidMesh;
}