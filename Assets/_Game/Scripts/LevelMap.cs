using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class LevelMap : ObjectIdentifier
{
    [SerializeField] private LevelMapType levelMapType;
    [SerializeField] private LevelDifficulty levelDifficulty;
    [SerializeField] private bool IsUsingSAMesh;

    [SerializeField] private EOLBase eol;
    public EOLBase EOL => eol;
    [SerializeField] private int levelId;
    [Header("Level Set Up")]
    [SerializeField] private PhysicsMaterial physicMaterial;
    [SerializeField] private Material matTrans;
    [SerializeField] private Material matWhite;
    [SerializeField] private Material matWarning;
    [SerializeField] private Material matBlink;

    [SerializeField] private GameObject hole;

    [SerializeField] float forceStrength = 5f;
    [SerializeField] int amountColor;
    [SerializeField] int amountLayerCustomsMap;
    [SerializeField] private List<LevelData> data;
    [SerializeField] private List<ScrewColor> lstLevelBoxColor;
    [SerializeField] private int countCheck = 0;

    [Header("Shape")]
    [SerializeField] private ShapeColor shapeColor = ShapeColor.Normal;
    [SerializeField] private LevelMapSize levelMapSize = LevelMapSize.Object;
    [SerializeField] private bool isTransparentLevel = false;
    [Header("Runtime Data")]
    [SerializeField] private List<Shape> lstShape = new List<Shape>();
    [SerializeField] private List<Screw> lstScrew = new List<Screw>();
    [SerializeField] private List<Screw> lstScrewBlocked = new List<Screw>();
    [SerializeField] private List<Screw> lstScrewCovered = new List<Screw>();
    [SerializeField] private List<Screw> lstScrewAvailable = new List<Screw>();
    [SerializeField] private List<Screw> lstScrewUsed = new List<Screw>();
    [SerializeField] private TextAsset levelMapDataJson;
    [HideInInspector][SerializeField] private List<Screw> lstScrewLayer = new List<Screw>();
    public LevelMapSize LevelMapSize { get => levelMapSize; set => levelMapSize = value; }

    [Header("Obstacle")]
    [SerializeField] private List<LinkObstacle> lstLinkObstacle = new List<LinkObstacle>();


    public List<Shape> LstShape { get => lstShape; }
    public List<Screw> LstScrew { get => lstScrew; }
    public List<ScrewColor> LstLevelBoxColor { get => lstLevelBoxColor; }
    public LevelMapType LevelMapType { get => levelMapType; }
    public int LevelId { get => levelId; set => levelId = value; }
    public LevelDifficulty LevelDifficulty { get => levelDifficulty; }
    public List<LinkObstacle> LstLinkObstacle { get => lstLinkObstacle; }
#if UNITY_EDITOR
    public Material MatTrans { get => matTrans; set => matTrans = value; }
    public Material MatWhite { get => matWhite; set => matWhite = value; }
#endif
    [SerializeField] private Vector3 defaultRotation;
    [SerializeField] private List<ScrewBlockedData> lstBlockedData = new List<ScrewBlockedData>();
    public List<ScrewBlockedData> LstBlockedData { get => lstBlockedData; set => lstBlockedData = value; }
    public override string UniqueId => string.IsNullOrEmpty(uniqueId) ? name : uniqueId;
    public TextAsset LevelMapDataJson { get => levelMapDataJson; set => levelMapDataJson = value; }

    private List<ScrewColor> origins;

    [SerializeField] private Vector3 startPostion;
    [SerializeField] private Vector3 startRotation;

    public Vector3 StartPostion => startPostion;
    public Vector3 StartRotation => startRotation;
    public List<Screw> LstScrewAvailable { get => lstScrewAvailable; }
    public bool IsTransparentLevel { get => isTransparentLevel; }

    [SerializeField] private Vector2 valueMinMaxZoom = new Vector2(0.5f, 2f);
    public Vector2 ValueMinMaxZoom { get => valueMinMaxZoom; }

    //private void Start()
    //{
    //    InitUI();
    //}

    public int TotalScrew = 0;
    private string levelName;

    public string LevelName => levelName;

    private void Start()
    {
        SetShapesIndex();
        TotalScrew = GeTotalScrewAmount();
    }

    public void ResetScrews()
    {
        lstScrew.Clear();
    }
    public void AddScrew(Screw screw)
    {
        lstScrew.Add(screw);
    }
    public void HideBeforeShow()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].Hide();

        }
        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstScrew[i].Hide();

        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].Hide();

        }
    }
    public async UniTask ShowLevelMap()
    {
        var lstTaskShape = new List<UniTask>();
        var lstTaskScrew = new List<UniTask>();
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstTaskShape.Add(lstShape[i].Show(0f));
            // await UniTask.Delay(10);
        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstTaskShape.Add(lstLinkObstacle[i].Show(0f));
            //  await UniTask.Delay(10);
        }
        await UniTask.WhenAll(lstTaskShape);
    }
    public async UniTask ShowLevelMapScrew()
    {
        var lstTaskScrew = new List<UniTask>();

        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstTaskScrew.Add(lstScrew[i].Show(0f));
            /*            if (i % 3 == 0)
                            await UniTask.Delay(15);*/
        }

        await UniTask.WhenAll(lstTaskScrew);
    }

    public void Init()
    {
        //ShuffleList(lstScrew);
        //ShuffleList(lstLevelBoxColor);
        SetUpDataColorAsync();
        defaultRotation = transform.eulerAngles;
        //ShuffleColorBox();
        bool isColorMode = Db.storage.IS_COLOR_MODE;
        origins = new List<ScrewColor>(LstLevelBoxColor);

        int indexScrew = 0;
        for (int i = 0; i < lstLevelBoxColor.Count; i++)
        {
            for (int j = indexScrew; j < indexScrew + 3; j++)
            {
                lstScrew[j].Init(lstLevelBoxColor[i]);
            }
            indexScrew += 3;
        }

        for (int i = 0; i < LstShape.Count; i++)
        {
            lstShape[i].Init(this);
        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].Init(this);
        }
        ExpBar.Instance.SetExpLevel(lstLevelBoxColor.Count);
        ChangeColorShape(isColorMode);
    }

    public void ChangeColorShape(bool isColorMode)
    {
        shapeColor = !isColorMode ? ShapeColor.White : ShapeColor.Normal;
        // var material = isColorMode ? matNor : matWhite;

        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].ChangeMaterial(isColorMode);
            lstShape[i].OnChangeMaterial(false);
        }

    }
    public void SetDataColor(List<ScrewColor> lstColors)
    {
        for (int i = 0; i < lstColors.Count; i++)
        {
            lstScrew[i].Init(lstColors[i]);
        }
    }

    #region SetUpMap
    public void GetScrewOnLinkObstacle()
    {
        lstLinkObstacle.Clear();
        Transform[] allChildren = GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {

            if (child.name.ToLower().Contains("link"))
            {
                if (child.GetComponent<LinkObstacle>() == null)
                {
                    child.gameObject.AddComponent<LinkObstacle>();
                }

                var linkObstacle = child.GetComponent<LinkObstacle>();
                for (int i = 0; i < linkObstacle.LstScrew.Count; i++)
                {
                    linkObstacle.LstScrew[i].LstLinkObstacle.Clear();
                }
                linkObstacle.SetUp(physicMaterial, forceStrength, matTrans, null, hole);
                lstLinkObstacle.Add(linkObstacle);
            }
        }

        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].ScrewObstacleToShape();
        }
    }
    public void GetAllScrewOnCustomsMap()
    {
#if UNITY_EDITOR
        matWhite = Database.MaterialWhite;
        physicMaterial = Database.PhysicMaterial;
        matBlink = Database.OverlayBlink;
        hole = Database.Hole;
#endif
        lstShape.Clear();
        lstScrew.Clear();
        Transform[] allChildren = GetComponentsInChildren<Transform>(true);

        int indexCube = 1;

        foreach (Transform child in allChildren)
        {
            if (child.gameObject.GetComponent<MeshFilter>() != null)
            {
                if (child.GetComponent<Shape>() == null && child.GetComponent<Screw>() == null)
                {
                    var childofChild = child.transform.GetComponentsInChildren<Transform>();
                    bool hasScrew = false;
                    foreach (var childChild in childofChild)
                    {
                        if (childChild.GetComponent<Screw>() != null)
                        {
                            hasScrew = true;
                            break;
                        }
                    }

                    if (hasScrew)
                    {
                        child.name = $"Cube_L{indexCube++}";
                        var shape = child.gameObject.AddComponent<Shape>();
                        shape.SetUp(physicMaterial, forceStrength, matTrans, null, hole);
                        lstShape.Add(shape);
                        lstScrewLayer.AddRange(shape.LstScrew);
                    }
                }
            }
        }

        ShuffleList(lstScrewLayer);
        lstScrew.AddRange(lstScrewLayer);
        lstScrewLayer.Clear();

        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains("screws") && child.GetComponent<Screw>() != null)
            {
                if (!child.parent.gameObject.name.Contains("Cube"))
                {
                    Debug.LogError("❌ Screw is not under a Shape object. Please check the map setup.", child);
                }
            }
        }

        EnableShapeConvex();

