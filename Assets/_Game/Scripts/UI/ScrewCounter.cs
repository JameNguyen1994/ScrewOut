using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrewCounter : Singleton<ScrewCounter>
{
    [SerializeField] private Text txtScrewPercent;
    [SerializeField] private Image imgFill;

    [SerializeField] private int amount = 0;
    [SerializeField] private int total = 0;

    [SerializeField] private List<ProcessChangeCenterPos> lstChangeCenterPos;

    public void Init(int amount)
    {
        this.total = amount;
        var percent = ((float)this.amount / (float)total) * 100f;
        txtScrewPercent.text = $"{(int)percent}%";
        imgFill.fillAmount = (percent) / 100f;
        lstChangeCenterPos.Clear();
        int percentStep = 7;
        for (int i = percentStep; i <= 100; i += percentStep)
        {
            lstChangeCenterPos.Add(new ProcessChangeCenterPos() { percent = i, hasChange = false });
        }
    }

    public void UpdateScrew()
    {
        amount++;
        var percent = ((float)amount / (float)total) * 100f;
        txtScrewPercent.text = $"{(int)percent}%";
        imgFill.fillAmount = percent / 100f;
        CheckProcess(percent);
    }
    public void UpdateScrew(int count)
    {
        amount += count;
        var percent = ((float)amount / (float)total) * 100f;
        txtScrewPercent.text = $"{(int)percent}%";
        imgFill.fillAmount = (percent) / 100f;
        CheckProcess(percent);
    }
    public int GetProcess()
    {
        var percent = ((float)amount / (float)total) * 100f;
        return (int)percent;
    }
    private void CheckProcess(float percent)
    {
        Debug.Log($"CheckProcess: {percent}");
        foreach (var item in lstChangeCenterPos)
        {
            if (!item.hasChange && percent >= item.percent)
            {
                item.hasChange = true;
                // Change center pos
                LevelController.Instance.ChangeCenterPos();
                break;
            }
        }
    }
}
[System.Serializable]
public class ProcessChangeCenterPos
{
    public float percent;
    public bool hasChange;
    public ProcessChangeCenterPos()
    {
        hasChange = false;
    }
}