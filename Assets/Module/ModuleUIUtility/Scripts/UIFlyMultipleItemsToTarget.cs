using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Coffee.UIExtensions;

public class UIFlyMultipleItemsToTarget : MonoBehaviour
{
    [Header("References")]
    public Image itemIcon;              // Icon image of the flying item
    public TMP_Text amountText;         // Text to display item amount
    public CanvasGroup canvasGroup;
    public UIParticle finalEffect;

    [Header("Flight Settings")]
    public float flightTime = 0.8f;     // Duration of the flight animation
    public float height = -200f;        // Height offset for the parabolic curve
    public Ease ease = Ease.InOutQuad;  // DOTween ease type
    public bool scaleDown = true;       // Scale down while flying
    public Vector3 scaleTarget = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector2Int randomObject = new Vector2Int(3, 8);

    [Header("Spawn Settings")]
    public RectTransform fromPosition;
    public RectTransform targetPosition;
    public RectTransform target;
    public Transform root;
    public UIParticle showTextParticle;

    public void SetIconAndValue(Sprite iconSprite, string textValue)
    {
        if (iconSprite != null)
            itemIcon.sprite = iconSprite;

        if (!string.IsNullOrEmpty(textValue))
            amountText.text = textValue;
    }

    [EasyButtons.Button]
    public void PlayMultiple(float radius = 20f)
    {
        PlayMultipleAsync(fromPosition.position, targetPosition.position, radius).Forget();
    }

    public async UniTask PlayMultiple(float radius = 20f, Action onAllComplete = null, Action onSingleComplete = null)
    {
        await PlayMultipleAsync(fromPosition.position, targetPosition.position, radius, onAllComplete, onSingleComplete);
    }

    public async UniTask PlayMultipleAsync(Vector3 spawnCenter, Vector3 targetPos, float radius = 100f, Action onAllComplete = null, Action onSingleComplete = null)
    {
        int count = UnityEngine.Random.Range(randomObject.x, randomObject.y);
        List<UniTask> tasks = new List<UniTask>();

        amountText.gameObject.SetActive(true);
        showTextParticle.Play();

        for (int i = 0; i < count; i++)
        {
            // Random spawn offset within circle
            Vector2 offset = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 spawnPos = spawnCenter + new Vector3(offset.x, offset.y, 0);

            tasks.Add(PlaySingleWithVariation(spawnPos, targetPos, height, 0.15f));

            await UniTask.Delay(TimeSpan.FromSeconds(0.15f));
        }

        // Wait for all animations to finish
        await UniTask.WhenAll(tasks);
        amountText.gameObject.SetActive(false);

        if (finalEffect != null)
        {
            finalEffect.Play();
        }

        onAllComplete?.Invoke();
    }

    private async UniTask PlaySingleWithVariation(Vector3 fromPos, Vector3 targetPos, float heightOffset, float delay, Action onSingleComplete = null)
    {
        // Clone this prefab as a temporary flying instance
        canvasGroup.alpha = 0;
        RectTransform clone = Instantiate(transform, root).GetComponent<RectTransform>();
        clone.gameObject.SetActive(true);
        clone.position = fromPos;
        clone.localScale = Vector3.one;
        CanvasGroup group = clone.GetComponent<CanvasGroup>();

        Vector3 start = fromPos;
        Vector3 end = targetPos;
        Vector3 mid = (start + end) / 2f + Vector3.up * heightOffset;
        Vector3[] path = new Vector3[] { start, mid, end };

        float t = 0f;

        group.DOFade(1, 0.1f);
        await clone.DOScale(1.5f, 0.075f).SetEase(Ease.OutBack);
        await clone.DOScale(1f, 0.075f).SetEase(Ease.OutQuad);

        await UniTask.Delay(TimeSpan.FromSeconds(delay));

        await clone.DOPath(path, flightTime, PathType.CatmullRom)
            .SetEase(ease)
            .OnUpdate(() =>
            {
                if (scaleDown)
                {
                    t += Time.deltaTime / flightTime;
                    clone.localScale = Vector3.Lerp(Vector3.one, scaleTarget, t);
                    group.alpha = Mathf.Lerp(1, 0.25f, t);
                }
            })
            .OnComplete
            (
                () =>
                {
                    Destroy(clone.gameObject, 0.05f);
                    onSingleComplete?.Invoke();
                    ScaleTarget();
                }
            );
    }

    private void ScaleTarget()
    {
        AudioController.Instance.PlaySound(SoundName.COLLECT_SCREW);

        target.DOKill();
        target.DOScale(1.15f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            target.DOScale(1f, 0.1f).SetEase(Ease.OutQuad);
        });
    }
}