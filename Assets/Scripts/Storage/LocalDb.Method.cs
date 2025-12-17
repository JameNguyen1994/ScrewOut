using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes.Converters;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using UnityEngine;

namespace Storage
{
    public partial class LocalDb
    {
        private T GetFromJson<T>(string key)
        {
            var json = ObscuredPrefs.Get(key, "");
            return JsonConvert.DeserializeObject<T>(json, new ObscuredTypesNewtonsoftConverter());
        }

        private void SetFromTData<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value, new ObscuredTypesNewtonsoftConverter());
            ObscuredPrefs.Set(key, json);
        }
    }
}