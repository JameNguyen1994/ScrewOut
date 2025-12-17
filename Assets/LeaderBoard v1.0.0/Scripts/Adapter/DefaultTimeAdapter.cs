using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class DefaultTimeAdapter : MonoBehaviour, ITimeAdapter
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow;
        }

        public async UniTask Init()
        {
        }
    }
}