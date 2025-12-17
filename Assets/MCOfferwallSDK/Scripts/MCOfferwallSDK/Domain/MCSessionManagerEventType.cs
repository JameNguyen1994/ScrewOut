using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.MCOfferwallSDK.Scripts.MCOfferwallSDK.Domain
{
    class MCSessionManagerEventType
    {
        public const string SessionCount = "totalSessions";
        public const string CurrentSessionTime = "currentSessionTime";
        public const string LastSessionTime = "lastSessionTime"; 
        public const string NewSession = "app_open";
    }
}
