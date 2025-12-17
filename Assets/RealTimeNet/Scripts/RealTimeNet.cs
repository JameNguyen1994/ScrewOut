using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PS.NetworkTime
{
    public class RealTimeNet
    {
        private IRealTimeNetBridge realTime;

        public IRealTimeNetBridge RealTime => realTime;

        public RealTimeNet()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            realTime = new RealTimeNetAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
            realTime = new RealTimeNetIos();
#else
            realTime = new RealTimeNetUnity();
#endif
        }

        public static DateTime ConvertTimeStringToDateTime(string strTime)
        {
            DateTimeOffset dto = DateTimeOffset.Parse(strTime);
            return dto.DateTime;
        }
    }

    public struct TimeNetData
    {
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public int seconds;
        public int milliSeconds;
        public string dateTime;
        public string date;
        public string time;
        public string timeZone;
        public string dayOfWeek;
        public bool dstActive;

        public override string ToString()
        {
            return $"year: {year}\n" +
                   $"month: {month}\n" +
                   $"day: {day}\n" +
                   $"hour: {hour}\n" +
                   $"minute: {minute}\n" +
                   $"sec: {seconds}\n" +
                   $"milliSec: {milliSeconds}\n" +
                   $"dateTime: {dateTime}\n" +
                   $"date: {date}\n" +
                   $"time: {time}\n" +
                   $"timeZone: {timeZone}\n" +
                   $"dayOfWeek: {dayOfWeek}\n" +
                   $"dstActive: {dstActive}";
        }
    }
}