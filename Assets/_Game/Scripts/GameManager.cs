using EasyButtons;
using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{


    [SerializeField] private Screw screwSelected;
    [SerializeField] private Shape shapeSelected;
    [SerializeField] private GameState gameState;
    [SerializeField] private AbsControlSelectScrew selectScrew;
    [SerializeField] private AwaitInputClick awaitInputClick;
    public GameState GameState { get => gameState; }
    public AwaitInputClick AwaitInputClick { get => awaitInputClick;}

    private void Start()
    {
      
        selectScrew = new ClickUpSelecter();
        InputHandler.Instance.actionClickUpObject += selectScrew.OnClickUp;
        InputHandler.Instance.actionClickDown += OnSelect;
        InputHandler.Instance.actionClickDown += selectScrew.OnClickDown;
        Input.multiTouchEnabled = true;
        InputHandler.Instance.actionClickUpFree += UnSelect;



    }

    public void OnSelect(RaycastHit hit)
    {
        Debug.Log("Test input On Select");
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<Box>() != null)
            {
                var box = hit.collider.GetComponent<Box>();

                box.OnClickBox();
            }
          //  Debug.Log("Test Select Shape 0");

           // if (IngameData.MODE_CONTROL == ModeControl.ControlV2) return;

            SelectShape(hit);


        }
    }

    public void SelectShape(RaycastHit hit)
    {
        Debug.Log("Test input Select Shape");
        // Debug.Log("Test Select Shape 1");
        if (hit.collider.GetComponent<Shape>() != null)
        {
            shapeSelected = hit.collider.GetComponent<Shape>();
            //Debug.Log("Test Select Shape 2");

            shapeSelected.OnSelectShape();
        }

    }

    public void UnSelect()
    {
        if (shapeSelected != null)
        {
            shapeSelected.OnCancelSelectShape();
            shapeSelected = null;
        }
    }

    public void ChangeGameState(GameState gameState)
    {
        this.gameState = gameState;
        
    }

    private void OnDisable()
    {
        InputHandler.Instance.actionClickDown -= OnSelect;
        InputHandler.Instance.actionClickUpFree -= UnSelect;
    }
    [Button]
    public void TurnOffAllScrew()
    {
        LevelController.Instance.Level.TurnOffAllScrew();
    }
}
public enum GameState
{
    Play = 0,
    Stop = 1,
}

public enum ModeControl
{
    ControlV1 = 0,
    ControlV2 = 1,
}