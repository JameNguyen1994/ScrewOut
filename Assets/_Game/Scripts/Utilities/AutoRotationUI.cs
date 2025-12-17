using System;
using UnityEngine;

public class AutoRotationUI : MonoBehaviour
{
    [SerializeField] private float speed;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime));
    }
}
