using Beebyte.Obfuscator;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public interface ISceneController
{
    UniTask ChangeScene(SceneType _sceneType, UnityAction onCompleteFade = null);
}
public class SceneController : Singleton<SceneController>, ISceneController
{
    [SerializeField] private Image imgFadeUI;
    public UnityAction callBackLoadScreen;
    [SerializeField] private SceneType previousScene;
    [SerializeField] private SceneType currentScene;


    public SceneType PreviousScene => previousScene;
    public SceneType CurrentScene => currentScene;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }



    private void Start()
    {
        currentScene = previousScene = SceneType.MainMenu;

        imgFadeUI.gameObject.SetActive(true);
        imgFadeUI.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            imgFadeUI.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                imgFadeUI.gameObject.SetActive(false);
            });
        });
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        callBackLoadScreen?.Invoke();
        callBackLoadScreen = FadeOutBlackScreen;

    }

    public async UniTask ChangeScene(SceneType _sceneType, UnityAction onCompleteFade = null)
    {
        previousScene = currentScene;
        currentScene = _sceneType;
/*        imgFadeUI.gameObject.SetActive(true);
        imgFadeUI.DOFade(0, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            onCompleteFade?.Invoke();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                SceneManager.LoadScene($"{currentScene}");
            });
        });
*/
        await LoadingFade.Instance.ShowLoadingFade();
        SceneManager.LoadScene($"{currentScene}");
        if (_sceneType == SceneType.MainMenu || _sceneType == SceneType.GamePlayNewControl)
        {
            return;
        }
        await LoadingFade.Instance.HideLoadingFade();
    }

    public async UniTask ChangeSceneOnly(SceneType _sceneType, UnityAction onCompleteFade = null)
    {
        previousScene = currentScene;
        currentScene = _sceneType;
        SceneManager.LoadScene($"{currentScene}");
    }

    public void FadeOutBlackScreen()
    {
        imgFadeUI.DOFade(0, 0.5f).SetEase(Ease.Linear).OnComplete(() => { imgFadeUI.gameObject.SetActive(false); });
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
public enum SceneType
{
    [SkipRename] Loading =0,
    [SkipRename] MainMenu =1,
    [SkipRename] Gameplay =3,
    [SkipRename] GamePlayNewControl = 2

}