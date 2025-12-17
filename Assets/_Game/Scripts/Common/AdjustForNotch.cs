using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class AutoSafeTitle : MonoBehaviour
{
    private RectTransform rectTransform;
    private bool applied = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplyAdjust();
    }

#if UNITY_EDITOR
    void Update()
    {
        // Cho phép xem thay đổi khi scale Game View trong Editor
       // ApplyAdjust();
    }
#endif

    void ApplyAdjust()
    {
        if (applied && !Application.isEditor)
            return;

        var safeArea = Screen.safeArea;
        var screenHeight = Screen.height;
        var topNotch = screenHeight - (safeArea.y + safeArea.height);

        // Không có tai thỏ thì thôi
        if (topNotch <= 0)
            return;

        applied = true;

        // 🔹 Tính tỷ lệ notch (so với màn hình)
        float notchRatio = topNotch / screenHeight; // vd: 0.05 cho tai thỏ 5% màn hình

        // 🔹 Giảm kích thước tỉ lệ theo độ notch (vừa đủ, không nhập tay)
        // tai thỏ càng cao -> scale càng nhỏ, nhưng không nhỏ hơn 0.9
        float scaleFactor = Mathf.Clamp(1f - notchRatio * 1.5f, 0.9f, 1f);
        rectTransform.localScale = Vector3.one * scaleFactor;

        // 🔹 Tính chiều cao title sau khi scale
        float height = rectTransform.rect.height * scaleFactor;

        // 🔹 Dời xuống đúng bằng phần tai thỏ + thêm 1/4 chiều cao để không đụng UI dưới
        float moveDown = topNotch + height * 0.25f;
        rectTransform.anchoredPosition -= new Vector2(0, moveDown);
    }
}
