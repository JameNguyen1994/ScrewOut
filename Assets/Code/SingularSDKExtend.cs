using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Proyecto26;
using UnityEngine;

public partial class SingularSDK : MonoBehaviour
{
    internal class Crypto
    {
        public Crypto() {}
        
        private readonly uint[] table =
        {
            0xf83da249, 0x15d12772, 0x40c50697, 0x984e2b6b, 0x14ec5ff8, 0xb2e24927, 0x3b8f77ae, 0x472474cd,
            0x5b0ce524, 0xa17e1a31, 0x6c60852c, 0xd86ad267, 0x832612b7, 0x1ca03645, 0x5515abc8, 0xc5feff52,
            0xffffac00, 0x0fe95cb6, 0x79cf43dd, 0xaa48a3fb, 0xe1d71788, 0x97663d3a, 0xf5cffea7, 0xee617632,
            0x4b11a7ee, 0x040ef0b5, 0x0606fc00, 0xc1530fae, 0x7a827441, 0xfce91d44, 0x8c4cc1b1, 0x7294c28d,
            0x8d976162, 0x8315435a, 0x3917a408, 0xaf7f1327, 0xd4bfaed7, 0x80d0abfc, 0x63923dc3, 0xb0e6b35a,
            0xb815088f, 0x9bacf123, 0xe32411c3, 0xa026100b, 0xbcf2ff58, 0x641c5cfc, 0xc4a2d7dc, 0x99e05dca,
            0x9dc699f7, 0xb76a8621, 0x8e40e03c, 0x28f3c2d4, 0x40f91223, 0x67a952e0, 0x505f3621, 0xbaf13d33,
            0xa75b61cc, 0xab6aef54, 0xc4dfb60d, 0xd29d873a, 0x57a77146, 0x393f86b8, 0x2a734a54, 0x31a56af6,
            0x0c5d9160, 0xaf83a19a, 0x7fc9b41f, 0xd079ef47, 0xe3295281, 0x5602e3e5, 0xab915e69, 0x225a1992,
            0xa387f6b2, 0x7e981613, 0xfc6cf59a, 0xd34a7378, 0xb608b7d6, 0xa9eb93d9, 0x26ddb218, 0x65f33f5f,
            0xf9314442, 0x5d5c0599, 0xea72e774, 0x1605a502, 0xec6cbc9f, 0x7f8a1bd1, 0x4dd8cf07, 0x2e6d79e0,
            0x6990418f, 0xcf77bad9, 0xd4fe0147, 0xfef4a3e8, 0x85c45bde, 0xb58f8e67, 0xa63eb8d7, 0xc69bd19b,
            0xda442dca, 0x3c0c1743, 0xe6f39d49, 0x33568804, 0x85eb6320, 0xda223445, 0x36c4a941, 0xa9185589,
            0x71b22d67, 0xf59a2647, 0x3c8b583e, 0xd7717ded, 0xdf05699c, 0x4378367d, 0x1c459339, 0x85133b7f,
            0x49800ce2, 0x3666ca0d, 0xaf7ab504, 0x4ff5b8f1, 0xc23772e3, 0x3544f31e, 0x0f673a57, 0xf40600e1,
            0x7e967417, 0x15a26203, 0x5f2e34ce, 0x70c7921a, 0xd1c190df, 0x5bb5da6b, 0x60979c75, 0x4ea758a4,
            0x078fe359, 0x1664639c, 0xae14e73b, 0x2070ff03
        };
        
        internal string EncryptFile(string plain)
        {
            byte[] src = Encoding.UTF8.GetBytes(plain);
            int len = src.Length;
            byte[] cloneSrc = new byte[src.Length];

            for (int i = 0; i < len * 4; i += 4)
            {
                int index = (i + (i / 132)) % 132;
                cloneSrc[i / 4] = (byte)(table[index] ^ src[i / 4]);
            }

            return Convert.ToBase64String(cloneSrc);
        }

        // internal string DecryptFile(string cipher)
        // {
        //     byte[] src = Convert.FromBase64String(cipher);
        //     int len = src.Length;
        //     byte[] cloneSrc = new byte[src.Length];
        //
        //     for (int i = 0; i < len * 4; i += 4)
        //     {
        //         int id = (i + (i / 132)) % 132;
        //         cloneSrc[i / 4] = (byte)(table[id] ^ src[i / 4]);
        //     }
        //
        //     return Encoding.UTF8.GetString(cloneSrc);
        // }
    }
    
#if UNITY_ANDROID
    private static AndroidJavaObject psSingular;

    public static string GetRawEvent(string eventName, Dictionary<string, object> jsonObjects)
    {
        AndroidJavaObject extra = new AndroidJavaObject("org.json.JSONObject",
            JsonConvert.SerializeObject(jsonObjects, Formatting.None));
        //string extra = JsonConvert.SerializeObject(jsonObjects);
        if (psSingular == null)
        {
            psSingular = new AndroidJavaObject("com.singular.sdk.internal.SingularSdkExtend");
        }

        string json = psSingular.Call<string>("getEventRaw", eventName, extra);

        return json;
    }

    public static string GetRawAdRevenueEvent(SingularAdData data)
    {
        AndroidJavaObject extra =
            new AndroidJavaObject("org.json.JSONObject", JsonConvert.SerializeObject(data, Formatting.None));
        //string extra = JsonConvert.SerializeObject(data);
        psSingular ??= new AndroidJavaObject("com.singular.sdk.internal.SingularSdkExtend");

        var json = psSingular.Call<string>("getEventRaw", ADMON_REVENUE_EVENT_NAME, extra);

        return json;
    }

    public static string GetDeviceInfo()
    {
#if UNITY_EDITOR
        return "{}";
#endif
        psSingular ??= new AndroidJavaObject("com.singular.sdk.internal.SingularSdkExtend");

        var json = psSingular.Call<string>("getDeviceInfo");

        return json;
    }

    public static void EventS2S(string name)
    {
        EventS2S(name, null);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void EventS2S(string name, Dictionary<string, object> extra)
    {
        if (!instance)
        {
            Debug.LogError("[SingularSDK] That method just work on mobile platform.");
            return;
        }

        string json = GetRawEvent(name, extra);
        json = new Crypto().EncryptFile(json);

#if STAGING_SERVER
        string topic = "staging-fraud-detection";
#else
        string topic = "production-fraud-detection";
#endif

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            ["records"] = new List<object>()
            {
                new Dictionary<string, object>()
                {
                    ["value"] = json
                }
            }
        };

        string dataJson = JsonConvert.SerializeObject(data, Formatting.None);

        var requestHelper = new RequestHelper()
        {
            Uri = $"http://ec2-54-144-176-15.compute-1.amazonaws.com:8082/topics/{topic}",
            Headers = new Dictionary<string, string>()
            {
                ["Content-Type"] = "application/vnd.kafka.json.v2+json",
                ["Authorization"] =
                    $"Basic dXNlcl9mZHJlc3RjbGllbnQ6ZlBRUUh4TFM4RmRQMTR6a0NzYm1veTZ4RkhjTWdzcHBBZGVtUGhpV0FzRExyNlNVYmttMUJFd3FGc2ZqdmViTQ=="
            },
            EnableDebug = false,
            BodyString = dataJson
        };

        RestClient.Post(requestHelper).Then(print, rej => { print($"error: {rej}"); });
    }
#endif
}