#if UNITY_EDITOR
        GenerateAllId();
#endif
        if (lstScrew.Count % 3 != 0)
        {
            Debug.LogError($"❌ The number of screws ({lstScrew.Count}) is not a multiple of 3, need more {3 - (lstScrew.Count % 3)} Please check the map setup.", this);
            return;
        }
    }

    [ContextMenu("SetMaterialShape")]
    public void SetMaterialShape()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            // lstShape[i].SetUp(physicMaterial, forceStrength, matTrans, matNor, hole);
        }
    }
    [ContextMenu("SetUpScrewFromObstacle")]
    public void SetUpScrewFromObstacle()
    {
        for (int j = 0; j < lstShape.Count; j++)
        {
            // lstShape[j].SetUp(physicMaterial, forceStrength, matTrans, matNor, hole);
            lstScrewLayer.AddRange(lstShape[j].LstScrew);
        }
    }

    public async void SetUpDataColorAsync()
    {
        data = new List<LevelData>();
        bool isEasyMode = PlayerPrefs.GetInt("EasyMode", 0) == 1;
        if (isEasyMode)
        {
            amountColor = 2;
        }
        data = GenerateLevelData(lstScrew.Count, amountColor);
        lstLevelBoxColor = new List<ScrewColor>();

        foreach (var levelData in data)
        {
            int repetitions = levelData.amount / 3;
            for (int i = 0; i < repetitions; i++)
            {
                lstLevelBoxColor.Add(levelData.screwColor);
            }
        }
        await Task.Delay(500);

        ShuffleColorBox();
    }

