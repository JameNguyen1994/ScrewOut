using DG.Tweening;
using Storage;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ps.modules.journey
{
    [System.Serializable]
    public class JourneyDB
    {
        public List<int> lstLevelClaim;
    }
    public class JourneyItemLevel : MonoBehaviour
    {
        [SerializeField] private JourneyMarkLevel journeyMarkLevel;
        [SerializeField] private TMP_Text txtLevel;
        [SerializeField] private RectTransform root;
        [SerializeField] private Image imgLeft;
        [SerializeField] private Image imgRight;
        [SerializeField] private Sprite sprLeftComplete;
        [SerializeField] private Sprite sprLeftNotComplete;
        [SerializeField] private Sprite sprRightComplete;
        [SerializeField] private Sprite sprRightNotComplete;

        [SerializeField] private ItemGiftJourney itemGiftJourney;
        [SerializeField] private RectTransform rtfmCheck;
        [SerializeField] private Button btnClaim;
        public void Init(JourneyMarkLevel journeyMarkLevel, float posY, int currentLevel)
        {
            this.journeyMarkLevel = journeyMarkLevel;
            txtLevel.text = $"{journeyMarkLevel.level}";
            root.anchoredPosition = new Vector2(0, posY);
            if (journeyMarkLevel.level < currentLevel)
            {
                imgLeft.sprite = sprLeftComplete;
                imgRight.sprite = sprRightComplete;

                var db = Db.storage.JOURNEY_DB;
                if (db.lstLevelClaim.Contains(journeyMarkLevel.level))
                {
                    rtfmCheck.gameObject.SetActive(true);
                }
                else
                {
                    rtfmCheck.gameObject.SetActive(false);
                    btnClaim.gameObject.SetActive(true);
                }
            }
            else
            {
                imgLeft.sprite = sprLeftNotComplete;
                imgRight.sprite = sprRightNotComplete;
                rtfmCheck.gameObject.SetActive(false);
            }

            itemGiftJourney.SetData(journeyMarkLevel.lstReward);
        }
        public void OnClickGetReward()
        {
            var db = Db.storage.JOURNEY_DB;
            if (db.lstLevelClaim.Contains(journeyMarkLevel.level))
            {
                Debug.LogError("Reward Getted");
            }
            else
            {
                rtfmCheck.gameObject.SetActive(true);
                rtfmCheck.localScale = Vector3.zero;
                rtfmCheck.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                btnClaim.gameObject.SetActive(false);
                itemGiftJourney.GetGift();
            }
            db.lstLevelClaim.Add(journeyMarkLevel.level);
            Db.storage.JOURNEY_DB = db;
        }
        public void CheckLevel(int level)
        {
            if (journeyMarkLevel.level < level)
            {
                imgLeft.sprite = sprLeftComplete;
                imgRight.sprite = sprRightComplete;
                var db = Db.storage.JOURNEY_DB;
                if (db.lstLevelClaim.Contains(journeyMarkLevel.level))
                {
                    rtfmCheck.gameObject.SetActive(true);
                    btnClaim.gameObject.SetActive(false);
                }
                else
                {
                    rtfmCheck.gameObject.SetActive(false);
                    btnClaim.gameObject.SetActive(true);
                }
            }
            else
            {
                imgLeft.sprite = sprLeftNotComplete;
                imgRight.sprite = sprRightNotComplete;
                rtfmCheck.gameObject.SetActive(false);
                btnClaim.gameObject.SetActive(false);
            }
        }
    }
}
