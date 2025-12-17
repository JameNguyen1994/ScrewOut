using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Ad.Utils
{
    public class Inform : MonoBehaviour
    {
        [SerializeField] private GameObject gobjCover;
        [SerializeField] private GameObject gobjBanner;
        private IAnimationInform formAnimation;
        private UnityAction onCompleted;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Open(UnityAction onCompleted)
        {
            gobjCover.SetActive(true);
            this.onCompleted = onCompleted;
            if (formAnimation == null)
            {
                formAnimation = GetComponent<IAnimationInform>();
            }

            formAnimation.onCompleted = OnAnimationCompleted;
            formAnimation.Open();
        }

        void OnAnimationCompleted()
        {
            onCompleted?.Invoke();
            onCompleted = null;
            
        }

        void OnClose()
        {
            gobjCover.SetActive(false);
            formAnimation.onCompleted = null;
            onCompleted?.Invoke();
            onCompleted = null;
        }

        public void Close(UnityAction onCompleted)
        {
            this.onCompleted = onCompleted;
            formAnimation.onCompleted = OnClose;
            formAnimation.Close();
        }

        public void SetBannerCoverActive(bool enable)
        {
            gobjBanner.SetActive(enable);
        }

        public void SetBannerHeight(float bannerHeight = 200)
        {
            var rect = gobjBanner.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, bannerHeight);
        }

        [ContextMenu("Test Open")]
        void CM_Open()
        {
            Open(() =>
            {
                print("open completed");
            });
        }
        
        [ContextMenu("Test Close")]
        void CM_Close()
        {
            Close(null);
        }
    }
}
