using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using Storage;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VHACD.Unity;

public class Shape : ObjectIdentifier
{
    [SerializeField] protected LevelMap levelMap;
    [SerializeField] protected List<Screw> lstScrew = new List<Screw>();
    [SerializeField] private List<Screw> lstScrewBlock;
    [SerializeField] private List<Screw> lstScrewCover;


    [SerializeField] protected List<Rigidbody> lstHoldRigi = new List<Rigidbody>();
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected HingeJoint joint;
    [SerializeField] private MeshCollider col;
    [SerializeField] private VHACD.Unity.ComplexCollider complexCol;
    [SerializeField] float forceStrength;
    [SerializeField] private Material matNor;
    [SerializeField] private List<Material> lstMatNor;
    [SerializeField] private Material matWhite;
    [SerializeField] private Material blinkOverlay;
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected GameObject hold;
    [SerializeField] protected Vector3 scaleDefault;
    [SerializeField] protected Screw lastScrew;
    [SerializeField] private int index;
    [SerializeField] private bool isColorMode = true;
    [SerializeField] private LayerMask layerMaskNor;
    [SerializeField] private LayerMask layerMaskHighlight;

    private MaterialPropertyBlock mpb;

    List<Coroutine> lstCoroutineTransparency;
    public void Hide()
    {
        scaleDefault = transform.localScale;
        transform.localScale = Vector3.zero;
    }
    public async UniTask Show(float Time)
    {
        if (Time == 0)
        {
            transform.localScale = scaleDefault;
            return;
        }
        await transform.DOScale(scaleDefault, Time).SetEase(Ease.OutBack);
    }
    void Awake()
    {
        mpb = new MaterialPropertyBlock();
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        }
        if (meshRenderer != null)
        {
            matNor = new Material(meshRenderer.material);
            lstMatNor = new List<Material>();
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                var mat = new Material(meshRenderer.materials[i]);
                lstMatNor.Add(mat);
            }
        }
        lstCoroutineTransparency = new List<Coroutine>();
    }

    public List<Screw> LstScrew { get => lstScrew; }
    public List<Rigidbody> LstHoldRigi { get => lstHoldRigi; }
    public int Index
    {
        get => index; set
        {
            index = value;
            gameObject.name = "Shape_" + index;
            // SetMaxHulls();
            for (int i = 0; i < lstScrew.Count; i++)
            {
                if (lstScrew[i] != null)
                    lstScrew[i].gameObject.name = "Screw_" + index + "_" + i + $"_{lstScrew[i].ScrewColor}";
                ;
            }
        }
    }
    [ContextMenu("Set MaxConvexHulls Runtime")]
    void SetMaxHulls()
    {
        if (complexCol == null) return;

        // Lấy field _parameters (private)
        var field = typeof(ComplexCollider).GetField("_parameters", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null)
        {

            // copy struct ra
            var param = field.GetValue(complexCol);

            /*   foreach (var f in param.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
               {
                   object value = f.GetValue(param);
                   Debug.Log($"[TEST] Field in Parameters: {f.Name} ({f.FieldType.Name}) = {value}");
               }*/


            // tìm field maxConvexHulls trong struct Parameters
            var paramType = param.GetType();
            var maxHullField = paramType.GetField("m_maxConvexHulls", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (maxHullField != null)
            {
                uint currentValue = (uint)maxHullField.GetValue(param);
                uint newValue = currentValue + 1;

                maxHullField.SetValue(param, newValue);
                field.SetValue(complexCol, param); // gán lại struct Parameters

                Debug.Log($"[TEST] m_maxConvexHulls set from {currentValue} → {newValue}");
            }
            else
            {
                Debug.LogError("[TEST] Không tìm thấy field maxConvexHulls trong Parameters");
            }
        }
        else
        {
            Debug.LogError("[TEST] Không tìm thấy field _parameters trong ComplexCollider");
        }
    }
    public void SetRB(Rigidbody r)
    {
        rb = r;
    }
    public void SetHingeJoint(HingeJoint j)
    {
        joint = j;
    }
    public void SetMeshCollider(MeshCollider c)
    {
        col = c;
    }
    public void SetMeshRenderer(MeshRenderer m)
    {
        meshRenderer = m;
    }
    public void Reset()
    {
        lstScrew.Clear();
        lstHoldRigi.Clear();
    }
    public void AddScrewAndHole(Screw screw, Rigidbody hole)
    {
        if (screw != null)
            lstScrew.Add(screw);
        if (hole != null)
            lstHoldRigi.Add(hole);
    }

    public virtual void Init(LevelMap levelMap)
    {
        var hinge = GetComponent<HingeJoint>();
        if (hinge != null)
        {
            // Debug.Log($"[{name}] Phát hiện có HingeJoint trên Shape, sẽ bị xoá. Vui lòng kiểm tra lại.", this);
            Destroy(hinge);

        }
        else
        {
            // Debug.Log($"[{name}] OK - Không có HingeJoint trên Shape.");
        }
        if (lstScrew.Count == 0)
        {
            EnableKinematic(false);
            if (lstHoldRigi.Count == 0)
            {
                this.gameObject.name = "ShapeError";
                return;
            }
            lstHoldRigi[0].isKinematic = false;

        }

        if (lstScrew.Count == 1)
        {
            /*joint.connectedBody = lstHoldRigi[0];
            joint.anchor = lstHoldRigi[0].transform.localPosition;
            joint.axis = lstHoldRigi[0].transform.up;*/
        }


        this.levelMap = levelMap;
        isColorMode = Db.storage.IS_COLOR_MODE;
        var matWhiteRoot = MaterialEffectController.Instance.MatWhite;
        matWhite = new Material(matWhiteRoot);
        blinkOverlay = MaterialEffectController.Instance.MatBlink;

        //matNor = new Material(matNor);
        OnChangeMaterial(false, new List<Material>() { matWhite });
        OnChangeMaterial(false, lstMatNor);
        SetCurrentMate();
        OnChangeMaterial(false);



    }

    public virtual void SetUp(PhysicsMaterial physicMaterial, float forceStrength, Material matTrans, Material matNor, GameObject hold, bool isForceSACollider = false)
    {
        lstScrew.Clear();
        lstHoldRigi.Clear();
        gameObject.layer = 7;


        this.forceStrength = forceStrength;
        this.hold = hold;

        MeshCollider[] colliders = gameObject.GetComponents<MeshCollider>();

        if (colliders != null && colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].material = physicMaterial;
            }
        }
        else if (gameObject.GetComponent<ComplexCollider>() != null)
        {
            complexCol = gameObject.GetComponent<ComplexCollider>();
            complexCol.Material = physicMaterial;
        }

        SetUpScrew();

        if (lstScrew.Count == 0 || lstHoldRigi.Count == 0)
        {
            Debug.LogError($"[{name}] Không có Screw nào được tìm thấy. Vui lòng kiểm tra lại.", this);
            this.gameObject.name = "ShapeError";
            return;
        }
    }


    [Button("TurnOnBlink")]
    public void TurnOnBlink()
    {
        // meshRenderer.materials = new Material[] { meshRenderer.materials[0], blinkOverlay };
        if (!isColorMode)
        {
            var mats = new List<Material>();
            for (int i = 0; i < lstMatNor.Count; i++)
            {
                mats.Add(matWhite);
            }
            mats.Add(blinkOverlay);
            meshRenderer.materials = mats.ToArray();
        }
        else
        {
            var mats = new List<Material>();
            for (int i = 0; i < lstMatNor.Count; i++)
            {
                mats.Add(lstMatNor[i]);
            }
            mats.Add(blinkOverlay);
            meshRenderer.materials = mats.ToArray();
        }
        int layerIndex = (int)Mathf.Log(layerMaskHighlight.value, 2);
        layerIndex = LayerMask.NameToLayer("screw_highlight");
        gameObject.layer = layerIndex;
        DOVirtual.DelayedCall(1f, () =>
        {
            TurnOffBlink();
        });
    }

    [Button("TurnOffBlink")]
    public void TurnOffBlink()
    {
        SetCurrentMate();
        int layerIndex = (int)Mathf.Log(layerMaskNor.value, 2);
        layerIndex = LayerMask.NameToLayer("shape");

        gameObject.layer = layerIndex;
    }
    private void SetCurrentMate()
    {
        var mat = isColorMode ? matNor : matWhite;
        if (!isColorMode)
        {
            var mats = new List<Material>();
            for (int i = 0; i < lstMatNor.Count; i++)
            {
                mats.Add(matWhite);
                //OnChangeMaterial(false, mats[mats.Count - 1]);
            }
            meshRenderer.materials = mats.ToArray();
        }
        else
        {
            var mats = new List<Material>();
            for (int i = 0; i < lstMatNor.Count; i++)
            {
                mats.Add(lstMatNor[i]);
            }
            meshRenderer.materials = mats.ToArray();
        }

    }

    [ContextMenu("SetUpScrew")]
    protected virtual void SetUpScrew()
    {
        Transform[] allChildren = GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains("screw"))
            {
                child.gameObject.layer = 6;
                if (child.GetComponent<Screw>() == null)
                {
                    child.gameObject.AddComponent<Screw>();
                }

                if (child.GetComponent<Rigidbody>() == null)
                {
                    child.gameObject.AddComponent<Rigidbody>();

                }

                rb = GetComponent<Rigidbody>();

                if (rb == null)
                {
                    rb = gameObject.AddComponent<Rigidbody>();
                }

                rb.isKinematic = true;

                // joint = GetComponent<HingeJoint>();
                DestroyImmediate(GetComponent<HingeJoint>());
                var screw = child.GetComponent<Screw>();
                screw.SetUpShapeAndColider(this);
                lstScrew.Add(screw);
            }
        }
        for (int i = 0; i < lstScrew.Count; i++)
        {
            var gobjHold = Instantiate(this.hold);
            gobjHold.transform.SetParent(transform);
            gobjHold.transform.position = lstScrew[i].transform.position;
            gobjHold.transform.rotation = lstScrew[i].transform.rotation;
            var rb = gobjHold.GetComponent<Rigidbody>();
            lstHoldRigi.Add(rb);
        }


    }

    public void ChangeMaterial(bool isColorMode)
    {
        if (meshRenderer == null)
        {
            return;
        }

        this.isColorMode = isColorMode;
        SetCurrentMate();
    }
    public void EnableKinematic(bool enable)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null. Cannot enable kinematic state.");
            this.gameObject.name = "ShapeError";
            return;
        }
        rb.isKinematic = enable;

    }

    public void ApplyForceToShape(float horizontalDelta)
    {
        Vector3 forceDirection = new Vector3(horizontalDelta > 0 ? 1 : -1, 0, 0).normalized;
        rb.AddForce(forceDirection * forceStrength, ForceMode.Impulse);
    }
    public void OnSelectShape(bool showGlass = true)
    {
        Debug.Log("Test input OnSelectShape");
        if (LevelController.Instance.CanUseTransparentMode == false)
            return;
        var time = 0.3f;

        if (countTimeShowTransparent != null)
        {
            StopCoroutine(countTimeShowTransparent);
        }
        try
        {

            countTimeShowTransparent = StartCoroutine(CountTimeShowTransparent(time, showGlass));
        }
        catch
        {
            Debug.LogWarning("OnSelectShape Exception");
        }
    }
    Coroutine countTimeShowTransparent;
    IEnumerator CountTimeShowTransparent(float time, bool showGlass)
    {
        Debug.Log("CountTimeShowTransparent");

        yield return new WaitForSeconds(time);
        if (showGlass)
            LevelController.Instance.PreBoosterGlass.ShowAtShape(this, time);
        TutorialPreBoosterGlass.Instance.OnHoldComplete();
        OnChangeMaterial(true, null, time);
        Debug.Log("CountTimeShowTransparent End");
    }
    public void OnCancelSelectShape()
    {
        if (LevelController.Instance.CanUseTransparentMode == false)
            return;
        LevelController.Instance.PreBoosterGlass.Cancel();

        if (countTimeShowTransparent != null)
        {
            StopCoroutine(countTimeShowTransparent);
            countTimeShowTransparent = null;
        }
        OnChangeMaterial(false);
    }
    [Button("OnChangeMaterial")]
    public void OnChangeMaterial(bool isTransparent, List<Material> materials = null, float time = 0.2f)

    {
        try
        {

            if (materials == null)
            {
                materials = new List<Material>();
                for (int i = 0; i < meshRenderer.sharedMaterials.Length; i++)
                {
                    materials.Add(meshRenderer.sharedMaterials[i]);
                }
            }
            //return;
            for (int i = 0; i < materials.Count; i++)
            {
                bool isTrans = SpecialMaterialDataHelper.Instance.IsInListExcept(materials[i]);
                if (isTrans)
                {
                    return;
                }
            }
            if (lstCoroutineTransparency == null)
            {
                lstCoroutineTransparency = new List<Coroutine>();
            }
            for (int i = 0; i < lstCoroutineTransparency.Count; i++)
            {
                if (lstCoroutineTransparency[i] != null)
                {
                    StopCoroutine(lstCoroutineTransparency[i]);
                }
            }
            for (int i = 0; i < materials.Count; i++)
            {
                var material = materials[i];
                //Debug.Log($"OnChangeMaterial: isTransparent={isTransparent}, material={material.name}, time={time}  isColorMode {isColorMode}");
                bool isTrans = SpecialMaterialDataHelper.Instance.IsInListExcept(material);
                if (!isTrans)
                {

                    if (isTransparent)
                        URPMaterialUtils.SetSurfaceDirect(material, URPMaterialUtils.SurfaceType.Transparent);
                    else
                        // Đặt lại thành Opaque
                        URPMaterialUtils.SetSurfaceDirect(material, URPMaterialUtils.SurfaceType.Opaque);
                    Debug.Log($"SetSurfaceDirect: {gameObject.name} material={material.name}, isTransparent={isTransparent}");



                    /*                if (coroutineTransparency != null)
                                    {
                                        StopCoroutine(coroutineTransparency);
                                        coroutineTransparency = null;
                                    }*/
                    float targetAlpha = isTransparent ? 0.4f : 1f;
                    var duration = isTransparent ? time : 0.1f;
                    if (gameObject.activeInHierarchy == true)

                        lstCoroutineTransparency.Add(StartCoroutine(DOTransparent(material, targetAlpha, duration)));
                }
                else
                {
                    Debug.Log($"Skip SetSurfaceDirect {gameObject.name} for material={material.name} as it's in exception list");
                    break;
                }

            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"OnChangeMaterial Exception: {ex}");
        }
    }
    private IEnumerator DOTransparent(Material material, float targetAlpha, float duration)
    {

        Color c = material.GetColor("_BaseColor");
        Debug.Log($"DOTransparent: targetAlpha={targetAlpha}, duration={duration}  isColorMode {isColorMode} - color {c}");
        if (!isColorMode)
            c = Color.white;
        float startAlpha = c.a;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // chọn easing
            float eased = t * t; // EaseInQuad (chậm → nhanh)
                                 // float eased = Mathf.SmoothStep(0f, 1f, t); // mượt 2 đầu
                                 // float eased = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); // EaseInSine
            eased = Mathf.SmoothStep(0f, 1f, t);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
            c.a = newAlpha;
            material.SetColor("_BaseColor", c);
            meshRenderer.SetPropertyBlock(mpb);

            yield return null;
        }

        // đảm bảo alpha chính xác cuối cùng
        c.a = targetAlpha;
        material.SetColor("_BaseColor", c);
        meshRenderer.SetPropertyBlock(mpb);

    }

    public void RemoveScrew(Screw screw)
    {
        var index = lstScrew.IndexOf(screw);
        if (lstHoldRigi.Count >= 1 && index >= 0 && lstHoldRigi.Count > index)
        {
            lstHoldRigi.Remove(lstHoldRigi[index]);

        }
        lstScrew.Remove(screw);
        if (lstScrew.Count == 0)
            lastScrew = screw;
        if (lstScrew.Count != 0) return;
    }


    [ContextMenu("CheckRemoveScrew")]
    public virtual void CheckRemoveScrew()
    {

        if (lstScrew.Count == 1)
        {
            //joint.connectedBody = lstHoldRigi[0];
            //joint.anchor = lstHoldRigi[0].transform.localPosition;
            Vector3 screwLocalUp = lstHoldRigi[0].transform.localRotation * Vector3.up;
            //joint.axis = screwLocalUp;
            if (levelMap.LevelMapType == LevelMapType.Normal)
            {
                //EnableKinematic(false);
            }

        }

        if (lstScrew.Count == 0)
        {
            OnThisShapeRemove();
            //col.isTrigger = false;
            //lstHoldRigi[0].isKinematic = false;
            levelMap.LstShape.Remove(this);
            //  transform.SetParent(null, true);

            /// 
            DestroyShape();

            var screwPos = lastScrew.transform.position + lastScrew.transform.up * 5;
            //col.isTrigger = true;
            ApplyReleaseForce(0.2f, 20, screwPos);

        }

    }
    protected void ApplyReleaseForce(float delay, float force, Vector3 targetPos)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            EnableKinematic(false);

            Vector3 dir;
            if (lastScrew != null)
            {

                Vector3 toScrew = targetPos - transform.position;

                // Bỏ XZ, chỉ lấy Y (vuông góc lên/xuống)
                dir = new Vector3(0, 0, 0);
                dir += new Vector3(toScrew.x, 0, 0);
                dir += new Vector3(0, toScrew.y, 0);
                dir += new Vector3(0, 0, toScrew.z);


                dir += Vector3.up * 0.5f; // thêm lực lên một chút để tránh rơi thẳng xuống
                Debug.Log(
                    $"[ApplyReleaseForce] Shape: {name}\n" +
                    $"- LastScrew: {lastScrew.name}\n" +
                    $"- Raw vector: {toScrew}\n" +
                    $"- Vertical only: {dir}"
                );

                Debug.DrawLine(transform.position, lastScrew.transform.position, Color.blue, 10f);
                Debug.DrawRay(transform.position, dir.normalized * force, Color.red, 10f);
            }
            else
            {
                dir = Vector3.up;
                Debug.Log($"[ApplyReleaseForce] {name} fallback straight up");
                Debug.DrawRay(transform.position, dir * force, Color.blue, 2f);
            }

            rb.AddForce(dir.normalized * force, ForceMode.Impulse);
        }).SetLink(gameObject);
    }


    public void RemoveAllScrew()
    {
        for (int i = lstScrew.Count - 1; i >= 0; i--)
        {
            var screw = lstScrew[i];
            if (screw == null)
                continue;
            LevelController.Instance.Level.RemoveScrewAvailable(screw);
            screw.CheckListLinkObtacle();
            var index = lstScrew.IndexOf(screw);
            if (lstHoldRigi.Count > 1 && index >= 0)
            {
                if (lstHoldRigi.Count > index && lstHoldRigi[index] != null)
                    lstHoldRigi.Remove(lstHoldRigi[index]);

            }

        }
        ScrewCounter.Instance.UpdateScrew(lstScrew.Count);
        LevelController.Instance.SetListScrewToSecretBox(lstScrew);
        LevelController.Instance.RemovScrewOnMap(lstScrew);
        LevelController.Instance.RemoveShape(this);
        lstScrew.Clear();
        transform.SetParent(null);
        gameObject.SetActive(false);
        OnThisShapeRemove();
    }

    public void RemoveAllScrewDontDestroy()
    {
        for (int i = lstScrew.Count - 1; i >= 0; i--)
        {
            var screw = lstScrew[i];
            if (screw == null)
                continue;
            LevelController.Instance.Level.RemoveScrewAvailable(screw);
            screw.CheckListLinkObtacle();
            var index = lstScrew.IndexOf(screw);
            if (lstHoldRigi.Count > 1 && index >= 0)
            {
                if (lstHoldRigi.Count > index && lstHoldRigi[index] != null)
                    lstHoldRigi.Remove(lstHoldRigi[index]);

            }

        }
        ScrewCounter.Instance.UpdateScrew(lstScrew.Count);
        LevelController.Instance.SetListScrewToSecretBox(lstScrew);
        LevelController.Instance.RemovScrewOnMap(lstScrew);
        OnThisShapeRemove();
        lstScrew.Clear();
        DOVirtual.DelayedCall(0, () =>
        {
            gameObject.SetActive(false);
        });

        //transform.SetParent(null);
        // gameObject.SetActive(false);
    }
    protected virtual void DestroyShape()
    {
        DOVirtual.DelayedCall(1, () =>
        {
            if (IngameData.GameMode == GameMode.Normal)
            {

                var par = PoolManager.Instance.Spawn(PoolKey.SOF_SHAPE_DESTROY, transform.position, transform.rotation);
                ParticleScaleByReference.Instance.SpawnScaledParticle(this.transform, par.transform);
            }
            else
            {
                var par = PoolManager.Instance.Spawn(PoolKey.SOF_SHAPE_DESTROY_GOLD, transform.position, transform.rotation);
                ParticleScaleByReference.Instance.SpawnScaledParticle(this.transform, par.transform, 4);
                int coin = Random.Range(3, 5);
                CoinCollector.Instance.Collect(transform, coin).Forget();
                VibrationController.Instance.Vibrate(VibrationType.Big);
                ShakeCamera.Instance.StartShake();

            }
            LevelController.Instance.RemoveShape(this);
            gameObject.SetActive(false);
        });
    }
    #region SetUp

    public bool IsCompareAllScrewWithFirstScrewLayer(List<Screw> lstScrewTemp)
    {

        var isCompare = AreAllElementsInList(lstScrew, lstScrewTemp);

        return isCompare;
    }

    public bool AreAllElementsInList<T>(List<T> listA, List<T> listB)
    {
        return listA.All(element => listB.Contains(element));
    }


    #endregion

    public void SetMatNor(Material material)
    {
        // matNor = material;
    }

    public void CheckHole()
    {
        if (lstHoldRigi != null && lstHoldRigi.Count == 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void AddScrewBlock(Screw screw)
    {
        if (lstScrewBlock == null)
        {
            lstScrewBlock = new List<Screw>();
        }
        lstScrewBlock.Add(screw);
    }
    public void AddScrewCover(Screw screw)
    {
        if (lstScrewCover == null)
        {
            lstScrewCover = new List<Screw>();
        }
        lstScrewCover.Add(screw);
    }
    protected void OnThisShapeRemove()
    {
        if (lstScrewBlock != null && lstScrewBlock.Count > 0)
        {
            for (int i = 0; i < lstScrewBlock.Count; i++)
            {
                if (lstScrewBlock[i] != null)
                {
                    lstScrewBlock[i].RemoveShapeBlock(this);
                }
            }
        }
        if (lstScrewCover != null && lstScrewCover.Count > 0)
        {
            for (int i = 0; i < lstScrewCover.Count; i++)
            {
                if (lstScrewCover[i] != null)
                {
                    lstScrewCover[i].RemoveShapeCover(this);
                }
            }
        }
    }

    [Button("ResetMaterial")]
    public void ResetMaterial()
    {
        matNor = meshRenderer.sharedMaterial;
    }
    public void SetUpHighlight(LayerMask layerHighlight, LayerMask layerNor)
    {
        this.layerMaskHighlight = layerHighlight;
        this.layerMaskNor = layerNor;
    }
}

