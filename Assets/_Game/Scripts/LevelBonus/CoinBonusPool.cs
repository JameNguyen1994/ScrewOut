using System.Collections.Generic;
using UnityEngine;

public class CoinBonusPool : MonoBehaviour
{
    public static CoinBonusPool Instance;

    [SerializeField] private CoinLevelBonus coinPrefab;
    [SerializeField] private int preloadCount = 15;

    private readonly Queue<CoinLevelBonus> pool = new();

    private void Awake()
    {
        Instance = this;
        Preload();
    }

    private void Preload()
    {
        for (int i = 0; i < preloadCount; i++)
            CreateNew();
    }

    private CoinLevelBonus CreateNew()
    {
        var item = Instantiate(coinPrefab, transform);
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
        return item;
    }

    public CoinLevelBonus Get()
    {
        if (pool.Count == 0)
            CreateNew();

        var coin = pool.Dequeue();
        coin.gameObject.SetActive(true);
        return coin;
    }

    public void Release(CoinLevelBonus coin)
    {
        coin.gameObject.SetActive(false);
        coin.transform.SetParent(transform);
        pool.Enqueue(coin);
    }
}
