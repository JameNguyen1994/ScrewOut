using Storage;
using System.Collections.Generic;
using UnityEngine;

public class EconomicTrackingUnity : IEconomicTracking
{
    private readonly HashSet<int> levelReachMap = new HashSet<int>()
    {
        2, 3, 5, 8, 10, 13, 15, 18, 20, 23, 25, 30, 35, 40, 45, 50, 55, 60
    };
    
    private readonly HashSet<int> levelFinishMap = new HashSet<int>()
    {
        1, 3, 5, 10, 15, 20, 25, 30, 40, 50, 60, 70, 80,
        90, 100, 120, 140, 150, 175, 200, 250, 300, 350, 400, 450, 500
    };
    
    public void SendLevelFinish(int level)
    {
        if (!levelFinishMap.Contains(level))
        {
            return;
        }
        
        Debug.Log($"<color=red>FINISH_LEVEL_{level}</color>");
    }

    public void SendReachLevel()
    {
        var level = Db.storage.USER_EXP.level.GetDecrypted();

        if (!levelReachMap.Contains(level))
        {
            return;
        }
        
        Debug.Log($"<color=red>EXP_LEVEL_{level}</color>");
    }
}
