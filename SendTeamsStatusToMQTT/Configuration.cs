using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendTeamsStatusToMQTT
{
    public class Configuration
    {
        public string MqttServer;
        public string MqttUsername;
        public string MqttPassword;
        public string MqttTopic;
    }
}
