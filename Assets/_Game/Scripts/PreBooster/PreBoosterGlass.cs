using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptsEffect;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PreBoosterGlass : MonoBehaviour
{
    [SerializeField] private Image imgGlass;
    [SerializeField] private Image imgFillGlass;
    [SerializeField] private RectTransform rtfmPar;
    [SerializeField] private ParticleSystem parEffect;
    [SerializeField] private RectTransform canvasRectTransform; // RectTransform của Canvas (phải được tham chiếu)

    public void ShowAtShape(Shape shape,float time)
    {
       // imgGlass.gameObject.SetActive(true);
        Vector3 trayWorldPosition = shape.transform.position;

        // Lấy screen point
        Vector3 trayScreenPoint = Camera.main.WorldToScreenPoint(trayWorldPosition);

        // Chuyển screen point sang local point trong canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            trayScreenPoint,
            null,
            out Vector2 trayCanvasLocalPoint
        ); 
        imgGlass.rectTransform.anchoredPosition = trayCanvasLocalPoint;
        rtfmPar.anchoredPosition = trayCanvasLocalPoint;
        DOFill(time);
    }
    public async UniTask DOFill(float time)
    {
        imgFillGlass.fillAmount = 0;
        await imgFillGlass.DOFillAmount(1, time);
        imgGlass.gameObject.SetActive(false);
        //parEffect.Play();
    }
    public void Cancel()
    {
        imgFillGlass.DOKill();
        imgFillGlass.fillAmount = 0;
        imgGlass.gameObject.SetActive(false);
    }
}
