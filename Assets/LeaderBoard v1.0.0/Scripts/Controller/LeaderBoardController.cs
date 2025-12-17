using Cysharp.Threading.Tasks;
using UnityEngine;
using EasyButtons;
using System.Net;
using Storage;
namespace ps.modules.leaderboard
{

    public class LeaderBoardController : LeaderBoardCtrBase
    {
        [SerializeField] private bool initData = false;

        public bool InitData { get => initData; set => initData = value; }

        private async UniTask Start()
        {
            initData = false;
            await UniTask.WaitForEndOfFrame();
            Init();
        }
        public override async UniTask Init()
        {
            await UniTask.WaitUntil(() => Db.storage.Inited);
            await base.Init();
            await manager.GetController<AdapterController>().Init();
            await manager.GetController<LeaderboardDataController>().Init();
            await manager.GetController<PlayerDataManager>().Init();
          

            initData = true;
        }

        [Button]
        public void Test()
        {
            Debug.Log("Test LeaderBoardController");
            Init();
        }
    }
}
