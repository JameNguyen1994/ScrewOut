using UnityEngine;
using UnityEngine.UI;

public class CoreRetentionCap : MonoBehaviour
{
    [SerializeField] private Image capTop;
    [SerializeField] private Image capTopRed;

    [SerializeField] private Image tagHard;

    [SerializeField] private Image capBottom;
    [SerializeField] private Image capBottomRed;

    [SerializeField] private Image levelThumnail;
    [SerializeField] private Image levelThumnailLock;

    [SerializeField] private StreamingImageBinder levelThumnailBinder;
    [SerializeField] private StreamingImageBinder levelThumnailLockBinder;

    [SerializeField] private Image glass;
    [SerializeField] private Image glassFront;
    [SerializeField] private GameObject glassEffect;
    [SerializeField] private GameObject effect;
    [SerializeField] private GameObject effectCircle;

    public RectTransform RectTransform;

    [SerializeField] private Coffee.UIExtensions.UIParticle effectDestroyGlass;
    [SerializeField] private RuntimeAnimatorController controller;
    [SerializeField] private GameObject animatorRoot;
    [SerializeField] private CanvasGroup canvasGroup;

    private Animator animator;
    private int currentLevelData;
    private int userLevelData;

    public bool IsBorderCap;

    public void UpdateData(int level, int userLevel)
    {
        if (level <= 0 || level >= userLevel + 6)
        {
            levelThumnail.transform.localScale = new Vector3(1, 1, 1);
            capTop.gameObject.SetActive(true);
            glass.gameObject.SetActive(true);
            glassFront.gameObject.SetActive(true);
            glassEffect.SetActive(true);
            effect.gameObject.SetActive(false);
            effectCircle.gameObject.SetActive(false);
            IsBorderCap = true;
            return;
        }

        IsBorderCap = false;

        EditorLogger.Log("[CoreRetentionCap] UpdateData [" + name + "] currentLevel: " + currentLevelData + " level " + level);

        currentLevelData = level;
        userLevelData = userLevel;

        LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(level);

        capTopRed.gameObject.SetActive(levelDifficulty == LevelDifficulty.Hard);
        tagHard.gameObject.SetActive(levelDifficulty == LevelDifficulty.Hard);
        capBottomRed.gameObject.SetActive(levelDifficulty == LevelDifficulty.Hard);

        bool isLock = currentLevelData > userLevelData;

        levelThumnailLock.gameObject.SetActive(isLock);
        levelThumnail.gameObject.SetActive(!isLock);

        string thumbnail = LevelMapService.GetLevelThumbnail(level);
        string thumbnailPath = $"LevelMap/{thumbnail}.png";

        if (!string.IsNullOrEmpty(thumbnail))
        {
            levelThumnailBinder.LoadFromStreaming(thumbnailPath);
            levelThumnailLockBinder.LoadFromStreaming(thumbnailPath);
        }
        else
        {
            EditorLogger.LogError("[CoreRetentionCap] Empty Thumbnail - level " + level);
        }

        capTop.gameObject.SetActive(currentLevelData >= userLevelData);
        glass.gameObject.SetActive(currentLevelData >= userLevelData);
        glassFront.gameObject.SetActive(currentLevelData >= userLevelData);
        glassEffect.SetActive(currentLevelData >= userLevelData);
        effect.gameObject.SetActive(currentLevelData < userLevelData);
        effectCircle.gameObject.SetActive(currentLevelData < userLevelData);

        CheckObjectScale();
    }

    public void CheckObjectScale()
    {
        if (!(currentLevelData >= userLevelData))
        {
            levelThumnail.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        }
        else
        {
            levelThumnail.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnClickCap()
    {
        if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
        {
            return;
        }

        if (CoreRetentionController.Instance.MenuElementController.IsHideElement)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        CoreRetentionController.Instance.MenuElementController.HideElement();
    }

    public void PlayEffectDestroyGlass()
    {
        AudioController.Instance.PlaySound(SoundName.CAP_EXPLOSION);
        effectDestroyGlass.Play();
        levelThumnail.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    }

    public void PlayAnim(string anim)
    {
        if (animator != null)
        {
            EndAnim();
        }

        animator = animatorRoot.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.Play(anim);
    }

    public void EndAnim()
    {
        DestroyImmediate(animator);
    }
}