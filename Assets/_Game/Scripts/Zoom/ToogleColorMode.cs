using System;
using System.Collections;
using System.Collections.Generic;
using PS.Analytic;
using Storage;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToogleColorMode : MonoBehaviour, IDragHandler
{
    [SerializeField] private Button btnColor;
    [SerializeField] private Color sprOn;
    [SerializeField] private Color sprOff;
    [SerializeField] private ToggleZoomUIStep toggleZoomUIStep;


    private LevelController lvlCtrl;

    public UnityAction onClicked;

    public void Init(LevelController lvlCtrl)
    {
        this.lvlCtrl = lvlCtrl;
        InitUI();
    }

    void InitUI()
    {
        Debug.Log($"lvlCtrl.Level: {lvlCtrl == null}");
        bool isColorMode = Db.storage.IS_COLOR_MODE;
        btnColor.image.color = isColorMode ? sprOn : sprOff;
        lvlCtrl.Level.ChangeColorShape(isColorMode);
    }

    public void OnTriggerDown()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        Db.storage.IS_COLOR_MODE = !Db.storage.IS_COLOR_MODE;
        InitUI();
        toggleZoomUIStep.OnCompleteShow();
        onClicked?.Invoke();
       // GameAnalyticController.Instance.Tracking().TrackingChangeColorModel(Db.storage.IS_COLOR_MODE);
    }

    void OnTriggerUp()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
