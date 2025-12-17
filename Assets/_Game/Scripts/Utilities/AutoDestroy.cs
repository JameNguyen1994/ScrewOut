using System;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 1f;

    private void Start()
    {
        DODestroy();
    }
    
    void DODestroy()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
