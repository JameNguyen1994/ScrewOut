using Cysharp.Threading.Tasks;
using DG.Tweening;
using Life;
using PS.Ad;
using Storage;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using PS.Analytic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingScreen : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private float startValue;
    [SerializeField] private Image imgLoadingFill;
    [SerializeField] private AnimationCurve curveLoadingBar;
    [SerializeField] private float timeLogoAnimation = 4.5f;
    

    private void Awake()
    {
        //   SceneController.Instance.gameObject.SetActive(true);
        imgLoadingFill.fillAmount = startValue;
        // Time.timeScale = 10;
    }
    
    // Start is called before the first frame update
    private async void Start()
    {
        Application.targetFrameRate = 120;

        float timeLoad = Time.time;
        await UniTask.Delay(10);

        var task1 = UniTask.WaitForSeconds(3);
        var task2 = UniTask.WaitUntil(() => GameAnalyticController.Instance.Remote().IsReadyRemote);

        await UniTask.WhenAny(task1, task2);

        var task3 = UniTask.WaitUntil(() => AssetReferenceController.Instance.IsCompleted);

        await task3;

        //adsController.StartAdByLevel(userInforController.GetValueByType(Storage.Model.UserInfoType.Level));
        await imgLoadingFill.DOFillAmount(1f, waitTime).SetEase(curveLoadingBar);
        
        InitOfferwall();
       // LoadingFade.Instance.ShowLoadingFade();
        while (Time.time - timeLoad < timeLogoAnimation)
        {
            await UniTask.DelayFrame(1);
        }


        LifeController.Instance.Init();
        await UniTask.WaitUntil(() => LifeController.Instance.Inited);
        /*        bool hasHeart = DBLifeController.Instance.LIFE_INFO.lifeAmount > 0 || DBLifeController.Instance.LIFE_INFO.timeInfinity > 0;
                AudioController.Instance.PlayMusic(SoundName.Music, true, 0.5f);
                if (hasHeart)
                {
                    SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);

                }
                else
                {
                    SceneController.Instance.ChangeScene(SceneType.MainMenu);
                }*/

        if (SerializationManager.HaveDataToReloadGamePlay())
        {
            if (PlayerPrefsSaveTime.GetMinutesSinceLastSave() < Define.AUTO_RELOAD_TIME)
            {
                await ReloadGamePlayAsync(SceneType.GamePlayNewControl);
                return;
            }

            bool userConfirmed = await SerializationManager.ShowConfirmAsync(Define.RELOAD_CONFIRM);

            if (userConfirmed)
            {
                await ReloadGamePlayAsync(SceneType.GamePlayNewControl);
                return;
            }

            SerializationManager.ClearAllDataInGamePlay();
        }

        if (Db.storage.USER_INFO.level<=3)
        {
            SceneController.Instance.ChangeScene(SceneType.GamePlayNewControl);
        }
        else
            SceneController.Instance.ChangeScene(SceneType.MainMenu);
        
    }

    void InitOfferwall()
    {
        var remote = GameAnalyticController.Instance.Remote();
        string offerwallData = remote.OfferWallRemote;
        
        OfferwallController.Instance.SetRemoteData(offerwallData);
        OfferwallController.Instance.Initialize();
    }

    private async UniTask ReloadGamePlayAsync(SceneType sceneType)
    {
        await LoadingFade.Instance.ShowLoadingFade();
        //LoginController.Instance.LoginOrRegister();
        await SceneController.Instance.ChangeSceneOnly(sceneType);
    }
}