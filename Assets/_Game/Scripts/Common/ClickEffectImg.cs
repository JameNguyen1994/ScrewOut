using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ClickEffectImg : MonoBehaviour
{
    [SerializeField] private float duration = 1;
    [SerializeField] Vector3 targetScale;
    [SerializeField] private AnimationCurve alphaColorCurve;
    [SerializeField] private Image img;
    
    [ContextMenu("Do Fx")]
    public void DoEffect()
    {
        Scale();
        DoAnimationAlpha();
    }

    void Scale()
    {
        transform.DOScale(targetScale, duration);
    }

    void DoAnimationAlpha()
    {
        var color = img.color;
        color.a = 0;
        img.DOColor(color, duration).SetEase(alphaColorCurve);
    }
}
