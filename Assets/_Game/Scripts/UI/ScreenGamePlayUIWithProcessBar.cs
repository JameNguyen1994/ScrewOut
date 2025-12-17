using System;
using System.Collections;
using System.Collections.Generic;
using Storage;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenGamePlayUIWithProcessBar : MonoBehaviour
{
    [SerializeField] private ProcessLevelBar processLevelBar;

    private void Start()
    {
        int lvl = Db.storage == null ? Random.Range(1, 100) : Db.storage.USER_INFO.level;
        //processLevelBar.InitUI(lvl);
    }
}
