using System;
using Storage;
using UnityEngine;

public class OfferwallEventListener : MonoBehaviour
{
    private void OnEnable()
    {
        OfferwallController.OnEarnCoinEvent += OnEarnCoinEvent;
    }

    private void OnEarnCoinEvent(int coin)
    {
        OfferwallController.Instance.HideOfferwallList();
        if (SceneController.Instance.CurrentScene == SceneType.MainMenu)
        {
            var storageRewardData = Db.storage.RewardData.DeepClone();
            storageRewardData.AddCoinValue(coin);
            Db.storage.RewardData = storageRewardData;
            _ = MainMenuRecieveRewardsHelper.Instance.OnGetReward();
        }
        else
        {
            var user = Db.storage.USER_INFO;
            user.coin += coin;
            Db.storage.USER_INFO = user;
            EventDispatcher.Push(EventId.UpdateCoinUI
                , new UpdateCoinData()
                {
                    coin = coin,
                    coinMode = CoinMode.Plus
                });
        }
    }

    private void OnDisable()
    {
        OfferwallController.OnEarnCoinEvent -= OnEarnCoinEvent;
    }
}