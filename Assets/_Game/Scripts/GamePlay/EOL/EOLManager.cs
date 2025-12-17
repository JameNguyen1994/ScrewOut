using System.Collections;
using System.Collections.Generic;
using PS.Utils;
using UnityEngine;

public class EOLManager : Singleton<EOLManager>
{
    [SerializeField] private GameObject eollvl10Led1, eollvl10Led2, eollvl10discoLed;

    public GameObject EolLvl10Led1 => eollvl10Led1;
    public GameObject EolLvl10Led2 => EolLvl10Led2;
    public GameObject EolLvl10DiscoLed => eollvl10discoLed;

    public void SetActiveLevel10()
    {
        eollvl10Led1.SetActive(true);
        eollvl10Led2.SetActive(true);
        eollvl10discoLed.SetActive(true);
    }
}
