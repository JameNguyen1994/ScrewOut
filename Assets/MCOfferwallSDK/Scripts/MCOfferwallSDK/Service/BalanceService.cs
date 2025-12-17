using Assets.Scripts.MCOfferwallSDK.Domain;
using Assets.Scripts.MCOfferwallSDK.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class BalanceService : MonoBehaviour
{
    public delegate void BalanceErrorEventHandler(Exception err);
    public event BalanceErrorEventHandler OnBalanceErrorReceived;

    public delegate void BalanceChangedEventHandler(BalanceDTO balance);
    public event BalanceChangedEventHandler OnBalanceReceived;
    public void GetBalance(string userId, string adUnitId, bool isDebug)
    {

        //if debug always fire bonus
        if (isDebug)
        {
            OnBalanceReceived?.Invoke(new BalanceDTO(1, 0));
            return;
        }

        //var resp = RateLimitService.CanMakeRequest("getBalance", 20, 1, 60);
        //if (!resp.Success)
        //{

        //    OnBalanceErrorReceived?.Invoke(new Exception(resp.Message));
        //    return;
        //}
        string url = Consts.API_URL + "/balance/" + userId + "?adunit_id=" + adUnitId;
        Debug.Log(url);
        WebClient webClient = new WebClient();
        webClient.DownloadStringCompleted += WebClient_DownloadStringCompleted;
        webClient.DownloadStringAsync(new Uri(url));

        Debug.Log("GetBalance");
    }

    private void WebClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
    {
        ((WebClient)sender).DownloadStringCompleted -= WebClient_DownloadStringCompleted;
        if (e.Error != null)
        {
            OnBalanceErrorReceived?.Invoke(e.Error);
        }
        else
        {
            string jsonResponse = e.Result;
            Debug.Log(jsonResponse);
            BalanceDTO balanceDTO = JsonUtility.FromJson<BalanceDTO>(jsonResponse);

            double delta = balanceDTO.userLTVInVirtualCurrency - balanceDTO.lastSyncUserLTVInVirtualCurrency;
            Debug.Log(delta + "");
            if (delta != 0)
            {
                // Reset

                RateLimitService.ResetSlidingWindow("getBalance");
                OnBalanceReceived?.Invoke(balanceDTO);

            }
        }

    }


    private void Update()
    {

    }




}
