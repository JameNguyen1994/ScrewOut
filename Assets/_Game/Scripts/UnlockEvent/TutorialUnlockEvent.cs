using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUnlockTab : Singleton<TutorialUnlockTab>
{
    [SerializeField] private HoleEffect holeEffect;
    [SerializeField] private Button btnTest;
    [SerializeField] private Button btnSmallImage;
    [SerializeField] private bool isCompleted = false;
    [SerializeField] private ImageHighLightType imageHighLightTypeTest;

    protected override void CustomAwake()
    {
        base.CustomAwake();
        holeEffect.gameObject.SetActive(false);
        isCompleted = true;
    }
    [Button]
    public void TestShowHoleEffect()
    {
        StartTutorial(btnTest, imageHighLightTypeTest);
    }
    public async UniTask StartTutorial(Button button, ImageHighLightType imageHighLightType,float delay = 0f)
    {
        if (!isCompleted)
            return;
        btnSmallImage.onClick.RemoveAllListeners();

        isCompleted = false;
        var pos = button.transform.position;
        btnSmallImage.transform.position = pos;
        var size = button.GetComponent<RectTransform>().sizeDelta;

        holeEffect.Init(size, pos, imageHighLightType);
        holeEffect.gameObject.SetActive(true);
        btnSmallImage.gameObject.SetActive(true);

        var countEvent = button.onClick.GetPersistentEventCount();
        for (int i = 0; i < countEvent; i++)
        {
            var target = button.onClick.GetPersistentTarget(i);
            var methodName = button.onClick.GetPersistentMethodName(i);
            Debug.Log($"Target: {target}, MethodName: {methodName}");
            if (target != null)
            {
                btnSmallImage.onClick.AddListener(() =>
                {
                    var method = target.GetType().GetMethod(methodName);
                    if (method != null)
                    {
                        method.Invoke(target, null);
                    }
                });
            }
        }
        btnSmallImage.onClick.AddListener(() =>
        {
            isCompleted = true;
        });

        
        await UniTask.WaitUntil(() => isCompleted);
        await UniTask.WaitForSeconds(delay);
    }
    [Button]
    public void TestHideHoleEffect()
    {
        holeEffect.gameObject.SetActive(false);
    }
}