#if UNITY_EDITOR

    public bool stopTest = false;

    public async void TestLevel()
    {
        EditorLogger.Log($">>>>> TestLevel: {name} - CountScrew: {lstScrew.Count} - CountShape: {lstShape.Count}");

        countCheck = 0;
        stopTest = false;

        while (countCheck++ < 500)
        {
            if (Application.isPlaying || lstScrew == null || lstScrew.Count == 0)
            {
                return;
            }

            for (int i = 0; i < lstScrew.Count; i++)
            {
                if (lstScrew[i] != null && !lstScrewLayer.Contains(lstScrew[i]))
                {
                    var isDetect = await lstScrew[i].CheckDetectShape(true);

                    if (!isDetect)
                    {
                        lstScrewLayer.Add(lstScrew[i]);
                        lstScrew[i].gameObject.SetActive(false);
                    }
                }
            }

            await UniTask.Delay(500);

            bool isDone = true;

            for (int i = 0; i < lstShape.Count; i++)
            {
                if (lstShape[i].IsCompareAllScrewWithFirstScrewLayer(lstScrewLayer)
                 && lstShape[i].gameObject.activeSelf)
                {
                    lstShape[i].gameObject.SetActive(false);
                }

                if (lstShape[i].gameObject.activeSelf)
                {
                    isDone = false;
                }
            }

            if (stopTest)
            {
                for (int i = 0; i < lstScrewLayer.Count; i++)
                {
                    lstScrewLayer[i].gameObject.SetActive(false);
                }

                lstScrewLayer.Clear();

                return;
            }

            if (isDone)
            {
                for (int i = 0; i < lstShape.Count; i++)
                {
                    lstShape[i].gameObject.SetActive(true);
                }

                for (int i = 0; i < lstScrew.Count; i++)
                {
                    lstScrew[i].gameObject.SetActive(true);
                }

                lstScrewLayer.Clear();

                EditorLogger.Log(">>>>> Test SUCCESS!!!");
                UnityEditor.EditorUtility.DisplayDialog("Info", ">>>>> Test SUCCESS!!!", "OK");
                return;
            }
        }

        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstScrew[i].gameObject.SetActive(true);
        }

        lstScrewLayer.Clear();

        EditorLogger.LogError(">>>>> Test FAILED!!!");
        UnityEditor.EditorUtility.DisplayDialog("Info", ">>>>> Test FAILED!!!", "OK");
    }

#endif

    [ContextMenu("Enable Convex")]
    public void EnableShapeConvex()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            var collider = lstShape[i].GetComponent<MeshCollider>();
            if (collider != null)
            {
                collider.convex = true;
            }
        }
    }

    [ContextMenu("Disable Capsule Collider")]
    public void RemoveCapsuleCollider()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            for (int j = 0; j < lstShape[i].LstScrew.Count; j++)
            {
                lstShape[i].LstScrew[j].gameObject.GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }


    [ContextMenu("ShuffleColorBox")]
    public void ShuffleColorBox()
    {
        ShuffleListWithRule(lstLevelBoxColor);

    }

    [ContextMenu("Auto Set Level Id From Name")]
    public void AutoSetLevelIdFromName()
    {
        if (name.StartsWith("Level_"))
        {
            string num = name.Substring("Level_".Length);
            if (int.TryParse(num, out int result))
            {
                LevelId = result;
                Debug.Log($"✅ Set LevelId = {result} for {name}", this);
            }
            else
            {
                Debug.LogWarning($"❌ Failed to parse LevelId from {name}", this);
            }
        }
        else
        {
            Debug.LogWarning($"❌ Name does not start with 'Level_': {name}", this);
        }
    }


    public void ShuffleListWithRule(List<ScrewColor> lstLevelBoxColor)
    {
        if (lstLevelBoxColor == null || lstLevelBoxColor.Count < 3)
        {
            return;
        }

        //Reload

        if (LevelController.Instance != null && LevelController.Instance.IsReload)
        {
            EditorLogger.Log(">>>> Reload Level Map - Color Box");
            LevelMapData mapData = Deserialize<LevelMapData>();

            if (mapData != null && mapData.Origins != null && mapData.Origins.Count == lstLevelBoxColor.Count)
            {
                lstLevelBoxColor.Clear();
                lstLevelBoxColor.AddRange(mapData.Origins);
                return;
            }
        }

        List<ScrewColor> result = new List<ScrewColor>();
        List<ScrewColor> remainingColors = new List<ScrewColor>(lstLevelBoxColor);
        System.Random random = new System.Random();

        while (remainingColors.Count > 0)
        {
            List<ScrewColor> group = new List<ScrewColor>();
            for (int i = 0; i < 3 && remainingColors.Count > 0; i++)
            {
                List<ScrewColor> validColors = group.Count > 0
                    ? remainingColors.FindAll(color => color != group[group.Count - 1])
                    : new List<ScrewColor>(remainingColors);

                if (validColors.Count == 0) validColors = new List<ScrewColor>(remainingColors);

                ScrewColor selectedColor = validColors[random.Next(validColors.Count)];
                group.Add(selectedColor);
                remainingColors.Remove(selectedColor);
            }

            result.AddRange(group);
        }

        lstLevelBoxColor.Clear();
        lstLevelBoxColor.AddRange(result);

    }
    private List<LevelData> GenerateLevelData(int totalAmount, int colorCount)
    {
        var availableColors = new List<ScrewColor>() { ScrewColor.Red, ScrewColor.Blue, ScrewColor.Yellow, ScrewColor.Green, ScrewColor.Brown, ScrewColor.Pink, ScrewColor.Sky,ScrewColor.Orange,ScrewColor.Be,ScrewColor.Purple };

      /*  List<ScrewColor> availableColors = Enum.GetValues(typeof(ScrewColor))
            .Cast<ScrewColor>()
            .Where(color => color != ScrewColor.None)
            .ToList();*/


        List<ScrewColor> selectedColors = new List<ScrewColor>();
        System.Random random = new System.Random();
        for (int i = 0; i < colorCount; i++)
        {
            selectedColors.Add(availableColors[i]);
        }
        /* while (selectedColors.Count < colorCount)
         {
             int index = 0;
             var randomColor = availableColors[random.Next(availableColors.Count)];
             if (!selectedColors.Contains(randomColor))
             {
                 selectedColors.Add(randomColor);
             }
         }*/

        List<int> amounts = DistributeAmountEvenly(totalAmount, colorCount);

        List<LevelData> levelDataList = new List<LevelData>();
        for (int i = 0; i < colorCount; i++)
        {
            levelDataList.Add(new LevelData
            {
                screwColor = selectedColors[i],
                amount = amounts[i]
            });
        }

        return levelDataList;
    }

    private List<int> DistributeAmountEvenly(int totalAmount, int count)
    {
        List<int> amounts = new List<int>();
        int remainingAmount = totalAmount;

        for (int i = 0; i < count; i++)
        {
            int maxAmount = remainingAmount / (count - i); // Chia đều số còn lại
            int amount = (maxAmount / 3) * 3; // Đảm bảo chia hết cho 3
            amounts.Add(amount);
            remainingAmount -= amount;
        }

        return amounts;
    }

    #endregion



    public void AddForceToShape(float horizontal)
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            if (lstShape[i].LstScrew.Count == 0)
            {
                lstShape[i].ApplyForceToShape(horizontal);
            }
        }
    }
    public void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public List<T> GetEnumList<T>() where T : Enum
    {
        return new List<T>((T[])Enum.GetValues(typeof(T)));
    }

    public static int? FindLayerIndexFromParents(Transform start)
    {
        Transform current = start;

        while (current != null)
        {
            if (current.name.StartsWith("layer ", System.StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = current.name.Split(' ');
                if (parts.Length == 2 && int.TryParse(parts[1], out int layerIndex))
                {
                    return layerIndex;
                }
            }

            if (current.name.StartsWith("layer_", System.StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = current.name.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int layerIndex))
                {
                    return layerIndex;
                }
            }

            current = current.parent;
        }

        return null; // Not found
    }
    public void SetLevelMapFocus()
    {
        transform.eulerAngles = defaultRotation;
    }

