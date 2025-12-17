using UnityEngine;

namespace WeeklyQuest
{
    public class GiftEventAnimation : MonoBehaviour
    {
        [SerializeField] private ItemGift itemGift;
        public void OnGiftAnimationComplete()
        {
            Debug.Log("Gift animation completed.");
            itemGift.OnDoneAnimOpenGift();
        }
    }
}

