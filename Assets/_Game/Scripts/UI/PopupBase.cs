using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupBase : MonoBehaviour
{
    [SerializeField] protected Image imgFade;
    [SerializeField] protected Image imgContent;

    public virtual async UniTask Show()
    {
        imgFade.gameObject.SetActive(true);
        imgContent.gameObject.SetActive(true);
    }
    public virtual void Hide()
    {
        imgFade.gameObject.SetActive(false);
        imgContent.gameObject.SetActive(false);
    }
    
    public virtual void InitData(object data){}
    public virtual void InitData(params object[] data){ }
}
