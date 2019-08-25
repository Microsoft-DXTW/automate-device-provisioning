using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace azfunc
{
    public static class iot_notification_handler
    {
        [FunctionName("iot_notification_handler")]
        public static Task Run([ServiceBusTrigger("iot-violation-events", "signalrsubscription", Connection = "ServiceBusConnection")]string mySbMsg, ILogger log,
            [SignalR(HubName = "IoTEventHub")]IAsyncCollector<SignalRMessage> signalRMessages
        )
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
            return signalRMessages.AddAsync(
                new SignalRMessage 
                {
                    Target = "notifyusers", 
                    Arguments = new [] { mySbMsg } 
                });
        }
    }
}