#if UNITY_EDITOR

    private LevelMapDatabase _data;
    private LevelMapDatabase Database
    {
        get
        {
            if (_data == null)
            {
                _data = Resources.Load<LevelMapDatabase>(Define.CONFIG_LEVEL_MAP);
            }

            return _data;
        }
    }


    public void UpdateShape()
    {
        bool isRandom = false;

        matTrans = Database.ConfigModels[0].material;
        // matNor = Database.ConfigModels[0].material;

        for (int i = 0; i < lstShape.Count; i++)
        {
            UpdateModelShape(lstShape[i], !isRandom ? Vector3.zero : new Vector3(90, 0, 0), true);
            isRandom = !isRandom;
        }

        EditorLogger.Log("Update Share DONE");
    }

    public void UpdateLinkObstacle()
    {
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            UpdateModelShape(lstLinkObstacle[i], Vector3.zero);
        }

        EditorLogger.Log("Update Link Obstacle DONE");
    }

    private void UpdateModelShape(Shape shape, Vector3 rotation, bool isShape = false)
    {
        MeshFilter meshFilter = shape.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = shape.GetComponent<MeshRenderer>();
        MapModel mapModel = Database.GetMapModelByMeshName(meshFilter.sharedMesh.name);

        if (mapModel == null)
        {
            EditorLogger.LogError($"Update Failed {shape.name} - Mesh Name {meshFilter.sharedMesh.name}");
            return;
        }

        // Set material
        //  shape.SetMatNor(mapModel.material);
        // shape.SetMaterialTrans(mapModel.material);

        //Remove mesh
        DestroyImmediate(meshFilter);
        DestroyImmediate(meshRenderer);

        Transform hole = shape.transform.GetChild(0);

        GameObject childInstance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(mapModel.prefab, shape.transform);

        childInstance.transform.localPosition = Vector3.zero;
        childInstance.transform.SetSiblingIndex(0);

        if (!isShape)
        {
            Quaternion q = hole.localRotation;
            Quaternion q1 = Quaternion.Euler(new Vector3(90, 0, 0));
            Quaternion q2 = Quaternion.Euler(new Vector3(-90, 0, 0));
            Quaternion q3 = Quaternion.Euler(new Vector3(0, 0, 0));
            Quaternion q4 = Quaternion.Euler(new Vector3(-180, 0, 0));
            Quaternion q5 = Quaternion.Euler(new Vector3(0, 0, -90));

            if (Quaternion.Angle(q, q1) < 0.01f)
            {
                childInstance.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            if (Quaternion.Angle(q, q2) < 0.01f)

            {
                childInstance.transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
            else
            if (Quaternion.Angle(q, q2) < 0.01f)

            {
                childInstance.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else
            if (Quaternion.Angle(q, q3) < 0.01f)

            {
                childInstance.transform.localRotation = Quaternion.Euler(0, 0, 90);
            }
            else
            if (Quaternion.Angle(q, q4) < 0.01f)
            {
                childInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }


        shape.SetMeshRenderer(childInstance.GetComponent<MeshRenderer>());

        if (isShape)
        {
            UpdateMeshColiderMeshShape(shape, mapModel);
        }
        else
        {
            UpdateMeshColiderMeshLink(shape, mapModel);
        }

        EditorLogger.Log($"Update {shape.name} - Model {mapModel.modelType}");
    }

    private void UpdateMeshColiderMeshShape(Shape shape, MapModel mapModel)
    {
        MeshFilter meshFilter = shape.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            MeshCollider collider = shape.GetComponent<MeshCollider>();
            MeshFilter meshFilterNew = shape.transform.GetChild(0).GetComponent<MeshFilter>();

            if (meshFilterNew != null)
            {
                collider.sharedMesh = meshFilterNew.sharedMesh;
                collider.material = physicMaterial;
            }
        }
        else
        {
            EditorLogger.LogError($"MeshFilter Null " + shape.name);
        }
    }

    private void UpdateMeshColiderMeshLink(Shape link, MapModel mapModel)
    {
        MeshFilter meshFilter = link.GetComponent<MeshFilter>();

        if (meshFilter == null)
        {
            MeshCollider collider = link.GetComponent<MeshCollider>();
            MeshFilter meshFilterNew = link.transform.GetChild(0).GetComponent<MeshFilter>();

            if (meshFilterNew != null)
            {
                DestroyImmediate(collider);
                MeshCollider meshCollider = meshFilterNew.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshFilterNew.sharedMesh;
                meshCollider.convex = true;
                meshCollider.material = physicMaterial;

                link.SetMeshCollider(meshCollider);
                //link.SetMatNor(mapModel.material);
                // link.SetMaterialTrans(mapModel.material);
            }
        }
        else
        {
            EditorLogger.LogError($"MeshFilter Null " + link.name);
        }
    }

    public void GenerateAllId()
    {
        for (int i = 0; i < LstScrew.Count; i++)
        {
            LstScrew[i].InitData();
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(LstScrew[i]);
        }

        for (int i = 0; i < LstShape.Count; i++)
        {
            LstShape[i].InitData();
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(LstShape[i]);
        }

        for (int i = 0; i < LstLinkObstacle.Count; i++)
        {
            LstLinkObstacle[i].InitData();
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(LstLinkObstacle[i]);
        }

        InitData();
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }

#endif

    public async UniTask SetLevelMapFocusAsync()
    {
        transform.DOKill();
        // transform.DORotate(defaultRotation, 0.5f, RotateMode.Fast).SetEase(Ease.InOutSine);
        /*        await transform.DORotate(new Vector3(180, 0, 0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine);
                await transform.DORotate(new Vector3(180, 0, 0), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine);*/
        await transform.DORotate(new Vector3(0, 360, 0), 1, RotateMode.WorldAxisAdd).SetEase(Ease.InOutSine);
    }

    [SerializeField] private List<string> removedScrews = new List<string>();
    private List<string> filledScrews = new List<string>();
    private ScrewDataList screwDataList = new ScrewDataList();
    private ShapeDataList shapeDataList = new ShapeDataList();
    private LinkObstacleDataList linkObstacleDataList = new LinkObstacleDataList();
    private List<string> wrenchs = new List<string>();
    private List<string> wrenchCollecteds = new List<string>();

    public override void Serialize()
    {
        base.Serialize();

        LevelMapData mapData = new LevelMapData(UniqueId, origins, LstLevelBoxColor, removedScrews, filledScrews, screwDataList, shapeDataList, linkObstacleDataList, wrenchs, wrenchCollecteds);
        Serialize(mapData);
    }

    public override void InitializeFromSave()
    {
        base.InitializeFromSave();

        LevelMapData mapData = Deserialize<LevelMapData>();

        if (mapData == null || mapData.Progress == null || mapData.Origins == null)
        {
            return;
        }

        List<Shape> shapes = new List<Shape>();

        for (int i = 0; i < LstShape.Count; i++)
        {
            shapes.Add(lstShape[i]);
        }

        removedScrews = new List<string>(mapData.RemovedScrews);
        filledScrews = new List<string>(mapData.FilledScrews);

        screwDataList = mapData.screwDataList;
        shapeDataList = mapData.shapeDataList;
        linkObstacleDataList = mapData.linkObstacleDataList;
        wrenchs = mapData.Wrenchs;
        wrenchCollecteds = mapData.WrenchCollecteds;

        lstLevelBoxColor = new List<ScrewColor>(mapData.Progress);

        ScreenGamePlayUI.Instance.UpdateLevelProgressBar(mapData.RemovedScrews.Count);
         
        for (int i = 0; i < LstScrew.Count; i++)
        {
            if (mapData.RemovedScrews.Contains(LstScrew[i].UniqueId))
            {
                LstScrew[i].Shape.RemoveScrew(LstScrew[i]);
                LstScrew[i].Shape.CheckRemoveScrew();
                LstScrew[i].gameObject.SetActive(false);
                LstScrew[i].Shape.CheckHole();
                LstScrew[i].RemoveLink();
            }
        }

        for (int i = 0; i < LstScrew.Count; i++)
        {
            if (mapData.FilledScrews.Contains(LstScrew[i].UniqueId) && !(mapData.RemovedScrews.Contains(LstScrew[i].UniqueId)))
            {
                LstScrew[i].Shape.RemoveScrew(LstScrew[i]);
                LstScrew[i].Shape.CheckRemoveScrew();
                LstScrew[i].Shape.CheckHole();
                LstScrew[i].RemoveLink();
            }
        }

        for (int i = 0; i < shapes.Count; i++)
        {
            if (shapes[i] != null && !shapes[i].gameObject.activeSelf)
            {
                Screw[] screws = shapes[i].GetComponentsInChildren<Screw>(true);

                if (screws != null && screws.Length > 0)
                {
                    for (int index = 0; index < screws.Length; index++)
                    {
                        if (screws[index].gameObject.activeSelf && !mapData.RemovedScrews.Contains(LstScrew[i].UniqueId))
                        {
                            if (mapData.FilledScrews.Contains(screws[index].UniqueId))
                            {
                                Debug.Log("[OnScrewSelected] " + screws[index].name);
                                screws[index].OnScrewSelected();
                            }
                            else
                            {
                                Debug.Log("[AddScrewToSecretBox] " + screws[index].name);
                                LevelController.Instance.AddScrewToSecretBox(screws[index]).Forget();
                            }
                        }
                    }
                }
            }
        }

        if (wrenchs != null && wrenchs.Count > 0)
        {
            int wrenchAmount = wrenchCollecteds.Count;

            for (int i = 0; i < wrenchs.Count; i++)
            {
                if (!wrenchCollecteds.Contains(wrenchs[i]))
                {
                    Screw screw = ScrewManager.GetScrewById(wrenchs[i]);

                    if (screw != null)
                    {
                        screw.SpawnWrench();
                    }
                }
            }

            if (wrenchAmount > 0)
            {
                WrenchCollectionGamePlayController.Instance.Reload(wrenchAmount);
            }
        }
    }

    public void RemoveScrew(string id)
    {
        if (!string.IsNullOrEmpty(id) && !removedScrews.Contains(id))
        {
            removedScrews.Add(id);
        }
    }

    public void FilledScrew(string id)
    {
        if (!string.IsNullOrEmpty(id) && !filledScrews.Contains(id))
        {
            filledScrews.Add(id);
        }
    }
    public void SetShapesIndex()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].Index = i;
        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].Index = i;
        }
    }

    [ContextMenu("Test")]
    public void TestDetectAllScrew()
    {
        SetShapesIndex();
        lstBlockedData = new List<ScrewBlockedData>();
        lstScrewBlocked.Clear();
        lstScrewAvailable.Clear();
        bool hasBlock = false;
        bool hasBlockLink = false;
        bool hasCover = false;
        for (int i = 0; i < lstScrew.Count; i++)
        {
            var lstShapeBlock = lstScrew[i].GetListShapeIsDetected();
            var isBlock = lstShapeBlock.Count > 0;
            var lstLinkObstacle = lstScrew[i].GetListLinkObstacleIsDetected();
            var isBlockLink = lstLinkObstacle.Count > 0;
            var cover = lstScrew[i].GetListShapeCover();

            var lstShapeCover = cover.Item1;
            var lstLinkObstacleCover = cover.Item2;

            var isCover = lstShapeCover.Count > 0 || lstLinkObstacleCover.Count > 0;

            if (isBlock)
                hasBlock = true;
            if (isBlockLink)
                hasBlockLink = true;
            if (!isBlock && isCover)
                hasCover = true;
            if (isBlock || isCover)
            {
                lstScrewBlocked.Add(lstScrew[i]);
                var lstShapeBlockIndex = new List<int>();
                for (int j = 0; j < lstShapeBlock.Count; j++)
                {
                    lstShapeBlockIndex.Add(lstShapeBlock[j].Index);
                }
                var lstLinkBlockIndex = new List<int>();
                for (int j = 0; j < lstLinkObstacle.Count; j++)
                {
                    lstLinkBlockIndex.Add(lstLinkObstacle[j].Index);
                }
                var lstShapeCoverIndex = new List<int>();
                var lstLinkCoverIndex = new List<int>();
                if (!isBlock) // Bị block rồi thì không cần kiểm tra cover
                {

                    for (int j = 0; j < lstShapeCover.Count; j++)
                    {
                        lstShapeCoverIndex.Add(lstShapeCover[j].Index);
                    }
                    for (int j = 0; j < lstLinkObstacleCover.Count; j++)
                    {
                        lstLinkCoverIndex.Add(lstLinkObstacleCover[j].Index);
                    }
                }

                var data = new ScrewBlockedData
                {
                    index = i,
                    lstIndexShapeBlock = lstShapeBlockIndex,
                    lstIndexObstacle = lstLinkBlockIndex,
                    lstIndexShapeCover = lstShapeCoverIndex,
                    lstIndexObstacleCover = lstLinkCoverIndex
                };
                if (isBlock)
                    lstBlockedData.Add(data);
                else
                    lstScrewCovered.Add(lstScrew[i]);
            }
            else
            {
                lstScrewAvailable.Add(lstScrew[i]);
                lstBlockedData.Add(new ScrewBlockedData()
                {
                    index = i,
                });
            }

        }
        Debug.Log($"TestDetectAllScrew {LevelId}: TotalScrew={lstScrew.Count}, Blocked={hasBlock},LinkBlocked={hasBlockLink}, Covered={hasCover}");


    }
    public void FakeData()
    {
        lstBlockedData = new List<ScrewBlockedData>();
        for (int i = 0; i < lstScrew.Count; i++)
        {
            var data = new ScrewBlockedData
            {
                index = i,
            };
            lstBlockedData.Add(data);
        }
    }
    [ContextMenu("ConvertDataSCrew")]
    public void ConverDataScrew()
    {
#if UNITY_EDITOR
        //var screwDataWindow = LevelScrewBlockedDataWindow.ShowWindow();
#endif
    }
    [ContextMenu("Test Convert")]
    public void ConvertIndexToShape()
    {
        // return;
        lstScrewAvailable.Clear();
        lstScrewBlocked.Clear();
        lstScrewCovered.Clear();

        for (int i = 0; i < lstScrew.Count; i++)
        {
            var data = lstBlockedData[i];
            var screw = lstScrew[i];
            if (screw == null || screw.gameObject==null || !screw.gameObject.activeSelf)
                continue;

            var screwDataBlock = new ScrewDataBlock();
            screwDataBlock.id = screw.UniqueId;

            //// Shape
            var lstShapeBlock = new List<Shape>();
            var lstShapeCover = new List<Shape>();
            bool isFreeByShape = false;
            if (data.lstIndexShapeBlock.Count > 0)
            {
                if (lstScrewBlocked.Contains(screw) == false)
                    lstScrewBlocked.Add(screw);
            }
            else
            if (data.lstIndexShapeCover.Count > 0)
            {
                if (!lstScrewCovered.Contains(screw))
                    lstScrewCovered.Add(screw);
            }
            else
            {
                isFreeByShape = true;

            }
            for (int j = 0; j < data.lstIndexShapeBlock.Count; j++)
            {
                var index = data.lstIndexShapeBlock[j];
                var shape = this.lstShape[index];
                if (shape != null)
                {
                    lstShapeBlock.Add(shape);
                    shape.AddScrewBlock(screw);
                    screwDataBlock.lstShapeBlocked.Add(shape.UniqueId);
                }
            }
            for (int j = 0; j < data.lstIndexShapeCover.Count; j++)
            {
                var index = data.lstIndexShapeCover[j];
                var shape = this.lstShape[index];
                if (shape != null)
                {
                    lstShapeCover.Add(shape);
                    shape.AddScrewCover(screw);
                    screwDataBlock.lstShapeCover.Add(shape.UniqueId);
                }
            }

            var lstObstacleBlock = new List<LinkObstacle>();
            var lstObstacleCover = new List<LinkObstacle>();
            if (data.lstIndexObstacle.Count > 0)
            {
                if (lstScrewBlocked.Contains(screw) == false)
                    lstScrewBlocked.Add(screw);
            }
            else
            if (data.lstIndexObstacleCover.Count > 0)
            {
                if (!lstScrewCovered.Contains(screw))
                    lstScrewCovered.Add(screw);
            }
            else
            {
                if (isFreeByShape)
                    if (lstScrewAvailable.Contains(screw) == false)
                        lstScrewAvailable.Add(screw);
            }
            for (int j = 0; j < data.lstIndexObstacle.Count; j++)
            {
                var index = data.lstIndexObstacle[j];
                Debug.Log($"ID Obstacle: {i} {index} {j} {data.lstIndexObstacle.Count}");
                var link = this.lstLinkObstacle[index];
                if (link != null)
                {
                    lstObstacleBlock.Add(link);
                    link.AddScrewBlock(screw);
                    screwDataBlock.lstLinkBlocked.Add(link.UniqueId);
                }
            }
            for (int j = 0; j < data.lstIndexObstacleCover.Count; j++)
            {
                var index = data.lstIndexObstacleCover[j];
                var link = this.lstLinkObstacle[index];
                if (link != null)
                {
                    lstObstacleCover.Add(link);
                    link.AddScrewCover(screw);
                    screwDataBlock.lstLinkCover.Add(link.UniqueId);

                }
            }

            screw.AddListShapeBlocked(lstShapeBlock, lstObstacleBlock);
            screw.AddListShapeCovered(lstShapeCover, lstObstacleCover);

            // Link Obstacle
            screwDataList.lstScrewDataBlock.Add(screwDataBlock);
        }
    }
    public void SetDataColor()
    {
        ScrewBlockedRealTimeController.Instance.Init();

        for (int i = 0; i < lstScrewAvailable.Count; i++)
        {
            var screw = lstScrewAvailable[i];
            ScrewBlockedRealTimeController.Instance.AddBlockedScrew(screw);
        }
        for (int i = 0; i < lstScrew.Count; i++)
            ScrewBlockedRealTimeController.Instance.AddTotalScrew(lstScrew[i].ScrewColor);

        Debug.Log($"SetDataColor: TotalScrew={lstScrew.Count}, Blocked={lstScrewBlocked.Count}, Covered={lstScrewCovered.Count}, Available={lstScrewAvailable.Count}");
    }
