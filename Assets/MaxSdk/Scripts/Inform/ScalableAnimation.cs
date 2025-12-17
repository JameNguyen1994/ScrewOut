using System;
using System.Collections;
using System.Collections.Generic;
using PS.Ad.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace PS.Ad.Utils
{
    public class ScalableAnimation : MonoBehaviour, IAnimationInform
    {
        public UnityAction onCompleted { get; set; }

        private UnityAction onScaleCompleted;

        private readonly Vector3 OPEN_SCALE = Vector3.one;
        private readonly Vector3 CLOSE_SCALE = Vector3.zero;

        [SerializeField] private Transform tfmSelf;
        [SerializeField] private float speed = 1;
        private Vector3 targetScale;
        private bool isScaling;

        public void Open()
        {
            onScaleCompleted = OnCompleted;
            targetScale = OPEN_SCALE;
            isScaling = true;
        }

        public void Close()
        {
            onScaleCompleted = OnCompleted;
            targetScale = CLOSE_SCALE;
            isScaling = true;
        }

        void OnCompleted()
        {
            onCompleted?.Invoke();
        }

        private void Update()
        {
            if (!isScaling)
            {
                return;
            }

            tfmSelf.localScale = Vector3.MoveTowards(tfmSelf.localScale, targetScale, Time.unscaledDeltaTime * speed);

            if (tfmSelf.localScale == targetScale)
            {
                onScaleCompleted?.Invoke();
                isScaling = false;
            }
        }
    }
}
