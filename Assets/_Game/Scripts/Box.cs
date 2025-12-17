using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using PS.Analytic;
using Storage;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Box : ObjectIdentifier
{
    [SerializeField] protected ScrewColor color;
    [SerializeField] protected BoxState boxState;
    [SerializeField] protected List<Tray> lstTray;
    [SerializeField] protected MeshFilter meshFilter;
    [SerializeField] protected MeshRenderer meshRenderer;
    [SerializeField] protected MeshRenderer meshRendererLid;
    [SerializeField] private GameObject btnLock;
    [SerializeField] protected MeshFilter meshFilterLid;
    [SerializeField] private Booster booster;
    [SerializeField] private ParticleSystem particleUnlock;
    [SerializeField] private GameObject fxCloseBox;
    [SerializeField] private GameObject gobjUnlockNew;
    [SerializeField] private GameObject gobjUnlockOld;
    [SerializeField] private KeyUnlockBox keyUnlockBox;
    [SerializeField] private Transform tfmUnlockPos;

    [SerializeField] bool waitingChangeToColor = false;

    public BoxState BoxState { get => boxState; }
    public ScrewColor Color { get => color; }

    protected static bool isAnEvenNumber = false;
    protected static int boxCount = 0;

    public int NeedCount => lstTray.FindAll(x => x.IsFill == false).Count;

    public bool WaitingChangeToColor { get => waitingChangeToColor; }
    public Transform TfmUnlockPos { get => tfmUnlockPos; }

    public override string UniqueId => SerializationService.GetIdByName(name);

    public List<Tray> LstTray => lstTray;

    private void Start()
    {
        // gobjUnlockNew.SetActive(false);
        gobjUnlockOld.SetActive(false);
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
            lstTray[i].Box = this;
            Debug.Log($"Tray {i} IsFill: {lstTray[i].IsFill}");
        }
    }
    [Button]

    public async UniTask Hide(float time)
    {
        transform.DOLocalMove(new Vector3(0, 0, -15), time);
        color = ScrewColor.None;
    }
    [Button]
    public async UniTask Show()
    {
        await transform.DOLocalMove(new Vector3(0, 0, 0), 0.3f);
        //gameObject.SetActive(true);
    }
    public void Init(BoxState boxState)
    {
        var color = LevelController.Instance.BaseBox.GetColorNextBoxLock();
        ScrewBlockedRealTimeController.Instance.RemoveBlockedScrew(color);

        Debug.Log($"InitUI Box State: {boxState} - Color: {color}");
        if (color != ScrewColor.None)
        {
            this.color = color;
            this.boxState = boxState;
            for (int i = 0; i < lstTray.Count; i++)
            {
                lstTray[i].InitNewTray();
            }
            meshFilter.mesh = DataScrewColor.Instance.GetBoxMeshByColor(color);
            meshFilterLid.mesh = DataScrewColor.Instance.GetLidMeshByColor(color);
            meshRenderer.material = DataScrewColor.Instance.GetMaterialByColor(color);
            meshRendererLid.material = DataScrewColor.Instance.GetMaterialByColor(color);
            var posYLid = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? 1.1f : 0.5f;

            meshFilterLid.transform.localPosition = new Vector3(0, posYLid, -40);
            if (boxState == BoxState.Lock)
            {
                if (Db.storage.USER_INFO.level == 1)
                    this.gameObject.SetActive(false);
                btnLock.SetActive(true);
                meshRenderer.enabled = false;
            }
            if (boxState == BoxState.Unlock)
            {
                btnLock.SetActive(false);
                meshRenderer.enabled = true;
            }
        }
        else
        {
            this.boxState = boxState;
            btnLock.SetActive(false);

        }
        gameObject.SetActive(true);

        LevelController.Instance.CheckNewBox(this);

    }
    public void Init(BoxState boxState, ScrewColor screwColor)
    {
        var color = screwColor;
        Debug.Log($"LevelBonus InitUI Box State: {boxState} - Color: {color}");
        this.color = color;
        this.boxState = boxState;
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
        }
        meshFilter.mesh = DataScrewColor.Instance.GetBoxMeshByColor(color);
        meshFilterLid.mesh = DataScrewColor.Instance.GetLidMeshByColor(color);
        meshRenderer.material = DataScrewColor.Instance.GetMaterialByColor(color);
        meshRendererLid.material = DataScrewColor.Instance.GetMaterialByColor(color);
        var posYLid = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? 1.1f : 0.5f;

        meshFilterLid.transform.localPosition = new Vector3(0, posYLid, -40);
        if (boxState == BoxState.Unlock)
        {
            meshRenderer.enabled = true;
        }
        gameObject.SetActive(true);
        //  LevelController.Instance.CheckNewBox(this);

    }
    public Tray GetTrayOnBoxNonFill()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill)
            {
                return lstTray[i];
            }
        }
        return null;
    }
    public List<Tray> GetAllTrayNoneFill()
    {
        List<Tray> lstTrayNoneFill = new List<Tray>();
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill)
            {
                lstTrayNoneFill.Add(lstTray[i]);
            }
        }
        return lstTrayNoneFill;
    }

    public bool IsBoxFill()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (lstTray[i].IsFill)
            {
                return true;
            }
        }
        return false;

    }
    public bool IsBoxFull()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill)
            {
                return false;
            }
        }
        return true;

    }
    public async UniTask ChangeState(BoxState state)
    {
        boxState = state;
        if (state == BoxState.Unlock)
        {
            //await keyUnlockBox.PlayAnimationKey();
            particleUnlock.Play();
            Init(BoxState.Unlock);

        }
    }

    public void AddScrew(Screw screw)
    {
        BoosterController.Instance.StopCountTimeToShowHighLight();
        transform.DOKill();
        var seq = DOTween.Sequence();
        transform.localScale = Vector3.one;
        seq.Append(transform.DOScaleY(transform.localScale.y - 0.2f, 0.125f));
        seq.Append(transform.DOScaleY(transform.localScale.y, 0.075f));
        //transform.DOShakeScale(0.2f, new Vector3(0, -0.2f, 0), 10, 90);
        seq.Play();
    }

    public void RemoveScrew()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            // Destroy(lstScrew[i].gameObject);
            lstTray[i].InitNewTray();
        }
    }
    public virtual async UniTask CheckFull()
    {

        if (boxState == BoxState.Move || boxState == BoxState.Lock) return;

        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill) return;
        }

        for (int i = 0; i < lstTray.Count; i++)
        {
            LevelController.Instance.Level.RemoveScrew(lstTray[i].GetScrewId());
        }

        ScreenGamePlayUI.Instance.UpdateLevelProgressBar(3);

        ChangeState(BoxState.Move);
        AudioController.Instance.PlaySound(SoundName.Merge);
        var delay = 50;
        int exp = GameAnalyticController.Instance.Remote().ExpCompleteBox;
        waitingChangeToColor = true;
        await UniTask.Delay(delay);
        await UniTask.WaitUntil(
            () =>
            {
                bool isAnimCompleted = true;
                for (int i = 0; i < lstTray.Count; i++)
                {
                    if (!lstTray[i].IsComletedAnim())
                    {
                        isAnimCompleted = false;
                        break;
                    }
                }
                return isAnimCompleted;
            });
        // old 0.3f
        meshFilterLid.transform.DOLocalJump(new Vector3(0, 0.4f, 0), 2, 1, 0.2f).OnComplete(() =>
        {
            transform.DOKill();
            fxCloseBox.SetActive(true);
            Sequence punchSequence = DOTween.Sequence();

            punchSequence.Append(transform.DOScale(new Vector3(1f, 0.6f, 1f), 0.18f)); // co lại
            punchSequence.Append(transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.14f)); // trở lại
            punchSequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.08f)); // trở lại
            punchSequence.Play();
            EditorLogger.Log("box count before increment: " + boxCount);
            boxCount++;
            EventDispatcher.Push(EventId.OnBoxCompleted, boxCount);

            Invoke(nameof(SpawnNewBox), 0.48f);
        });
    }

    protected virtual async UniTask SpawnNewBox()
    {
        transform.DOLocalMove(new Vector3(0, 0, -15), 0.3f).OnComplete(async () =>
            {
                RemoveScrew();
                //transform.localScale = Vector3.zero;
                //.localPosition = Vector3.zero;
                meshFilterLid.transform.localScale = Vector3.zero;
                AudioController.Instance.PlaySound(SoundName.ClearBox);

                var holeFill = LevelController.Instance.GetHoleFill();

                /*  GameAnalyticController.Instance.Tracking().TrackingBoxCompleted(Db.storage.USER_INFO.level,
                      holeFill);*/

                // if (ExpBar.Instance != null)
                // {
                //     var trkData = Db.storage.TRK_DATA.DeepClone();
                //     boxCount++;
                //
                //     if (boxCount >= 7)
                //     {
                //         boxCount = 1;
                //     }
                //
                //     switch (boxCount)
                //     {
                //         case 1:
                //             trkData.numX1OfLine++;
                //             break;
                //         case 2:
                //             trkData.numX2OfLine++;
                //             break;
                //         case 3:
                //             trkData.numX3OfLine++;
                //             break;
                //         case 4:
                //             trkData.numX4OfLine++;
                //             break;
                //         case 5:
                //             trkData.numX5OfLine++;
                //             break;
                //         case 6:
                //             trkData.numX6OfLine++;
                //             break;
                //     }
                //     Db.storage.TRK_DATA = trkData;
                // }
                //this.color = ScrewColor.None;
                var hasColor = !ScrewBlockedRealTimeController.Instance.IsFullAll();
                if (hasColor)
                {
                    Init(BoxState.Unlock);
                }
                else
                {
                    ChangeState(BoxState.Move);
                    Debug.Log("CheckWin");
                    gameObject.SetActive(false);
                    LevelController.Instance.AssignScrewsToBoxesWhenBoxOut();
                    LevelController.Instance.CheckWin();
                    return;
                }
                await transform.DOLocalMove(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutBack);

                transform.DOScale(Vector3.one, 0f).OnComplete(() =>
                // transform.DOScale(Vector3.one, 0.3f).OnComplete(() =>
                {
                    fxCloseBox.SetActive(false);
                    meshFilterLid.transform.localScale = Vector3.one;
                    LevelController.Instance.CheckNewBox(this);
                    waitingChangeToColor = false;
                });
            });
    }

    public void OnClickBox()
    {
        if (Db.storage.USER_INFO.level == 1)
            return;
        if (PopupController.Instance.PopupCount > 0)
            return;
        if (boxState == BoxState.Lock)
        {

            /*            if (GameConfig.OLD_VERSION)
                        {
                            AdsController.Instance.ShowRewardAds(RewardAdsPos.unlock_box, () =>
                            {
                                IngameData.TRACKING_UNLOCK_BOX_COUNT++;
                                GameAnalyticController.Instance.Tracking().TrackingUnlockBox();
                                Init(BoxState.Unlock);
                            }, null, null, "unlock_box");
                        }
                        else
                        {
                            BoosterController.Instance.OnClickCancelScrewState();
                            booster.OnClickBooster();
                            AudioController.Instance.PlaySound(SoundName.Click);
                        }*/
            BoosterController.Instance.OnClickCancelScrewState();
            booster.OnClickBooster();
            AudioController.Instance.PlaySound(SoundName.Click);

        }
    }

    public override void Serialize()
    {
        base.Serialize();

        BoxData boxData = new BoxData(UniqueId);

        boxData.Color = color;
        boxData.State = BoxState;

        for (int i = 0; i < lstTray.Count; i++)
        {
            boxData.Screws.Add(lstTray[i].GetScrewId());
        }

        Serialize(boxData);
    }

    public override void InitializeFromSave()
    {
        base.InitializeFromSave();

        BoxData boxData = Deserialize<BoxData>();

        if (boxData.State == BoxState.Lock)
        {
            return;
        }

        boxState = boxData.State;
        color = boxData.Color;

        //Reload Box State
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
        }

        meshFilter.mesh = DataScrewColor.Instance.GetBoxMeshByColor(color);
        meshFilterLid.mesh = DataScrewColor.Instance.GetLidMeshByColor(color);
        meshRenderer.material = DataScrewColor.Instance.GetMaterialByColor(color);
        meshRendererLid.material = DataScrewColor.Instance.GetMaterialByColor(color);
        var posYLid = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? 1.1f : 0.5f;
        meshFilterLid.transform.localPosition = new Vector3(0, posYLid, -40);

        if (boxState == BoxState.Lock)
        {
            btnLock.SetActive(true);
            meshRenderer.enabled = false;
        }
        else if (boxState == BoxState.Unlock)
        {
            btnLock.SetActive(false);
            meshRenderer.enabled = true;
        }
        else if (boxState == BoxState.Move)
        {
            meshFilterLid.transform.DOLocalJump(new Vector3(0, 0.4f, 0), 2, 1, 0.2f).OnComplete(() =>
            {
                transform.DOKill();
                fxCloseBox.SetActive(true);
                Sequence punchSequence = DOTween.Sequence();

                punchSequence.Append(transform.DOScale(new Vector3(1f, 0.6f, 1f), 0.18f)); // co lại
                punchSequence.Append(transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.14f)); // trở lại
                punchSequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.08f)); // trở lại
                punchSequence.Play();
                EditorLogger.Log("box count before increment: " + boxCount);
                boxCount++;
                EventDispatcher.Push(EventId.OnBoxCompleted, boxCount);

                Invoke(nameof(SpawnNewBox), 0.48f);
            });

            return;
        }

        for (int i = 0; i < boxData.Screws.Count; i++)
        {
            Screw screw = ScrewManager.GetScrewById(boxData.Screws[i]);

            if (screw != null)
            {
                lstTray[i].Fill(screw);
                lstTray[i].SetComletedAnim(true);
                screw.ChangeState(ScrewState.OnBox);
                screw.transform.parent = lstTray[i].transform;
                screw.transform.localRotation = Quaternion.Euler(0, 90, -90);
                screw.transform.localPosition = new Vector3(0, 0, -0.1f);

                screw.transform.localScale = Vector3.one;
            }
        }

        CheckFull();
    }
}


public enum BoxState
{
    Unlock = 0,
    Move = 1,
    Lock = 2
}