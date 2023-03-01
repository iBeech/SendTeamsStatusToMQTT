// See https://aka.ms/new-console-template for more information
using System;
using Newtonsoft.Json;
using SendTeamsStatusToMQTT;

if (!File.Exists("config.json"))
{
    Console.WriteLine("Configuration file not present");
    return;
}

var config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
if(config == null)
{
    Console.WriteLine("Error reading Configuration file");
    return;
}

var mqtt = new MQTT(config.MqttServer, config.MqttUsername, config.MqttPassword);

Status lastStatus = Status.Unknown;

while (true)
{
    
    // Check teams logs for the current status of the local user
    var status = TeamsStatus.GetCurrentStatus();

    // Status has not changed, don't update the MQTT server
    if (lastStatus == status.CurrentStatus) continue;

    // Publish the MQTT topic for this user
    await mqtt.Publish_Application_Message($"{config.MqttTopic}/presence", JsonConvert.SerializeObject(status));

    // Update the last status so we don't DDOS the MQTT broker
    lastStatus = status.CurrentStatus;

    Thread.Sleep(5000);
}