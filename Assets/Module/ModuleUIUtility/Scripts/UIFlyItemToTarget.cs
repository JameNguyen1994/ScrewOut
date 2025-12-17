using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using Coffee.UIExtensions;

public class UIFlyItemToTarget : MonoBehaviour
{
    [Header("References")]
    public Image itemIcon;              // Icon image of the flying item
    public TMP_Text amountText;         // Text to display item amount

    [Header("Flight Settings")]
    public float flightTime = 0.8f;     // Duration of the flight animation
    public float height = -200f;         // Height offset for the parabolic curve
    public Ease ease = Ease.InOutQuad;  // DOTween ease type
    public bool scaleDown = true;       // Scale down while flying
    public Vector3 scaleTarget;

    public void SetIconAndValue(Sprite iconSprite, string textValue)
    {
        if (iconSprite != null)
        {
            itemIcon.sprite = iconSprite;
        }

        if (!string.IsNullOrEmpty(textValue))
        {
            amountText.text = textValue;
        }
    }

    public async UniTask ShowText()
    {
        amountText.transform.localScale = Vector3.zero;
        amountText.gameObject.SetActive(true);
        showTextParticle.Play();

        await amountText.transform.DOScale(1.25f, 0.075f).SetEase(Ease.OutBack);
        await amountText.transform.DOScale(1f, 0.075f).SetEase(Ease.OutQuad);
        await UniTask.Delay(100);
        await UniTask.Delay(1000);
    }

    public async void ShowObject()
    {
        await transform.DOScale(1.25f, 0.075f).SetEase(Ease.OutBack);
        await transform.DOScale(1f, 0.075f).SetEase(Ease.OutQuad);
    }

    public async UniTask HideText()
    {
        await amountText.transform.DOScale(0f, 0.075f).SetEase(Ease.OutQuad);
    }

    public RectTransform fromPosition;
    public RectTransform targetPosition;
    public UIParticle showTextParticle;

    public async UniTask Play(Action onComplete = null)
    {
        amountText.gameObject.SetActive(false);
        amountText.transform.localScale = Vector3.zero;

        // Clone this prefab as a temporary flying instance
        RectTransform clone = Instantiate(transform, transform.parent).GetComponent<RectTransform>();
        clone.gameObject.SetActive(true);
        clone.localScale = Vector3.zero;

        // Copy data to the new instance
        UIFlyItemToTarget cloneScript = clone.GetComponent<UIFlyItemToTarget>();

        // Calculate parabolic path
        Vector3 start = fromPosition.position;
        Vector3 end = targetPosition.position;
        Vector3 mid = (start + end) / 2f + Vector3.up * 100;

        float runTime = 0;
        Vector3 init = new Vector3(start.x + 150, start.y, start.z);
        clone.position = init;
        Vector3 initMid = (init + start) / 2f + Vector3.up * -100;

        clone.DOScale(1f, 0.45f).SetEase(Ease.OutBack);
        await clone.transform.DOPath(new Vector3[] { init, initMid, start }, 0.25f, PathType.CatmullRom).SetEase(Ease.InOutFlash);

        cloneScript.ShowObject();

        await cloneScript.ShowText();
        await cloneScript.HideText();

        await clone.transform.DOPath(new Vector3[] { start, mid, targetPosition.position }, 0.25f, PathType.CatmullRom)
           .SetEase(Ease.InOutFlash)
           .OnUpdate(() =>
           {
               if (scaleDown)
               {
                   runTime += Time.deltaTime / flightTime;
                   clone.localScale = Vector3.Lerp(Vector3.one, scaleTarget, runTime);
               }
           })
           .OnComplete(() =>
           {
               // Invoke callback when animation finishes
               onComplete?.Invoke();

               // Destroy temporary clone
               Destroy(clone.gameObject, 0.05f);
           });
    }
}