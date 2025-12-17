using Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupReviveOld : MonoBehaviour
{
    [SerializeField] private Text txtContent;
    public void Init()
    {
        var level = Db.storage.USER_INFO.level;
        txtContent.text = $"Level {level} Lose!";
    }
}
