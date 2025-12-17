using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace ps.modules.leaderboard
{
    public class EffectFlare : MonoBehaviour
    {
        private Image image;
        private RectTransform rectTransform;
        public bool enableFade = false;
        public bool enableScale = false;
        public bool enableRotate = false;

        public float timeDuration = 1f;
        public float endValueAlpha = 0.5f;
        public float scaleTo = 1.5f;
        public Vector3 originScale;

        public float euler;

        void Start()
        {
            image = GetComponent<Image>();
            originScale = this.transform.localScale;
            rectTransform = GetComponent<RectTransform>();

            if (enableFade)
            {
                DoFade().Forget();
            }

            if (enableScale)
            {
                OnScale().Forget();
            }
        }

        void Update()
        {
            if (enableRotate)
            {
                rectTransform.Rotate(new Vector3(0, 0, euler * Time.deltaTime));
            }
        }

        async UniTask DoFade()
        {
            while (enableFade)
            {
                await image.DOFade(endValueAlpha, timeDuration).ToUniTask();
                await image.DOFade(1, timeDuration).ToUniTask();
            }
        }

        async UniTaskVoid OnScale()
        {
            while (enableScale)
            {
                await rectTransform.DOScale(originScale * scaleTo, timeDuration).ToUniTask();
                await rectTransform.DOScale(originScale, timeDuration).ToUniTask();
            }
        }
        public void StopCurrentEffect()
        {
            enableFade = false;
            enableScale = false;
            enableRotate = false;

            if (image != null)
            {
                image.DOKill();
            }
            if (rectTransform != null)
                rectTransform.DOKill();
        }
        private void OnDestroy()
        {
            enableFade = false;
            enableScale = false;
            enableRotate = false;
            if (image != null)
            {
                image.DOKill();
            }
            if (rectTransform != null)
                rectTransform.DOKill();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EffectFlare))]
    public class EffectFlairEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EffectFlare effectFlair = (EffectFlare)target;

            effectFlair.enableFade = EditorGUILayout.Toggle("Enable Fade", effectFlair.enableFade);
            if (effectFlair.enableFade)
            {
                effectFlair.timeDuration = EditorGUILayout.FloatField("Time Duration", effectFlair.timeDuration);
                effectFlair.endValueAlpha = EditorGUILayout.FloatField("End Value Alpha", effectFlair.endValueAlpha);
            }

            effectFlair.enableScale = EditorGUILayout.Toggle("Enable Scale", effectFlair.enableScale);
            if (effectFlair.enableScale)
            {
                effectFlair.scaleTo = EditorGUILayout.FloatField("Scale To", effectFlair.scaleTo);
                effectFlair.timeDuration = EditorGUILayout.FloatField("Time Duration", effectFlair.timeDuration);
            }

            effectFlair.enableRotate = EditorGUILayout.Toggle("Enable Rotate", effectFlair.enableRotate);
            if (effectFlair.enableRotate)
            {
                effectFlair.euler = EditorGUILayout.FloatField("Euler", effectFlair.euler);
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(effectFlair);
            }
        }

    }
#endif
}