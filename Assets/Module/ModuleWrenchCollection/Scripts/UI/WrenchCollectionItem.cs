using Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class WrenchCollectionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtReward;

    [SerializeField] private Image imgLine;
    [SerializeField] private Image imgIconReward;

    [SerializeField] private GameObject lockGO;
    [SerializeField] private GameObject doneGO;

    [SerializeField] private GameObject lineComplete;
    [SerializeField] private GameObject levelComplete;

    private int level;
    private WrenchCollectionRewardData reward;
    private bool isShowLine;
    private bool isLevelComplete;

    public int Level => level;

    public void UpdateData(int level, WrenchCollectionRewardData reward, bool isShowLine)
    {
        this.level = level;
        this.reward = reward;
        this.isShowLine = isShowLine;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        WrenchCollectionData data = Db.storage.WrenchCollectionData;

        txtLevel.text = (level + 1).ToString();
        imgIconReward.sprite = reward.GetIcon();
        txtReward.text = reward.RewardAmoutString();

        lockGO.SetActive(data.level <= level);
        doneGO.SetActive(data.claimRewars.Contains(level));
        levelComplete.SetActive(data.level > level);

        isLevelComplete = data.level > level;

        if (isShowLine)
        {
            lineComplete.gameObject.SetActive(data.level > level);
            imgLine.gameObject.SetActive(true);
        }
        else
        {
            lineComplete.gameObject.SetActive(false);
            imgLine.gameObject.SetActive(false);
        }
    }

    public void OnClickClaim()
    {
        ClaimHandler();
    }

    private async void ClaimHandler()
    {
        if (isLevelComplete && !WrenchCollectionService.IsClaimed(level))
        {
            AudioController.Instance.PlaySound(SoundName.Click);

            WrenchCollectionService.ClaimReward(level);
            WrenchCollectionController.Instance.IsHaveReward = true;
            WrenchCollectionController.Instance.OnClickCancel();
            await UniTask.Delay(500);
            UpdateUI();
        }
    }
}