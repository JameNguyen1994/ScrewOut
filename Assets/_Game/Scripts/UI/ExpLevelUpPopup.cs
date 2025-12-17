using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ExpLevelUpPopup : Singleton<ExpLevelUpPopup>
{
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private GameObject gobjFade, gobjContent;
    [SerializeField] private GameObject clickContinue, btnClickContinue;
    
    
    public static UnityAction onCallback;

    public bool IsShow;

    public void Show(int level)
    {
        txtLevel.text = $"{level}";
        
        gobjFade.SetActive(true);
        gobjContent.SetActive(true);

        IsShow = true;

        Invoke(nameof(ShowContinue), 2);
    }

    void ShowContinue()
    {
        clickContinue.SetActive(true);
        btnClickContinue.SetActive(true);
    }

    public void OnContinueClick()
    {
        gobjFade.SetActive(false);
        gobjContent.SetActive(false);
        clickContinue.SetActive(false);
        btnClickContinue.SetActive(false);
        onCallback?.Invoke();

        IsShow = false;
    }

    public static bool CheckShowUI()
    {
        return Instance != null && Instance.IsShow;
    }
}
