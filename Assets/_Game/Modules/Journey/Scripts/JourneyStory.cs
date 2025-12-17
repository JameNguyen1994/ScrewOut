using Cysharp.Threading.Tasks;
using EasyButtons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace ps.modules.journey
{

    public class JourneyStory : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] private GameObject prbMark;
        [SerializeField] private List<GameObject> lstItemMark;
        [SerializeField] private JourneyItemLevel prbLevel;
        [SerializeField] private List<JourneyItemLevel> lstItemLevel;

        [SerializeField] private Transform tfmMarkParent;
        [SerializeField] private Transform tfmLevelParent;
        [SerializeField] private JourneyData journeyData;
        [SerializeField] private Image imgBG;
        [SerializeField] private TMP_Text txtName;
        [SerializeField] private TMP_Text txtLevel;
        [SerializeField] private TMP_Text txtLevelFly;
        [SerializeField] private RectTransform rtfmPath;
        [SerializeField] private RectTransform rtfmBG;
        [SerializeField] private RectTransform rtfmRoot;
        [SerializeField] private RectTransform rtfmPathFill;
        [SerializeField] private RectTransform rtfmLevelFlyHolder;
        [SerializeField] private VerticalLayoutGroup verticalLayout;
        [SerializeField] private ContentSizeFitter contentSizeFitter;

        [SerializeField] private GameObject gobjTail;
        [SerializeField] private GameObject gobjLock;
        [SerializeField] private GameObject gobjUnLock;


        [Header("Settings")]
        [SerializeField] private int spacingMarkLevel = 150;
        public JourneyData JourneyData => journeyData;
        public int ID => id;
        public async UniTask Init(int id, JourneyData journeyData, int currentLevel)
        {
            this.journeyData = journeyData;
            this.id = id;


            txtName.text = $"{journeyData.name}";
            txtLevel.text = $"Level {journeyData.levelStart + 1} - {journeyData.levelEnd}";
            imgBG.sprite = journeyData.sprBG;

            verticalLayout.padding.top = spacingMarkLevel;
            int countMarkLevel = (journeyData.levelEnd - journeyData.levelStart + 1) / 5;
            rtfmPath.sizeDelta = new Vector2(rtfmPath.sizeDelta.x, spacingMarkLevel * (countMarkLevel));


            var levelFill = currentLevel - journeyData.levelStart;
            var totalLevel = journeyData.levelEnd - journeyData.levelStart + 1;
            var fillAmount = (float)levelFill / (float)totalLevel;
            fillAmount = Mathf.Clamp01(fillAmount);
            var heightFill = rtfmPath.sizeDelta.y * fillAmount;
            rtfmPathFill.sizeDelta = new Vector2(rtfmPathFill.sizeDelta.x, heightFill);
            rtfmLevelFlyHolder.anchoredPosition = new Vector2(rtfmLevelFlyHolder.anchoredPosition.x, heightFill);
            for (int i = 0; i < countMarkLevel - 1; i++)
            {
                GameObject markObj = Instantiate(prbMark, tfmMarkParent);
                lstItemMark.Add(markObj);
            }

            for (int i = 0; i < journeyData.lstMarkLevel.Count; i++)
            {
                var levelData = journeyData.lstMarkLevel[i];
                var index = (levelData.level - journeyData.levelStart) / 5;
                //Debug.Log($"ID Level Mark: {lstItemMark.Count - id} - Level: {levelData.level}");
                var parent = lstItemMark[lstItemMark.Count - index].transform;
                var positionY = spacingMarkLevel * (lstItemMark.Count - index);
                positionY = 0;
                var itemLevel = Instantiate(prbLevel, parent);
                itemLevel.Init(levelData, positionY, currentLevel);
                lstItemLevel.Add(itemLevel);
            }
            bool showFly = currentLevel > journeyData.levelStart && currentLevel <= journeyData.levelEnd;
            txtLevelFly.text = $"{currentLevel}";
            rtfmLevelFlyHolder.gameObject.SetActive(showFly);
            gobjLock.SetActive(currentLevel < journeyData.levelStart);
            gobjUnLock.SetActive(currentLevel >= journeyData.levelStart);
        }
        public void OnChangeLevel(int currentLevel)
        {

            var levelFill = currentLevel - journeyData.levelStart;
            var totalLevel = journeyData.levelEnd - journeyData.levelStart + 1;
            var fillAmount = (float)levelFill / (float)totalLevel;
            fillAmount = Mathf.Clamp01(fillAmount);
            var heightFill = rtfmPath.sizeDelta.y * fillAmount;
            rtfmPathFill.sizeDelta = new Vector2(rtfmPathFill.sizeDelta.x, heightFill);
            rtfmLevelFlyHolder.anchoredPosition = new Vector2(rtfmLevelFlyHolder.anchoredPosition.x, heightFill);


            bool showFly = currentLevel >= journeyData.levelStart && currentLevel <= journeyData.levelEnd;
            txtLevelFly.text = $"{currentLevel}";
            rtfmLevelFlyHolder.gameObject.SetActive(showFly);
            gobjLock.SetActive(currentLevel < journeyData.levelStart);
            gobjUnLock.SetActive(currentLevel >= journeyData.levelStart);

            for (int i = 0; i < lstItemLevel.Count; i++)
            {
                var level = lstItemLevel[i];
                level.CheckLevel(currentLevel);
            }
        }
        public void EnableTail()
        {
            gobjTail.SetActive(true);
        }
        [Button]
        public void ShowStory()
        {
            rtfmPath.gameObject.SetActive(true);
            rtfmBG.gameObject.SetActive(true);
        }
        [Button]
        public void HideStory()
        {
            rtfmPath.gameObject.SetActive(false);
            rtfmBG.gameObject.SetActive(false);
        }
        public RectTransform GetRectTransform()
        {
            return rtfmRoot;
        }
        public void TurnOffContenSizeFitter()
        {
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        }
        public float GetCurrentSizeFill()
        {
            return rtfmPathFill.sizeDelta.y;
        }
    }
}
