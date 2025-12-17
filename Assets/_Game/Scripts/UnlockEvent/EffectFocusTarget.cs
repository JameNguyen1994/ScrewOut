using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectFocusTarget : MonoBehaviour
{
    [SerializeField] private List<Image> lstImageFocus;
    [SerializeField] private float startScale = 4f;
    [SerializeField] private float endScale = 0.1f;
    [SerializeField] private float delayPerImage = 0.1f;
    [SerializeField] private float durationPerImage = 0.1f;
    [SerializeField] private bool isEffecting = false;

    public async UniTask StartEffect()
    {
        if (isEffecting)
            return;
        isEffecting = true;
        var lstTasks = new List<UniTask>();
        for (int i = 0; i < lstImageFocus.Count; i++)
        {
            var image = lstImageFocus[i];
            image.gameObject.SetActive(true);
            lstTasks.Add(image.rectTransform.DOScale(endScale, durationPerImage).From(startScale).ToUniTask());
            await UniTask.WaitForSeconds(delayPerImage);
        }

        await UniTask.WhenAll(lstTasks);

        isEffecting = false;
    }
}
