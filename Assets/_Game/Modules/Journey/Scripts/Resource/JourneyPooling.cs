using UnityEngine;

namespace ps.modules.journey
{
    public class JourneyPooling : Singleton<JourneyPooling>
    {
        [SerializeField] private ItemResourceJourney itemResourceJourney;
        public ItemResourceJourney GetInstance(Transform tfmParent = null)
        {
            return Instantiate(itemResourceJourney, tfmParent);
        }
    }
}