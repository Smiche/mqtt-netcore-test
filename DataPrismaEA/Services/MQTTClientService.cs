using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DataPrismaEA.Types;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace DataPrismaEA.Services
{
    public class MQTTClientService : IMQTTClientService
    {
        private static IMqttClient mqttClient;

        public MQTTClientService()
        {
            var factory = new MqttFactory();

            if (mqttClient == null)
            {
                Console.WriteLine("Creating a new MQTTClient internally");

                mqttClient = factory.CreateMqttClient();

                var options = new MqttClientOptionsBuilder().WithWebSocketServer("localhost:5000/mqtt").Build();
                connectClient(options);
            }
        }

        private async Task<IMqttClient> connectClient(IMqttClientOptions options)
        {
            await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
            return mqttClient;
        }

        private static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks =(long) (dateTime.ToUniversalTime() - unixStart).TotalSeconds;
            return unixTimeStampInTicks;
        }

        public Boolean SendMQTTMessage(ChatMessage msg)
        {
            Console.WriteLine("Trying to send chat message to MQTT");

            if (mqttClient.IsConnected)
            {
                ChatMessage toSend = msg;
                User newUser = new User();
                newUser.id = msg.user.id;
                newUser.name = "TestUser HI_" + new Random().Next(10);

                toSend.timestamp = DateTimeToUnixTimestamp(DateTime.Now) + "";
                toSend.user = newUser;

                string payload = JsonSerializer.Serialize<ChatMessage>(toSend);

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("/event/" + msg.eventId + "/" + msg.chatRoomId)
                    .WithPayload(payload)
                    .WithExactlyOnceQoS()
                    .WithRetainFlag(false)
                    .Build();
                mqttClient.PublishAsync(message);
            }
            else
            {
                Console.WriteLine("MQTT not connected locally.");
            }
            return true;
        }
    }
}

