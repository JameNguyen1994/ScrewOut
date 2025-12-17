using DG.Tweening;
using Life;
using MainMenuBar;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHeart : MonoBehaviour
{
    [SerializeField] private RectTransform rtfmCoin;
    [SerializeField] private GameObject gobjAddLife;
    [SerializeField] private Transform tfmIconHeart;

    public Transform RtfmHeart { get => tfmIconHeart; }

    [SerializeField] private float activeValue = -50f;
    [SerializeField] private float deActiveValue = 350f;

    private void Start()
    {
    }
    public void TogglePanel(bool active, float time = 1)

    {
        rtfmCoin.DOKill();
        if (active)
        {
            rtfmCoin.DOAnchorPosY(activeValue, time / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            rtfmCoin.DOAnchorPosY(deActiveValue, time / 2).SetEase(Ease.InQuad);
        }
    }

    public void EnableAddLife(bool enable)
    {
        gobjAddLife.SetActive(enable);
    }
    public void CheatSubLife()
    {
        if (DBLifeController.Instance.LIFE_INFO.lifeAmount > 0)
        {
            LifeController.Instance.UseLife();
        }
    }


}