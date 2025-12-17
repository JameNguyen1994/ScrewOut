using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using System.Collections.Generic;
using UnityEngine;

public class BoxLevelBonusController : ObjectIdentifier
{
    [SerializeField] private List<Box> lstBoxOnLevel;
    [SerializeField] private List<Transform> lstPosBoxOnLevel;
    [SerializeField] private Transform maskPosA, maskPosB;
    [SerializeField] private List<ScrewColor> lstBoxColor = new List<ScrewColor>();


    private ScreenRatioDatabase _ratioDatabase;
    private ScreenRatioDatabase RatioDatabase
    {
        get
        {
            if (_ratioDatabase == null)
            {
                _ratioDatabase = Resources.Load<ScreenRatioDatabase>(Define.CONFIG_SCREEN_RATIO);
            }

            return _ratioDatabase;
        }
    }

    private void Awake()
    {
        ApplyAspectRatioSettings();

        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].Hide(0);
        }
    }

    public void Init(List<ScrewColor> lstScrewColor)
    {
        lstBoxColor = new List<ScrewColor>();
        for (int i = 0; i < lstScrewColor.Count; i++)
        {
            lstBoxColor.Add(lstScrewColor[i]);
        }
        CaculaterLstBoxPos();
    }
    public async UniTask InitBoxs()
    {
        await UniTask.Delay(500);
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            if (lstBoxColor.Count == 0) break;
            var nextColor = lstBoxColor[0];
            lstBoxColor.RemoveAt(0);
            lstBoxOnLevel[i].Init(BoxState.Unlock, nextColor);
        }
        await UniTask.Delay(300);
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].Show();
        }
    }
    public ScrewColor GetNextColor()
    {
        if (lstBoxColor.Count > 0)
        {
            var color = lstBoxColor[0];
            lstBoxColor.RemoveAt(0);
            return color;
        }
        return ScrewColor.None;
    }

    public Box GetBoxUnlockByColor(ScrewColor screwColor)
    {
        Debug.Log($"GetBoxUnlockByColor: {screwColor}");
        var lstBoxColorUnlock = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock && x.Color == screwColor);
        if (lstBoxColorUnlock.Count > 0)
        {
            Debug.Log($"Found BoxUnlockByColor: {screwColor} - Count: {lstBoxColorUnlock.Count}");
            var boxFill = lstBoxColorUnlock.Find(x => x.IsBoxFill());
            var boxNoneFill = lstBoxColorUnlock.Find(x => !x.IsBoxFill());
            if (boxFill != null)
            {
                return boxFill;
            }
            else
            {
                return boxNoneFill;
            }
        }
        return null;
    }
    public Box GetBoxToFill()
    {
        return lstBoxOnLevel.FindLast(x => x.BoxState == BoxState.Unlock && !x.IsBoxFull());
    }
    public List<ScrewColor> GetListColorNotMatch()
    {
        var lstColor = new List<ScrewColor>();
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            if (lstBoxOnLevel[i].BoxState == BoxState.Unlock && !lstColor.Contains(lstBoxOnLevel[i].Color))
                lstColor.Add(lstBoxOnLevel[i].Color);
        }
        return lstColor;
    }
    public Box GetBoxLock()
    {
        return lstBoxOnLevel.Find(x => x.BoxState == BoxState.Lock);

    }

    public void CaculaterLstBoxPos(bool isRunAnimation = false)
    {
        var lstTransform = new List<Transform>();
        for (int i = 0; i < lstPosBoxOnLevel.Count; i++)
        {
            var box = lstBoxOnLevel[i];
            if (box.gameObject.activeSelf)
            {
                lstTransform.Add(lstPosBoxOnLevel[i]);
            }
        }

        if (lstTransform.Count == 0) return;

        // Lấy 2 điểm A và B
        Vector3 posA = maskPosA.position;
        Vector3 posB = maskPosB.position;

        // Vector hướng từ A → B
        Vector3 dir = (posB - posA).normalized;
        float totalLength = Vector3.Distance(posA, posB);

        // Giả sử mỗi box có cùng khoảng cách (spacing) theo trục AB
        Vector3 center = (posA + posB) * 0.5f;
        float spacing = totalLength / (lstBoxOnLevel.Count - 1);


        float halfIndex = (lstTransform.Count - 1) * 0.5f;

        for (int i = 0; i < lstTransform.Count; i++)
        {
            float offset = (i - halfIndex) * spacing;           // đối xứng quanh 0
            Vector3 pos = center + dir * offset;

            if (isRunAnimation)
            {
                if (pos != lstTransform[i].position)
                {
                    lstTransform[i].DOMove(pos, 0.15f);
                }
            }
            else
            {
                lstTransform[i].position = pos;
            }
        }
    }
    public List<Box> GetAllUnlock()
    {
        return lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock);
    }

#if UNITY_EDITOR

    [EasyButtons.Button]
    private void TestApplyAspectRatioSettings()
    {
        ApplyAspectRatioSettings();
        CaculaterLstBoxPos();
    }

#endif

    public void ApplyAspectRatioSettings()
    {
        Vector2 screenResolution = GetScreenResolution();
        ApplyAspectRatioSettings(screenResolution.x, screenResolution.y);
    }

    private void ApplyAspectRatioSettings(float width, float height)
    {
        float currentRatio = RatioService.CalculateAspectRatio(width, height);
        const float tolerance = 0.01f;

        EditorLogger.Log($">>>Current ratio: {width}x{height} ~ {currentRatio}");

        foreach (var setting in RatioDatabase.configs)
        {
            if (Mathf.Abs(setting.Ratio - currentRatio) <= tolerance)
            {
                ApplyTransformSettings(setting);
                ApplyMaskSettings(setting);

                EditorLogger.Log($"\">>>Applied ratio: {setting.Width}x{setting.Height} - {setting.Ratio:F2} → Y={setting.PosY}, Scale={setting.Scale}, PosX={setting.PosX}");
                return;
            }
        }

        EditorLogger.LogWarning("\">>>No matching ratio found in settings!");
    }

    private void ApplyTransformSettings(ScreenRatioDatabase.RatioConfig setting)
    {
        Vector3 newPosition = transform.localPosition;
        newPosition.y = setting.PosY;

        transform.localPosition = newPosition;
        transform.localScale = Vector3.one * setting.Scale;
    }

    private void ApplyMaskSettings(ScreenRatioDatabase.RatioConfig setting)
    {
        Vector3 maskAPosition = maskPosA.localPosition;
        maskAPosition.x = setting.PosX;
        maskPosA.localPosition = maskAPosition;

        Vector3 maskBPosition = maskPosB.localPosition;
        maskBPosition.x = -setting.PosX;
        maskPosB.localPosition = maskBPosition;
    }

    private Vector2 GetScreenResolution()
    {
#if UNITY_EDITOR
        return UnityEditor.Handles.GetMainGameViewSize();
#else
        return new Vector2(Screen.width , Screen.height);
#endif
    }

    public override void Serialize()
    {
        base.Serialize();

        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].Serialize();
        }
    }

    public override void InitializeFromSave()
    {
        base.InitializeFromSave();

        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].InitializeFromSave();
        }
    }

    public override void ClearData()
    {
        base.ClearData();

        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].ClearData();
        }
    }

    public bool IsBoxWithColorUnlocked(ScrewColor color)
    {
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            if (lstBoxOnLevel[i].BoxState == BoxState.Unlock
             && lstBoxOnLevel[i].Color == color)
            {
                return true;
            }
        }

        return false;
    }

    public void HideAll()
    {
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].Hide(0.3f);
        }
    }
}