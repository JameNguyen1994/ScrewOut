using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rest.API;
using UnityEngine;

namespace Rest.API
{
    public class BAL
    {
        private DAL dal;
        public BAL()
        {
            dal = new DAL();
        }

        public async Task<ResponseResult<RewardBitLabResponse>> GetRewardBitLabResponse(string userId, string token, string packageName)
        {
            var request = dal.Request<RewardBitLabResponse>(HttpMethod.GET, "", "reward", new Dictionary<string, string>()
            {
                ["accept"] = "application/json",
                ["Authorization"] = token
            }, new Dictionary<string, string>()
            {
                ["uuid"] = userId,
                ["package_name"] = packageName
            });

            var result = await request;
            return result;
        }

        public async Task<ResponseResult<RegisterUserResponse>> RegisterUser()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>()
            {
                ["package_name"] = Application.identifier
            };

            string jsonBody = JsonConvert.SerializeObject(dict);
            
            var request = dal.Request<RegisterUserResponse>(HttpMethod.POST, jsonBody, "register-user", new Dictionary<string, string>()
            {
                ["accept"] = "application/json",
                ["Authorization"] = "token_abc123"
            });

            var result = await request;
            return result;
        }
    }
}

