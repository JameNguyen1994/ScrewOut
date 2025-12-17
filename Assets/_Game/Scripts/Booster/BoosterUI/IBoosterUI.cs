using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IBoosterUI 
{
    public abstract void SetUI(BoosterData boosterData, int amount);
    public abstract void HighLightBooster(bool moveing = true); 
    public abstract void ToggleUI(bool active,bool resetUI); 
    public abstract void ShowBannerTutorial(bool active);
}

