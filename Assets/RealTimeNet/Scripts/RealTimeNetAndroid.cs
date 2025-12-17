using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace PS.NetworkTime
{
#if UNITY_ANDROID
    public class RealTimeNetAndroid : AndroidJavaProxy, IRealTimeNetBridge
    {
        private AndroidJavaObject jvaClass;

//        public static long GetCurrentTime()
//        {
//            long CurrentTime = 0;
//#if UNITY_ANDROID && !UNITY_EDITOR
//            AndroidJavaClass androidClass = new AndroidJavaClass("com.unitylib.realtime.net.RealTimeNet");
//            CurrentTime = androidClass.CallStatic<long>("getCurrentTimeForAPI29OrAbove");
//#elif UNITY_EDITOR
//            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
//#endif
//            return CurrentTime;
//        }
//
//        public void GetNetworkTimeByIp(string ip)
//        {
//#if UNITY_ANDROID && !UNITY_EDITOR
//            jvaClass.Call("getNetworkTimeByPublicIp", ip);
//#elif UNITY_EDITOR
//            Debug.Log("[RealTimeNet] This method only work on Android Or iOS platform.");
//#endif
//
//        }


        public RealTimeNetAndroid() : base("com.unitylib.realtime.net.ServerCallback")
        {
            jvaClass = new AndroidJavaObject("com.unitylib.realtime.net.RealTimeNet", this);
        }

        public void Invoke(string error, string dateTime)
        {
            //OnTimeCallback?.Invoke(error, dateTime);
        }

        public void Invoke(AndroidJavaObject obj, string evt)
        {
            Debug.Log($"=========> {evt}");

            try
            {
                var data = JsonUtility.FromJson<TimeNetData>(evt);
                OnCompleted?.Invoke(data, null);
            }
            catch (Exception e)
            {
                OnCompleted?.Invoke(default, $"can not convert from json to TimeNetData. Details: \n {e}");
            }
        }

        public void Invoke2(AndroidJavaObject obj, string json)
        {
            Debug.Log($"=====> data time: {json}");
            try
            {
                DateTime.TryParse(json, out DateTime dt);
                
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
                OnCompleted?.Invoke(timeNetData, null);
            }
            catch (Exception e)
            {
                OnCompleted?.Invoke(default, $"can not get datetime. Details:\n{e}");
            }
        }
        
        public void Invoke2(string json, AndroidJavaObject obj)
        {
            Debug.Log($"=====> data time invert method: {json}");
            try
            {
                DateTime.TryParse(json, out DateTime dt);
                
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
                OnCompleted?.Invoke(timeNetData, null);
            }
            catch (Exception e)
            {
                OnCompleted?.Invoke(default, $"can not get datetime. Details:\n{e}");
            }
        }

        public UnityAction<TimeNetData, string> OnCompleted { get; set; }

        public void GetTimeByPublicIp(string publicIp)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            jvaClass.Call("getNetworkTimeByPublicIp", publicIp);
#elif UNITY_EDITOR
            Debug.Log("[RealTimeNet] This method only work on Android Or iOS platform.");
#endif
        }

        public void GetUtcTime()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            jvaClass.Call("getNetworkUTCTime");
#elif UNITY_EDITOR
            Debug.Log("[RealTimeNet] This method only work on Android Or iOS platform.");
#endif
        }

        public void GetNASATime()
        {
            jvaClass.Call("getNASATTime");
        }
    }
#endif
}