#if UNITY_EDITOR

    public void ShowScrewAndShapeTarget(Screw screw)
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].OnChangeMaterial(true);
        }
        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstScrew[i].gameObject.SetActive(false);
        }
        //screws.EnableHighlight();
        screw.gameObject.SetActive(true);
        var lstShapeBlock = screw.GetListShapeIsDetected();
        for (int j = 0; j < lstShapeBlock.Count; j++)
        {
            lstShapeBlock[j].OnChangeMaterial(false);
        }
    }
    public void ShowLevelMapScrews(List<Screw> screws, bool showHighlight)
    {
        if (showHighlight)
            for (int i = 0; i < lstShape.Count; i++)
            {
                lstShape[i].OnChangeMaterial(true);
            }
        if (showHighlight)
            for (int i = 0; i < lstScrew.Count; i++)
            {
                if (lstScrew[i].State == ScrewState.OnShape)
                    lstScrew[i].gameObject.SetActive(false);
            }
        for (int i = 0; i < screws.Count; i++)
        {
            var screw = screws[i];
            screw.EnableHighlight();
            if (showHighlight && screw.State == ScrewState.OnShape)

            {

                screw.gameObject.SetActive(true);
                screw.Shape.OnChangeMaterial(false);
            }
        }

    }
    public void ResetTarget()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].OnChangeMaterial(false);
        }
        for (int i = 0; i < lstScrew.Count; i++)
        {
            if (lstScrew[i].State != ScrewState.OnShape)
                continue;
            lstScrew[i].gameObject.SetActive(true);
            lstScrew[i].DisableHighlight();
        }
    }

