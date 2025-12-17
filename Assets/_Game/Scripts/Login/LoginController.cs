using System;
using System.Collections.Generic;
using Rest.API;
using Storage;
using Storage.Model;
using UnityEngine;

public class LoginController : Singleton<LoginController>
{
    public bool IsLogin { get; private set; }
    
    public void LoginOrRegister()
    {
        var uInfo = Db.storage.OW_USER_INFO;
        
        if(string.IsNullOrEmpty(uInfo.uuid) || string.IsNullOrEmpty(uInfo.token))
        {
            BAL bal = new BAL();
            bal.RegisterUser().ContinueWith(task => OnRegisterUserCallback(task.Result));
            return;
        }

        IsLogin = true;
    }

    void OnRegisterUserCallback(ResponseResult<RegisterUserResponse> result)
    {
        if (result.Error != null)
        {
            print($"[Login] Error: {result.Error}");
            IsLogin = false;
            return;
        }

        var uInfo = new OfferwallUserInfo();
        uInfo.uuid = result.Data.UserId;
        uInfo.token = result.Data.Token;
        
        UnityMainThreadDispatcher.Instance.Enqueue(OnQueueRegister, uInfo);
        
    }

    void OnQueueRegister(OfferwallUserInfo uInfo)
    {
        Db.storage.OW_USER_INFO = uInfo;
        IsLogin = true;
    }
}