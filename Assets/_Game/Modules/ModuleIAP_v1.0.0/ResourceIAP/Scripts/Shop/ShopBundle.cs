using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopBundle : MonoBehaviour
{
    [SerializeField] private ResourceDataSO resourceDataBundle;
    [SerializeField] private ResourceDataSO resourceDataBundleHeart;
    [SerializeField] private List<ItemIAPBundle> lstItemBundle;
    [SerializeField] private List<ItemIAPBundleHeart> lstItemBundleHeart;
    [SerializeField] private List<Sprite> lstSpriteCoin;
    [SerializeField] private List<Sprite> lstSpriteBundle;

    private void OnEnable()
    {
        Init();
    }
    public void Init()
    {
        Debug.Log("InitUI");
        var data = resourceDataBundle.data;
        for (int i = 0; i < lstItemBundle.Count; i++)
        {
            var buyItemCoinHandler = new BuyBundleHandler();
            buyItemCoinHandler.SetCoinDestination(lstItemBundle[i].TfmImagCoin());
            var itemIAPCoin = lstItemBundle[i];
            itemIAPCoin.Init(data[i], buyItemCoinHandler);
        }

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
        for (int i = 0; i < lstItemBundle.Count; i++)
        {
            lstItemBundle[i].SetImageCoinPack(lstSpriteCoin[index]);
            lstItemBundle[i].SetImageBundle(lstSpriteBundle[index]);
            index++;
        }
        for (int i = 0; i < lstItemBundleHeart.Count; i++)
        {
            lstItemBundleHeart[i].SetImageCoinPack(lstSpriteCoin[index]);
            lstItemBundleHeart[i].SetImageBundle(lstSpriteBundle[index]);
            index++;
        }
    }
}
