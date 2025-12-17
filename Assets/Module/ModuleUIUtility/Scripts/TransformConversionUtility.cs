using UnityEngine;

public static class TransformConversionUtility
{
    private static GameObject _tempObject;
    private static Transform _tempTransform;
    private static Transform TempTransform
    {
        get
        {
            if (_tempTransform == null)
            {
                _tempObject = new GameObject("TransformConversionCache");
                _tempObject.hideFlags = HideFlags.HideAndDontSave;
                _tempTransform = _tempObject.transform;
            }

            return _tempTransform;
        }
    }

    public static void SetWorldToLocalPosition(this Transform target, Vector3 worldPosition)
    {
        target.localPosition = WorldToLocal(target.parent, worldPosition);
    }

    public static Vector3 WorldToLocalPosition(this Transform target, Vector3 worldPosition)
    {
        return WorldToLocal(target.parent, worldPosition);
    }

    public static Vector3 WorldToLocal(Transform parent, Vector3 worldPosition)
    {
        TempTransform.SetParent(null);
        TempTransform.position = worldPosition;
        TempTransform.SetParent(parent, true); // true keeps world position, calculates local
        return TempTransform.localPosition;
    }

    public static Vector3 LocalToWorld(Transform parent, Vector3 localPosition)
    {
        TempTransform.SetParent(parent, false);
        TempTransform.localPosition = localPosition;
        return TempTransform.position;
    }

    public static void Dispose()
    {
        if (_tempObject != null)
        {
#if UNITY_EDITOR
            Object.DestroyImmediate(_tempObject);
#else
            Object.Destroy(_tempObject);
#endif
        }
    }
}