using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
using UnityEngine;

namespace ps.modules.leaderboard
{

    public class LeaderBoardCtrBase : MonoBehaviour
    {
        [SerializeField] protected LeaderboardManager manager;

        protected void Awake()
        {
            AutoRegiter();
        }
        private async UniTask AutoRegiter()
        {
            await UniTask.WaitUntil(() => LeaderboardManager.Instance != null);
            manager = LeaderboardManager.Instance;
            manager.RegisterController(this);
        }
        public virtual async UniTask Init()
        {
            await UniTask.WaitUntil(() => manager != null);
        }
    }
}
