using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;

namespace Life
{

    public class LifeUI : MonoBehaviour
    {
        [SerializeField] private Text txtLifeAmount;
        [SerializeField] private Text txtDetail;

        [SerializeField] private Text txtLifeAmountPopup;
        [SerializeField] private Text txtDetailPopup;
        [SerializeField] private GameObject gobjImgInfinity;
        [SerializeField] private GameObject gobjImgInfinityPopup;
        [SerializeField] private GameObject gobjAddLife;
        [SerializeField] private Button btnBuy;
        [SerializeField] private GameObject gobjButtonsBuy;
        [SerializeField] private GameObject gobjAdd;
        [SerializeField] private List<LifeState> lifeStates;

        public void InitLifeStates(int lifeCount)
        {
            if (lifeStates == null || lifeStates.Count == 0)
            {
                Debug.LogError("Life states are not assigned.");
                return;
            }
            for (int i = 0; i < lifeStates.Count; i++)
            {
                if (i < lifeCount)
                {
                    lifeStates[i].InitState(true);
                }
                else
                {
                    lifeStates[i].InitState(false);
                }
            }
        }
        public void Init(bool isInfinity, bool isFull, int life, TimeSpan timeSpan)
        {
            txtLifeAmount.text = $"{life}";
            txtLifeAmountPopup.text = $"{life}";
            InitLifeStates(life);

            if (!isFull)
            {
                var detail = timeSpan.ToString(@"hh\:mm\:ss");
              
                if (timeSpan.TotalHours < 100)
                {
                    // Hiển thị tổng số giờ, phút và giây
                    detail = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                else
                {
                    // Hiển thị ngày, giờ, phút và giây
                    detail = $"{timeSpan.Days:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
                if (timeSpan.TotalHours == 0)
                {
                    // Hiển thị tổng số phút và giây
                    detail = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
                }
               /// detail = Utility.CountDownTimeToString((int)timeSpan.TotalDays, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                txtDetail.text = detail;
                txtDetailPopup.text = detail;
            }
            else
            {
                txtDetail.text = "FULL";
                txtDetailPopup.text = "FULL";
            }
            gobjAddLife.SetActive(!isFull);
            gobjImgInfinity.SetActive(isInfinity);
            gobjImgInfinityPopup.SetActive(isInfinity);
            txtLifeAmount.gameObject.SetActive(!isInfinity);
            txtLifeAmountPopup.gameObject.SetActive(!isInfinity);

            // btnBuy.interactable = !isFull;
            gobjButtonsBuy.SetActive(!isFull);
            gobjAdd.SetActive(!isFull && !isInfinity);

        }
        public void UpdateTextLife(int life)
        {
            txtLifeAmount.text = $"{life}";
            txtLifeAmountPopup.text = $"{life}";
            InitLifeStates(life);

        }
        public void UpdateTextDetail(TimeSpan timeSpan)
        {
            var detailInPopup = timeSpan.ToString(@"hh\:mm\:ss");
            var detailInPanel = timeSpan.ToString(@"hh\:mm\:ss");
            if (timeSpan.TotalHours < 100)
            {
                // Hiển thị tổng số giờ, phút và giây
                detailInPopup = $"{(int)timeSpan.TotalHours:D2}H {timeSpan.Minutes:D2}M {timeSpan.Seconds:D2}S";
                detailInPanel = $"{(int)timeSpan.TotalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            else
            {
                // Hiển thị ngày, giờ, phút và giây
                detailInPopup = $"{(int)timeSpan.TotalDays:D2}D {timeSpan.Hours:D2}H {timeSpan.Minutes:D2}M {timeSpan.Seconds:D2}S";
                detailInPanel = $"{(int)timeSpan.TotalDays:D2}:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }
            if (timeSpan.TotalHours == 0)
            {
                // Hiển thị tổng số phút và giây
                detailInPopup = $"{timeSpan.Minutes:D2}M {timeSpan.Seconds:D2}S";
                detailInPanel = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            }

            txtDetail.text = $"{detailInPanel}";
            txtDetailPopup.text = $"{detailInPopup}";

            //Debug.Log($"Time {timeSpan.ToString()}");
        }
    }
}
