using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class TabGiftController : LeaderBoardCtrBase
    {
        [SerializeField] private List<ItemGiftLD> lstItemDailyGifts;
        [SerializeField] private List<ItemGiftLD> lstItemMonthlyGifts;

        [SerializeField] private bool initedDay = false;
        [SerializeField] private bool initedMonth = false;


        public void ShowLstItemGiftDaily()
        {
            if (!initedDay)
            {
                initedDay = true;
                var dataManager = manager.GetController<GiftDataManager>();
                for (int i = 0; i < lstItemDailyGifts.Count; i++)
                {
                    var itemGift = lstItemDailyGifts[i];

                    var giftImage = dataManager.GiftSpriteSO.GetSprite(i);
                    var giftData = dataManager.GiftDay;
                    itemGift.SetData(giftImage, giftData.rewards[i]);
                }
            }
            for (int i = 0; i < lstItemDailyGifts.Count; i++)
            {
                lstItemDailyGifts[i].Show();
            }
        }
        public void HideLstItemGiftDaily()
        {
            for (int i = 0; i < lstItemDailyGifts.Count; i++)
            {
                lstItemDailyGifts[i].Hide();
            }
        }
        public void ShowLstItemGiftMonthly()
        {
            if (!initedMonth)
            {
                initedMonth = true;
                var dataManager = manager.GetController<GiftDataManager>();
                for (int i = 0; i < lstItemDailyGifts.Count; i++)
                {
                    var itemGift = lstItemMonthlyGifts[i];

                    var giftImage = dataManager.GiftSpriteSO.GetSprite(i);
                    var giftData = dataManager.GiftMonth;
                    itemGift.SetData(giftImage, giftData.rewards[i]);
                }
            }
            for (int i = 0; i < lstItemMonthlyGifts.Count; i++)
            {
                lstItemMonthlyGifts[i].Show();
            }
        }
        public void HideLstItemGiftMonthly()
        {
            for (int i = 0; i < lstItemMonthlyGifts.Count; i++)
            {
                lstItemMonthlyGifts[i].Hide();
            }
        }
        
    }
}
