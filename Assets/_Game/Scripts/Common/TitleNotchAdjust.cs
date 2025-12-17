using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TitleNotchAdjust : MonoBehaviour
{
    [Header("Bình thường (không tai thỏ)")]
    [SerializeField] private Vector2 normalPosition;
    [SerializeField] private Vector2 normalSize;

    [Header("Màn tai thỏ")]
    [SerializeField] private Vector2 notchPosition;
    [SerializeField] private Vector2 notchSize;

    private RectTransform rectTransform;
    private bool applied = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplyLayout();
    }

#if UNITY_EDITOR
    void Update()
    {
        // Cho phép test trực tiếp trong Editor khi thay đổi GameView
      //  ApplyLayout();
    }
#endif

    void ApplyLayout()
    {
        if (applied && !Application.isEditor) return;

        var safeArea = Screen.safeArea;
        var topNotch = Screen.height - (safeArea.y + safeArea.height);
        bool hasNotch = topNotch > 0;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (hasNotch)
        {
            rectTransform.anchoredPosition = notchPosition;
            rectTransform.sizeDelta = notchSize;
        }
        else
        {
            rectTransform.anchoredPosition = normalPosition;
            rectTransform.sizeDelta = normalSize;
        }

        applied = true;
    }
}
