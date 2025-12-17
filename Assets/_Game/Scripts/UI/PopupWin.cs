using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWin : MonoBehaviour
{
    [SerializeField] private Text txtContent;
    [SerializeField] private ProcessLevelBarData data;
    [SerializeField] private Image imgIcon;

    public void Init()
    {
        var level = Db.storage.USER_INFO.level;
        txtContent.text = $"Level {level} Completed!";

        if (data != null)
        {
            DoSetupIconWin(level);
        }
        
    }

    void DoSetupIconWin(int lvl)
    {
        int index = lvl % data.lstData.Count == 0 ? data.lstData.Count - 1 : lvl % data.lstData.Count - 1;
        imgIcon.sprite = data.lstData[index].icon;
    }
    
    public void OnNextClick()
    {
        AudioController.Instance.PlaySound(SoundName.Click);

        var userInfo = Db.storage.USER_INFO;
        userInfo.level++;
        Db.storage.USER_INFO = userInfo;
        var scene = IngameData.MODE_CONTROL == ModeControl.ControlV2 ? SceneType.GamePlayNewControl : SceneType.Gameplay;
        SceneController.Instance.ChangeScene(scene);
    }
}
