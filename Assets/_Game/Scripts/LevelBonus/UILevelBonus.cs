using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UILevelBonus : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ClockTimer clockTimer;

    [SerializeField] private Transform tfmCoinUI;
    [SerializeField] private Transform tfmCoinCenterScreen;
    [SerializeField] private Transform tfmCoinStart;
    [SerializeField] private TMP_Text txtCoin;
    [SerializeField] private Image imgFade;

    [SerializeField] private GameObject gobjEffectFlare;
    [SerializeField] private GameObject gobjEffectComplete;

    private void Start()
    {
        HideUI();
    }

    private void HideUI()
    {
        if (clockTimer != null)
            clockTimer.HideClock();

        if (tfmCoinUI != null)
            tfmCoinUI.gameObject.SetActive(false);
        imgFade.color = new Color(0, 0, 0, 0);
    }

    // ============================================================================
    // SHOW CLOCK BONUS (POP → COUNT → MOVE → HIT → SHOW COIN)
    // ============================================================================
    [Button]
    public async UniTask ShowClock(int time = 15)
    {
        if (clockTimer == null)
        {
            Debug.LogWarning("ClockTimer not assigned!");
            return;
        }

        txtCoin.text = "0";
        ShowFade();
        await clockTimer.PlayClockAsync(time);

        // Show coin UI
        tfmCoinUI.gameObject.SetActive(true);
        tfmCoinUI.localScale = Vector3.zero;

        await tfmCoinUI.DOScale(1f, 0.3f)
                 .SetEase(Ease.OutBack);

        HideFade();
    }
    private async UniTask ShowFade()
    {
        await imgFade.DOFade(0.9f, 0.3f);
    }
    private async UniTask HideFade()
    {
        await imgFade.DOFade(0, 0.3f);
    }
    [Button]
    public void ResetCoinPos()
    {
        tfmCoinUI.SetParent(tfmCoinStart);
        tfmCoinUI.localPosition = Vector3.zero;
        tfmCoinUI.localScale = Vector3.one;
        gobjEffectFlare.gameObject.SetActive(false);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
    [Button]
    public async UniTask ShowCoinEndScreen()
    {
        var path = new Vector3[]
        {
            tfmCoinUI.localPosition,
           tfmCoinUI.localPosition + new Vector3(0, 10, 0),
            Vector3.zero,
        };
        tfmCoinUI.gameObject.SetActive(true);
        await ShowCoinEndScreenAlongPath(path);
    }
    [SerializeField] private float timeMove = 0.5f;
    public async UniTask ShowCoinEndScreenAlongPath(Vector3[] path)
    {
        tfmCoinUI.SetParent(tfmCoinCenterScreen);

        tfmCoinUI.DOScale(1.5f, timeMove).SetEase(Ease.OutBack);

        await tfmCoinUI.DOLocalPath(path, timeMove, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .AsyncWaitForCompletion();
        gobjEffectFlare.gameObject.SetActive(true);
        await tfmCoinUI.DOScale(2, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(500);
        await tfmCoinUI.DOScale(0, 0.3f);

    }

    private void ActionOnFinish()
    {
        Debug.Log("Countdown finished!");
    }
    // ============================================================================
    // START COUNTDOWN MODE (kim quay 360°)
    // ============================================================================
    [Button]
    public async UniTask StartCountTime(int startTime = 10, UnityAction actionOnFinish = null)
    {
        if (clockTimer == null)
        {
            Debug.LogWarning("ClockTimer not assigned!");
            return;
        }
        // chạy countdown
        await clockTimer.StartCountDownTime(startTime, async () =>
        {
            ShowFade();
            InputHandler.Instance.IsLockInput = true;

            await UniTask.WaitUntil(()=>CoinCollector.Instance.Completed);
            gobjEffectComplete.SetActive(true);
            AudioController.Instance.PlaySound(SoundName.LEVEL_BONUS_COMPLETE);
            await UniTask.Delay(1000);
            await ShowCoinEndScreen();
            AudioController.Instance.PlaySound(SoundName.CollectExp);

            HideFade();
            actionOnFinish?.Invoke();
        });
    }
}
