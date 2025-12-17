using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum PurchaseRewardType
{
    Coin = 0,
    Booster_Hammer = 1,
    Booster_Drill = 2,
    Booster_Bloom = 3,
    Booster_UnlockBox = 4,
    Heart = 5,
    Item = 6,  //Exp
    FreeRevive = 7
}
public class PurchaseRewardIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtAmount;
    [SerializeField] private Vector3 backPos;
    [SerializeField] private Vector2 scaleTarget = new Vector2(0.3f, 0.3f);
    [SerializeField] private int amount;
    [SerializeField] private Image imgIcon;
    [SerializeField] private float showDuration = 0.3f;
    [SerializeField] private Sprite sprBoosterHammer, sprBoosterDrill, sprBoosterBloom, sprBoosterUnlockBox, sprBoosterRevive;
    [SerializeField] private PurchaseRewardType purchaseRewardType;

    public int Amount { get => amount; }

    public async UniTask Show(Vector3 startPos)
    {
        transform.localScale = Vector3.zero;
        imgIcon.transform.localScale = Vector3.one;
        SetText();
        gameObject.SetActive(true);
        txtAmount.gameObject.SetActive(true);
        await transform.DOScale(Vector3.one, showDuration).SetEase(Ease.OutBack);

        // await UniTask.Delay(1000);
        // Hide();
    }
    public void InitRewardBooster(PurchaseRewardType type, int amount)
    {
        purchaseRewardType = type;
        this.amount = amount;
        GetIcon();
        SetText();
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }
    public void SetRewardAmount(int amount)
    {
        this.amount = amount;
        SetText();
    }
    private void SetText()
    {
        switch (purchaseRewardType)
        {
            case PurchaseRewardType.Coin:
                txtAmount.text = $"+{amount}";
                break;
            case PurchaseRewardType.Booster_Hammer:
            case PurchaseRewardType.Booster_Drill:
            case PurchaseRewardType.Booster_Bloom:
            case PurchaseRewardType.Booster_UnlockBox:
                txtAmount.text = $"+{amount}";
                break;
            case PurchaseRewardType.Heart:
                txtAmount.text = $"+{Utility.TimeSpanRewardToString(System.TimeSpan.FromMilliseconds(amount))}";
                break;
            case PurchaseRewardType.Item:
                txtAmount.text = $"+{amount}";
                break;
        }
    }

    public async UniTask Hide()
    {
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
        imgIcon.transform.localPosition = Vector3.zero;
    }
    public async UniTask Move(Vector3 targetPos)
    {
        txtAmount.gameObject.SetActive(false);
        imgIcon.transform.DOScale(scaleTarget, 0.5f).SetEase(Ease.OutBack);

        await imgIcon.transform.DOMove(targetPos, 0.5f).SetEase(Ease.OutQuad);
        await imgIcon.transform.DOScale(scaleTarget * 1.1f, 0.1f).SetEase(Ease.OutBack);
        await imgIcon.transform.DOScale(0.1f, 0.1f).SetEase(Ease.OutQuad);
        // await Hide();
    }
    void GetIcon()
    {
        switch (purchaseRewardType)
        {
            case PurchaseRewardType.Coin:
                break;
            case PurchaseRewardType.Booster_Hammer:
                imgIcon.sprite = sprBoosterHammer;
                break;
            case PurchaseRewardType.Booster_Drill:
                imgIcon.sprite = sprBoosterDrill;
                break;
            case PurchaseRewardType.Booster_Bloom:
                imgIcon.sprite = sprBoosterBloom;
                break;
            case PurchaseRewardType.Booster_UnlockBox:
                imgIcon.sprite = sprBoosterUnlockBox;
                break;
            case PurchaseRewardType.Heart:
                break;
            case PurchaseRewardType.FreeRevive:
                imgIcon.sprite = sprBoosterRevive;
                break;
        }
    }

    public string GetText()
    {
        return txtAmount != null ? txtAmount.text : string.Empty;
    }

}
