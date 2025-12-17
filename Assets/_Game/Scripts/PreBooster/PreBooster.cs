using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using Life;
using Newtonsoft.Json.Linq;
using Storage;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreBooster : MonoBehaviour
{
    [SerializeField] private PreBoosterType preBoosterType;
    [SerializeField] private GameObject gobjAdd, gobjCount, gobjTick;
    [SerializeField] private Transform tfmBack, tfmBooster, tfmAdd, tfmCount, tfmTick;
    [SerializeField] private TextMeshProUGUI txtCount;
    [SerializeField] private bool select;
    [SerializeField] private int levelLock;
    [SerializeField] private TextMeshProUGUI txtLock;
    [SerializeField] private Transform tfmLock;
    [SerializeField] private bool isTutorial;
    [SerializeField] private GameObject gobjTutorial;

    [SerializeField] private Image imgBack;
    [SerializeField] private Sprite sprNor;
    [SerializeField] private Sprite sprSelect;

    [SerializeField] private GameObject gobjFreeTime;
    [SerializeField] private TMP_Text txtTimeFree;
    public bool Select { get => select; }
    public PreBoosterType PreBoosterType { get => preBoosterType; }
    Coroutine countTime;
    public async UniTask Show(bool showTuto = false)
    {
        Init();
        var level = Db.storage.USER_INFO.level;
        if (level < levelLock)
        {
            txtLock.gameObject.SetActive(true);
            tfmLock.gameObject.SetActive(true);
            txtLock.text = $"LV.{levelLock}";
            return;
        }
        else
        {
            txtLock.gameObject.SetActive(false);
            tfmLock.gameObject.SetActive(false);

            var time = 0.2f;
            tfmBack.localScale = Vector3.zero;
            tfmBooster.localScale = Vector3.zero;
            tfmAdd.localScale = Vector3.zero;
            tfmCount.localScale = Vector3.zero;
            tfmTick.localScale = Vector3.zero;

            await tfmBack.DOScale(Vector3.one, time).SetEase(Ease.OutBack);
            await tfmBooster.DOScale(Vector3.one, time).SetEase(Ease.OutBack);

            if (gobjAdd.activeSelf)
                await tfmAdd.DOScale(Vector3.one, time).SetEase(Ease.OutBack);
            if (gobjCount.activeSelf)
                await tfmCount.DOScale(Vector3.one, time).SetEase(Ease.OutBack);
            if (showTuto && level == levelLock && Db.storage.PreBoosterData.IsFree(preBoosterType))
            {
                isTutorial = true;
                gobjTutorial.SetActive(true);
                PreBoosterController.Instance.StartTutorial();
            }
            else
            {
                isTutorial = false;
                gobjTutorial.SetActive(false);
            }
        }



    }
    public async UniTask Hide()
    {
        var time = 0.3f;
        tfmBack.DOScale(Vector3.zero, time);
        tfmBooster.DOScale(Vector3.zero, time);
        tfmAdd.DOScale(Vector3.zero, time);
        tfmCount.DOScale(Vector3.zero, time);
        transform.DOScale(Vector3.zero, time); ;
        await tfmTick.DOScale(Vector3.zero, time);
    }
    public void Reset()
    {
        tfmBack.localScale = Vector3.zero;
        tfmAdd.localScale = Vector3.zero;
        tfmCount.localScale = Vector3.zero;
        tfmBooster.localScale = Vector3.zero;
        tfmTick.localScale = Vector3.zero;
        gobjTutorial.SetActive(false);
        tfmLock.gameObject.SetActive(false);
        gobjFreeTime.transform.localScale = Vector3.zero;
        select = false;

    }
    public void Init(bool force = false)
    {

        Debug.Log($"PreBooster Init");

        var preBoosterData = Db.storage.PreBoosterData;
        preBoosterData.CheckOfflineTime(TimeGetter.Instance.CurrentTime, preBoosterType);
        Debug.Log($"PreBooster Init preBoosterData==null:{preBoosterData == null} {preBoosterType}");
        int count = Db.storage.PreBoosterData.CountValue(preBoosterType);
        txtCount.text = $"{count}";
        select = false;
        imgBack.sprite = select ? sprSelect : sprNor;

        if (count == 0)
        {
            Debug.Log($"PreBooster Init count=0 {preBoosterType}");
            gobjAdd.SetActive(true);
            gobjCount.SetActive(false);
            gobjTick.SetActive(false);
        }
        else
        {
            Debug.Log($"PreBooster Init count>0 {preBoosterType}");
            gobjAdd.SetActive(false);
            gobjCount.SetActive(true);

            gobjTick.SetActive(false);
        }
        if (force)
        {
            if (count > 0)
            {
                tfmCount.localScale = Vector3.one;
                tfmCount.gameObject.SetActive(true);
            }

        }
        CheckFreeTime();
    }
    public void OnClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        if (isTutorial)
        {
            isTutorial = false;
            gobjTutorial.SetActive(false);
            PreBoosterController.Instance.OnDoneTutorial();
        }
        if (IsFreeTime())
        { return; }



        var level = Db.storage.USER_INFO.level;
        if (level < levelLock)
        {
            txtLock.gameObject.SetActive(true);
            txtLock.text = $"LV.{levelLock}";
            return;
        }
        var preBoosterData = Db.storage.PreBoosterData;
        Debug.Log($"PreBooster Init preBoosterData==null:{preBoosterData == null} {preBoosterType}");
        int count = Db.storage.PreBoosterData.CountValue(preBoosterType);
        if (count <= 0)
        {
            /*   AdsController.Instance.ShowRewardAds(RewardAdsPos.NONE, () =>
               {
                   Db.storage.PreBoosterData.AddValue(preBoosterType, 1);
                   UseIfHaveCount();
                   return;
               }, null, null, "PreBooster");*/
            PreBoosterController.Instance.OnNotHaveNotStock(preBoosterType);
            return;

        }

        select = !select;
        imgBack.sprite = select ? sprSelect : sprNor;

        if (select)
        {

            gobjAdd.SetActive(false);
            gobjCount.SetActive(false);
            gobjTick.SetActive(true);
            tfmTick.localScale = Vector3.zero;

            tfmTick.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            Init();
            tfmCount.localScale = Vector3.one;
        }
    }
    public void UseIfHaveCount()
    {

        if (isTutorial)
        {
            isTutorial = false;
            gobjTutorial.SetActive(false);
            PreBoosterController.Instance.OnDoneTutorial();
        }
        if (IsFreeTime())
        { return; }



        var level = Db.storage.USER_INFO.level;
        if (level < levelLock)
        {
            txtLock.gameObject.SetActive(true);
            txtLock.text = $"LV.{levelLock}";
            return;
        }
        var preBoosterData = Db.storage.PreBoosterData;
        Debug.Log($"PreBooster Init preBoosterData==null:{preBoosterData == null} {preBoosterType}");
        int count = Db.storage.PreBoosterData.CountValue(preBoosterType);
        if (count <= 0)
        {
            return;

        }

        select = true;
        imgBack.sprite = select ? sprSelect : sprNor;

        if (select)
        {

            gobjAdd.SetActive(false);
            gobjCount.SetActive(false);
            gobjTick.SetActive(true);
            tfmTick.localScale = Vector3.zero;
            tfmTick.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            Init();
            tfmCount.localScale = Vector3.one;
        }
    }
    public void SelectBooster(bool select)
    {
        this.select = select;
        imgBack.sprite = select ? sprSelect : sprNor;
        if (select)
        {
            gobjAdd.SetActive(false);
            gobjCount.SetActive(false);
            gobjTick.SetActive(true);
            tfmTick.localScale = Vector3.zero;
            tfmTick.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            Init();
        }
    }
    [Button]
    public void TestAdd5Minus()
    {
        var preBoosterClone = Db.storage.PreBoosterData.Deepclone();
        var preBoosterData = Db.storage.PreBoosterData;

        var data = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
        data.timeFree += 5 * 60 * 1000;
        preBoosterData.SaveToDB(preBoosterClone);
        Init();
    }

    [Button]
    public void CheckFreeTime()
    {
        var preBoosterClone = Db.storage.PreBoosterData.Deepclone();
        var preBoosterData = Db.storage.PreBoosterData;

        var data = preBoosterData.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
        if (data.timeFree > 0)
        {
            gobjAdd.SetActive(false);
            gobjCount.SetActive(false);
            gobjFreeTime.SetActive(true);
            gobjFreeTime.transform.localScale = Vector3.one;
            imgBack.sprite = sprSelect;
            preBoosterData.CheckOfflineTime(TimeGetter.Instance.CurrentTime, preBoosterType);
            FormatTextDetail(TimeSpan.FromMilliseconds(data.timeFree), txtTimeFree);
            select = true;
            if (countTime != null)
            {
                StopCoroutine(countTime);
            }
            countTime = StartCoroutine(CountInfinity());
        }
        else
        {
            gobjFreeTime.SetActive(false);
            if (countTime != null)
            {
                StopCoroutine(countTime);
            }
        }
    }
    private bool IsFreeTime()
    {

        var preBoosterData = Db.storage.PreBoosterData;
        preBoosterData.CheckOfflineTime(TimeGetter.Instance.CurrentTime, preBoosterType);
        var data = preBoosterData.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
        return data.timeFree > 0;
    }
    private IEnumerator CountInfinity()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log($"CountInfinity PreBoosterType:{preBoosterType}");
            var preBoosterClone = Db.storage.PreBoosterData.Deepclone();
            var preBoosterData = Db.storage.PreBoosterData;

            var data = preBoosterClone.lstPreBoosterData.Find(x => x.preBoosterType == preBoosterType);
            if (data.timeFree > 0)
            {
                preBoosterClone.CheckOfflineTime(TimeGetter.Instance.CurrentTime, preBoosterType);
                FormatTextDetail(TimeSpan.FromMilliseconds(data.timeFree), txtTimeFree);
                if (data.timeFree <= 0)
                {
                    gobjFreeTime.SetActive(false);
                    transform.localScale = Vector3.zero; ;
                    Init();
                    preBoosterData.SaveToDB(preBoosterClone);
                    StopCoroutine(CountInfinity());
                }
            }
            else
            {
                gobjFreeTime.SetActive(false);
                // transform.localScale = Vector3.zero;;
                preBoosterData.SaveToDB(preBoosterClone);
                StopCoroutine(CountInfinity());
                Init(true);
            }
        }
    }
    public void FormatTextDetail(TimeSpan timeSpan, TMP_Text text)
    {
        var textS = timeSpan.ToString(@"hh\:mm\:ss");
        if (timeSpan.TotalHours < 100)
        {
            // Hiển thị tổng số giờ, phút và giây
            textS = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            // Hiển thị ngày, giờ, phút và giây
            textS = $"{(int)timeSpan.TotalDays:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        if (timeSpan.TotalHours == 0)
        {
            // Hiển thị tổng số phút và giây
            textS = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        text.text = $"{textS}";
    }

}
public enum PreBoosterType
{
    Rocket = 0,
    Glass = 1
}
public enum PreBoosterPlace
{
    Home = 0,
    Popup_end = 1
}