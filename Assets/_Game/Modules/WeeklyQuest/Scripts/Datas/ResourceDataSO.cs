using System.Collections.Generic;
using UnityEngine;

namespace WeeklyQuest
{
    [CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Scriptable Objects/WeeklyQuest/ResourceDataSO")]
    public class ResourceDataSO : ScriptableObject
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

