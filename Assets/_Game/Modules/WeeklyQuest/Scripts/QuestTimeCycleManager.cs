using Storage;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace WeeklyQuest
{
    public class QuestTimeCycleManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtTime;
        [SerializeField] private float timeOnline;
        [SerializeField] private long lastTime = -1;

        private void Start()
        {
            lastTime = -1;
            txtTime.text = "Loading...";
            TimeGetter.Instance.RegisActionOnUpdateTime(UpdateTime);
        }
        private void OnDestroy()
        {
            TimeGetter.Instance.UnRegisActionOnUpdateTime(UpdateTime);
        }
        public void UpdateTime()
        {
            if (lastTime < 0)
            {
                lastTime = TimeGetter.Instance.CurrentTime;
                timeOnline += 1;
                //return;
            }
            var timeToEndWeek = TimeHelperController.Instance.GetTimeToEndWeek();
            txtTime.text = FormatTime(timeToEndWeek);
            timeOnline += TimeGetter.Instance.CurrentTime - lastTime;
            lastTime = TimeGetter.Instance.CurrentTime;
           // Debug.Log($"Time Online: {timeOnline} seconds");
            if (timeOnline >= 60 * 1000)
            {
                timeOnline -= 60*1000;
                WeeklyQuestManager.Instance.QuestTriggerController.OnTrigger1Quest(QuestType.StayOnline_minues, 1);
            }
        }
        string FormatTime(long milliseconds)
        {
            var timeSpan = System.TimeSpan.FromMilliseconds(milliseconds);
            return $"{timeSpan.Days:D1}d {timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
    }
}
