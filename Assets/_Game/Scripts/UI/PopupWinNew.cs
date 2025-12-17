using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptsEffect;
using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWinNew : PopupBase
{
    [SerializeField] private MinigameBar minigameBar;
    [SerializeField] private SimpleAnimationCanvas animationWellDone;
    [SerializeField] private SimpleAnimationCanvas animationLevelComplete;
    [SerializeField] private EffectTextWare effectTextWare;
    [SerializeField] private GameObject gobjAfterLevelComplete;
    [SerializeField] private Image imgFlyStar;
    [SerializeField] private ParticleSystem parFlare;
    [SerializeField] private ParticleSystem parFire;

    [SerializeField] private ParticleSystem parLevelComplete1;
    [SerializeField] private ParticleSystem parLevelComplete2;

    [SerializeField] private float valueMiniBar;
    [SerializeField] private Text txtValue;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnX;
    [SerializeField] private RectTransform rectAddCoin;
    [SerializeField] private Image imgWellDone;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] private Image imgCoin;

    [SerializeField] private Text txtCoin;
    [SerializeField] private Text txtLevel;
    [SerializeField] private bool showing = false;

    float finalValueBar = 0;

    public bool Showing { get => showing; }



    public async UniTask Init()
    {
        if (showing)
            return;
        showing = true;
        rectAddCoin.gameObject.SetActive(false);
        btnContinue.gameObject.SetActive(false);
        minigameBar.gameObject.SetActive(false);
        animationWellDone.gameObject.SetActive(false);
        imgWellDone.gameObject.SetActive(false);
        btnX.gameObject.SetActive(false);
        gobjAfterLevelComplete.SetActive(false);
        txtLevel.gameObject.SetActive(false);
        animationLevelComplete.gameObject.SetActive(true);
        parLevelComplete1.gameObject.SetActive(true);
        parLevelComplete2.gameObject.SetActive(true);

        // AudioController.Instance.PlaySound(SoundName.Win_Popup);
        await animationLevelComplete.StartAnimation(1);
        // effectTextWare.gameObject.SetActive(true);
        // await effectTextWare.StartAnimationText();
        await UniTask.Delay(1500);
        parLevelComplete1.gameObject.SetActive(false);
        parLevelComplete2.gameObject.SetActive(false);
        animationLevelComplete.gameObject.SetActive(false);
        gobjAfterLevelComplete.SetActive(true);

        txtCoin.text = $"+{GameConfig.COIN_WIN}";
        txtLevel.text = $"Level {Db.storage.USER_INFO.level} passed";
        minigameBar.Setup();


        await animationWellDone.StartAnimation(1, 0.03f);
        imgWellDone.gameObject.SetActive(true);
        parFlare.Play();

        await imgWellDone.rectTransform.DOScale(Vector2.one, 0.03f * 28).From(Vector2.zero);
        txtLevel.gameObject.SetActive(true);

        //await UniTask.Delay(300);

        parFire.Play();

        await UniTask.Delay(500);
        UITopController.Instance.OnShowWinGame();

        rectAddCoin.gameObject.SetActive(true);

        minigameBar.gameObject.SetActive(true);
        minigameBar.StartEvent();

        await UniTask.Delay(500);
        btnContinue.gameObject.SetActive(true);
        btnContinue.image.rectTransform.DOScale(Vector2.one, 0.3f).From(Vector2.zero);
        // UniTask.Delay(200);

        btnX.gameObject.SetActive(true);
        await btnX.image.rectTransform.DOScale(Vector2.one, 0.3f).From(Vector2.zero);

    }
    public async UniTask AfterContinue()
    {
        targetPos = UITopController.Instance.GetStarPos();

        /*        btnContinue.interactable = false;
                btnX.interactable = false;*/
        imgWellDone.DOFade(0, 0.3f);
        rectAddCoin.gameObject.SetActive(false);
        btnContinue.gameObject.SetActive(false);
        btnX.gameObject.SetActive(false);
        minigameBar.gameObject.SetActive(false);
        parFlare.gameObject.SetActive(false);
        //imgFade.DOFade(0, 0.3f);
        animationWellDone.gameObject.SetActive(false);
        // imgFlyStar.DoMove;
        var startPos = imgFlyStar.transform.position;

        // Tính điểm giữa (tạo đường vòng cung chỉ với trục x và y)
        var middlePos = new Vector3(
            Mathf.Max(startPos.x, targetPos.x) + 1.5f, // Cao hơn để tạo vòng cung
            (startPos.y + targetPos.y) / 2, // Điểm giữa trục x
            0 // Giữ nguyên z = 0
        );

        // Tạo đường path với điểm bắt đầu, giữa, và kết thúc
        Vector3[] path = { startPos, middlePos, targetPos };
        imgFlyStar.rectTransform.DORotate(new Vector3(0, 0, 720), 1f, RotateMode.FastBeyond360);
        imgFlyStar.rectTransform.DOSizeDelta(new Vector2(100, 100), 1f);
        imgFlyStar.gameObject.SetActive(true);
        // Bay theo đường vòng cung
        imgCoin.gameObject.SetActive(false);
        txtLevel.gameObject.SetActive(false);
        await imgFlyStar.transform.DOPath(path, 1f, PathType.CatmullRom).OnComplete(() => imgFlyStar.gameObject.SetActive(false))
             .SetEase(Ease.InOutQuad);
        AudioController.Instance.PlaySound(SoundName.Star);

        Vector3 coinTarget = imgCoin.transform.position;
        coinTarget.x += 100f;

        Vector3 expTarget = imgCoin.transform.position;
        expTarget.x -= 100f;

        EventDispatcher.Push(EventId.MakeCoinFly, coinTarget);
        EventDispatcher.Push(EventId.MakeExpFly, expTarget);

        await UniTask.Delay(200);
        EventDispatcher.Push(EventId.UpdateCoinUI, GameConfig.COIN_WIN * (int)finalValueBar);
      //  ExpBar.Instance.AddExp();

        var userInfo = Db.storage.USER_INFO;
        userInfo.level++;
        userInfo.star++;
        Db.storage.USER_INFO = userInfo;
        UITopController.Instance.UpdateStartUI();

        await UniTask.Delay(1000);
        //
        // await UniTask.Delay(1500);
        //
        var scene = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
        await UniTask.Delay(1500);
        UITopController.Instance.OnStartGameplay();
        SceneController.Instance.ChangeScene(scene);
    }
    private void Update()
    {
        if (minigameBar.CanTouch)
        {
            valueMiniBar = minigameBar.GetValue();
            txtValue.text = $"x{valueMiniBar}";
        }
    }
    public void OnClickContinue()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        minigameBar.OnStopEvent(out valueMiniBar);
        valueMiniBar = 1;
        AddCoin(valueMiniBar);
        AfterContinue();

    }


    void AddCoin(float valueBar)
    {
        finalValueBar = valueBar;
        int coin = GameConfig.COIN_WIN * (int)valueBar;
        /*        var user = Db.storage.USER_INFO;
                user.coin += coin;
                Db.storage.USER_INFO = user;*/

        Debug.Log($"Coin {coin}");
    }
    public void OnClickXValue()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        AdsController.Instance.ShowRewardAds(RewardAdsPos.win, () =>
        {
            minigameBar.OnStopEvent(out valueMiniBar);
            valueMiniBar = minigameBar.GetValue();
            AddCoin(valueMiniBar);

            AfterContinue();
        }, null, null, "win");

    }
    /*public void OnNextClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        var userInfo = Db.storage.USER_INFO;
        userInfo.level++;
        Db.storage.USER_INFO = userInfo;
        var scene = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
        SceneController.Instance.ChangeScene(scene);
    }*/
}
