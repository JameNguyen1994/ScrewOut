using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.NetworkTime
{
    public interface IRealTimeNetBridge
    {
        UnityAction<TimeNetData, string> OnCompleted { get; set; }

        void GetTimeByPublicIp(string publicIp);
        void GetUtcTime();
        void GetNASATime();
    }
}
