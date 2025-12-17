using System.Collections.Generic;
using UnityEngine;

namespace ResourceIAP
{
    [CreateAssetMenu(fileName = "ResourceDataIAPSO", menuName = "ResourceDataIAPSO")]
    public class ResourceDataIAPSO : ScriptableObject
    {
        public List<ResourceData> data;
        public ResourceData GetResourceData(ResourceType type)
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

