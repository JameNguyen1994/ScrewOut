using PS.Utils;
using UnityEngine;
using UnityEngine.Events;

public class InputHandler : Singleton<InputHandler>
{
    public bool IsLockInput;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask raycastLayerMask;
    [SerializeField] private LayerMask screwLayer;
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private SelectedType selectedType;
    public UnityAction<RaycastHit> actionClickDown;
    public UnityAction actionClickUpFree;
    public UnityAction<RaycastHit> actionClickUpObject;
    [SerializeField] private bool isSelect = true;
    private float radius = 0.15f;
    public SelectedType SelectedType { get => selectedType; }
    public bool IsSelect { get => isSelect; set => isSelect = value; }

    void Start()
    {

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        
        if (IsLockInput || !isSelect) return;
        if (GameManager.Instance.GameState == GameState.Stop || PopupController.Instance.PopupCount > 0) return;
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        Debug.Log("Test input HandleMouseInput 1");
        if (BoosterController.Instance.CurrentAnimationBooster != BoosterType.None || BoosterController.Instance.IsShowTutorial)
            return;
        Debug.Log("Test input HandleMouseInput 2");
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GetRay(); //mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitSphere;

            RaycastHit hitRay;
           // BoosterController.Instance.StopCountTimeToShowHighLight();
            bool rayHit = Physics.Raycast(ray, out hitRay, rayDistance, raycastLayerMask);
            bool sphereHit = Physics.SphereCast(ray, radius, out hitSphere, rayDistance, raycastLayerMask);

            if (sphereHit && rayHit)
            {
                if (hitRay.collider.gameObject == hitSphere.collider.gameObject)
                {
                    actionClickDown?.Invoke(hitSphere);
                }
                else if ((screwLayer.value & (1 << hitRay.collider.gameObject.layer)) != 0)
                {
                    actionClickDown?.Invoke(hitRay);
                }
                else
                {
                    actionClickDown?.Invoke(hitSphere);
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 2f);
        }
        if (Input.GetMouseButtonDown(0) && BoosterController.Instance.UsingHammer)
        {
            // actionClickUpFree?.Invoke();
            Ray ray = GetRay(); //mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, radius, out hit, rayDistance, raycastLayerMask))
            {
                if (hit.collider.GetComponent<Shape>() != null)
                {
                    var shape = hit.collider.GetComponent<Shape>();
                    Debug.Log($"Click Shape {shape.name}");
                    BoosterController.Instance.OnUseShape(shape);
                    return;
                }
                if (hit.collider.GetComponent<Screw>() != null)
                {
                    var screw = hit.collider.GetComponent<Screw>();
                    Debug.Log($"Click screw {screw.name}");
                    if (screw.LstLinkObstacle.Count > 0)
                        BoosterController.Instance.OnUseShape(screw.LstLinkObstacle[0]);
                    else
                        BoosterController.Instance.OnUseShape(screw.Shape);

                    return;
                }

            }
        }
        else
     if (Input.GetMouseButtonUp(0) && !BoosterController.Instance.UsingHammer)
        {
            actionClickUpFree?.Invoke();
            Ray ray = GetRay(); //mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitSphere;
            RaycastHit hitRay;

            bool rayHit = Physics.Raycast(ray, out hitRay, rayDistance, raycastLayerMask);
            bool sphereHit = Physics.SphereCast(ray, radius, out hitSphere, rayDistance, raycastLayerMask);

            if (sphereHit && rayHit)
            {
                if (hitRay.collider.gameObject == hitSphere.collider.gameObject)
                {
                    actionClickUpObject?.Invoke(hitSphere);
                }
                else if ((screwLayer.value & (1 << hitRay.collider.gameObject.layer)) != 0)
                {
                    actionClickUpObject?.Invoke(hitRay);
                }
                else
                {
                    actionClickUpObject?.Invoke(hitSphere);
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = GetRay(); // mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.SphereCast(ray, radius, out hit, rayDistance, raycastLayerMask))
            {
                actionClickDown?.Invoke(hit);
            }

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green, 2f);
        }


    }

    Ray GetRay()
    {
        return mainCamera.ScreenPointToRay(Input.mousePosition);
    }

    void OnRaycastHit(RaycastHit hit)
    {
        Debug.Log("Raycast hit at point: " + hit.point);
    }


}

public enum SelectedType
{
    ClickDown = 0, ClickUp = 1,
}