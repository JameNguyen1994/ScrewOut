using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class AdapterController : LeaderBoardCtrBase
    {
        [SerializeField] private SettingAdapterSO settingAdapterSO;
        private ITimeAdapter timeAdapter;
        private IPlayerDataAdapter playerDataAdapter;

        public ITimeAdapter TimeAdapter { get => timeAdapter; }
        public IPlayerDataAdapter PlayerDataAdapter { get => playerDataAdapter; }

        public override async UniTask Init()
        {
            await base.Init();
            
            if (settingAdapterSO.alternativeTimeAdapterBehaviour != null && settingAdapterSO.alternativeTimeAdapterBehaviour is ITimeAdapter adapter)
            {
                timeAdapter = adapter;
            }
            else
            {
                timeAdapter = settingAdapterSO.defaultTimeAdapterBehaviour as ITimeAdapter;
                Debug.LogError("The assigned MonoBehaviour does not implement ITimeAdapter interface.=>>Adapter will be default");
            }
            if (settingAdapterSO.alternativePlayerDataAdapterBehaviour != null && settingAdapterSO.alternativePlayerDataAdapterBehaviour is IPlayerDataAdapter pAdapter)
            {
                playerDataAdapter = pAdapter;
            }
            else
            {
                playerDataAdapter = settingAdapterSO.defaultPlayerDataAdapterBehaviour as IPlayerDataAdapter;
                Debug.LogError("The assigned MonoBehaviour does not implement IPlayerDataAdapter interface.=>>Adapter will be default");
            }

            await timeAdapter.Init();
            await playerDataAdapter.Init();
        }
        public void OnShow()
        {
            //timeAdapter.Init();
            playerDataAdapter.Init();
        }
    }
}