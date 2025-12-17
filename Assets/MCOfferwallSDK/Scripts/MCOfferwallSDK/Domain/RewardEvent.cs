using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.MCOfferwallSDK.Scripts.MCOfferwallSDK.Domain
{
    [Serializable]
    public class RewardEvent : UnityEvent<RewardDTO>
    {
        private int listenerCount = 0;

        public new void AddListener(UnityAction<RewardDTO> call)
        {
            base.AddListener(call);
            listenerCount++;

        }

        public new void RemoveListener(UnityAction<RewardDTO> call)
        {
            base.RemoveListener(call);
            listenerCount = Mathf.Max(0, listenerCount - 1);
        }

        public int GetEventCount()
        {
            return listenerCount + GetPersistentEventCount();
        }
    }

}
