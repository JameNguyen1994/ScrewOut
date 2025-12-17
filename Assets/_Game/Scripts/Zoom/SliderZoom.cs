using Cysharp.Threading.Tasks;
using DG.Tweening;
using PS.Analytic;
using PS.Utils;
using Storage;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderZoom : Singleton<SliderZoom>
{
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Image fillImg;
    [SerializeField] private TextMeshProUGUI txtPercentZoom;
    [SerializeField] private Transform tfmContentBoxZoom;
    [SerializeField] private Transform tfmContentBoxColorMode;
    [SerializeField] private Button btnAdd;
    [SerializeField] private Button btnSub;
    [SerializeField] private Outline olbtnSub;
    [SerializeField] private Outline olbtnAdd;
    private PinchToZoom pinchToZoom;
    float valueChange = 0.15f;
    [SerializeField] float value = 0.5f;

    [SerializeField] private RectTransform rectContent;
    public Slider ZoomSlider { get => zoomSlider; }

    protected override void CustomAwake()
    {
        base.CustomAwake();
        olbtnAdd.enabled = false;
        olbtnSub.enabled = false;
        Hide();
    }

    private void Start()
    {
        //EventDispatcher.Register(EventId.OnToggleBanner, OnChangePos);

    }
    public void SetDefaultValue(float value)
    {
        this.value = value;
    }
    public void Hide()
    {
        tfmContentBoxZoom.localScale = Vector3.zero;
        tfmContentBoxColorMode.localScale = Vector3.zero;
    }
    public async UniTask ShowSlideZoom()
    {
        tfmContentBoxZoom.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public async UniTask ShowBoxColor()
    {
        tfmContentBoxColorMode.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
    }
    public async UniTask Initialize(PinchToZoom pinchToZoom, float initValue)
    {
        this.pinchToZoom = pinchToZoom;

        if (zoomSlider != null)
        {

            //zoomSlider.minValue = minMax.x;
            zoomSlider.minValue = 0;
            zoomSlider.maxValue = 1;
            zoomSlider.value = initValue;
            Debug.Log($"[SliderZoom] Initialize initValue: {initValue}");
            fillImg.fillAmount = initValue;

          await  UniTask.WaitForEndOfFrame();
            zoomSlider.onValueChanged.AddListener(OnSliderValueChanged);

            txtPercentZoom.text = $"Zoom: {(int)(zoomSlider.value * 100)}%";
            this.value = initValue;
        }


        //OnChangePos(false);
    }

    public void UpdateSlider(float value)
    {

        if (zoomSlider != null)
        {
            zoomSlider.value = value;
            fillImg.fillAmount = value;
            txtPercentZoom.text = $"Zoom: {(int)(zoomSlider.value * 100)}%";
            if (value == 0)
                btnSub.interactable = false;
            else if (value == 1)
                btnAdd.interactable = false;
            else
            {
                btnAdd.interactable = true;
                btnSub.interactable = true;
            }

        }
    }

    public void SetSliderValueIncrease()
    {
        value = Mathf.Clamp(value + valueChange, 0, 1);
        UpdateSlider(value);
    }

    public void SetSliderValueReduce()
    {
        value = Mathf.Clamp(value - valueChange, 0, 1);
        UpdateSlider(value);
    }

    private void OnSliderValueChanged(float value)
    {
        if (pinchToZoom != null)
        {
            

            pinchToZoom.SetZoom(value);
        }
    }

    public async void OnChangePos(object data)
    {
        var remote = GameAnalyticController.Instance.Remote();

        bool isPopupTutorial = data is bool && (bool)data;
        float offsetPopupTutorial = isPopupTutorial ? 230 : 0;

        bool isActiveBooster = IngameData.IS_SHOW_BOOSTER;
        float offsetBooster = isActiveBooster ? 200 : 0;
        bool isShowBanner = AdsController.Instance.IsShowBanner;
        var offsetBanner = isShowBanner ? 180 : 0;

        var posY = offsetPopupTutorial + offsetBooster + offsetBanner + 180;
        if (isPopupTutorial)
        {
            posY = 530;
        }
        Debug.Log($"isPopupTutorial: {isPopupTutorial} - {offsetPopupTutorial} - {offsetBooster} - {offsetBanner}");
        //rectContent.anchoredPosition = new Vector3(0, posY, 0);
        rectContent.DOAnchorPosY(posY, 0.3f).SetEase(Ease.Linear);
        Debug.Log($"AdsController.Instance.IsShowBanner: {AdsController.Instance.IsShowBanner}");
    }

    private void OnDisable()
    {
        //EventDispatcher.RemoveCallback(EventId.OnToggleBanner, OnChangePos);
    }
    public void HighLight2ButtonZoom(bool isHighLight)
    {

        Debug.Log($"HighLight2ButtonZoom: {isHighLight}");
        if (isHighLight)
        {
            btnAdd.transform.DOScale(Vector3.one * 1.35f, 0.3f).SetLoops(-1, LoopType.Yoyo);
            btnSub.transform.DOScale(Vector3.one * 1.35f, 0.3f).SetLoops(-1, LoopType.Yoyo);
         /*   olbtnAdd.enabled = true;
            olbtnSub.enabled = true;
            var color = Color.red;
            color.a = 0.5f;
            var colorSub = Color.red;
            olbtnAdd.DOColor(colorSub, 0.5f).SetLoops(-1, LoopType.Yoyo).From(color);
            olbtnSub.DOColor(colorSub, 0.5f).SetLoops(-1, LoopType.Yoyo).From(color);*/
        }
        else
        {
            olbtnAdd.enabled = false;
            olbtnSub.enabled = false;
            olbtnAdd.DOKill();
            olbtnSub.DOKill();
            btnAdd.transform.DOKill();
            btnSub.transform.DOKill();
            btnSub.transform.localScale = Vector3.one;
            btnAdd.transform.localScale = Vector3.one;
        }
    }
}
