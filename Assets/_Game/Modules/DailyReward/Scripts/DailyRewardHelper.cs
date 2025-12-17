using DailyReward;
using UnityEngine;

namespace DailyReward
{

    public class DailyRewardHelper : MonoBehaviour
    {
        [SerializeField] private ItemResource prbResource;
        [SerializeField] private ResourceDataSO resourceData;
        [SerializeField] private WeeklyDataSO weeklyData;

        public WeeklyDataSO WeeklyData { get => weeklyData; }

        public ItemResource CreateResource(ResourceValue resourceValue,float scale)
        {
            ItemResource itemResource = Instantiate(prbResource, transform);
            itemResource.InitResource(resourceValue,scale);
            return itemResource;
        }
        public Sprite GetSpriteByResourceType(ResourceType resourceType)
        {
            //Debug.Log($"Getting sprite for resource type: {resourceType}");
            return resourceData.GetResourceData(resourceType)?.icon ?? null;
        }
        public DayData GetDayDataByIndex(int day)
        {
            return weeklyData.GetData(day);
        }
    }
}