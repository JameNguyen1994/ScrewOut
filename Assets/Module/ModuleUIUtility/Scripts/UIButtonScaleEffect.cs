using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

// Adds press-scale feedback to a UI button (or any selectable UI object)
public class UIButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    public float pressedScale = 0.9f;     // Scale when pressed
    public float tweenDuration = 0.1f;    // Duration of scale animation
    public Ease ease = Ease.OutBack;      // Easing curve for smooth feel
    public Transform target;      // Easing curve for smooth feel

    private Vector3 originalScale;
    private bool isPressed;

    private void Awake()
    {
        if (originalScale == Vector3.zero)
        {
            originalScale = target.localScale;
        }
    }

    // Called when the user presses down on the button
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        target.DOScale(originalScale * pressedScale, tweenDuration).SetEase(ease);
    }

    // Called when the user releases the button
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;
        target.DOScale(originalScale, tweenDuration).SetEase(ease);
    }
}