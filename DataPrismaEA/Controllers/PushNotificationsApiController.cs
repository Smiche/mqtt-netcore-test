using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lib.Net.Http.WebPush;
using Demo.AspNetCore.PushNotifications.Model;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using System;
using DataPrismaEA.Types;
using DataPrismaEA.Services;

namespace Demo.AspNetCore.PushNotifications.Controllers
{
    [Route("push-notifications-api")]
    public class PushNotificationsApiController : Controller
    {
        private readonly IPushSubscriptionStore _subscriptionStore;
        private readonly IPushNotificationService _notificationService;
        private readonly IPushNotificationsQueue _pushNotificationsQueue;
        private readonly IMQTTClientService _mqttClientService;

        public PushNotificationsApiController(IPushSubscriptionStore subscriptionStore, IPushNotificationService notificationService, IPushNotificationsQueue pushNotificationsQueue, IMQTTClientService mqttClientService)
        {
            _subscriptionStore = subscriptionStore;
            _notificationService = notificationService;
            _pushNotificationsQueue = pushNotificationsQueue;
            _mqttClientService = mqttClientService;
        }

        // GET push-notifications-api/public-key
        [HttpGet("public-key")]
        public ContentResult GetPublicKey()
        {
            Console.WriteLine("Request for Public key gotted.");
            return Content(_notificationService.PublicKey, "text/plain");
        }

        // GET push-notifications-api/public-key
        [HttpPost("send-message")]
        public async Task<ContentResult> SendMessage([FromBody]ChatMessage chatMessage)
        {
            Console.WriteLine("Request for Public key gotted.");
            _mqttClientService.SendMQTTMessage(chatMessage);
            return Content(_notificationService.PublicKey, "text/plain");
        }


        // POST push-notifications-api/subscriptions
        [HttpPost("subscriptions")]
        public async Task<IActionResult> StoreSubscription([FromBody]PushSubscription subscription)
        {
            await _subscriptionStore.StoreSubscriptionAsync(subscription);

            return NoContent();
        }

        // DELETE push-notifications-api/subscriptions?endpoint={endpoint}
        [HttpDelete("subscriptions")]
        public async Task<IActionResult> DiscardSubscription(string endpoint)
        {
            await _subscriptionStore.DiscardSubscriptionAsync(endpoint);

            return NoContent();
        }

        // POST push-notifications-api/notifications
        [HttpPost("notifications")]
        public IActionResult SendNotification([FromBody]PushMessageViewModel message)
        {
            _pushNotificationsQueue.Enqueue(new PushMessage(message.Notification)
            {
                Topic = message.Topic,
                Urgency = message.Urgency
            });

            return NoContent();
        }
    }
}
