using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using ScrewCraze3D;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Screw : ObjectIdentifier
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Shape shape;
    [SerializeField] private List<LinkObstacle> lstLinkObstacle = new List<LinkObstacle>();
    [SerializeField] private ScrewColor screwColor;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material matHighLight;
    [SerializeField] private ScrewState state;
    [SerializeField] private GameObject gobjTutorialHand;
    [SerializeField] private List<Shape> lstShapeBlocked;
    [SerializeField] private List<Shape> lstShapeCovered;
    [SerializeField] private List<LinkObstacle> lstObstacleBlocked;
    [SerializeField] private List<LinkObstacle> lstObstacleCovered;

    [SerializeField] private LayerMask layerMaskNor;
    [SerializeField] private LayerMask layerMaskHighlight;
    private float rayCastDistance = 0.25f;
    private float rayRadius = 0.3f;
    private float rayOffset = 0.35f;


    private Material[] originalMaterials; // Lưu lại các material gốc
    private bool isHighlighted = false;
    private Vector3 initialPosition;
    private Tray tray;
    private bool isMove = false;
    private bool isBack = false;
    private float localRotateOnBox = 0.3f;
    private float localScaleOnBox = 0.3f;


    public Rigidbody Rb { get => rb; }
    public Shape Shape { get => shape; }
    public ScrewColor ScrewColor { get => screwColor; }
    public ScrewState State { get => state; }
    public Tray Tray { get => tray; set => tray = value; }
    public bool IsBack { get => isBack; }
    public List<LinkObstacle> LstLinkObstacle { get => lstLinkObstacle; }
    public List<Shape> LstShapeBlocked { get => lstShapeBlocked; }
    public List<Shape> LstShapeCovered { get => lstShapeCovered; }
    public List<LinkObstacle> LstObstacleBlocked { get => lstObstacleBlocked; }
    public List<LinkObstacle> LstObstacleCovered { get => lstObstacleCovered; }

    [SerializeField] protected Vector3 scaleDefault;
    public bool IsFree => lstObstacleBlocked.Count == 0 && lstShapeBlocked.Count == 0 && lstObstacleCovered.Count == 0 && lstShapeCovered.Count == 0;

    private bool isContainWrech;
    private Wrench wrench;
    public bool IsContainWrech => isContainWrech;

    private void Awake()
    {
        ScrewManager.AddScrew(this);
    }

    public void Hide()
    {
        scaleDefault = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    public async UniTask Show(float Time)
    {
        await transform.DOScale(scaleDefault, Time).SetEase(Ease.OutBack);
    }
    public void OnEnable()
    {
        var collider = gameObject.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Screw does not have BoxCollider, adding one now.");
            collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(0, 0.35f, 0);
            collider.size = new Vector3(0.75f, 0.45f, 0.75f);
            collider.isTrigger = true;
        }
    }
    public void SetShape(Shape shape)
    {
        this.shape = shape;
    }
    public void Init(ScrewColor screwColors)
    {
        screwColor = screwColors;
        meshRenderer.material = DataScrewColor.Instance.GetMaterialScrewByColor(screwColor);
        gameObject.name = $"Screw_{screwColor}";
        initialPosition = transform.localPosition;
        originalMaterials = meshRenderer.sharedMaterials;
        DisableHighlight();

        if (IngameData.MODE_CONTROL == ModeControl.ControlV2)
        {
            localRotateOnBox = 1f;
            localScaleOnBox = 1f;
            var box = gameObject.GetComponent<BoxCollider>();
            box.center = new Vector3(0, 0.45f, 0);
            box.size = new Vector3(1f, 0.45f, 1f);
        }

        InitData();
    }

    private void OnDestroy()
    {
        ScrewManager.RemoveScrew(this);
    }

    public void SetState(ScrewState state)
    {
        this.state = ScrewState.OnReviveBox;
    }

    public void EnableHighlight()
    {
        int layerIndex = (int)Mathf.Log(layerMaskHighlight.value, 2);

        layerIndex = LayerMask.NameToLayer("screw_highlight");

        Debug.Log($"Set layer to Highlight: {layerIndex}");
        gameObject.layer = layerIndex;
        Material[] newMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = originalMaterials[i];
        }
        newMaterials[newMaterials.Length - 1] = matHighLight;
        meshRenderer.materials = newMaterials;
        isHighlighted = true;

       
    }

    public void DisableHighlight()
    {
        meshRenderer.materials = originalMaterials;
        isHighlighted = false;
        int layerIndex = (int)Mathf.Log(layerMaskNor.value, 2);
        layerIndex = LayerMask.NameToLayer("screw");
        gameObject.layer = layerIndex;

    }
    public void SetUp(Shape shape)
    {
        //rb = gameObject.GetComponent<Rigidbody>();
        //rb.isKinematic = true;
        this.shape = shape;
        //meshRenderer = gameObject.GetComponent<MeshRenderer>();
        //if (gameObject.GetComponent<BoxCollider>() == null)
        //{
        //    gameObject.AddComponent<BoxCollider>();

        //}
        //var box = gameObject.GetComponent<BoxCollider>();
        //box.isTrigger = true;
        //box.center = new Vector3(0, 0.55f, 0);
        //box.size = new Vector3(2, 0.55f, 2);


        //if (gameObject.GetComponent<CapsuleCollider>() == null)
        //{
        //    gameObject.AddComponent<CapsuleCollider>();
        //}
        //var cap = gameObject.GetComponent<CapsuleCollider>();
        //cap.isTrigger = true;
        //cap.center = new Vector3(0, -0.35f, 0);
        //cap.radius = 0.5f;
        //cap.height = 1.5f;
    }


    public void SetUpShapeAndColider(Shape shape)
    {
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();

        }
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        this.shape = shape;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (gameObject.GetComponent<BoxCollider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();

        }
        var box = gameObject.GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = new Vector3(0, 0.35f, 0);
        box.size = new Vector3(0.75f, 0.45f, 0.75f);

        //if (gameObject.GetComponent<CapsuleCollider>() == null)
        //{
        //    gameObject.AddComponent<CapsuleCollider>();
        //}
        //var cap = gameObject.GetComponent<CapsuleCollider>();
        //cap.isTrigger = true;
        //cap.center = new Vector3(0, -0.1f, 0);
        //cap.radius = 0.2f;
        //cap.height = 0.6f;
    }
    public async UniTask<bool> CheckDetectShape(bool isCheckForTest = false)
    {


        Ray ray = new Ray(transform.position + transform.up * rayOffset * transform.lossyScale.y, transform.up);


        float distance = rayCastDistance;

        float radius = rayRadius * transform.lossyScale.x;
        var lstShape = new List<Shape>();

        float height = rayCastDistance; // tương ứng với "distance" trước đây

        // Xác định 2 điểm đầu capsule
        Vector3 p1 = transform.position + transform.up * rayOffset * transform.lossyScale.y;       // đáy
        Vector3 p2 = p1 + transform.up * height;                       // đỉnh


        // Debug: vẽ đường capsule
        Debug.DrawLine(p1, p2, Color.red, 1);
        DrawCross(p1, radius, Color.green, 1);
        DrawCross(p2, radius, Color.blue, 1);

        bool detected = false;
        Shape shapeDetected = null;
        RaycastHit hitDetected = new RaycastHit();
        //   RaycastHit[] hits = Physics.SphereCastAll(ray, radius, distance);
        // Overlap capsule để tìm collider
        Collider[] hits = Physics.OverlapCapsule(p1, p2, radius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider col in hits)
        {
            if (col != null)
            {
                Debug.Log($"Raw hit: {col.name}, has Shape? {col.GetComponent<Shape>() != null}");
                var shape = col.GetComponent<Shape>();
                var obstacle = col.GetComponent<LinkObstacle>();
                if (shape != null && shape != this.shape && !lstLinkObstacle.Contains(obstacle))
                {
#if UNITY_EDITOR
#endif
                    Debug.Log($"Screw {gameObject.name} bị chắn bởi {col.name}");
                    detected = true;
                    shapeDetected = shape;
                }
            }
        }

        if (isCheckForTest)
        {
            return detected;
        }

        if (detected)
        {
            var distanceToHit = Vector3.Distance(transform.position, hitDetected.point);
            await MoveUp(true, shapeDetected, distanceToHit, distance);
            return true;
        }
        else
        {
            var distanceToHit = Vector3.Distance(transform.position, hitDetected.point);
            await MoveUp(false, shapeDetected, distanceToHit, distance);
            return false;
        }
    }


    private async UniTask MoveUp(bool isBack, Shape shape, float distanceToHit, float distanceMax)
    {
        var time = 0.3f; //* (!isBack ? 1 : (distanceToHit / distanceMax));
        transform.DOLocalRotate(new Vector3(0, -280, 0), time, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

        var currentPos = transform.position;
        var targetPosition = transform.position + transform.up * (isBack ? distanceToHit : distanceMax);

        if (!isBack)
        {
            //LevelController.Instance.Level.RemoveScrewAvailable(this);
            Debug.Log("Move Up Screw");
            //EffectHelper.Instance.ShowFxScrew(transform.position, transform.eulerAngles, transform.lossyScale * 0.5f, null);
            PoolManager.Instance.Spawn(PoolKey.SOF_SCREW_OPEN, transform.position, transform.rotation);

            Tray trayNoneFill = null;
            Box boxCanFill = null;

            if (IngameData.GameMode == GameMode.Normal)
            {
                trayNoneFill = LevelController.Instance.GetTrayNonFill();
                boxCanFill = LevelController.Instance.BaseBox.GetBoxUnlockByColor(this.ScrewColor);
            }
            else
            {
                trayNoneFill = LevelBonusController.Instance.GetTrayNonFill();
                boxCanFill = LevelBonusController.Instance.BoxLevelBonusController.GetBoxUnlockByColor(this.ScrewColor);
#if UNITY_EDITOR
                UnityEditor.EditorGUIUtility.PingObject(boxCanFill);
#endif
                Debug.LogWarning("Check tray and box in Level Bonus Mode");
            }
            var trayOnBox = boxCanFill != null ? boxCanFill.GetTrayOnBoxNonFill() : null;
            bool isBugBox = boxCanFill == null || (boxCanFill != null && trayOnBox == null);

            Debug.Log($"Fix bug Screw {gameObject.name} state:{state} can fill tray: {trayNoneFill != null}, box: {boxCanFill != null}");
            if ((isBugBox && trayNoneFill == null) || state != ScrewState.OnShape)
            {
                Debug.LogWarning("Fix bug: Không có tray hoặc box nào để đặt screw này");
                transform.DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
                isBack = true;
                transform.DOLocalMove(initialPosition, 0.5f).OnComplete(async () =>
                {
                    isBack = false;

                });
                return;
            }

            if (trayOnBox != null)
            {
                if (trayOnBox.Box.Color == ScrewColor && !trayOnBox.IsFill)
                {
                    trayOnBox.Fill(this);
                    trayOnBox.SetComletedAnim(false);
                }
                else
                {
                    return;
                }
            }
            else if (trayNoneFill != null)
            {
                if (!trayNoneFill.IsFill)
                {
                    trayNoneFill.Fill(this);
                    trayNoneFill.SetComletedAnim(false);
                }
                else
                {
                    return;
                }
            }

            if (IngameData.GameMode == GameMode.Normal)
                LevelController.Instance.Level.FilledScrew(UniqueId);
            else
                LevelBonusController.Instance.Level.FilledScrew(UniqueId);

            if (lstLinkObstacle.Count > 0)
            {
                for (int i = 0; i < lstLinkObstacle.Count; i++)
                {
                    lstLinkObstacle[i].RemoveScrew(this);
                    lstLinkObstacle[i].CheckRemoveScrew();
                }
            }

            IngameData.TRACKING_UN_SCREW_COUNT++;
            IngameData.BREAK_SCREW_COUNT++;
            Debug.Log($"Move Up Break Screw: {IngameData.BREAK_SCREW_COUNT}");

            if (trayOnBox != null)
            {
                if (trayOnBox.Box.Color == ScrewColor)
                {
                    MoveToTray(trayOnBox, true, OnScrewMoveToTrayComplete);
                }
            }
            else if (trayNoneFill != null)
            {
                MoveToTray(trayNoneFill, false, OnScrewMoveToTrayComplete);
            }
            if (IngameData.GameMode == GameMode.Normal)
                LevelController.Instance.Level.RemoveScrewAvailable(this);

            this.shape.RemoveScrew(this);
            this.shape.CheckRemoveScrew();

          //  AdsController.Instance.ShowAdBreakUnScrew();
        }

        if (isBack)
        {
            shape.TurnOnBlink();

            Debug.Log("Move Up Screw Back");
            EnableHighlight();


            // Bounce nhún lên xuống 4 lần giảm dần
            Vector3 startPos = transform.localPosition;
            int bounceCount = 4;             // số lần nhún
            float currentDistance = distanceMax * 15;
            float currentDuration = time / 3; // chia nhỏ thời gian
            transform.DOLocalRotate(new Vector3(0, -280, 0), currentDuration * bounceCount * 2, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

            for (int i = 0; i < bounceCount; i++)
            {
                // đi lên và về vị trí gốc trong một nhịp nhún
                await DOVirtual.Float(0, 1, currentDuration * 2, t =>
                {
                    transform.Translate(Vector3.up * currentDistance * Time.deltaTime, Space.Self);
                });
                await transform.DOLocalMove(startPos, currentDuration).SetEase(Ease.OutQuad);

                // giảm biên độ và thời gian
                currentDistance *= 0.8f;
                currentDuration *= 0.8f;
            }
            DisableHighlight();

            //await transform.DOMove(currentPos, time);pa

        }
    }
    [Button]

    private void OnScrewMoveToTrayComplete()
    {
        EditorLogger.Log("[SCREW] OnScrewMoveToTrayComplete");
        ScrewCounter.Instance.UpdateScrew();
        AudioController.Instance.PlaySound(SoundName.ScrewDown);
        LevelController.Instance.AssignScrewsToBoxesWhenBoxOut();
    }

    public async void OnScrewSelected()
    {
        if (IsContainWrech)
        {
            RemoveWrench();
            return;
        }

        if (this.state == ScrewState.OnTray || state == ScrewState.OnBox) return;
        IngameData.UNLOCK_SWIP = true;

        var isDetect = await CheckDetectShape();
        IngameData.UNLOCK_SWIP = false;

        if (isDetect)
            return;

        VibrationController.Instance.Vibrate(VibrationType.VerySmall);

        // if (isBack) return;



        ScreenGamePlayUI.Instance.OnEnableTutorialSwip(true);

        if (gobjTutorialHand != null)
        {
            gobjTutorialHand.SetActive(false);
        }
        // InputHandler.Instance.IsSelect = false;
        //isMove = true;
        AudioController.Instance.PlaySound(SoundName.ScrewUp);
        //await UniTask.Delay(300);
        isMove = false;
        InputHandler.Instance.IsSelect = true;
        //if (!isBack)
        {

        }
    }
    public void ChangeState(ScrewState state)
    {
        this.state = state;
        /* if (state != ScrewState.OnShape)
         {
             LevelController.Instance.Level.RemoveScrewAvailable(this);
         }*/
    }
    // Secret Box

    public async UniTask MoveToTray(Tray tray, bool animRotateFirst, UnityAction onComplete, float moveUpDistance)
    {
        if (IsContainWrech)
        {
            RemoveWrench();
        }

        this.tray = tray;

        tray.Fill(this);
        tray.SetComletedAnim(false);
        var scale = Vector3.one;
        var rotateAngleAtShape = new Vector3(0, -270, 0);
        var rotateAngleWhenFly = new Vector3(0, -90, 90);
        var rotateAngleAtTray = new Vector3(180, -90, 90);

        var timeRotate = 0.15f;
        var timeMove = 0.3f;
        //timeMove = 3f;
        // Rotate tại chỗ
        if (animRotateFirst)
        {

            /* if (moveUpDistance > 0)
             {
                 // Dot product giữa hướng "up" của vật và Vector3.up (trục Z dương của thế giới)
                 float dot = Vector3.Dot(transform.forward, Vector3.forward);
                 bool isUp = dot > 0;
                 var direction = isUp ? 1 : -1;
                 transform.DOLocalMoveZ(transform.localPosition.z + moveUpDistance* direction, timeMove);
             }*/
            if (moveUpDistance > 0)
            {
                DOVirtual.Float(0, 1, timeMove, (value) =>
                {
                    transform.Translate(Vector3.up * moveUpDistance * Time.deltaTime, Space.Self);
                });

            }
            await transform.DOLocalRotate(rotateAngleAtShape, timeRotate, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        }

        // Move tới vị trí tiếp theo
        var tfmTray = tray.transform;
        transform.SetParent(tfmTray);
        Vector3 pos = new Vector3(0, 0, -0.6f);
        transform.DOLocalRotate(rotateAngleWhenFly, timeMove);
        transform.DOScale(scale, timeRotate);

        //await DOLocalJumpZ(transform, pos, 0.0f, timeMove);
        await transform.DOLocalJump(pos, 0.2f, 1, timeMove);
        transform.DOLocalMove(new Vector3(0, 0, -0.1f), timeRotate / 3);
        await transform.DOLocalRotate(rotateAngleAtTray, timeRotate / 3, RotateMode.Fast).SetEase(Ease.Linear);
        tray.SetComletedAnim(true);
        onComplete?.Invoke();
    }


    public async UniTask MoveToSecretBox(Tray tray)
    {
        if (IsContainWrech)
        {
            RemoveWrench();
        }

        if (this.tray != null)
        {
            this.tray.Fill(null);
            this.tray.SetComletedAnim(false);
        }
        this.tray = tray;
        var tfmTray = tray.transform;

        var duration = 0.5f;
        var firstDuration = duration * 0.7f;
        var secondDuration = duration * 0.3f;
        transform.SetParent(tfmTray);
        await transform.DOLocalRotate(new Vector3(0, -280, 0), 0.3f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
        transform.DOLocalJump(Vector3.zero, 7f, 1, duration);
        await transform.DOScale(Vector3.one * 0.8f, firstDuration);
        transform.DOScale(Vector3.zero, secondDuration);

        await UniTask.Delay(TimeSpan.FromSeconds(duration));
    }

    public async UniTask DOLocalJumpZ(Transform target, Vector3 endValue, float jumpPower, float duration)
    {
        var path = new Vector3[3];
        float maxZ = Mathf.Max(target.localPosition.z, endValue.z) / 2 + jumpPower;
        path[0] = target.localPosition;
        path[1] = new Vector3((target.localPosition.x + endValue.x) / 2, (target.localPosition.z + endValue.z) / 2, maxZ);
        path[2] = endValue;

        await transform.DOLocalPath(path, duration, PathType.CatmullRom).SetEase(Ease.InOutSine);
    }
    public void OnFillByBooster()
    {
        CheckListLinkObtacle();
        shape.RemoveScrew(this);
        shape.CheckRemoveScrew();
        IngameData.TRACKING_UN_SCREW_COUNT++;
    }
    public void CheckListLinkObtacle()
    {
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            if (lstLinkObstacle[i] != null)
                lstLinkObstacle[i].RemoveScrew(this);
            lstLinkObstacle[i].CheckRemoveScrew();
        }
    }


    #region SetUp

    public void SetShapeParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public bool IsDetectShape()
    {
        Ray ray = new Ray(transform.position, transform.up);
        RaycastHit hit;
        float rayDistance = rayCastDistance;
        if (Physics.Raycast(ray, out hit, rayDistance, 1 << 7))
        {

            if (shape != null && hit.collider.name == shape.name)
            {
                return false;
            }

            Debug.Log($"Screw {gameObject.name} bị chắn bởi {hit.collider.name} tại {hit.point}");
            return true;
        }
        return false;
    }
    public List<Shape> GetListShapeIsDetected()
    {
        var lstShape = new List<Shape>();

        // Cấu hình capsule
        float height = 2f; // tương ứng với "distance" trước đây
        float radius = rayRadius * transform.lossyScale.x;

        // Xác định 2 điểm đầu capsule
        Vector3 p1 = transform.position + transform.up * rayOffset * transform.lossyScale.y;       // đáy
        Vector3 p2 = p1 + transform.up * height;                       // đỉnh


        // Debug: vẽ đường capsule
        Debug.DrawLine(p1, p2, Color.red, 1);
        DrawCross(p1, radius, Color.green, 1);
        DrawCross(p2, radius, Color.blue, 1);

        /*     DrawCircle(p1, transform.up, radius, Color.yellow, 1);
             DrawCircle(p2, transform.up, radius, Color.yellow, 1);*/
        // Overlap capsule để tìm collider
        Collider[] hits = Physics.OverlapCapsule(p1, p2, radius, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider col in hits)
        {
            if (col == null) continue;

            Shape shape = col.GetComponent<Shape>();
            var obstacle = col.GetComponent<LinkObstacle>();
            if (shape != null && shape != this.shape && !lstLinkObstacle.Contains(obstacle))
            {
#if UNITY_EDITOR
                DrawCross(col.transform.position, 0.3f, Color.red, 5);
#endif
                Debug.Log($"Screw {gameObject.name} phát hiện {col.name} trong vùng capsule");
                if (!lstShape.Contains(shape))
                    lstShape.Add(shape);
            }
        }
        var color = lstShape.Count > 0 ? Color.red : Color.green;
        RaycastDebugExtension.DrawCapsule(p1, p2, radius, color, 2);

        return lstShape;
    }
    public List<LinkObstacle> GetListLinkObstacleIsDetected()
    {
        var lstObstacle = new List<LinkObstacle>();

        // Cấu hình capsule
        float height = 2f; // tương ứng với "distance" trước đây
        float radius = rayRadius * transform.lossyScale.x;

        // Xác định 2 điểm đầu capsule
        Vector3 p1 = transform.position + transform.up * rayOffset * transform.lossyScale.y;       // đáy
        Vector3 p2 = p1 + transform.up * height;                       // đỉnh


        // Debug: vẽ đường capsule
        Debug.DrawLine(p1, p2, Color.red, 1);
        DrawCross(p1, radius, Color.green, 1);
        DrawCross(p2, radius, Color.blue, 1);

        // Overlap capsule để tìm collider
        Collider[] hits = Physics.OverlapCapsule(p1, p2, radius, ~0, QueryTriggerInteraction.Collide);

        foreach (Collider col in hits)
        {
            if (col == null) continue;
            LinkObstacle obstacle = col.GetComponent<LinkObstacle>();

            if (obstacle != null && !lstLinkObstacle.Contains(obstacle))
            {
#if UNITY_EDITOR
                DrawCross(col.transform.position, 0.3f, Color.red, 5);
#endif
                Debug.Log($"Screw {gameObject.name} phát hiện Obstacle {col.name} trong vùng capsule");
                if (!lstObstacle.Contains(obstacle))
                    lstObstacle.Add(obstacle);
            }
        }

        return lstObstacle;
    }

    [Button]
    public void TestDetectList()
    {
        var lstShape = GetListShapeIsDetected();
        if (lstShape.Count > 0)
        {
            Debug.Log($"Screw {gameObject.name} bị chắn bởi {lstShape.Count} shape");
            for (int i = 0; i < lstShape.Count; i++)
            {
                Debug.Log($" - Shape {i}: {lstShape[i].name}");
            }
        }
        else
        {
            Debug.Log($"Screw {gameObject.name} không bị chắn bởi shape nào");
        }
    }
    public (List<Shape>, List<LinkObstacle>) GetListShapeCover()
    {
        var lstShape = new List<Shape>();
        var lstLinkObstacle = new List<LinkObstacle>();
        float rayDistance = 50f;

        int yawStep = 1;   // bước theo trục Y (ngang)
        int pitchStep = 1; // bước theo trục X (dọc)

        for (int yaw = 0; yaw < 360; yaw += yawStep)
        {
            for (int pitch = -90; pitch <= 90; pitch += pitchStep)
            {
                // Tạo hướng từ yaw + pitch
                Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
                Vector3 rayDirection = rotation * Vector3.forward;

                // Vẽ debug
                Debug.DrawRay(transform.position, rayDirection * rayDistance, Color.red, 1f);

                Ray ray = new Ray(transform.position, rayDirection);

                if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
                {
                    var shape = hit.collider.GetComponent<Shape>();
                    var obstacle = hit.collider.GetComponent<LinkObstacle>();
                    if (shape == null || this.shape == shape)
                        return (new List<Shape>(), new List<LinkObstacle>());
                    if (obstacle == null || lstLinkObstacle.Contains(obstacle))
                        return (new List<Shape>(), new List<LinkObstacle>());

                    if (!lstShape.Contains(shape))
                        lstShape.Add(shape);
                    if (!lstLinkObstacle.Contains(obstacle))
                        lstLinkObstacle.Add(obstacle);
                }
            }
        }

        return (lstShape, lstLinkObstacle);
    }

    [ContextMenu("ScrewObstacleToShape")]
    public void ScrewObstacleToShape()
    {
        if (shape != null) return;
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.up);
        float rayDistance = 0.8f;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, 1 << 7))
        {
            if (hit.collider.name.ToLower().Contains($"cube"))
            {
                Debug.Log("ScrewObstacleToShape");

                SetShapeParent(hit.transform);
            }
        }
    }
    private void OnDrawGizmos()
    {
        /*  float radius = rayRadius * transform.lossyScale.x;
          Gizmos.color = Color.red;
          Gizmos.DrawSphere(transform.position + transform.up * rayCastDistance, radius);
  */
        Ray ray = new Ray(transform.position, transform.up);
        float rayDistance = rayCastDistance;
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, 1 << 7))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, hit.point);
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ray.origin, ray.origin + transform.up * rayDistance);
        }

        if (shape != null) return;
        float rayDistanceCheckShape = 2f;
        Ray rayCheckShape = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), -transform.up * rayDistanceCheckShape);

        if (Physics.Raycast(rayCheckShape, out RaycastHit hitCheckShape, rayDistanceCheckShape, 1 << 7))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayCheckShape.origin, hitCheckShape.point);
            Gizmos.DrawSphere(hitCheckShape.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(rayCheckShape.origin, rayCheckShape.origin - transform.up * rayDistanceCheckShape);
        }

    }
    // Utility function to draw a cross at a position in 3D
    void DrawCross(Vector3 position, float size, Color color, float duration = 0)
    {
        Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color, duration);
        Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color, duration);
        Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color, duration);
    }

    // Utility to draw a circle (for SphereCast radius visualization)
    void DrawCircle(Vector3 center, Vector3 direction, float radius, Color color, float duration = 0, int segments = 16)
    {
        // Find two perpendicular vectors to direction
        Vector3 up = Vector3.up;
        if (Vector3.Dot(direction, up) > 0.99f) up = Vector3.right; // If looking straight up, pick another axis
        Vector3 right = Vector3.Cross(direction, up).normalized;
        up = Vector3.Cross(direction, right).normalized;

        float angleStep = 360f / segments;
        Vector3 prevPoint = center + right * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 nextPoint = center + (Mathf.Cos(angle) * right + Mathf.Sin(angle) * up) * radius;
            if (duration > 0)
            {
                Debug.DrawLine(prevPoint, nextPoint, color, duration);
            }
            else
            {
                Debug.DrawLine(prevPoint, nextPoint, color);
            }
            prevPoint = nextPoint;
        }
    }
    #endregion
    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, transform.up * rayCastDistance, Color.red);
    }

    public ScrewData GetScrewData()
    {
        return new ScrewData()
        {
            Id = UniqueId,
            Color = screwColor,
            State = State
        };
    }

    public void SetTray(Tray tray)
    {
        this.tray = tray;
    }

    public void RemoveLink()
    {
        if (LstLinkObstacle != null && LstLinkObstacle.Count > 0)
        {
            for (int i = 0; i < LstLinkObstacle.Count; i++)
            {
                LstLinkObstacle[i].RemoveScrew(this);
                LstLinkObstacle[i].CheckRemoveScrew();
                LstLinkObstacle[i].CheckHole();
            }
        }
    }
    public void AddListShapeBlocked(List<Shape> shapes, List<LinkObstacle> linkObstacles)
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            if (!lstShapeBlocked.Contains(shapes[i]))
            {
                lstShapeBlocked.Add(shapes[i]);
            }
        }
        for (int i = 0; i < linkObstacles.Count; i++)
        {
            if (!lstObstacleBlocked.Contains(linkObstacles[i]))
            {
                lstObstacleBlocked.Add(linkObstacles[i]);
            }
        }
    }
    public void AddListShapeCovered(List<Shape> shapes, List<LinkObstacle> obstacles)
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            if (!lstShapeCovered.Contains(shapes[i]))
            {
                lstShapeCovered.Add(shapes[i]);
            }
        }
        for (int i = 0; i < obstacles.Count; i++)
        {
            if (!lstObstacleCovered.Contains(obstacles[i]))
            {
                lstObstacleCovered.Add(obstacles[i]);
            }
        }
    }


    public void RemoveShapeBlock(Shape shape)
    {
        if (lstShapeBlocked.Contains(shape))
        {
            lstShapeBlocked.Remove(shape);
        }
        LevelController.Instance.Level.MarkThisScrewAvailable(this);
    }
    public void RemoveLinkObstacle(LinkObstacle linkObstacle)
    {
        if (lstObstacleBlocked.Contains(linkObstacle))
        {
            lstObstacleBlocked.Remove(linkObstacle);
        }
        LevelController.Instance.Level.MarkThisScrewAvailable(this);
    }
    public void RemoveShapeCover(Shape shape)
    {
        if (lstShapeCovered.Contains(shape))
        {
            lstShapeCovered.Clear();
            lstObstacleCovered.Clear();
            if (lstShapeBlocked.Count == 0)
                LevelController.Instance.Level.MarkThisScrewAvailable(this);
        }
    }
    public void RemoveLinkObstacleCover(LinkObstacle linkObstacle)
    {
        if (lstObstacleCovered.Contains(linkObstacle))
        {
            lstObstacleCovered.Clear();
            lstShapeCovered.Clear();
            if (lstShapeBlocked.Count == 0)
                LevelController.Instance.Level.MarkThisScrewAvailable(this);
        }
    }
    private async void MoveToTray(Tray tray, bool isTrayInBox, UnityAction onConplete)
    {
        var distanceUp = 6f;
        if (isTrayInBox) // move to box
        {
            if (State == ScrewState.OnTray)
            {
                Tray.Fill(null);
            }

            ChangeState(ScrewState.OnBox);
            if (IngameData.GameMode == GameMode.Normal)
                LevelController.Instance.RemoveScrew(this);
            else
                LevelBonusController.Instance.RemoveScrew(this);

            await MoveToTray(tray, true, () =>
            {
                if (IngameData.GameMode == GameMode.Normal)
                    LevelController.Instance.OnCollectScrew(ScrewColor);

                tray.Box.AddScrew(this);
                tray.Box.CheckFull();
                if (IngameData.GameMode == GameMode.Normal)

                    LevelController.Instance.CheckShowWarning();
                onConplete.Invoke();

            }, distanceUp);
        }
        else
        {
            ChangeState(ScrewState.OnTray);
            if (IngameData.GameMode == GameMode.Normal)
                LevelController.Instance.AddScrew(this);
            else
                LevelBonusController.Instance.AddScrew(this);

            await MoveToTray(tray, true, () =>
            {
                if (IngameData.GameMode == GameMode.Normal)

                    LevelController.Instance.CheckShowWarning();
                onConplete.Invoke();
            }, distanceUp);
        }
    }

    public void MoveToBox(Box box)
    {
        for (int i = 0; i < box.LstTray.Count; i++)
        {
            if (box.LstTray[i].TryFill(this))
            {
                MoveToTray(box.LstTray[i], true, OnScrewMoveToTrayComplete);
                break;
            }
        }
    }
    public void SetUpHighlight(Material matHighLight, LayerMask layerMaskHighlight, LayerMask layerMaskNor)
    {
        this.matHighLight = matHighLight;
        this.layerMaskHighlight = layerMaskHighlight;
        this.layerMaskNor = layerMaskNor;
    }

    public void SpawnWrench()
    {
        isContainWrech = true;
        GameObject wrenchGO = Instantiate(WrenchCollectionManager.Instance.Config.Wrench, transform);
        wrench = wrenchGO.GetComponent<Wrench>();
        wrench.Screw = this;
        wrench.transform.localPosition = new Vector3(0, 0.35f, 0);
        wrenchGO.SetActive(true);
    }

    public void RemoveWrench(Wrench wrenchGO = null)
    {
        isContainWrech = false;

        if (wrenchGO != null)
        {
            Destroy(wrenchGO);
        }

        LevelController.Instance.Level.CollectWrench(UniqueId);
        WrenchCollectionGamePlayController.Instance.CollectWrench(wrench);
        wrench = null;
    }

#if UNITY_EDITOR

    [Button]
    private void MoveToNoneTrayWithComplete()
    {
        var trayNoneFill = LevelController.Instance.GetTrayNonFill();
        MoveToTray(trayNoneFill, false, OnScrewMoveToTrayComplete);
    }

    [Button]
    private void MoveToNoneTrayNoneComplete()
    {
        var trayNoneFill = LevelController.Instance.GetTrayNonFill();
        MoveToTray(trayNoneFill, false, () => { });
    }

#endif
}

public enum ScrewState
{
    OnShape = 0,
    OnTray = 1,
    OnBox = 3,
    OnReviveBox = 4
}