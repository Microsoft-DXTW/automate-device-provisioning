// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class DPSDeviceClient
    {
        private const string GlobalDeviceEndpoint = "global.azure-devices-provisioning.net";

        bool _sendTelemetry = true;
        DataStorage _storage = null; 
        public DPSDeviceClient(DataStorage storage){
            _storage = storage;
        }
        
        #region Direct Methods
        private async Task<Microsoft.Azure.Devices.Client.MethodResponse> ReprovisionHandler(MethodRequest methodRequest, object userContext){
            Console.WriteLine("Cloud Invokes Reprovision method...");
            await StartAsync();

            return new MethodResponse(0);
        }
        #endregion

        #region Telemetry loop
        private async Task SendTelemetryAsync(DeviceClient iotClient)
        {
            using (iotClient)
            {
                Console.WriteLine("DeviceClient OpenAsync.");
                await iotClient.OpenAsync().ConfigureAwait(false);

                Console.WriteLine("Receiving cloud configuration...");
                await GetCloudConfiguration(iotClient);
                var i = 0;
                _sendTelemetry = true;
                var telemetryGenerator = new RandomTelemetry();
                while( i ++ < 100 && _sendTelemetry){
                    Console.WriteLine("DeviceClient SendEventAsync.");
                    foreach(var line in telemetryGenerator.GenerateTelemetryLines())
                    {
                        var message = new Message(Encoding.UTF8.GetBytes(line));
                        message.ContentType = "application/JSON";
                        message.ContentEncoding = "utf-8";
                        Console.WriteLine($"Sending telemetry...{line}");
                        await iotClient.SendEventAsync(message).ConfigureAwait(false);
                        await Task.Delay(1000 * 3);
                    }

                }
                Console.WriteLine("DeviceClient CloseAsync.");
                await iotClient.CloseAsync().ConfigureAwait(false);
            }       
        }
        #endregion

        public async Task<Microsoft.Azure.Devices.Client.DeviceClient> ProvisionAsync<DeviceClient>(DataStorage storage)
        {
            var connStringContent = storage.Read("ConnectionString");
            DevicePortalInfoModel model = JsonConvert.DeserializeObject<DevicePortalInfoModel>(connStringContent);
            string scope = model.Device.ID_Scope;
            string primaryKey = model.Device.PrimaryKey;
            string secondaryKey = model.Device.SecondaryKey;
            string registrationId = model.Device.RegistrationId;
            
            _sendTelemetry = false;

            using (var security = new SecurityProviderSymmetricKey(registrationId, primaryKey, secondaryKey))
            {
                using (var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
                {
                    ProvisioningDeviceClient provClient =
                        ProvisioningDeviceClient.Create(GlobalDeviceEndpoint, scope, security, transport);
                    DeviceRegistrationResult result = await provClient.RegisterAsync().ConfigureAwait(false);
                    
                    if (result.Status != ProvisioningRegistrationStatusType.Assigned)
                        return null;
                    Console.WriteLine($"Provisioninged device : {result.DeviceId} at {result.AssignedHub}");
                    var client = Microsoft.Azure.Devices.Client.DeviceClient.Create(result.AssignedHub, model.Auth, TransportType.Amqp);
                    client.SetMethodHandlerAsync("Reprovision", ReprovisionHandler, null).GetAwaiter().GetResult();
                    client.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdated, null).GetAwaiter().GetResult();
                    
                    //  TODO:Save new connection information
                    
                    return client;
                }
            }
        }
        public async Task StartAsync()
        {
            _sendTelemetry = false;
            var iotClient = await ProvisionAsync<DeviceClient>(_storage);
            await SendTelemetryAsync(iotClient);
        }
        #region Device Twins
        private Task DesiredPropertyUpdated(TwinCollection desiredProperties, object userContext){
            var json = JsonConvert.SerializeObject(desiredProperties);
            Console.WriteLine($"Received Device Twins configuration:{json}");

            return Task.CompletedTask;
        }
        
        private Task GetCloudConfiguration(DeviceClient client){
            var twin = client.GetTwinAsync().GetAwaiter().GetResult();
            return DesiredPropertyUpdated(twin.Properties.Desired, null);
        }
        #endregion
        
    }
}
