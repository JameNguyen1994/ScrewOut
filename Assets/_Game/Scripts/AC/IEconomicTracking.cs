using UnityEngine;

public interface IEconomicTracking
{
    void SendLevelFinish(int level);
    void SendReachLevel();
}