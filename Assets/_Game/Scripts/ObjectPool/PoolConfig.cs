using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoolConfig ", menuName = "Configs/Pool Config")]
public class PoolConfig : ScriptableObject
{
    [SerializeField]
    private List<PoolConfigItem> configs = new List<PoolConfigItem>();

    public IReadOnlyList<PoolConfigItem> Configs => configs;
}

[System.Serializable]
public class PoolConfigItem
{
    public string key;
    public PooledBehaviour prefab;
    public int size;
}