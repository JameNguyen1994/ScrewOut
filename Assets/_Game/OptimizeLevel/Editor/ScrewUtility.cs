using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class ScrewUtility
{
    public static void PingAll(List<Screw> lstTotalScrewAvailable)
    {
        List<Object> objs = new List<Object>();
        foreach (var screw in lstTotalScrewAvailable)
        {
            if (screw != null && screw.gameObject != null)
            {
                objs.Add(screw.gameObject);
            }
        }

        if (objs.Count > 0)
        {
            Selection.objects = objs.ToArray();                 // chọn tất cả
            EditorGUIUtility.PingObject(objs[0]);               // ping 1 cái để focus
            Debug.Log($"Pinged {objs.Count} screws");
        }
        else
        {
            Debug.Log("No screws to ping");
        }
    }
}
