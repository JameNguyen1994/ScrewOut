using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CenterOfMassCalculator : MonoBehaviour
{
    [SerializeField] private Transform parent;

    [ContextMenu("Do G Position")]
    void CmDoCenterOfMass()
    {
        List<Transform> children = parent.GetComponentsInChildren<Transform>(false).ToList();

        List<Transform> cubes = new List<Transform>();

        Vector3 totalPosition = Vector3.zero;

        int length = 0;
        
        for (int i = 1; i < children.Count; i++)
        {
            if (children[i].name.IndexOf("cube", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                cubes.Add(children[i]);
                totalPosition += children[i].localPosition;
                length++;
            }
        }

        Vector3 gPosition = totalPosition / length;

        for (int i = 0; i < cubes.Count; i++)
        {
            cubes[i].localPosition -= gPosition;
        }
    }
}
