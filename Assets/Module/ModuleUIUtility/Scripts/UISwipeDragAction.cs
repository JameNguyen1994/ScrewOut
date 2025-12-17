using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[System.Serializable]
public class PointerEventDataEvent : UnityEvent<PointerEventData> { }

public class UISwipeDragAction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Swipe Settings")]
    public float dragLimit = 200f;
    public float swipeThreshold = 60f;
    public float returnSpeed = 10f;

    [Header("Swipe Events")]
    public UnityEvent OnSwipeLeft;
    public UnityEvent OnSwipeRight;

    [Header("Drag Callback Events")]
    public PointerEventDataEvent OnDragging;
    public PointerEventDataEvent OnBeginDragging;
    public PointerEventDataEvent OnEndDragging;

    public RectTransform rectTransform;

    private Vector2 startPos;
    private bool isDragging;

    private void Awake()
    {
        startPos = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        OnBeginDragging?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        DragHandler(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        OnEndDragging?.Invoke(eventData);

        float moved = rectTransform.anchoredPosition.x - startPos.x;

        if (Mathf.Abs(moved) >= swipeThreshold)
        {
            if (moved > 0)
            {
                EditorLogger.Log("➡ Swipe Right Action");
                OnSwipeRight?.Invoke();
            }
            else
            {
                EditorLogger.Log("⬅ Swipe Left Action");
                OnSwipeLeft?.Invoke();
            }

            return;
        }

        StopAllCoroutines();
        StartCoroutine(ReturnToStart());
    }

    private System.Collections.IEnumerator ReturnToStart()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, startPos) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, startPos, Time.deltaTime * returnSpeed);
            yield return null;
        }
        rectTransform.anchoredPosition = startPos;
    }

    public void DragHandler(PointerEventData eventData)
    {
        float deltaX = eventData.delta.x;
        Vector2 newPos = rectTransform.anchoredPosition + new Vector2(deltaX, 0);
        newPos.x = Mathf.Clamp(newPos.x, startPos.x - dragLimit, startPos.x + dragLimit);
        rectTransform.anchoredPosition = newPos;

        // 🔥 Gọi ra ngoài (Inspector hoặc script khác)
        OnDragging?.Invoke(eventData);

        float moved = rectTransform.anchoredPosition.x - startPos.x;

        if (Mathf.Abs(moved) >= swipeThreshold)
        {
            if (moved > 0)
            {
                EditorLogger.Log("➡ Swipe Right Action");
                OnSwipeRight?.Invoke();
            }
            else
            {
                EditorLogger.Log("⬅ Swipe Left Action");
                OnSwipeLeft?.Invoke();
            }

            isDragging = false;
            return;
        }
    }
}