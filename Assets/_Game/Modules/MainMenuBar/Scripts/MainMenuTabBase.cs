using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainMenuTabBase : MonoBehaviour, IMainMenuTab
{
    [SerializeField] private GameObject gobjTab;
    [SerializeField] private RectTransform rtfmTab;
    [SerializeField] protected float width;
    [SerializeField] private int index;

    public int Index { get => index; }

    public virtual void Init(int index)
    {
        this.index = index;
        width = rtfmTab.rect.width;
        gobjTab.SetActive(false);
    }
    public virtual void GoToThisTab()
    {
        //throw new System.NotImplementedException();
    }
    public virtual void ExitThisTab()
    {
        //throw new System.NotImplementedException();
    }
    public void SetActive(bool isActive)
    {
        gobjTab.SetActive(isActive);
        // if (isActive)
        //     ExitThisTab();
    }

    public void SetTabPos(bool isLeft)
    {
        rtfmTab.anchoredPosition = new Vector2(isLeft ? -width : width, 0);
    }
    public void DOMoveRight(float time, UnityAction actionOnComplete)
    {
        rtfmTab.DOAnchorPosX(width, time).OnComplete(() => { actionOnComplete?.Invoke(); });
    }
    public void DOMoveLeft(float time, UnityAction actionOnComplete)
    {
        rtfmTab.DOAnchorPosX(-width, time).OnComplete(() => { actionOnComplete?.Invoke(); });
    }
    public void DOMoveCurrentPos(float time, UnityAction actionOnComplete)
    {
        rtfmTab.DOAnchorPosX(0, time).OnComplete(() => { actionOnComplete?.Invoke(); });
    }
    public virtual void DoKill()
    {
        rtfmTab.DOKill();
    }


}
