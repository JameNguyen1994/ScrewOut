using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProcessLevelBar : MonoBehaviour
{
    [SerializeField] private Image imgAmount;
    [SerializeField] private int fragmentCount = 5;
    [SerializeField] private ProcessLevelBarData data;
    [SerializeField] private RectTransform rectStartPoint, rectEndPoint;
    [SerializeField] private LevelDot lvlPool;
    [SerializeField] private Transform tfmContent;
    [SerializeField] private Image imgBgFill;
    private List<LevelDot> lstLevelDots = new List<LevelDot>();

    [ContextMenu("InitUI UI")]
    void Test()
    {
        Init(46);
    }
    
    public void Init(int currentLvl)
    {
        lstLevelDots.Clear();

        var dataLvl = currentLvl % fragmentCount;

        imgAmount.fillAmount = GetAmountAt(dataLvl);

        int toLevel = currentLvl % fragmentCount != 0 ? (currentLvl / fragmentCount + 1) * fragmentCount : currentLvl; 
        
        int fromLevel = toLevel - fragmentCount + 1;
        
        print(fromLevel);
        

        for (int i = fromLevel; i <= toLevel; i++)
        {
            var lvlDot = Instantiate(lvlPool, tfmContent);
            var iconIndex = LevelMapService.GetLevelMap(i) - 1;//(i - 1) % data.lstData.Count;

            var positionIndex = (i - 1) % fragmentCount;
            
            lvlDot.Init(i, currentLvl, data.lstData[iconIndex].icon);
            lvlDot.GetComponent<RectTransform>().anchoredPosition = new Vector2(GetPositionX(positionIndex), 0);
            lstLevelDots.Add(lvlDot);
            lvlDot.gameObject.SetActive(true);
        }
    }

    public void DoFill(int currentLvl)
    {
        var dataLvl = currentLvl % data.lstData.Count;
        float amount = GetAmountAt(dataLvl);
        imgAmount.DOFillAmount(amount, 1);
    }

    public void UpdateUI(int currentLvl)
    {
        if (lstLevelDots.Count < fragmentCount)
        {
            return;
        }

    }

    float GetAmountAt(int index)
    {
        float distanceX = Mathf.Abs(rectStartPoint.anchoredPosition.x - rectEndPoint.anchoredPosition.x);
        float space = distanceX / (fragmentCount - 1);

        if (index == 0)
        {
            index = fragmentCount;
        }
        
        return space * (index - 1) / distanceX;
    }

    float GetPositionX(int index)
    {
        float distanceX = Mathf.Abs(rectStartPoint.anchoredPosition.x - rectEndPoint.anchoredPosition.x);
        float space = distanceX / (fragmentCount - 1);

        return rectStartPoint.anchoredPosition.x + index * space;

    }
}
