using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ImageHighLightType
{
    Circle,
    Rectangle
}
[System.Serializable]
public class ImageHighLight
{
    public ImageHighLightType imageHighLightType;
    public Sprite sprite;


}
public static class ImageHighLightService
{
    public static Vector3 GetSize(ImageHighLightType imageHighLightType, Vector3 originSize)
    {
        Debug.Log($"GetSize: {imageHighLightType}, originSize: {originSize}");
        switch (imageHighLightType)
        {
            case ImageHighLightType.Circle:
                // lấy width và height
                float w = originSize.x;
                float h = originSize.y;

                // đường chéo hình chữ nhật
                float diameter = Mathf.Sqrt(w * w + h * h);
                Debug.Log($"GetSize Circle diameter: {diameter}");
                return new Vector2(diameter, diameter);
            case ImageHighLightType.Rectangle:
                Debug.Log($"GetSize Rectangle originSize: {originSize}");
                return originSize * 1.2f;
            default:
                return originSize;
        }

        return originSize;
    }
}

public class HoleEffect : MonoBehaviour
{
    public Image bigImage;
    public Image smallImage;

    [SerializeField] private Material holeMaterial;
    [SerializeField] private RectTransform bigImageRect;
    [SerializeField] private RectTransform smallImageRect;
    [SerializeField] private Canvas canvas;
    [SerializeField] private EffectFocusTarget effectFocusTarget;
    [SerializeField] private List<ImageHighLight> lstImageHighLight;

    private void Awake()
    {
        // Tạo một instance của Material để tránh thay đổi trực tiếp vào Material gốc
        holeMaterial = Instantiate(bigImage.material);
        bigImage.material = holeMaterial;

        bigImageRect = bigImage.rectTransform;
        smallImageRect = smallImage.rectTransform;
        canvas = bigImage.canvas;
        holeMaterial.SetTexture("_SmallTex", smallImage.sprite.texture);
    }

    void Update()
    {
        UpdateHole();
    }
    public void Init(Vector3 size, Vector3 position, ImageHighLightType imageHighLightType)
    {
        var smallSize = ImageHighLightService.GetSize(imageHighLightType, size);
        smallImageRect.sizeDelta = smallSize;
        smallImage.sprite = lstImageHighLight.Find(x => x.imageHighLightType == imageHighLightType).sprite;
        smallImage.transform.position = position;
        holeMaterial.SetTexture("_SmallTex", smallImage.sprite.texture);
        UpdateHole();
    }
    public void OnClickSmallImage()
    {
        Debug.Log("OnClickSmallImage");
        gameObject.SetActive(false);
    }
    public void OnClickBigImage()
    {
        Debug.Log("OnClickBigImage");
        //  effectFocusTarget.StartEffect();
    }
    private void UpdateHole()
    {
        // Xác định camera theo chế độ render của Canvas
        Camera cam = canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // Chuyển vị trí SmallImage sang không gian local của BigImage
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bigImageRect, smallImageRect.position, cam, out Vector2 localPoint
        );

        // Quy đổi toạ độ local sang UV (0 - 1)
        Vector2 uvPos = new Vector2(
            Mathf.InverseLerp(-bigImageRect.rect.width / 2, bigImageRect.rect.width / 2, localPoint.x),
            Mathf.InverseLerp(-bigImageRect.rect.height / 2, bigImageRect.rect.height / 2, localPoint.y)
        );

        // Tính kích thước SmallImage theo UV
        Vector2 uvSize = new Vector2(
            smallImageRect.rect.width / bigImageRect.rect.width,
            smallImageRect.rect.height / bigImageRect.rect.height
        );

        // Gửi dữ liệu cho shader
        holeMaterial.SetVector("_HoleCenter", uvPos);
        holeMaterial.SetVector("_HoleSize", uvSize);
    }




}
