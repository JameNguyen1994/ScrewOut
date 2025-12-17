using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatToolDatabase
{
    private const string key = "1232432354237754";

    private bool isEnabledCheat;

    public bool IsEnabledCheat
    {
        get => isEnabledCheat;
        set
        {
            isEnabledCheat = value;
            PlayerPrefs.SetInt(key, value? 1: 0);
        }
    }

    public CheatToolDatabase()
    {
        InitData();
    }

    void InitData()
    {
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, 0);
        }
        
        Load();
    }

    void Load()
    {
        isEnabledCheat = PlayerPrefs.GetInt(key) == 1;
    }
}
