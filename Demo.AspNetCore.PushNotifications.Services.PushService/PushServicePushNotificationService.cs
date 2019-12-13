using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebPush;
using Demo.AspNetCore.PushNotifications.Services.Abstractions;
using Lib.Net.Http.WebPush;
using System.Collections;
using System.Collections.Generic;

namespace Demo.AspNetCore.PushNotifications.Services.PushService
{
    internal class PushServicePushNotificationService : IPushNotificationService
    {
        private WebPushClient _webPushClient;
        private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;
        private VapidDetails vapidDetails = new VapidDetails("https://website/", "BPikcpCw8J-zXPi7Es3CapL69rlaLJVgwlMspFHV5MIuabObnx0eWksGOViZSM-uwCszh3uDzNNFwspBgRKrO8M", "mgDZOkmxuPs2Txn80ANnttWCrR9LGHN8WVjDogafmdQ");

        private readonly ILogger _logger;

        public string PublicKey { get { return "BPikcpCw8J-zXPi7Es3CapL69rlaLJVgwlMspFHV5MIuabObnx0eWksGOViZSM-uwCszh3uDzNNFwspBgRKrO8M"; } }

        public PushServicePushNotificationService(IPushSubscriptionStoreAccessorProvider subscriptionStoreAccessorProvider, ILogger<PushServicePushNotificationService> logger)
        {
            _webPushClient = new WebPushClient();
            _subscriptionStoreAccessorProvider = subscriptionStoreAccessorProvider;
            _logger = logger;
        }

        public Task SendNotificationAsync(Lib.Net.Http.WebPush.PushSubscription subscription, PushMessage message)
        {
            return SendNotificationAsync(subscription, message, CancellationToken.None);
        }

        public async Task SendNotificationAsync(Lib.Net.Http.WebPush.PushSubscription subscription, PushMessage message, CancellationToken cancellationToken)
        {
            try
            {
                WebPush.PushSubscription sub = new WebPush.PushSubscription();

                foreach(KeyValuePair<string,string> en in subscription.Keys)
                {
                    Console.WriteLine("Keys and Values: ");

                    Console.WriteLine(en.Key + " :  "+ en.Value);
                }
                sub.Auth = subscription.Keys["auth"];
                sub.Endpoint = subscription.Endpoint;
                sub.P256DH = subscription.Keys["p256dh"];

                await _webPushClient.SendNotificationAsync(sub, message.Content, vapidDetails);
            }
            catch (Exception ex)
            {
                await HandlePushMessageDeliveryExceptionAsync(ex, subscription);
            }
        }

        private async Task HandlePushMessageDeliveryExceptionAsync(Exception exception, Lib.Net.Http.WebPush.PushSubscription subscription)
        {
            PushServiceClientException pushServiceClientException = exception as PushServiceClientException;

            if (pushServiceClientException is null)
            {
                _logger?.LogError(exception, "Failed requesting push message delivery to {0}.", subscription.Endpoint);
            }
            else
            {
                if ((pushServiceClientException.StatusCode == HttpStatusCode.NotFound) || (pushServiceClientException.StatusCode == HttpStatusCode.Gone))
                {
                    using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    {
                        await subscriptionStoreAccessor.PushSubscriptionStore.DiscardSubscriptionAsync(subscription.Endpoint);
                    }
                    _logger?.LogInformation("Subscription has expired or is no longer valid and has been removed.");
                }
            }
        }
    }
}
