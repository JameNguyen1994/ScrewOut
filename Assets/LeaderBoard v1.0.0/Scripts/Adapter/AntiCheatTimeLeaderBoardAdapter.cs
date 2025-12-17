using Cysharp.Threading.Tasks;
using ps.modules.leaderboard;
using System;
using System.Threading;
using UnityEngine;

public class AntiCheatTimeLeaderBoardAdapter :MonoBehaviour, ITimeAdapter
{
    public async UniTask Init()
    {
        await UniTask.WaitUntil(() => TimeGetter.Instance.IsGettedTime, cancellationToken: CancellationToken.None);

    }
    public DateTime GetCurrentTime()
    {
        return TimeGetter.Instance.Now;
    }

}
