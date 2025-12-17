using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    [CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Scriptable Objects/Reward/ResourceDataSO")]
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