#endif
    public void MarkThisScrewAvailable(Screw screw)
    {
        if (screw.State != ScrewState.OnShape)
            return;
        Debug.Log($"MarkThisScrewAvailable: {screw.name}");
        if (screw.IsFree)
        {
            if (lstScrewCovered.Contains(screw))
                lstScrewCovered.Remove(screw);
            if (lstScrewCovered.Contains(screw))
                lstScrewCovered.Remove(screw);
            if (!lstScrewAvailable.Contains(screw))
                lstScrewAvailable.Add(screw);
            ScrewBlockedRealTimeController.Instance.AddBlockedScrew(screw);
        }
    }
    public void SetAmountColor(int amountColor)
    {
        this.amountColor = amountColor;
    }

    public int GetAmountColor()
    {
        return amountColor;
    }

    public void SetStartPosition(Vector3 pos)
    {
        startPostion = pos;
    }

    public void SetStartRotation(Vector3 rotation)
    {
        startRotation = rotation;
    }
    public void RemoveScrewAvailable(Screw screw)
    {
        ScrewBlockedRealTimeController.Instance.AddCurrScrewResolved(screw.ScrewColor);
        if (lstScrewAvailable.Contains(screw))
            lstScrewAvailable.Remove(screw);
        if (!lstScrewUsed.Contains(screw))
            lstScrewUsed.Add(screw);
    }

    public void SetLevelDifficulty(LevelDifficulty levelDifficulty)
    {
        this.levelDifficulty = levelDifficulty;
    }

    [ContextMenu("Reset Material")]
    public void ResetMaterial()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].ResetMaterial();
        }
