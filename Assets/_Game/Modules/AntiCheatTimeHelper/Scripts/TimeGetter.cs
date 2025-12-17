using Cysharp.Threading.Tasks;
using PS.NetworkTime;
using PS.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;


public class TimeGetter : Singleton<TimeGetter>
{
    // Start is called before the first frame update
    [SerializeField] private bool isGettedTime = false;
    [SerializeField] private long startTime = 0;
    [SerializeField] private long offset = 0;
    [SerializeField] private long currentTime = 0;
    [SerializeField] private string sDateTime;
    [SerializeField] private string sDateTimeCheat;
    [SerializeField] private TextMeshProUGUI txtCheatTime;
    [SerializeField] private TextMeshProUGUI txtRealTime;
    [SerializeField] private TextMeshProUGUI txtCurrentTime;
    [SerializeField] private GameObject gobjCheatTime;
    [SerializeField] private TMP_InputField inDay;
    [SerializeField] private TMP_InputField inHour;
    [SerializeField] private TMP_InputField inMinus;
    [SerializeField] private Toggle toggleEnable;
    [SerializeField] GameObject gobjCheat;
    [SerializeField] GameObject cheatArea;

    public long CurrentTime => currentTime + (!Db.storage.Inited ? 0 : Db.storage.WEEK_INFO.cheatTime);
    public DateTime Now => DateTimeOffset.FromUnixTimeMilliseconds(CurrentTime).DateTime;
    public DateTime currentDateTimeCheat;
    public bool IsGettedTime => isGettedTime;

    private UnityAction actionOnUpdateTime;

    void Start()
    {
        GetTime();
        StartCoroutine(CountPlayTime());

        gobjCheat.SetActive(Debug.unityLogger.logEnabled); // Hide cheat button by default
        toggleEnable.isOn = false;

    }

    public void OnToggle()
    {
        var isOn = toggleEnable.isOn;
        cheatArea.SetActive(isOn);
    }

    private IEnumerator CountPlayTime()
    {
        yield return new WaitForSecondsRealtime(1);
        Db.storage.LVL_LENGTH_EXP++;

        StartCoroutine(CountPlayTime());
    }

    void OnRealTimeCallBack(TimeNetData data, string error)
    {
        Debug.Log("Get time");
        if (error != null)
        {
            Debug.Log($"OnRealTimeCallBack {error}");

            //print($"error: {error}");
            /* GetTime();
             return;*/
            offset = (int)(Time.realtimeSinceStartup * 1000);
            var dt = DateTime.Now;
            StartCoroutine(CheckResetDateTime());
            startTime = new DateTimeOffset(dt).ToUnixTimeMilliseconds();
        }
        else
        {
            offset = (int)(Time.realtimeSinceStartup * 1000);
           
            var dtReal = RealTimeNet.ConvertTimeStringToDateTime(data.dateTime);
            startTime = new DateTimeOffset(dtReal).ToUnixTimeMilliseconds();
        }
        currentTime = startTime + (int)(Time.realtimeSinceStartup * 1000) - offset;
        isGettedTime = true;
        StartCoroutine(CheckResetDateTime());
        StartCoroutine(UpdateTime());
        Debug.Log("GettedTime1");
    }
    public async UniTask GetTime()
    {
        Debug.Log("Start get time");
        RealTimeNet realTimeNet = new RealTimeNet();
        realTimeNet.RealTime.OnCompleted = OnRealTimeCallBack;
        realTimeNet.RealTime.GetNASATime();
    }
    public async UniTask<long> GetCurrentTime()
    {
        await UniTask.WaitUntil(() => IsGettedTime);
        return CurrentTime;

    }
    public async UniTask<string> GetCurrentTimeString()
    {
        await UniTask.WaitUntil(() => IsGettedTime);
        return DateTimeOffset.FromUnixTimeMilliseconds(CurrentTime).UtcDateTime.ToString();
    }

    private IEnumerator UpdateTime()
    {

        while (isGettedTime)
        {
            // if (!Db.storage.Inited)
            // {
            //     Debug.LogError("LocalDb is not initialized. Please call InitializeDataTime first.");
            //     continue;
            // }
            yield return new WaitForSeconds(1);
            UpdateUICheat();
            actionOnUpdateTime?.Invoke();
        }
    }
    private void UpdateUICheat()
    {
        currentTime = startTime + (int)(Time.realtimeSinceStartup * 1000) - offset;
        currentDateTimeCheat = DateTimeOffset.FromUnixTimeMilliseconds(CurrentTime).UtcDateTime;
        var currentDateTime = DateTimeOffset.FromUnixTimeMilliseconds(currentTime).UtcDateTime;
        sDateTime = $"{currentDateTime}";
        sDateTimeCheat = $"{currentDateTimeCheat}";
        //gobjCheatTime.SetActive(Db.storage.WEEK_INFO.cheatTime > 0);
        txtCheatTime.text = $"{TimeSpan.FromMilliseconds(Db.storage.WEEK_INFO.cheatTime):dd\\:hh\\:mm}";

        txtRealTime.text = $"{currentDateTimeCheat}";
        txtCurrentTime.text = $"{currentDateTime}";
    }
    public void RegisActionOnUpdateTime(UnityAction action)
    {
        actionOnUpdateTime += action;
    }
    public void UnRegisActionOnUpdateTime(UnityAction action)
    {
        actionOnUpdateTime -= action;
    }

    IEnumerator CheckResetDateTime()
    {
        yield return new WaitForSeconds(1);
        if (currentDateTimeCheat.Date > Db.storage.PREV_DATE_TIME.Date)
        {
            Db.storage.PREV_DATE_TIME = currentDateTimeCheat;
            Db.storage.REWARD_BOOSTER_HAMMER_COUNT = 0;
            Db.storage.REWARD_BOOSTER_ADD_HOLE_COUNT = 0;
            Db.storage.REWARD_BOOSTER_BLOOM_COUNT = 0;
            Db.storage.REWARD_BOOSTER_UNLOCK_BOX_COUNT = 0;
            Db.storage.REWARD_LIFE_COUNT = 0;
            Db.storage.REWARD_COIN_FREE_COUNT = 0;
            Db.storage.REWARD_SHOP_COIN_IAP_COUNT = 0;
        }
        StartCoroutine(CheckResetDateTime());
    }
    public void Cheat()
    {
        int day = inDay.text == "" ? 0 : int.Parse(inDay.text);
        int hour = inHour.text == "" ? 0 : int.Parse(inHour.text);
        int minus = inMinus.text == "" ? 0 : int.Parse(inMinus.text);
        Debug.Log($"Cheat Day {day} Hour {hour} Minus {minus}");
        var value = (day * 24 * 60 + hour * 60 + minus) * 60 * 1000;
        Db.storage.WEEK_INFO.CheatTime(value); // Add 6 hours in milliseconds
        UpdateUICheat();
    }
}
