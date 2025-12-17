using Cysharp.Threading.Tasks;
using PS.IAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBundleHeart : MonoBehaviour
{
    [SerializeField] private ResourceDataSO resourceDataBundleHeart;
    [SerializeField] private List<ItemIAPBundleHeart> lstItemBundleHeart;

    private void OnEnable()
    {
        Init();
    }
    public async UniTask Init()
    {        
        await UniTask.WaitUntil(() => InAppPurchase.Instance.IsInitialized());
        var dataHeart = resourceDataBundleHeart.data;
        for (int i = 0; i < lstItemBundleHeart.Count; i++)
        {
            var buyItemCoinHandler = new BuyBundleHeartHandler();
            buyItemCoinHandler.SetCoinDestination(lstItemBundleHeart[i].TfmImagCoin());

            var itemIAPCoin = lstItemBundleHeart[i];
            itemIAPCoin.Init(dataHeart[i], buyItemCoinHandler);
        }
    }

    [ContextMenu("InitUI sprite")]
    public void SetUpCoiImagePack()
    {
        int index = 0;
        for (int i = 0; i < lstItemBundleHeart.Count; i++)
        {
            //lstItemBundleHeart[i].SetImageCoinPack(lstSpriteCoin[id]);
            //lstItemBundleHeart[i].SetImageBundle(lstSpriteBundle[id]);
            index++;
        }
    }
}
