using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public interface ITimeAdapter
    {
        public UniTask Init();
        public DateTime GetCurrentTime();
    }
}
