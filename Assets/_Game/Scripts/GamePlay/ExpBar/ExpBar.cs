using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Utils;
using Storage;
using Storage.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ExpBar : Singleton<ExpBar>
{
    [Header("UI")]
    [SerializeField] private Text txtLevel;
    [SerializeField] private Text txtExp;
    [SerializeField] private Text txtPercent;
    [SerializeField] private Image imgFillBar;
    [Header("Data")]
    [SerializeField] private UserExp levelTarget = new UserExp();
    [SerializeField] private int expLevel = 0;
    [Header("Tracking")]
    [SerializeField] private ExpTracking expTracking;
    [SerializeField] private ParticleSystem parPoint;

    public Text TxtLevel { get => txtLevel; }
    public int ExpLevel { get => expLevel; }

    [SerializeField] private float lerpMin = 0.1f;
    [SerializeField] private float lerpMax = 0.9f;

    private void Start()
    {
        Init();
    }
    public async void Init()
    {
        var userExp = Db.storage.USER_EXP;
        levelTarget.level = userExp.level + 1;
        levelTarget.exp = userExp.level * 1000;

        await UpdateUIExp(userExp);
    }

    public void SetExpLevel(int box)
    {
        expLevel = box;

    }
    public async void AddExp()
    {
        var userExp = Db.storage.USER_EXP.DeepClone();
        userExp.exp += expLevel;
        userExp.totalExp += expLevel;
        parPoint.Play();
        var expRedundant = userExp.exp - levelTarget.exp;
        if (expRedundant >= 0)
        {
            // Level Up
            userExp.exp = expRedundant;
            userExp.level++;
            Db.storage.USER_EXP = userExp;
#if UNITY_EDITOR
            IEconomicTracking tracking = new EconomicTrackingUnity();
#elif UNITY_ANDROID
            IEconomicTracking tracking = new EconomicTrackingAndroid();
#else
            IEconomicTracking tracking = new EconomicTrackingIos();
#endif

            tracking.SendReachLevel();
            await UpdateUIExp(levelTarget);
            Init();
            //ShowLevelUp();
        }
        else
        {
            Db.storage.USER_EXP = userExp;
            await UpdateUIExp(userExp);
        }
    }

    public async void AddExp(int exp, UnityAction onShowCompleted = null)
    {
        var userExp = Db.storage.USER_EXP.DeepClone();
        userExp.exp += exp;
        userExp.totalExp += exp;

        var expRedundant = userExp.exp - levelTarget.exp;
        if (expRedundant >= 0)
        {
            // Level Up
            userExp.exp = expRedundant;
            userExp.level++;
            Db.storage.USER_EXP = userExp;
#if UNITY_EDITOR
            IEconomicTracking tracking = new EconomicTrackingUnity();
#elif UNITY_ANDROID
                IEconomicTracking tracking = new EconomicTrackingAndroid();
#else
                IEconomicTracking tracking = new EconomicTrackingIos();
#endif

            tracking.SendReachLevel();
            await UpdateUIExp(levelTarget);
            Init();
            ShowLevelUp(userExp.level, onShowCompleted);
        }
        else
        {
            Db.storage.USER_EXP = userExp;
            await UpdateUIExp(userExp);
            ExpLevelUpPopup.onCallback?.Invoke();
        }
    }

    private async UniTask UpdateUIExp(UserExp userExpUI)
    {
        txtExp.text = $"{userExpUI.exp}/{levelTarget.exp}";
        txtLevel.text = $"{userExpUI.level}";

        var fill = (float)userExpUI.exp / (float)levelTarget.exp;

        fill = Mathf.Lerp(lerpMin, lerpMax, fill);

        if (txtPercent != null)
        {
            txtPercent.text = ((int)(fill * 100) + "%");
        }

        await imgFillBar.DOFillAmount(fill, 0.5f);
    }

    public void ShowLevelUp(int level, UnityAction onShowCompleted)
    {
        Debug.Log("Level Up");
        // var remote = AmplitudeController.Instance.Remote();
        // if (!remote.SegmentFlow.enableExpBar)
        // {
        //     ExpLevelUpPopup.onCallback?.Invoke();
        //     return;
        // }
        //PopupController.Instance.PopupExpLevelUp.Show();
        ExpLevelUpPopup.Instance.Show(level);
    }

    private bool isUplevel;

    public void AddExpToUser(int exp)
    {
        var userExp = Db.storage.USER_EXP.DeepClone();
        userExp.exp += exp;
        userExp.totalExp += exp;

        var expRedundant = userExp.exp - levelTarget.exp;
        if (expRedundant >= 0)
        {
            // Level Up
            userExp.exp = expRedundant;
            userExp.level++;
            Db.storage.USER_EXP = userExp;
#if UNITY_EDITOR
            IEconomicTracking tracking = new EconomicTrackingUnity();
#elif UNITY_ANDROID
                IEconomicTracking tracking = new EconomicTrackingAndroid();
#else
                IEconomicTracking tracking = new EconomicTrackingIos();
#endif

            tracking.SendReachLevel();
        }
        else
        {
            Db.storage.USER_EXP = userExp;
        }

        isUplevel = expRedundant >= 0;
    }

    public async void AddExpUpdateUI(UnityAction onShowCompleted = null)
    {
        var userExp = Db.storage.USER_EXP.DeepClone();

        if (isUplevel)
        {
            isUplevel = false;
            await UpdateUIExp(levelTarget);
            Init();
            ShowLevelUp(userExp.level, onShowCompleted);
        }
        else
        {
            await UpdateUIExp(userExp);
            ExpLevelUpPopup.onCallback?.Invoke();
        }
    }
}
