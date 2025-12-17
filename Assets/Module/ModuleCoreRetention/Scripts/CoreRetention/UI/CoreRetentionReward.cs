using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Storage.Model;
using Storage;
using Spin;

public class CoreRetentionReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtObjectName;
    [SerializeField] private Image thumnail;
    [SerializeField] private Image thumnailLock;
    [SerializeField] private Image frame;

    [SerializeField] private Sprite normalLevel;
    [SerializeField] private Sprite hardLevel;

    [SerializeField] private RewardView reward1;
    [SerializeField] private RewardView reward2;
    [SerializeField] private RewardView reward3;
    [SerializeField] private RewardView reward4;

    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private GameObject lockPanel;

    [SerializeField] private GameObject rewardNormal;
    [SerializeField] private GameObject rewardComplete;

    [SerializeField] private GameObject reward1Complete;
    [SerializeField] private GameObject reward3Complete;
    [SerializeField] private GameObject reward4Complete;

    private int totalScrew;
    private int level;

    public void UpdateUI(int level, int userLevel)
    {
        this.level = level;
        bool isLock = level > userLevel;
        LevelDifficulty levelDifficulty = LevelMapService.GetLevelDifficulty(level);

        thumnail.sprite = CoreRetentionService.GetLevelThumnail(level);
        thumnailLock.sprite = CoreRetentionService.GetLevelThumnail(level);

        txtObjectName.text = LevelMapService.GetLevelName(level);

        frame.sprite = levelDifficulty == LevelDifficulty.Hard ? hardLevel : normalLevel;

        unlockPanel.SetActive(!isLock);
        lockPanel.SetActive(isLock);

        if (!isLock)
        {
            rewardNormal.SetActive(level == userLevel);
            rewardComplete.SetActive(level != userLevel);
        }

        totalScrew = LevelMapService.GetLevelTotalScrew(level);
        int cup = ps.modules.leaderboard.LeaderBoardService.GetCupByLevel(levelDifficulty,level);

        reward1.UpdateUI(cup);
        reward2.UpdateUI(GameConfig.COIN_WIN);
        reward3.UpdateUI(totalScrew);

        reward1.gameObject.SetActive(cup > 0);
        reward1Complete.gameObject.SetActive(cup > 0);

        reward3.gameObject.SetActive(SpinService.IsUnlock(level));
        reward3Complete.gameObject.SetActive(SpinService.IsUnlock(level));

        UpdateEXPReward();
    }

    public void UpdateEXPReward()
    {
        int exp = (totalScrew / 3) * PS.Analytic.GameAnalyticController.Instance.Remote().ExpCompleteBox;
        reward4.UpdateUI(exp);
    }

    public void OnClickShowElement()
    {
        if (!CoreRetentionController.Instance.MenuElementController.IsHideElement)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.Click);
        CoreRetentionController.Instance.MenuElementController.ShowElement();
    }
}