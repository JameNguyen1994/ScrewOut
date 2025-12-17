using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 
    BoosterUIBase : MonoBehaviour, IBoosterUI
{
    public virtual void HighLightBooster(bool moveing = true)
    {
      //  throw new System.NotImplementedException();
    }

    public virtual void SetUI(BoosterData boosterData, int amount)
    {
//throw new System.NotImplementedException();
    }

    public virtual void ShowBannerTutorial(bool active)
    {
        //throw new System.NotImplementedException();
    }

    public virtual void ToggleUI(bool active,bool resetUI)
    {
        //throw new System.NotImplementedException();
    }
    public virtual async UniTask ShowAnimation()
    {
        
    }
    public virtual async UniTask ShowHighLightIdle()
    {

    }
}
