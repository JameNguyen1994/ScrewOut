using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosBox : MonoBehaviour
{
    [SerializeField] private Box box;

    public void Init(Box box)
    {
        this.box = box;
        this.box.transform.SetParent(transform);
    }

  
}
