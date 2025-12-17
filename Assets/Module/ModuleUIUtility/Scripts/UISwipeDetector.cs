using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UISwipeAndClickDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Swipe Settings")]
    public float swipeThreshold = 60f;

    [Header("Events")]
    public UnityEvent OnClick;
    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;
    public UnityEvent OnSwipeUp;
    public UnityEvent OnSwipeDown;

    private Vector2 startPos;

    public void OnPointerDown(PointerEventData eventData)
    {
        startPos = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 endPos = eventData.position;
        Vector2 diff = endPos - startPos;

        if (diff.magnitude < swipeThreshold)
        {
            EditorLogger.Log("🖱 Click detected");
            OnClick?.Invoke();
            return;
        }

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            if (diff.x > 0)
            {
                EditorLogger.Log("➡ Swipe Right");
                OnSwipeRight?.Invoke();
            }
            else
            {
                EditorLogger.Log("⬅ Swipe Left");
                OnSwipeLeft?.Invoke();
            }
        }
        else
        {
            if (diff.y > 0)
            {
                EditorLogger.Log("⬆ Swipe Up");
                OnSwipeUp?.Invoke();
            }
            else
            {
                EditorLogger.Log("⬇ Swipe Down");
                OnSwipeDown?.Invoke();
            }
        }
    }
}
