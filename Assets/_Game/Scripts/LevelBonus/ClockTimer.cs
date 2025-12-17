using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ClockTimer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform rtfmClock;
    [SerializeField] private TMP_Text txtTime;

    [SerializeField] private RectTransform rtfmStartParent;
    [SerializeField] private RectTransform rtfmFinalParent;

    [Header("Needle (Clock hand)")]
    [SerializeField] private Transform needle;                 // kim đồng hồ
    [SerializeField] private float needleStartSpin = 360f;     // hiệu ứng xoay khi show Clock

    [Header("Settings")]
    [SerializeField] private float scaleDuration = 0.25f;
    [SerializeField] private float moveDuration = 0.55f;
    [SerializeField] private float countTimeDuration = 0.6f;
    [SerializeField] private float delayAfterShow = 0.5f;

    private Tween currentTween;

    private void Awake()
    {
        if (rtfmClock != null)
            rtfmClock.gameObject.SetActive(false);
    }

    // ============================================================================
    //  PLAY CLOCK (POP → COUNT → MOVE → HIT)
    // ============================================================================
    public async UniTask PlayClockAsync(int time)
    {
        if (rtfmClock == null || txtTime == null) return;

        rtfmClock.gameObject.SetActive(true);
        currentTween?.Kill();

        txtTime.text = "0";

        rtfmClock.SetParent(rtfmStartParent);
        rtfmClock.localScale = Vector3.zero;
        rtfmClock.localPosition = Vector3.zero;

        // reset kim
        if (needle != null)
            needle.localRotation = Quaternion.identity;

        Vector3 finalWorldPos = rtfmFinalParent.position;

        DG.Tweening.Sequence seq = DOTween.Sequence();

        // POP
        seq.Append(rtfmClock.DOScale(1.2f, scaleDuration * 0.6f).SetEase(Ease.OutBack).OnComplete(() =>
        {

        }));


        seq.Append(rtfmClock.DOScale(1f, scaleDuration * 0.4f).SetEase(Ease.OutSine));

        seq.AppendInterval(delayAfterShow);

        // COUNT UP
        float v = 0;
        seq.Append(
            DOTween.To(() => v, x =>
            {
                v = x;
                txtTime.text = Mathf.FloorToInt(v).ToString();
            }, time, countTimeDuration)
        );
        seq.Join(

    needle?.DOLocalRotate(
        new Vector3(0, 0, needleStartSpin),
        countTimeDuration, RotateMode.LocalAxisAdd
    ).SetEase(Ease.OutCubic).OnStart(() =>
    {
        AudioController.Instance.PlaySound(SoundName.CLOCK_ROTATE);
    })
);

        // MOVE TO FINAL
        seq.Append(rtfmClock.DOMove(finalWorldPos, moveDuration).SetEase(Ease.OutBack));
        seq.Join(
            rtfmClock.DORotate(new Vector3(0, 0, -25f), moveDuration / 2)
                     .SetLoops(2, LoopType.Yoyo)
        );

        // HIT
        seq.Append(rtfmClock.DOScale(1.15f, 0.15f));
        seq.Append(rtfmClock.DOScale(1f, 0.15f));

        // FIX PARENT
        seq.AppendCallback(() =>
        {
            rtfmClock.SetParent(rtfmFinalParent);
            rtfmClock.localPosition = Vector3.zero;
            rtfmClock.localRotation = Quaternion.identity;
        });

        currentTween = seq;
        await seq.Play().AsyncWaitForCompletion();
        AudioController.Instance.PlaySound(SoundName.COIN_LEVEL_BONUS);

    }

    public void HideClock()
    {
        rtfmClock?.gameObject.SetActive(false);
    }

    // ============================================================================
    //  COUNTDOWN (KIM QUAY ĐỦ 1 VÒNG)
    // ============================================================================
    public async UniTask StartCountDownTime(int startTime, UnityAction onFinish = null)
    {
        if (txtTime == null) return;

        rtfmClock.gameObject.SetActive(true);
        currentTween?.Kill();

        txtTime.text = startTime.ToString();
        txtTime.color = Color.white;

        // reset kim về 0°
        if (needle != null)
            needle.localRotation = Quaternion.identity;

        int current = startTime;

        // 🎯 Tốc độ xoay = 360° / tổng thời gian
        float degreePerSecond = -360f / startTime;

        while (current > 0)
        {
            if (LevelBonusController.Instance.IsCompleted)
            {
                break;
            }
            txtTime.text = current.ToString();

            // scale nhún cơ bản
            rtfmClock.DOKill();
            rtfmClock.localScale = Vector3.one;


            // ===========================================
            // ⚠️ CẢNH BÁO dưới 10 giây
            // ===========================================
            if (current <= 10)
            {
                rtfmClock.DOScale(1.15f, 0.12f).SetEase(Ease.OutQuad)
                   .OnComplete(() => rtfmClock.DOScale(1f, 0.12f));
                // đổi màu đỏ
                txtTime.color = Color.red;

                // nháy alpha
                txtTime.DOFade(0.3f, 0.15f)
                       .SetLoops(2, LoopType.Yoyo)
                       .SetEase(Ease.OutQuad);

                // nhún mạnh hơn
                rtfmClock.DOScale(1.25f, 0.14f)
                         .SetEase(Ease.OutBack)
                         .OnComplete(() =>
                         {
                             rtfmClock.DOScale(1f, 0.12f);
                         });
            }

            // 🎯 Xoay kim đúng trong 1 giây
            if (needle != null)
            {
                needle.DOLocalRotate(
                    needle.localEulerAngles + new Vector3(0, 0, degreePerSecond),
                    1f
                ).SetEase(Ease.Linear);
            }

            await UniTask.Delay(1000);
            current--;
        }


        // ============================
        // HẾT GIỜ
        // ============================
        txtTime.text = "0";

        DG.Tweening.Sequence endSeq = DOTween.Sequence();

        endSeq.Append(txtTime.DOColor(Color.red, 0.1f));
        endSeq.Join(rtfmClock.DOScale(1.4f, 0.2f).SetEase(Ease.OutBack));

        endSeq.Append(rtfmClock.DOShakeScale(0.3f, 0.4f));

        CanvasGroup cg = rtfmClock.GetOrAddComponent<CanvasGroup>();
        endSeq.Append(cg.DOFade(0, 0.3f));

        endSeq.AppendCallback(() =>
        {
            cg.alpha = 1;
            rtfmClock.localScale = Vector3.one;
            rtfmClock.gameObject.SetActive(false);
        });

        await endSeq.Play().AsyncWaitForCompletion();

        onFinish?.Invoke();
    }
}

