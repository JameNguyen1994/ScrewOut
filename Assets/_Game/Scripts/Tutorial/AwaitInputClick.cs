using EasyButtons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwaitInputClick : MonoBehaviour
{
    [SerializeField] private WaitUntilStep stepWaiter;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private RectTransform imgHand;
    [SerializeField] private RectTransform rectUICanvas;
    [SerializeField] private Camera cam;

    [SerializeField] private Transform tfmZoom;
    private void Start()
    {
       // InputHandler.Instance.actionClickUpObject += OnScrewClicked;
       // InputHandler.Instance.actionClickDown +=
        Get3DScrew();
    }
    [Button]
    public void Get3DScrew()
    {
        var screw = LevelController.Instance.Level.LstScrew[0];
        // 1. World → Screen point (bằng camera 3D)
        Vector3 screenPos = cam.WorldToScreenPoint(screw.transform.position);


        // 2. Screen point → Local point trong imgSafeArea (Canvas Overlay -> camera = null)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectUICanvas,
            screenPos,
            null, // 🔥 Vì canvas overlay
            out Vector2 localPos
        );
        imgHand.anchoredPosition = localPos;
    }
   public  void OnScrewClicked(RaycastHit hit)
    {
        //print(LayerMask.LayerToName(hit.collider.gameObject.layer));
        if (hit.collider.GetComponent<Screw>() == null)
        {
            return;
        }

        Invoke(nameof(WaitToCompleted), 1);
    }

    void WaitToCompleted()
    {
        stepWaiter.isContinue = true;
      //  InputHandler.Instance.actionClickUpObject -= OnScrewClicked;
    }
    public void GetScrewDown()
    {
        var screw = LevelController.Instance.Level.LstScrew[0];
        // 1. World → Screen point (bằng camera 3D)
        Vector3 screenPos = cam.WorldToScreenPoint(screw.transform.position);
    }
}
