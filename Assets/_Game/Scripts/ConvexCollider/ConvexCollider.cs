using UnityEngine;

public class ConvexCollider : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private VHACD.Unity.Parameters _parameters;
    public VHACD.Unity.Parameters Parameters => _parameters;

    [SerializeField, HideInInspector]
    private bool _hideColliders = true;

    [SerializeField, HideInInspector]
    private int _quality = -1;

    [SerializeField, HideInInspector]
    private bool _combineMeshesBeforeCompute = true;
}