using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Utils;
using ScriptsEffect;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUnlockBooster : Singleton<TutorialUnlockBooster>
{
    [SerializeField] private Image imgPopup;
    [SerializeField] private Image imgFade;
    [SerializeField] private Image imgBooster;
    [SerializeField] private Image imgTitle;
    [SerializeField] private List<Image> lstImgBoosterFly;
    [SerializeField] private List<GameObject> lstContent;
    [SerializeField] private Booster boosterTutorial;
    [SerializeField] private bool isShowing = false;
    [SerializeField] private Text txtTitle;
    [SerializeField] private Text txtContent;

    public bool IsShowing { get => isShowing; }

    public async UniTask StartTutorial(Booster booster)
    {
        if (booster.BoosterData.boosterType == BoosterType.Clears)
           await  LevelController.Instance.Get3ScrewNotMatch();
        isShowing = true;
        boosterTutorial = booster;
        imgFade.gameObject.SetActive(true);
        imgPopup.gameObject.SetActive(true);
        var sprite = BoosterDataHelper.Instance.GetBoosterData(booster.BoosterType).sprBooster;
        imgBooster.sprite = sprite;
        for (int i = 0; i < lstImgBoosterFly.Count; i++)
        {
            lstImgBoosterFly[i].sprite = sprite;
        }
        //txtTitle.text = $"{booster.BoosterData.title}".ToUpper();
        txtContent.text = $"{booster.BoosterData.content}";
        imgTitle.sprite = booster.BoosterData.sprTitle;
        //await imgPopup.rectTransform.DOAnchorPosX(0, 1f);
        await imgPopup.rectTransform.DOScale(Vector3.one, 0.3f).From(Vector3.zero);

    }
    public void OnClickGet()
    {
        GetBooster();
    }
    public async UniTask GetBooster()
    {
        // Làm mờ popup
        imgPopup.DOFade(0, 0.5f);
        imgFade.DOFade(0, 0.5f);
        // imgPopup.gameObject.SetActive(false);

        // Thời gian delay giữa các lần bay
        float delayBetweenBoosters = 0.2f;
        imgBooster.gameObject.SetActive(false);
        // Lặp qua danh sách các booster
        for (int i = 0; i < lstContent.Count; i++)
        {
            lstContent[i].gameObject.SetActive(false);
        }
        var lstTask = new List<UniTask>();
        for (int i = 0; i < lstImgBoosterFly.Count; i++)
        {
            var imgFly = lstImgBoosterFly[i];
            imgFly.gameObject.SetActive(true);
            imgFly.rectTransform.DOSizeDelta(new Vector2(150, 150), 1f);
            // Lấy vị trí ban đầu và vị trí đích
            var startPos = imgFly.transform.position;
            var targetPos = boosterTutorial.transform.position;

            // Tính điểm giữa (tạo đường vòng cung chỉ với trục x và y)
            var middlePos = new Vector3(
                (startPos.x + targetPos.x) / 2, // Điểm giữa trục x
                Mathf.Max(startPos.y, targetPos.y) + 2.0f, // Cao hơn để tạo vòng cung
                0 // Giữ nguyên z = 0
            );

            // Tạo đường path với điểm bắt đầu, giữa, và kết thúc
            Vector3[] path = { startPos, middlePos, targetPos };

            // Bay theo đường vòng cung
            lstTask.Add(imgFly.transform.DOPath(path, 1f, PathType.CatmullRom).OnComplete(()=>imgFly.gameObject.SetActive(false))
                .SetEase(Ease.InOutQuad)
                .SetDelay(i * delayBetweenBoosters).ToUniTask() // Thêm delay dựa trên thứ tự
            );
        }

        // Chờ một khoảng nhỏ để hiệu ứng hoàn thiện
        await UniTask.WhenAll(lstTask);
        for (int i = 0; i < lstImgBoosterFly.Count; i++)
        {
            lstImgBoosterFly[i].gameObject.SetActive(false);
        }
        imgFade.gameObject.SetActive(false);
        Debug.Log("All boosters completed.");
        boosterTutorial.BoosterUI.HighLightBooster(false);
        boosterTutorial.BoosterUI.ShowBannerTutorial(true);
        boosterTutorial.gameObject.transform.SetAsLastSibling();
        isShowing = false;
        imgPopup.gameObject.SetActive(false);

    }

}
