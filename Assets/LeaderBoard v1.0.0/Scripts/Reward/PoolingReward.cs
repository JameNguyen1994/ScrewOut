using UnityEngine;

namespace ps.modules.leaderboard
{
    public class PoolingReward : LeaderBoardCtrBase
    {
        [SerializeField] private ItemResource itemResource;
        public ItemResource GetItemResource()
        {
            ItemResource item = Instantiate(itemResource, transform);
            item.gameObject.SetActive(false);
            return item;
        }

    }
}
