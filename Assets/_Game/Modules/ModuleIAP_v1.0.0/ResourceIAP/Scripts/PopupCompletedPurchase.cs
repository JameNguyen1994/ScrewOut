using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using ResourceIAP;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PopupCompletedPurchase : MonoBehaviour
{
    [SerializeField] private Image imgFade;
    [SerializeField] private Image imgPopup;
    [SerializeField] private Button btnClose;
    [SerializeField] private List<ItemResourceIAP> lstItem;
    [SerializeField] private bool isShowing = false;
    public async UniTask Show(List<ResourceIAP.ResourceValue> lstResource)
    {
        isShowing = true;
        Reset();
        btnClose.onClick.AddListener(OnClickHidePopup);
        imgFade.color = new Color(0, 0, 0, 0);
        imgFade.gameObject.SetActive(true);
        imgFade.DOFade(0.9f, 0.2f).SetEase(Ease.OutBack);
        imgPopup.transform.localScale = Vector3.zero;
        btnClose.transform.localScale = Vector3.zero;

        await imgPopup.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        await ShowListItem(lstResource);

        await btnClose.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        await UniTask.WaitUntil(() => !isShowing);
    }

    private async UniTask ShowListItem(List<ResourceIAP.ResourceValue> lstResource)
    {
        for (int i = 0; i < lstItem.Count; i++)
        {
            if (i < lstResource.Count)
            {
                lstItem[i].gameObject.SetActive(true);
                lstItem[i].InitResource(lstResource[i]);
                lstItem[i].Show();
            }
            else
            {
                lstItem[i].gameObject.SetActive(false);
            }
        }

    }
    private void OnClickHidePopup()
    {
        btnClose.onClick.RemoveAllListeners();
        Hide();
    }
    private void Reset()
    {
        for (int i = 0; i < lstItem.Count; i++)
        {
            lstItem[i].gameObject.SetActive(false);
        }
    }
    public async UniTask Hide()
    {
        await btnClose.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        imgFade.DOFade(0f, 0.2f).SetEase(Ease.InBack);
        await imgPopup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack);
        imgFade.gameObject.SetActive(false);

        isShowing = false;

    }

}
