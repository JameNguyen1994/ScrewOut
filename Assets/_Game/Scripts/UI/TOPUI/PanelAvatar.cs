 using Coffee.UIExtensions;
using DG.Tweening;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelAvatar : MonoBehaviour
{

    [SerializeField] private RectTransform rtfmAvatar;
    [SerializeField] private CoinFlyAnim expFly;
    [SerializeField] private Transform tfmTargetBar;
    [SerializeField] private UIParticle expEffect;

    public Transform TfmTargetBar => tfmTargetBar;

    private void Start()
    {
        UpdateStartUI();
    }

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.MakeExpFly, OnCoinFlying);

    }
    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.MakeExpFly, OnCoinFlying);

    }
    public void OnCoinFlying(object data)
    {
        if (data == null) return;
        Vector3 fromPos = (Vector3)data;
        expFly.PlayCoinFlyWithPos(0.3f, fromPos, isHaveSound: true).Forget();
    }
    public void UpdateStartUI()
    {

    }

    [SerializeField] private float activeValue = -50f;
    [SerializeField] private float deActiveValue = 350f;

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

        rtfmAvatar.DOKill();
        if (active)
        {
            rtfmAvatar.DOAnchorPosY(activeValue, time / 2).SetEase(Ease.OutQuad);
        }
        else
        {
            rtfmAvatar.DOAnchorPosY(deActiveValue, time / 2).SetEase(Ease.InQuad);
        }
    }

    public void PlayEffect()
    {
        expEffect.Play();
    }
}