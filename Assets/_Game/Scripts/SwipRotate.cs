using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    public float rotationSpeed = 0.2f; // Tốc độ xoay của object

    private Vector2 startInputPosition;   // Vị trí bắt đầu của swipe hoặc click chuột
    private Vector2 currentInputPosition; // Vị trí hiện tại của swipe hoặc chuột
    private Vector2 inputDelta;           // Khoảng cách swipe hoặc chuột

    private bool isSwiping = false;

    public bool unlockSwip = false;
    void Update()
    {
       /* if (GameManager.Instance.GameState == GameState.Stop) return;
        if (BoosterController.Instance.IsShowTutorial || PopupController.Instance.PopupCount > 0) return;
        if (!BoosterController.Instance.UsingHammer && BoosterController.Instance.CurrentAnimationBooster != BoosterType.None) 
                {
            Debug.Log("None");
            return;
        }
                ;
        if (!IngameData.UNLOCK_SWIP) return;
        if (Input.GetMouseButtonDown(0))
        {
            startInputPosition = Input.mousePosition;
            isSwiping = true;
        }

        if (Input.GetMouseButton(0) && isSwiping)
        {
            currentInputPosition = Input.mousePosition;
            inputDelta = (currentInputPosition - startInputPosition);

            RotateObject(inputDelta.x);
            if (inputDelta.x > 0.1f || inputDelta.x < -0.1f)
            {
                //LevelMap.Instance.AddForceToShape(inputDelta.x);
                ScreenGamePlayUI.Instance.OnEnableTutorialSwip(false);
                IngameData.DONE_TUTORIAL = true;

            }
            startInputPosition = currentInputPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;
        }*/

    }

    void RotateObject(float horizontalDelta)
    {
        float rotationY = -horizontalDelta * rotationSpeed;
        transform.Rotate(Vector3.up, rotationY, Space.World);
    }
}
