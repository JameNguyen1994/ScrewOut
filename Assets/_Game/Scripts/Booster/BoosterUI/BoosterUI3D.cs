using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterUI3D : BoosterUIBase
{
   // [SerializeField] private SpriteRenderer imgBooster;
    [SerializeField] private TextMeshPro txtAmount;
    [SerializeField] private GameObject gobjAdd;
    [SerializeField] private GameObject gobjCount;
    public override void SetUI(BoosterData boosterData, int amount)
    {
      //  this.imgBooster.sprite = boosterData.sprBooster;
        txtAmount.text = $"{amount}";
        gobjAdd.SetActive(amount == 0);
        gobjCount.SetActive(amount > 0);
    }
    public override void HighLightBooster(bool moving)
    {
        //throw new System.NotImplementedException();
    }
}