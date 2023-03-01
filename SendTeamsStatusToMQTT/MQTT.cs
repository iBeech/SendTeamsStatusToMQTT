// See https://aka.ms/new-console-template for more information
using MQTTnet.Client;
using MQTTnet;

public class MQTT : IDisposable
{
    private IMqttClient client;

    public MQTT(string server, string username, string password)
    {
        var mqttFactory = new MqttFactory();
        client = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(server)
            .WithCredentials(username, password)
            .Build();

        client.ConnectAsync(mqttClientOptions, CancellationToken.None);
    }

    public void Dispose()
    {
        client?.DisconnectAsync();
        client?.Dispose();
    }

    public async Task Publish_Application_Message(string topic, string payload)
    {
        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        await client.PublishAsync(applicationMessage, CancellationToken.None);

        Console.Clear();
        Console.WriteLine($"Wrote {payload} to topic {topic}");
    }
}