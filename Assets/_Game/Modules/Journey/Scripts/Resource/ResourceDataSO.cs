using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.journey
{
    [CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Scriptable Objects/Journey/ResourceDataSO")]
    public class ResourceDataSO : ScriptableObject
    {
        public List<ResourceDataJourney> data;
        public ResourceDataJourney GetResourceData(ResourceTypeJourney type)
        {
            foreach (var resource in data)
            {
                if (resource.type == type)
                {
                    return resource;
                }
            }
            return null; // or throw an exception, or return a default value
        }
    }
}

