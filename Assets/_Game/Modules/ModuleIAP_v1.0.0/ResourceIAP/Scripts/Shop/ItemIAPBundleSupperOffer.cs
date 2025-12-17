using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ItemIAPBundleSupperOffer : ItemIAPBase
{
    [SerializeField] private Text txtAmountCoin;
    [SerializeField] private Text txtAddHoleAmount;
    [SerializeField] private Text txtHammerAmount;
    [SerializeField] private Text txtClearAmount;
    [SerializeField] private Text txtFreeReviveAmount;
    [SerializeField] private Text txtUnlockAmount;

    private IAPItemData bundleData;

    public override async UniTask Init(object data, IPurchaseHandler purchaseHandler)
    {
        await base.Init(data, purchaseHandler);

        this.purchaseHandler = purchaseHandler;
        this.data = data;

        bundleData = (IAPItemData)data;
        productID = bundleData.iapKey;

        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();

        int valueCoin = bundleData.data.Find(x => x.resourceType == ResourceType.Coin).value;
        int valueAddHole = bundleData.data.Find(x => x.resourceType == ResourceType.ADD_HOLE).value;
        int valueHammer = bundleData.data.Find(x => x.resourceType == ResourceType.HAMMER).value;
        int valueClear = bundleData.data.Find(x => x.resourceType == ResourceType.CLEAR).value;
        int valueUnlockBox = bundleData.data.Find(x => x.resourceType == ResourceType.UNLOCK_BOX).value;
        int valueFreeRevive = bundleData.data.Find(x => x.resourceType == ResourceType.FREE_REVIVE).value;

        txtAddHoleAmount.text = $"x{valueAddHole}";
        txtHammerAmount.text = $"x{valueHammer}";
        txtClearAmount.text = $"x{valueClear}";
        txtAmountCoin.text = $"{valueCoin}";
        txtUnlockAmount.text = $"x{valueUnlockBox}";
        txtFreeReviveAmount.text = $"{valueFreeRevive}";
    }
}