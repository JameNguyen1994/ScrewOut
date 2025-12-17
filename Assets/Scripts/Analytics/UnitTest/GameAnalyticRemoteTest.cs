using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PS.Analytic;
using PS.Analytic.RemoteConfig;
using UnityEngine;

namespace PS.Analytic.RemoteConfig
{
    public class GameAnalyticRemoteTest
    {
        public static List<PropertyInfo> GetListVariable(GameAnalyticRemoteConfig remote)
        {
            var type = remote.GetType();
        
            var listVars = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            return listVars.ToList();

//        foreach (var item in listVars)
//        {
//            print($"{item.Name}: {item.GetValue(remote)}");
//        }
        }

        public static string GetValue(object obj, PropertyInfo info)
        {
            return info.GetValue(obj).ToString();
        }
    }

}
