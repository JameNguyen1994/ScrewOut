using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] private List<MainMenuTabBase> lstTab;
    [SerializeField] private MainMenuTabBase currentTab;
    [SerializeField] private MainMenuTabBase nextTab;
    [SerializeField] private MainMenuTabBase preTab;

    public MainMenuTabBase NextTab { get => nextTab; }

    public void Init(int currIndex)
    {
        for (int i = 0; i < lstTab.Count; i++)
        {
            lstTab[i].Init(i);
        }
        currentTab = lstTab[currIndex];
        currentTab.DOMoveCurrentPos(0, null);
        currentTab.SetActive(true);
    }
    public void OnGoToTab(int nextIndex)
    {
        if (currentTab.Index == nextIndex)
            return;
        CancelAnimationTab();


        nextTab = lstTab[nextIndex];
        DoAnimationTab();
    }
    public void DoAnimationTab()
    {
        currentTab.SetActive(true);
        currentTab.ExitThisTab();
        
        nextTab.SetActive(true);

        bool isGoLeft = nextTab.Index < currentTab.Index;
        float timeAnimation = 0.5f;
        nextTab.SetTabPos(isGoLeft);
        nextTab.GoToThisTab();

        if (isGoLeft)
        {
            currentTab.DOMoveRight(timeAnimation, () => { });
            nextTab.DOMoveCurrentPos(timeAnimation, () => { });
            preTab = currentTab;
            currentTab = nextTab;
        }
        else
        {
            currentTab.DOMoveLeft(timeAnimation, () => { });
            nextTab.DOMoveCurrentPos(timeAnimation, () => { });
            preTab = currentTab;
            currentTab = nextTab;
        }
    }
    public void CancelAnimationTab()
    {
        if (currentTab != null)
        {
            currentTab.DoKill();
            currentTab.SetActive(false);
        }
        if (nextTab != null)
        {
            nextTab.DoKill();
            nextTab.SetActive(false);

        }
        if (preTab != null)
        {
            preTab.DoKill();
            preTab.SetActive(false);


        }
    }

}
