using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxLevelBonus : Box
{
    [SerializeField] private ParticleSystem parComplete;
    public List<Tray> LstTray => lstTray;

    private void Start()
    {
        for (int i = 0; i < lstTray.Count; i++)
        {
            lstTray[i].InitNewTray();
            lstTray[i].Box = this;
            Debug.Log($"Tray {i} IsFill: {lstTray[i].IsFill}");
        }
    }
    public async UniTask Hide(float time)
    {
        transform.DOLocalMove(new Vector3(0, 0, -15), time);
        color = ScrewColor.None;
    }
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
                meshRenderer.enabled = false;
            }
            if (boxState == BoxState.Unlock)
            {
                meshRenderer.enabled = true;
            }
        }
        else
        {

        }
        gameObject.SetActive(true);
        LevelBonusController.Instance.CheckAllBox();

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
            Init(BoxState.Unlock);

        }
    }

    public void AddScrew(Screw screw)
    {
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
    public override async UniTask CheckFull()
    {
        Debug.Log("Check Full BoxLevelBonus");
        if (boxState == BoxState.Move || boxState == BoxState.Lock) return;

        for (int i = 0; i < lstTray.Count; i++)
        {
            if (!lstTray[i].IsFill) return;
        }

        for (int i = 0; i < lstTray.Count; i++)
        {
            LevelBonusController.Instance.Level.RemoveScrew(lstTray[i].GetScrewId());
        }

        ChangeState(BoxState.Move);
        AudioController.Instance.PlaySound(SoundName.Merge);
        var delay = 50;
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
            Sequence punchSequence = DOTween.Sequence();

            punchSequence.Append(transform.DOScale(new Vector3(1f, 0.6f, 1f), 0.18f)); // co lại
            punchSequence.Append(transform.DOScale(new Vector3(1f, 1.1f, 1f), 0.14f)); // trở lại
            punchSequence.Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.08f)); // trở lại
            punchSequence.Play();

            Invoke(nameof(SpawnNewBox), 0.48f);
            LevelBonusController.Instance.OnComplete1Box();

        });

    }

    protected override async UniTask SpawnNewBox()
    {
        parComplete.Play();
        CoinCollector.Instance.Collect(transform, 3).Forget();
        // FlyEffectController.Instance.
        await transform.DOLocalMove(new Vector3(0, 0, -15), 0f);

        /// SpawnGold
        await UniTask.Delay(300);
        RemoveScrew();
        //transform.localScale = Vector3.zero;
        //.localPosition = Vector3.zero;
        // meshFilterLid.transform.localScale = Vector3.zero;
        AudioController.Instance.PlaySound(SoundName.ClearBox);

        var holeFill = LevelController.Instance.GetHoleFill();

        /*  GameAnalyticController.Instance.Tracking().TrackingBoxCompleted(Db.storage.USER_INFO.level,
              holeFill);*/

        var color = LevelBonusController.Instance.BoxLevelBonusController.GetNextColor();
        if (color != ScrewColor.None)
        {
            Init(BoxState.Unlock, color);
        }
        else
        {
            ChangeState(BoxState.Move);

            return;
        }

        //transform.DOLocalMove(new Vector3(0, 0, -15), 0.3f);
        await transform.DOLocalMove(new Vector3(0, 0, 0), 0.3f).SetEase(Ease.OutBack);
        LevelBonusController.Instance.CheckAllBox();

    }
}
