using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ResourceIAP
{
    public class ItemResourceIAP : MonoBehaviour
    {
        [SerializeField] private ResourceValue resourceValue;
        [SerializeField] private TextMeshProUGUI txtValue;
        [SerializeField] private Image imgResource;
        public void InitResource(ResourceValue resourceValue)
        {
            this.resourceValue = resourceValue;
            txtValue.text = $"{resourceValue.ValueToString()}";
            imgResource.sprite = ShopIAPController.Instance.GetSpriteResource(resourceValue.type);
        }
        public async UniTask Show()
        {
            imgResource.transform.localScale = Vector3.zero;
            txtValue.transform.localScale = Vector3.zero;
            await imgResource.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
            txtValue.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
        }
    }
}
