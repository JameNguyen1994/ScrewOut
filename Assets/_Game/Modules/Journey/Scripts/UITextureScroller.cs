using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UITextureScrollMatchPixels : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private ScrollRect scrollRect;     // Bắt buộc
    [SerializeField] private bool matchHorizontal = true;
    [SerializeField] private bool matchVertical = true;

    [Header("Fine Tune")]
    [Tooltip("Hệ số nhân cho tốc độ cuộn theo pixel -> UV (1 = khớp 1:1 theo tile)")]
    [SerializeField] private Vector2 multiplier = Vector2.one;
    [Tooltip("Đảo chiều cuộn nếu cần")]
    [SerializeField] private Vector2 invert = Vector2.zero; // (1,0) đảo X; (0,1) đảo Y; (1,1) đảo cả 2

    private Image img;
    private RectTransform imgRT;

    private Material runtimeMat;
    private static readonly int OffsetID = Shader.PropertyToID("_Offset");
    private static readonly int TilingID = Shader.PropertyToID("_Tiling");

    private void Awake()
    {
        img = GetComponent<Image>();
        imgRT = (RectTransform)transform;

        // clone material
        runtimeMat = new Material(img.material);
        img.material = runtimeMat;

        if (scrollRect != null)
            scrollRect.onValueChanged.AddListener(OnScrollChanged);

        // Cập nhật 1 lần khi bật
        ApplyFromScrollRect();
    }

    private void OnEnable()
    {
        ApplyFromScrollRect();
    }

    public void OnScrollChanged(Vector2 _)
    {
        ApplyFromScrollRect();
    }

    private void ApplyFromScrollRect()
    {
        if (scrollRect == null) return;

        // Kích thước
        var content = scrollRect.content;
        if (content == null) return;

        var viewport = scrollRect.viewport != null ? scrollRect.viewport : (RectTransform)scrollRect.transform;

        float contentW = content.rect.width;
        float contentH = content.rect.height;
        float viewportW = viewport.rect.width;
        float viewportH = viewport.rect.height;

        // Phạm vi có thể cuộn (pixel)
        float scrollableX = Mathf.Max(0f, contentW - viewportW);
        float scrollableY = Mathf.Max(0f, contentH - viewportH);

        // Normalized (0..1)
        float normX = scrollRect.horizontalNormalizedPosition;         // 0 = trái, 1 = phải
        float normYTopToBottom = scrollRect.verticalNormalizedPosition; // 0 = dưới, 1 = trên

        // Pixel đã cuộn
        float scrolledPixelsX = matchHorizontal ? (normX * scrollableX) : 0f;

        // Với Y của ScrollRect: 1 = top (content ở đầu), 0 = bottom
        // Nhưng cảm nhận thường là cuộn xuống -> pixel dương. Nên dùng: (1 - normY) * scrollableY
        float scrolledPixelsY = matchVertical ? ((1f - normYTopToBottom) * scrollableY) : 0f;

        // Đổi Pixel -> UV theo số tile trên Image
        Vector4 tiling = runtimeMat.GetVector(TilingID);
        float imgW = Mathf.Max(1f, imgRT.rect.width);
        float imgH = Mathf.Max(1f, imgRT.rect.height);

        // UV tăng 1.0 ứng với 1 ô tile; nếu _Tiling.x = 4, nghĩa là 4 tiles trong bề ngang Image
        // Vậy UV/pixel = _Tiling.x / imgWidth
        float uvPerPixelX = tiling.x / imgW;
        float uvPerPixelY = tiling.y / imgH;

        float offX = scrolledPixelsX * uvPerPixelX * (invert.x != 0 ? -1f : 1f) * multiplier.x;
        float offY = scrolledPixelsY * uvPerPixelY * (invert.y != 0 ? -1f : 1f) * multiplier.y;

        // Gộp với offset hiện tại (để hỗ trợ set offset ban đầu nếu có)
        Vector4 cur = runtimeMat.GetVector(OffsetID);
        // Ta không cộng dồn vô hạn; mà set theo vị trí hiện tại để "khớp"
        cur.x = offX;
        cur.y = offY;
        runtimeMat.SetVector(OffsetID, cur);
    }

    // Call nếu muốn cập nhật thủ công (ví dụ khi đổi tiling hoặc thay đổi layout runtime)
    public void ForceRefresh() => ApplyFromScrollRect();
}
