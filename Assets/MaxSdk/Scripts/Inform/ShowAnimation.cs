using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Ad.Utils
{
    public class ShowAnimation : MonoBehaviour, IAnimationInform
    {
        [SerializeField] private GameObject gobjIcon, gobjLoadingTxt;
        public UnityAction onCompleted { get; set; }
        public void Open()
        {
            gobjIcon.SetActive(true);
            gobjLoadingTxt.SetActive(true);
            StartCoroutine(WaitForCallCompletedOpen());
        }

        IEnumerator WaitForCallCompletedOpen()
        {
            yield return new WaitForSecondsRealtime(1);
            onCompleted?.Invoke();
        }

        public void Close()
        {
            gobjIcon.SetActive(false);
            gobjLoadingTxt.SetActive(false);
            onCompleted?.Invoke();
        }
    }
}
