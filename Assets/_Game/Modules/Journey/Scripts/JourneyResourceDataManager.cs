using UnityEngine;

namespace ps.modules.journey
{
    public class JourneyResourceDataManager : Singleton<JourneyResourceDataManager>
    {
        [SerializeField] private ResourceDataSO resourceDataSO;
        public ResourceDataSO ResourceDataSO => resourceDataSO;
    }
}