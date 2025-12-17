using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MCOfferwallSDK.Service
{

    public class RateLimitServiceResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public static class RateLimitService
    {
        private const string prefix = "io.mychips.";

        // Checks if a request can be made based on the sliding window and rate limits
        public static RateLimitServiceResponse CanMakeRequest(string methodName, int dailyCap, long minuteCapDurationMinutes, int slidingWindowsDay)
        {
            string dailyCountKey = $"{prefix}{methodName}_dailyCount";
            string lastRequestTimeKey = $"{prefix}{methodName}_lastRequestTime";
            string eventTimeKey = $"{prefix}{methodName}_eventTime";

            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            long lastRequestTime = GetLong(lastRequestTimeKey, 0); // Adjusted to handle long values
            int dailyCount = PlayerPrefs.GetInt(dailyCountKey, 0);
            long eventTime = GetLong(eventTimeKey, 0); // Adjusted to handle long values


            Debug.Log($"MCOfferwallSDK:RL currentTime {currentTime}");
            Debug.Log($"MCOfferwallSDK:RL lastRequest {lastRequestTime}");
            Debug.Log($"MCOfferwallSDK:RL dailyCount {dailyCount}");
            Debug.Log($"MCOfferwallSDK:RL eventTime {eventTime}");

            // If no event has occurred to start the sliding window, block all requests
            if (eventTime == 0 || currentTime - eventTime > TimeSpan.FromDays(slidingWindowsDay).TotalMilliseconds)
            {
                Debug.Log("no event tracked in the last 60 days");
                return new RateLimitServiceResponse() { Success = false, Message = "User should open the offerwall at least once" };

            }

            // Check and reset daily cap if it's a new day
            if (!IsSameDay(currentTime, lastRequestTime))
            {
                PlayerPrefs.SetInt(dailyCountKey, 0);
                dailyCount = 0;
            }

            // Check minute cap
            if (currentTime - lastRequestTime < TimeSpan.FromMinutes(minuteCapDurationMinutes).TotalMilliseconds)
            {
                Debug.Log($"MCOfferwallSDK:RL minute cap reached");

                return new RateLimitServiceResponse() { Success = false, Message = "Rate limit - minute cap reached" };
            }

            // Check daily cap
            if (dailyCount >= dailyCap)
            {
                Debug.Log($"MCOfferwallSDK:RL daily cap reached");
                return new RateLimitServiceResponse() { Success = false, Message = "Rate limit - daily cap reached" };
            }

            // All checks passed, update PlayerPrefs and allow request
            SetLong(lastRequestTimeKey, currentTime); // Adjusted to handle long values
            PlayerPrefs.SetInt(dailyCountKey, dailyCount + 1);
            PlayerPrefs.Save();

            return new RateLimitServiceResponse() { Success = true };
        }

        // Resets the sliding window on specific events
        public static void ResetSlidingWindow(string methodName)
        {

            string eventTimeKey = $"{prefix}{methodName}_eventTime";
            try
            {
                SetLong(eventTimeKey, DateTimeOffset.Now.ToUnixTimeMilliseconds()); // Adjusted to handle long values
                PlayerPrefs.Save();

            }
            catch (Exception err)
            {


            }


        }



        private static bool IsSameDay(long date1, long date2)
        {
            DateTime dt1 = DateTimeOffset.FromUnixTimeMilliseconds(date1).DateTime;
            DateTime dt2 = DateTimeOffset.FromUnixTimeMilliseconds(date2).DateTime;
            return dt1.Date == dt2.Date;
        }

        private static long GetLong(string key, long defaultValue)
        {
            string valueStr = PlayerPrefs.GetString(key, defaultValue.ToString());
            long value;
            return long.TryParse(valueStr, out value) ? value : defaultValue;
        }

        // Helper method to set a long value in PlayerPrefs
        private static void SetLong(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

    }
}
