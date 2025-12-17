using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private Image imgButton;
    [SerializeField] private Sprite sprOn;
    [SerializeField] private Sprite sprOff;

    public void SetUIButton(bool active)
    {
        imgButton.sprite = active ? sprOn : sprOff;
    }
}
