using Cysharp.Threading.Tasks;
using Storage;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PreBoosterController : Singleton<PreBoosterController>
{
    [SerializeField] private PreBooster preBoosterRocket, preBoosterGlass;
    [SerializeField] private PopupPreBooster popupPreBooster;
    [SerializeField] private PopupBuyPreBooster popupBuyPreBooster;
    [SerializeField] private GameObject gobjTutoruial;
    [SerializeField] private bool isInited = false;
    public bool IsShow => popupPreBooster.IsShow;
     
    public PreBooster PreBoosterRocket { get => preBoosterRocket; }
    public PreBooster PreBoosterGlass { get => preBoosterGlass; }

    public async UniTask Init()
    {
        
        //CheckOfflineTime();
        preBoosterRocket.Init();
        preBoosterGlass.Init();
        isInited = true;
        //TimeGetter.Instance.RegisActionOnUpdateTime(SaveCurrentTimeTime);
    }
    private void Reset()
    {

    }
    public void ShowPopupBooster()
    {
        Debug.Log("ShowPopupBooster");
        popupPreBooster.Setup();
        popupPreBooster.Show();
    }

    public void OnStartGame()
    {
        Debug.Log($"====== {Db.storage.PreBoosterData==null}");

        bool isUsedRocket = preBoosterRocket.Select || Db.storage.PreBoosterData.IsFreeTime(PreBoosterType.Rocket);
        bool isUsedGlass = preBoosterGlass.Select || Db.storage.PreBoosterData.IsFreeTime(PreBoosterType.Glass);

        if (isUsedRocket)
        {
            var preBooster = Db.storage.PreBoosterData;
            if (preBooster.IsFree(PreBoosterType.Rocket))
            {
                preBooster.UseFree(PreBoosterType.Rocket);
            } else
                preBooster.AddValue(PreBoosterType.Rocket, -1);
            TrackingController.Instance.TrackingPowerUP(PreBoosterType.Rocket, IngameData.preBoosterPlace);
        }
        if (isUsedGlass)
        {
            var preBooster = Db.storage.PreBoosterData;
            if (preBooster.IsFree(PreBoosterType.Glass))
            {
                preBooster.UseFree(PreBoosterType.Glass);
            }
            else
                preBooster.AddValue(PreBoosterType.Glass, -1);
            preBooster.SaveBooster(PreBoosterType.Glass);
            TrackingController.Instance.TrackingPowerUP(PreBoosterType.Glass, IngameData.preBoosterPlace);
        }
    }

    public void StartTutorial()
    {
        gobjTutoruial.SetActive(true);
    }
    public void OnDoneTutorial()
    {
        gobjTutoruial.SetActive(false);
    }
    public void SetSellectPreBooster(PreBoosterType type, bool select)
    {
        if (type == PreBoosterType.Rocket)
        {
            preBoosterRocket.SelectBooster(select);
        }
        else if (type == PreBoosterType.Glass)
        {
            preBoosterGlass.SelectBooster(select);
        }
    }
    public void OnNotHaveNotStock(PreBoosterType preBoosterType)
    {
        popupBuyPreBooster.ShowBooster(preBoosterType);
    }
    public void OnBuy()
    {
        popupBuyPreBooster.Hide();
        preBoosterGlass.Init(true);
        preBoosterRocket.Init(true);
        if (PreBoosterFailControll.Instance != null)
        {
            PreBoosterFailControll.Instance.ReInitUI();
        }
    }
}
