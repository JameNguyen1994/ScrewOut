using System;
using Newtonsoft.Json;

namespace Rest.API
{
    [Serializable]
    public struct RewardBitLabResponse
    {
        [JsonProperty("user_id")] public string UserId;
        [JsonProperty("app")] public string App;
        [JsonProperty("total_value")] public float TotalValue;
        [JsonProperty("total_raw")] public float TotalRev;

        public override string ToString()
        {
            return @"uuid: " + UserId + 
                   @" app: " + App + 
                   @" total_value: " + TotalValue + 
                   @" total_raw: " + TotalRev;
        }
    }
    
    [Serializable]
    public struct RegisterUserResponse
    {
        [JsonProperty("user_id")] public string UserId;
        [JsonProperty("token")] public string Token;

        public override string ToString()
        {
            return @"user_id: " + UserId + "\n" +
                   @" token: " + Token;
        }
    }
}