#if Unity_Editor
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
    public void SetLevelMapSize(LevelMapSize levelMapSize)
    {
        LevelMapSize = levelMapSize;
    }
    public List<Shape> GetListShapeNearestCamera(int count)
    {
        var lstShape = new List<Shape>(this.lstShape);
        lstShape.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, Camera.main.transform.position);
            float distB = Vector3.Distance(b.transform.position, Camera.main.transform.position);
            return distA.CompareTo(distB);
        });

        return lstShape.Take(count).ToList();
    }
    public async UniTask AllShapeTransparent()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].OnSelectShape(false);
        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].OnSelectShape(false);
        }
    }
    public async UniTask AllShapeNormal()
    {
        for (int i = 0; i < lstShape.Count; i++)
        {
            lstShape[i].OnCancelSelectShape();
        }
        for (int i = 0; i < lstLinkObstacle.Count; i++)
        {
            lstLinkObstacle[i].OnCancelSelectShape();
        }
    }

    public int GetRemovedScrewAmount()
    {
        return removedScrews != null ? removedScrews.Count : 0;
    }

    public int GeTotalScrewAmount()
    {
        return LstScrew != null ? LstScrew.Count : 0;
    }



    public void SetIsTransparentLevel(bool value)
    {
        isTransparentLevel = value;
    }

    public void SetZoomSize(Vector2 value)
    {
        valueMinMaxZoom = value;
    }

    private int wrenchTospawn;

    public void SetWrench(int wrenchTospawn)
    {
        this.wrenchTospawn = wrenchTospawn;
    }

    public async void SpawnWrench()
    {
        if (WrenchCollectionService.IsActive() && !WrenchCollectionService.IsMaxLevel())
        {
            wrenchTospawn = WrenchCollectionService.WrenchAmountToSpawnInGamePlay(GeTotalScrewAmount());

            int maxScrew = 10 + wrenchTospawn;
            List<Screw> screws = new List<Screw>();

            for (int i = 0; i < LstScrew.Count; i++)
            {
                var isDetect = await LstScrew[i].CheckDetectShape(true);

                if (!isDetect)
                {
                    screws.Add(lstScrew[i]);
                }

                if (screws.Count >= maxScrew)
                {
                    break;
                }
            }

            if (screws.Count >= wrenchTospawn)
            {
                screws.Shuffle();

                for (int i = 0; i < wrenchTospawn; i++)
                {
                    screws[i].SpawnWrench();
                    wrenchs.Add(screws[i].UniqueId);
                }
            }
        }

    }

    public void CollectWrench(string wrench)
    {
        wrenchCollecteds.Add(wrench);
    }

    public void SetLevelName(string value)
    {
        levelName = value;
    }
    public void TurnOffAllScrew()
    {
        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstScrew[i].gameObject.SetActive(false);
        }
    }
}

[Serializable]
public class LevelData
{
    public ScrewColor screwColor;
    public int amount;
}

public enum ScrewColor
{
    None = 0,
    Blue = 1,
    Orange = 2,
    Brown = 3,// Sea
    Purple = 4,
    Be = 5, // Brown
    Sky = 6, // Cyan
    Pink = 7,
    Green = 8,
    Yellow = 9,
    Red = 10
}

public enum ShapeColor
{
    Normal = 0,
    White = 1,
}
public enum LevelMapType
{
    Normal = 0,
    LinkObstacle = 1
}

public enum LevelDifficulty
{
    Easy = 0,
    Normal = 1,
    Hard = 2
}

[Serializable]
public enum LevelMapSize
{
    BigMap = 0,
    Object = 1,
    Mid1 = 2,
}

[Serializable]
public enum QualityNames
{
    Low = 0,
    Medium = 1,
    High = 2,
    Insane = 3,
}