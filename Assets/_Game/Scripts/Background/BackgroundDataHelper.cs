using Unity.VisualScripting;
using UnityEngine;

public class BackgroundDataHelper : Singleton<BackgroundDataHelper>
{
    [SerializeField] private BackgroundDataSO backgroundDataSO;
    public BackgroundDataSO BackgroundDataSO => backgroundDataSO;
}
