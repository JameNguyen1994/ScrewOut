using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using EasyButtons;

namespace ScriptsEffect
{
    public class SimpleAnimationCanvas : MonoBehaviour
    {
        [SerializeField] private Image img;
        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 startSize;
        [SerializeField] private Vector3 offset;
        [SerializeField] private List<Sprite> lstSprite;
       private float time = 0.4f;

        private void Start()
        {
           /* startPos = img.rectTransform.anchoredPosition;
            startSize = img.rectTransform.sizeDelta;
            img.gameObject.SetActive(false);*/
        }
        public async UniTask DoSize(Vector3 endSize, float timeMove)
        {
            img.gameObject.SetActive(true);

            await img.rectTransform.DOSizeDelta(endSize, timeMove);
        }
        public async UniTask Move(Vector3 endPos, float timeMove)
        {
            img.gameObject.SetActive(true);

            Debug.Log($"End pos:{endPos}");
            await img.rectTransform.DOAnchorPos(endPos, timeMove);
            Debug.Log($"anchoredPosition:{img.rectTransform.anchoredPosition}");

        }
        [Button("Test Animation")]

        public async UniTask StartAnimation(int loop = 1,float time = 0.02f)
        {
            img.gameObject.SetActive(true);
            bool isInfinity = loop == -1;
            if (loop == -1)
                loop = 1;
            for (int lop = 0; lop < loop; lop++)
            {
               // img.gameObject.SetActive(true);
                for (int i = 0; i < lstSprite.Count; i++)
                {
                    //Debug.Log(i);
                    if (img == null)
                        return;
                    img.sprite = lstSprite[i];
                  
                    await UniTask.WaitForSeconds(time);
                }
                if (isInfinity)
                    lop -= 1;
            }
           // img.gameObject.SetActive(false);


        }

        public Sprite GetSpriteAt(int index)
        {
            if (index >= lstSprite.Count)
            {
                return lstSprite[^1];
            }

            if (index < 0)
            {
                return lstSprite[0];
            }
            
            return lstSprite[index];
        }
    }
}