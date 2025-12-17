using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using PS.Analytic;
using ScrewCraze3D;
using Storage;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelMapSafeArea
{
    public LevelMapSize size;
    public Image safeArea;
}
public class LevelScaleHelper : MonoBehaviour
{
    [SerializeField] private Transform levelTransform;
    [SerializeField] private Transform tfmHolder;
    private Vector3 defaultScale = new Vector3(1, 1, 1);

    [SerializeField] private Canvas canvasUI;
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform prefabMarker;
    [SerializeField] private List<LevelMapSafeArea> lstSafeArea;



    public void SetLevelTransform(Transform levelTransform)
    {
        this.levelTransform = levelTransform;
        levelTransform.SetParent(tfmHolder);
        levelTransform.localPosition = Vector3.zero;
        levelTransform.localRotation = Quaternion.identity;
        levelTransform.localScale = defaultScale;
    }
    public void SetParentTransformOnly(Transform transform)
    {
        transform.SetParent(tfmHolder);
    }
    public async UniTask CastListScrewTranform(List<Screw> lstScrew, LevelMapSize levelMapSize)
    {
        int level = Db.storage.USER_INFO.level;
        Debug.Log($"LevelScaleHelper Level {level} LevelMapSize {levelMapSize}");
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        var imgSafeArea = GetImageSafeAreaBySiz(levelMapSize);
        levelTransform.localScale = defaultScale;

        foreach (Screw screw in lstScrew)
        {
            if (screw == null) continue;
            // 1. World → Screen point (bằng camera 3D)
            Vector3 screenPos = cam.WorldToScreenPoint(screw.transform.position);

            // Nếu object nằm sau camera => bỏ qua
            if (screenPos.z < 0) continue;

            // 2. Screen point → Local point trong imgSafeArea (Canvas Overlay -> camera = null)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                imgSafeArea.rectTransform,
                screenPos,
                null, // 🔥 Vì canvas overlay
                out Vector2 localPos
            );

            // 3. Spawn marker UI vào safe area
            RectTransform marker = Instantiate(prefabMarker, imgSafeArea.transform);
            marker.anchoredPosition = localPos;

            if (localPos.x < minX) minX = localPos.x;
            if (localPos.x > maxX) maxX = localPos.x;
            if (localPos.y < minY) minY = localPos.y;
            if (localPos.y > maxY) maxY = localPos.y;
        }

        var sizeX = maxX - minX;
        var sizeY = maxY - minY;
        var scale = Mathf.Min(imgSafeArea.rectTransform.rect.width / sizeX, imgSafeArea.rectTransform.rect.height / sizeY);
        Debug.Log($"Scale: {scale}, sizeX: {sizeX}, sizeY: {sizeY}, safeArea: {imgSafeArea.rectTransform.rect.size}");
        //  levelTransform.localScale = defaultScale * scale;
        var time = 0.5f;
#if UNITY_EDITOR
        time = 0f;
#endif
        await levelTransform.DOScale(defaultScale * scale, 0.5f).SetEase(Ease.InOutSine);
    }

    public async UniTask ChangCenterPos(List<Screw> lstScrew)
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;


        foreach (Screw screw in lstScrew)
        {
            if (screw == null) continue;

            totalX += screw.transform.localPosition.x;
            totalY += screw.transform.localPosition.y;
            totalZ += screw.transform.localPosition.z;
        }
        var centerX = totalX / lstScrew.Count;
        var centerY = totalY / lstScrew.Count;
        var centerZ = totalZ / lstScrew.Count;
        var center = new Vector3(centerX, centerY, centerZ);
        // transform.localPosition = -new Vector3(centerX, centerY, 0);
        Debug.Log($"Center: {center}");
        await levelTransform.DOLocalMove(-new Vector3(centerX, centerY, centerZ), 0.5f).SetEase(Ease.InOutSine);

    }
    public async UniTask ChangCenterPos(List<Shape> lstShape, List<LinkObstacle> lstLinkObstacles)
    {
        var totalX = 0f;
        var totalY = 0f;
        var totalZ = 0f;


        foreach (Shape shape in lstShape)
        {
            if (shape == null) continue;

            totalX += shape.transform.localPosition.x;
            totalY += shape.transform.localPosition.y;
            totalZ += shape.transform.localPosition.z;
        }
        foreach (LinkObstacle shape in lstLinkObstacles)
        {
            if (shape == null) continue;

            totalX += shape.transform.localPosition.x;
            totalY += shape.transform.localPosition.y;
            totalZ += shape.transform.localPosition.z;
        }
        var centerX = totalX / lstShape.Count;
        var centerY = totalY / lstShape.Count;
        var centerZ = totalZ / lstShape.Count;
        var center = new Vector3(centerX, centerY, centerZ);
        // transform.localPosition = -new Vector3(centerX, centerY, 0);

        var lstTask = new List<UniTask>();
        Debug.Log($"Center: {center} {lstShape.Count} {lstLinkObstacles.Count}");
        foreach (Shape shape in lstShape)
        {
            if (shape == null) continue;
            var newPos = shape.transform.localPosition - new Vector3(centerX, centerY, centerZ);
            lstTask.Add(shape.transform.DOLocalMove(newPos, 0.5f).SetEase(Ease.InOutSine).ToUniTask());
        }
        foreach (LinkObstacle shape in lstLinkObstacles)
        {
            if (shape == null) continue;
            var newPos = shape.transform.localPosition - new Vector3(centerX, centerY, centerZ);
            lstTask.Add(shape.transform.DOLocalMove(newPos, 0.5f).SetEase(Ease.InOutSine).ToUniTask());
        }

    }
    private Image GetImageSafeAreaBySiz(LevelMapSize size)
    {
        foreach (var item in lstSafeArea)
        {
            if (item.size == size)
            {
                return item.safeArea;
            }
        }
        return null;
    }
    public async UniTask MoveOffset(bool isShowBanner)
    {
        var remote = GameAnalyticController.Instance.Remote();

        Debug.Log($"isShowBanner: {isShowBanner} - {remote.SegmentFlow.bannerLvl} - {Db.storage.USER_INFO.level}");
        if (isShowBanner)
            levelTransform.DOLocalMoveY(0, 0.5f);// new Vector3(0, 5, 0);
        else
            levelTransform.DOLocalMoveY(-3f, 0.5f);

    }
}
