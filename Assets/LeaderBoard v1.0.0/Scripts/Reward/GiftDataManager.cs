using UnityEngine;

namespace ps.modules.leaderboard
{
    public class GiftDataManager : LeaderBoardCtrBase
    {
        [SerializeField] private LeaderBoardGiftDataSO giftData;
        [SerializeField] private ResourceDataSO resourceDataSO;
        [SerializeField] private GiftSpriteSO giftSpriteSO;
        public GiftDataSO GiftDay => giftData.giftDay;
        public GiftDataSO GiftMonth => giftData.giftMonth;
        public ResourceDataSO ResourceDataSO => resourceDataSO;
        public GiftSpriteSO GiftSpriteSO => giftSpriteSO;
    }
}