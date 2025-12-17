using PS.Analytic;
using Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCoinFreeTime : MonoBehaviour
{
    [SerializeField] private Text txtDetail;
    [SerializeField] private Text txtCoinAmount;
    [SerializeField] private ItemCoinAds itemCoinAds;
    [SerializeField] private Image imgCoin;
    [SerializeField] private Button btnGetFree;
    [SerializeField] private Image iconNoti;

    Coroutine countDownTime;
    private void Start()
    {
        txtCoinAmount.text = $"x{GameAnalyticController.Instance.Remote().RewardFreeCoin.CoinAmount}";
        // itemCoinAds.gameObject.SetActive(true);
        CheckTime();
    }
    public void CheckTime()
    {
        if (Db.storage.FREE_COIN_MARK)
        {
            txtDetail.text = "Free";
            btnGetFree.interactable = true;
            iconNoti.gameObject.SetActive(true);
            //itemCoinAds.gameObject.SetActive(false);
        }
        else
        {

            btnGetFree.interactable = false;
            txtDetail.text = "Claimed";
            iconNoti.gameObject.SetActive(false);

        }

    }
    public void OnClickGetFree()
    {
        AudioController.Instance.PlaySound(SoundName.Click);
        if (Db.storage.FREE_COIN_MARK)
        {
            Debug.Log("Getted");
            var user = Db.storage.USER_INFO;
            user.coin += GameAnalyticController.Instance.Remote().RewardFreeCoin.CoinAmount;
            Db.storage.USER_INFO = user;


            EventDispatcher.Push(EventId.UpdateCoinUI, GameConfig.SHOP_COIN_FREE);
            Db.storage.FREE_COIN_MARK = false;

            EventDispatcher.Push(EventId.MakeCoinFly, imgCoin.transform.position);

            CheckTime();

        }
    }
    public void NextDay()
    {
        Db.storage.FREE_COIN_MARK = true;
        CheckTime();
    }
}
