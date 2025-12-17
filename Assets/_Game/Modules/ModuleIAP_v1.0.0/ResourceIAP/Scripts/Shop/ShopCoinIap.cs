using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCoinIap : MonoBehaviour
{
    [SerializeField] private ResourceDataSO shopCoinData;

    [SerializeField] private List<ItemIAPCoin> lstItemCoin;
    [SerializeField] private List<Sprite> lstSprite;

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        Debug.Log("InitUI");
        var data = shopCoinData.data;
        for (int i = 0; i < lstItemCoin.Count; i++)
        {
            var buyItemCoinHandler = new BuyItemCoinHandler();
            buyItemCoinHandler.SetTransform(lstItemCoin[i].TfmImagCoin());
            var itemIAPCoin = lstItemCoin[i];
            itemIAPCoin.Init(data[i], buyItemCoinHandler);
        }
    }
    [ContextMenu("InitUI sprite")]
    public void SetUpCoiImagePack()
    {
        for (int i = 0; i < lstItemCoin.Count; i++)
        {
            lstItemCoin[i].SetImageCoinPack(lstSprite[i]);
        }
    }
}
