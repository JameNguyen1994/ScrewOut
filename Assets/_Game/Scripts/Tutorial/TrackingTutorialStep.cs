using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PS.Analytic;
using Storage;
using UnityEngine;

public class TrackingTutorialStep : IStep
{
    [SerializeField] private string eventName;
    [SerializeField] private int step;
    [SerializeField] private bool isCompleted;
    
    public override async UniTask Execute()
    {
        TrackingController.Instance.TrackingTutorial(TUTORIAL_TYPE.OnBoarding);
    }
}