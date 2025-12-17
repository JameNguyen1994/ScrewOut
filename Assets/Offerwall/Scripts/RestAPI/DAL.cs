using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

namespace Rest.API
{
    public class DAL
    {
        public DAL()
        {
            //RestClient.DefaultRequestHeaders["X-S2S-Token"] = "token_abc123";
        }
        
        static readonly string BaseUrl = "https://expuzzle.ddns.net";

        public async Task<ResponseResult<T>> Request<T>(string method, string body, string endpoint, Dictionary<string, string> headers = null) where T : struct
        {
            Uri uri = new Uri(new Uri(BaseUrl), $"api/{endpoint}");
            var request = RestClient.Request(new RequestHelper()
            {
                Uri = uri.AbsoluteUri,
                BodyString = body,
                Method = method,
                Headers = headers ?? new Dictionary<string, string>()
            });

            try
            { 
                var response = await request.ToTask();
                return new ResponseResult<T>()
                {
                    Data = JsonConvert.DeserializeObject<T>(response.Request.downloadHandler.text),
                    Error = null
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"request error: {e.Message}");
                return new ResponseResult<T>()
                {
                    Data = default,
                    Error = e.Message
                };
            }
        }
        
        public async Task<ResponseResult<T>> Request<T>(string method, string body, string endpoint, Dictionary<string, string> headers = null, Dictionary<string, string> queryParams = null) where T : struct
        {
            Uri uri = new Uri(new Uri(BaseUrl), $"api/{endpoint}");
            
            var request = RestClient.Request(new RequestHelper()
            {
                Uri = uri.AbsoluteUri,
                BodyString = body,
                Method = method,
                Headers = headers ?? new Dictionary<string, string>(),
                Params = queryParams ?? new Dictionary<string, string>()
            });

            try
            { 
                var response = await request.ToTask();
                return new ResponseResult<T>()
                {
                    Data = JsonConvert.DeserializeObject<T>(response.Request.downloadHandler.text),
                    Error = null
                };
            }
            catch (Exception e)
            {
                return new ResponseResult<T>()
                {
                    Data = default,
                    Error = e.Message
                };
            }
        }
    }

    public class HttpMethod
    {
        public const string GET = "GET";
        public const string POST = "POST";
        public const string PUT = "PUT";
        public const string DELETE = "DELETE";
        public const string PATCH = "PATCH";
    }
}
