using DG.Tweening;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelStar : MonoBehaviour
{
    [SerializeField] private Text txtStar;
    [SerializeField] private Image imgStar;
    public Image ImgStar { get => imgStar; }
    [SerializeField] private RectTransform rtfmCoin;

    private void Start()
    {
        UpdateStartUI();
    }
    public void UpdateStartUI()
    {
        txtStar.text = $"{Db.storage.USER_INFO.star}";

    }
    public void TogglePanel(bool active, float time = 1)

    {
        //rtfmCoin.DOKill();
        //if (active)
        //{
        //    rtfmCoin.DOAnchorPosX(-10, time/2).SetEase(Ease.OutQuad);
        //}
        //else
        //{
        //    rtfmCoin.DOAnchorPosX(300, time/2).SetEase(Ease.InQuad);
        //}

        rtfmCoin.DOKill();
        if (active)
        {
            rtfmCoin.DOAnchorPosY(-50, time / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            rtfmCoin.DOAnchorPosY(350, time / 2).SetEase(Ease.InQuad);
        }
    }
}