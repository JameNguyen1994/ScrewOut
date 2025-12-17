using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDot : MonoBehaviour
{
    [SerializeField] private GameObject objLvlLock, objLvlActive, objLvlCompleted, gobjFlare;
    [SerializeField] private Image imgPreviewIcon;
    [SerializeField] private Text txtLvl;

    public void Init(int level, int currentLvl, Sprite icon)
    {
        if (level == currentLvl)
        {
            objLvlActive.SetActive(true);
            gobjFlare.SetActive(true);
            objLvlLock.SetActive(false);
            objLvlCompleted.SetActive(false);
        }else if (level < currentLvl)
        {
            objLvlActive.SetActive(false);
            gobjFlare.SetActive(false);
            objLvlLock.SetActive(false);
            objLvlCompleted.SetActive(true);
        }
        else
        {
            objLvlActive.SetActive(false);
            gobjFlare.SetActive(false);
            objLvlLock.SetActive(true);
            objLvlCompleted.SetActive(false);
        }

        imgPreviewIcon.sprite = icon;
        txtLvl.text = $"{level}";
    }
}
