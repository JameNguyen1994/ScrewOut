using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupWinLikeKing : PopupBase
{
    [SerializeField] private MinigameBar miniGame;
    [SerializeField] private Transform title;
    [SerializeField] private GameObject pillow;
    [SerializeField] private GameObject gojPillowFx;
    [SerializeField] private Image imgFlyStar;
    [SerializeField] private Image imgFlyBoxExp;
    
    [SerializeField] private GameObject completeFx1, completeFx2;
    

    [SerializeField] private Transform coinShelf;
    [SerializeField] private Transform boxExpShelf;
    [SerializeField] private GameObject expFx;
    
    
    [SerializeField] private Transform btnContinue, btnContinueReward;
    [SerializeField] private TextMeshProUGUI txtCoinWin, txtExpBoxWin;
    [SerializeField] private Text txtRewardValue;

    private float valueMiniBar = 0;
    
    private bool isShowing = false;
    public bool IsShowing => isShowing;

    public void Init()
    {
        if (isShowing)
        {
            return;
        }

        var userInfo = Db.storage.USER_INFO;
        userInfo.level++;
        Db.storage.USER_INFO = userInfo;
        
        title.localScale = Vector3.zero;
        miniGame.transform.localScale = Vector3.zero;
        pillow.SetActive(false);
        gojPillowFx.SetActive(false);
        coinShelf.localScale = Vector3.zero;
        boxExpShelf.localScale = Vector3.zero;
        btnContinue.localScale = Vector3.zero;
        btnContinueReward.localScale = Vector3.zero;
        expFx.SetActive(false);
        
        isShowing = true;
        txtCoinWin.text = $"+{GameConfig.COIN_WIN}";
        txtExpBoxWin.text = $"{LevelController.Instance.Level.LstScrew.Count / 3}";
        miniGame.OnValueChanged = OnMiniGameValueChanged;
        miniGame.Setup();
        InitAsync().Forget();
        
    }

    private void OnMiniGameValueChanged(float value)
    {
        txtRewardValue.text = $"x{value}";
    }

    async UniTask InitAsync()
    {
        completeFx1.SetActive(true);
        completeFx2.SetActive(true);
        await UniTask.Delay(500);
        title.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        await UniTask.Delay(150);
        pillow.SetActive(true);
        await UniTask.Delay(1400);
        completeFx1.SetActive(false);
        completeFx2.SetActive(false);
        gojPillowFx.SetActive(true);
        coinShelf.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        await miniGame.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        miniGame.StartEvent();
        btnContinue.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        btnContinueReward.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnContinueClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        DOContinue(false).Forget();
    }

    public void OnContinueRewardClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        
        AdsController.Instance.ShowRewardAds(RewardAdsPos.win, () =>
        {
            //Debug.Log("Rewarded");
            DOContinue(true).Forget();
        }, null, null, "win");
        
        // DOContinue(true).Forget();
    }

    async UniTask DOContinue(bool isReward)
    {
        UITopController.Instance.OnShowWinGame();
        miniGame.OnStopEvent( out valueMiniBar);

        valueMiniBar = isReward ? miniGame.GetValue() : 1;

        int coinReceived = (int)valueMiniBar * GameConfig.COIN_WIN;
        txtCoinWin.text = $"+{coinReceived}";

        btnContinueReward.DOScale(Vector3.zero, 0.3f);
        miniGame.transform.DOScale(Vector3.zero, 0.3f);
        title.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
        await btnContinue.DOScale(Vector3.zero, 0.3f);

        await UniTask.Delay(250);
        
        
        var targetPos = UITopController.Instance.GetStarPos();
        var startPos = imgFlyStar.transform.position;
        
        var middlePos = new Vector3(
            Mathf.Max(startPos.x, targetPos.x) + 1.5f, // Cao hơn để tạo vòng cung
            (startPos.y + targetPos.y) / 2, // Điểm giữa trục x
            0 // Giữ nguyên z = 0
        );
        
        Vector3[] path = { startPos, middlePos, targetPos };
        imgFlyStar.transform.SetParent(UITopController.Instance.FxParent);
        imgFlyStar.rectTransform.DORotate(new Vector3(0, 0, 720), 1f, RotateMode.FastBeyond360);
        imgFlyStar.rectTransform.DOSizeDelta(new Vector2(100, 100), 1f);
        pillow.SetActive(false);
        imgFlyStar.gameObject.SetActive(true);
        
        
        
        await imgFlyStar.transform.DOPath(path, 1f, PathType.CatmullRom).SetEase(Ease.InOutQuad);
        EventDispatcher.Push(EventId.OnIncreaseStarWithFx, 1);
        imgFlyStar.gameObject.SetActive(false);
        AudioController.Instance.PlaySound(SoundName.Star);

        await UniTask.Delay(500);
        
        coinShelf.localScale = Vector3.zero;
        var coinTargetPos  = UITopController.Instance.GetCoinPos();
        await FlyEffectController.Instance.DOFlyCoin(coinReceived, coinShelf.transform.position,
            coinTargetPos);

        await UniTask.Delay(1000);
        int expReceived = (LevelController.Instance.Level.LstScrew.Count / 3) * GameAnalyticController.Instance.Remote().ExpCompleteBox;
        txtExpBoxWin.text = $"{expReceived}";
        await boxExpShelf.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        expFx.SetActive(true);
        await UniTask.Delay(1000);
        boxExpShelf.localScale = Vector3.zero;
        await FlyBoxExp(expReceived);

    }
    
    async UniTask FlyBoxExp(int expReceived)
    {
        var startPos = imgFlyBoxExp.transform.position;
        var targetPos = UITopController.Instance.GetExpUserPos().position;
        
        var middlePos = new Vector3(
            Mathf.Max(startPos.x, targetPos.x) - 1.5f, // Cao hơn để tạo vòng cung
            (startPos.y + targetPos.y) / 2, // Điểm giữa trục x
            0 // Giữ nguyên z = 0
        );
        
        Vector3[] path = { startPos, middlePos, targetPos };
        imgFlyBoxExp.transform.SetParent(UITopController.Instance.FxParent);
        imgFlyBoxExp.rectTransform.DORotate(new Vector3(0, 0, -720), 1f, RotateMode.FastBeyond360);
        imgFlyBoxExp.rectTransform.DOSizeDelta(new Vector2(100, 100), 1f);
        imgFlyBoxExp.gameObject.SetActive(true);
        ExpLevelUpPopup.onCallback = () =>
        {
            GoToNextLevel().Forget();
        };
        
        await imgFlyBoxExp.transform.DOPath(path, 0.7f, PathType.CatmullRom).SetEase(Ease.InOutQuad);
        
        // print($"[win popup] screw count: {LevelController.Instance.Level.LstScrew.Count}");
        
        EventDispatcher.Push(EventId.OnIncreaseExpBoxWithFx, expReceived);
        imgFlyBoxExp.gameObject.SetActive(false);
    }

    async UniTask GoToNextLevel()
    {
        var scene = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
        await UniTask.Delay(1500);
        UITopController.Instance.OnStartGameplay();
        SceneController.Instance.ChangeScene(scene);
    }

#if UNITY_EDITOR
    [ContextMenu("Show")]
    void CmShow()
    {
        Init();
        Show();
    }
#endif
    
}
