using UnityEngine;


namespace WeeklyQuest
{
    public class ObjectPoolWeekly : MonoBehaviour
    {
        [SerializeField] private ItemGift itemGift;
        [SerializeField] private ItemResource itemResource;
        [SerializeField] private ItemQuest itemQuest;

        public ItemGift GetItemGift()
        {
            ItemGift item = Instantiate(itemGift, transform);
            item.gameObject.SetActive(false);
            return item;
        }
        public ItemResource GetItemResource()
        {
            ItemResource item = Instantiate(itemResource, transform);
            item.gameObject.SetActive(false);
            return item;
        }
        public ItemQuest GetItemQuest()
        {
            ItemQuest item = Instantiate(itemQuest, transform);
            item.gameObject.SetActive(false);
            return item;
        }
    }
}
