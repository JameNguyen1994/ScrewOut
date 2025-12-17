using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using System.Collections.Generic;
using UnityEngine;

public class BaseBox : ObjectIdentifier
{
    [SerializeField] private List<Box> lstBoxOnLevel;
    [SerializeField] private List<Transform> lstPosBoxOnLevel;
    [SerializeField] private Transform maskPosA, maskPosB;


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

    public void Init()
    {
        if (Db.storage.USER_INFO.level < 2)
        {
            lstBoxOnLevel[2].gameObject.SetActive(false);
            lstBoxOnLevel[3].gameObject.SetActive(false);

        }

        CaculaterLstBoxPos();
        //InitBoxs();
    }
    public async UniTask InitBoxs(bool isReload = false)
    {

        await UniTask.Delay(!isReload ? 500 : 0);
        LevelDifficultyManager.Instance.InitLevel(LevelController.Instance.Level);

        var lstBoxUnlock = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock);

        for (int i = 0; i < lstBoxUnlock.Count; i++)
        {
            lstBoxUnlock[i].Init(BoxState.Unlock);
        }

        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].Show();
        }
    }
    public int GetBoxUnlock()
    {
        var lstBoxColorUnlock = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock);
        return lstBoxColorUnlock.Count;
    }

    public void ForceDataWin()
    {
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            lstBoxOnLevel[i].ChangeState(BoxState.Lock);
        }
    }
    

    public List<ScrewColor> GetColorAllBoxUnlock()
    {
        var lstBoxColorUnlock = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock);
        List<ScrewColor> lstColor = new List<ScrewColor>();
        for (int i = 0; i < lstBoxColorUnlock.Count; i++)
        {
            lstColor.Add(lstBoxColorUnlock[i].Color);
        }
        return lstColor;

    }

    public bool HaveBoxFull()
    {
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            if (lstBoxOnLevel[i].IsBoxFull())
            {
                return true;
            }
        }

        return false;
    }

    public bool HaveBoxWaitingChangeColor()
    {
        for (int i = 0; i < lstBoxOnLevel.Count; i++)
        {
            if (lstBoxOnLevel[i].WaitingChangeToColor)
            {
                return true;
            }
        }

        return false;
    }

    public Box GetBoxUnlockByColor(ScrewColor screwColor)
    {
        var lstBoxColorUnlock = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock && x.Color == screwColor);
        if (lstBoxColorUnlock.Count > 0)
        {
            var boxFill = lstBoxColorUnlock.Find(x => x.IsBoxFill());

            if (boxFill != null)
            {
                return boxFill;
            }
            else
            {
                return lstBoxColorUnlock[0];
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
    public ScrewColor GetColorNextBoxLock()
    {
        var lstCurrBox = lstBoxOnLevel.FindAll(x => x.BoxState == BoxState.Unlock);
        var lstCurrColor = new List<ScrewColor>();
        for (int i = 0; i < lstCurrBox.Count; i++)
        {
            if (!lstCurrColor.Contains(lstCurrBox[i].Color))
                lstCurrColor.Add(lstCurrBox[i].Color);
        }

        var colorNextBox = BoxColorController.Instance.CheckDifficultyAndGetBox(lstCurrColor);
        return colorNextBox;
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