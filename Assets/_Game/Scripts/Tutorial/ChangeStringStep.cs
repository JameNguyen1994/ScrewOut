using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStringStep : IStep
{
    [SerializeField] private string content;
    [SerializeField] private Text txtContent;
    public override async UniTask Execute()
    {
        txtContent.text = content;
    }
}
