using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using AOT;

namespace PS.NetworkTime
{
#if UNITY_IOS
    public class RealTimeNetIos : IRealTimeNetBridge
    {
        private static RealTimeNetIos instance;

        public RealTimeNetIos()
        {
            instance = null;
            instance = this;
        }
        
        public UnityAction<TimeNetData, string> OnCompleted { get; set; }
        public void GetTimeByPublicIp(string publicIp)
        {
            nativeGetTimeByIp(publicIp, OnCallback);
        }

        public void GetUtcTime()
        {
            nativeGetUtcTime(OnCallback);
        }

        public void GetNASATime()
        {
            nativeGetNASATime(OnNASACallback);
        }

        [MonoPInvokeCallback(typeof(Action<string, string>))]
        static void OnCallback(string json, string error)
        {
            try
            {
                var data = JsonUtility.FromJson<TimeNetData>(json);
                instance.OnCompleted?.Invoke(data, null);
            }
            catch (Exception e)
            {
                instance.OnCompleted?.Invoke(default, $"can not convert from json to TimeNetData. Details: \n {e}");
            }
        }
        
        [MonoPInvokeCallback(typeof(Action<string, string>))]
        static void OnNASACallback(string data, string error)
        {
            try
            {
                DateTime.TryParse(data, out DateTime dt);
                
                TimeNetData timeNetData = new TimeNetData()
                {
                    year = dt.Year,
                    month = dt.Month,
                    day = dt.Day,
                    hour = dt.Hour,
                    minute = dt.Minute,
                    seconds = dt.Second,
                    milliSeconds = dt.Millisecond,
                    dateTime = dt.ToString("o"),
                    date = $"{dt:MM/dd/yyyy}",
                    time = $"{dt:hh:mm}",
                    timeZone = "Local Zone",
                    dayOfWeek = $"{dt.DayOfWeek}",
                    dstActive = true
                };
                
                instance.OnCompleted?.Invoke(timeNetData, null);
            }
            catch (Exception e)
            {
                instance.OnCompleted?.Invoke(default, $"can not convert from json to TimeNetData. Details: \n {e}");
            }
        }
    
    
        [DllImport ("__Internal")]
        static extern void nativeGetTimeByIp(string publicIp, Action<string, string> callback);

        [DllImport("__Internal")]
        static extern void nativeGetUtcTime(Action<string, string> callback);
        
        [DllImport("__Internal")]
        static extern void nativeGetNASATime(Action<string, string> callback);
    }
#endif
}