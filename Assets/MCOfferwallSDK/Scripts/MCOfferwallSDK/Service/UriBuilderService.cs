using Assets.Scripts.MCOfferwallSDK.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MCOfferwallSDK.Service
{
    class UriBuilderService : MonoBehaviour
    {
        public string BuildOfferwallUrl(QueryParameterBuilder queryParameter)
        {
             string finalUrl = Consts.WEB_BASE_URL + queryParameter.BuildQueryString();
             return finalUrl;
        }
    }
}

