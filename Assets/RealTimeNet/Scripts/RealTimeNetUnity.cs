using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;

namespace PS.NetworkTime
{
    public class RealTimeNetUnity : IRealTimeNetBridge
    {
        public UnityAction<TimeNetData, string> OnCompleted { get; set; }
        public void GetTimeByPublicIp(string publicIp)
        {
            Debug.LogWarning("[RealTimeNet] anti-cheat time just work on <color=red><b>Android/iOS</b></color> platform only. This is just your device time on Unity Editor.");
            OnCompleted?.Invoke(FakeData(), null);
        }

        public void GetUtcTime()
        {
            Debug.LogWarning("[RealTimeNet] anti-cheat time just work on <color=red><b>Android/iOS</b></color> platform only. This is just your device time on Unity Editor.");
            OnCompleted?.Invoke(FakeData(), null);
        }

        public void GetNASATime()
        {
            Debug.LogWarning("[RealTimeNet] anti-cheat time just work on <color=red><b>Android/iOS</b></color> platform only. This is just your device time on Unity Editor.");
            OnCompleted?.Invoke(FakeData(), null);
        }

        TimeNetData FakeData()
        {
            var dt = DateTime.UtcNow;
            return new TimeNetData()
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
                timeZone = "Local UTC",
                dayOfWeek = $"{dt.DayOfWeek}",
                dstActive = true
            };
        }
    }
}
