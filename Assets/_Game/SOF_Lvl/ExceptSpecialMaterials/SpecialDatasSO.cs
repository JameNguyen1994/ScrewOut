using EasyButtons;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SpecialDatasSO", menuName = "Scriptable Objects/SpecialDatasSO")]
public class SpecialDatasSO : ScriptableObject
{
#if UNITY_EDITOR
    public List<Material> lstSpecialMaterialData;
#endif
    public List<string> lstSpecialMaterialDataStr;

#if UNITY_EDITOR
    [Button]

    public void Convert()
    {
        lstSpecialMaterialDataStr = new List<string>();
        for (int i = 0; i < lstSpecialMaterialData.Count; i++)
        {
            lstSpecialMaterialDataStr.Add(lstSpecialMaterialData[i].name);
        }
    }
#endif
}
