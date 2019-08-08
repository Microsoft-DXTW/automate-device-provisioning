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

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class DPSDeviceClient
    {
        bool _sendTelemetry = true;
        IAuthenticationMethod _auth = null;
        string _iotHub = string.Empty;
        string _connectionString = string.Empty;
        public DPSDeviceClient(string iotHubName, IAuthenticationMethod auth)
        {
            _auth = auth;
            _iotHub = iotHubName;
        }
        public DPSDeviceClient(string connString)
        {
            _connectionString = connString;
        }
        
        #region Direct Methods
        private async Task<Microsoft.Azure.Devices.Client.MethodResponse> ReprovisionHandler(MethodRequest methodRequest, object userContext){
            Console.WriteLine("Cloud Invokes Reprovision method...");

            await StartAsync();

            return new MethodResponse(0);
        }
        #endregion

        #region Telemetry loop
        private DeviceClient CreateDeviceClient(){
            DeviceClient client = null;

            if(!string.IsNullOrEmpty(_iotHub) && _auth != null){
                Console.WriteLine($"CreateDeviceClient::{_iotHub}");
                client = DeviceClient.Create(_iotHub, _auth, TransportType.Mqtt_WebSocket_Only);
            }else if(!string.IsNullOrEmpty(_connectionString)){
                Console.WriteLine($"CreateDeviceClient::{_connectionString}");
                client = DeviceClient.CreateFromConnectionString(_connectionString);
            }else{
                throw new ArgumentNullException("Connection information");
            }
            
            client.SetMethodHandlerAsync("Reprovision", ReprovisionHandler, null).GetAwaiter().GetResult();
            client.SetDesiredPropertyUpdateCallbackAsync(DesiredPropertyUpdated, null).GetAwaiter().GetResult();
            return client;
        }
        private async Task SendTelemetryAsync(){
            using (DeviceClient iotClient = CreateDeviceClient())
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
#if true
                    foreach(var line in telemetryGenerator.GenerateTelemetryLines())
                    {
                        var message = new Message(Encoding.UTF8.GetBytes(line));
                        message.ContentType = "application/JSON";
                        message.ContentEncoding = "utf-8";
                        Console.WriteLine($"Sending telemetry...{line}");
                        await iotClient.SendEventAsync(message).ConfigureAwait(false);
                        await Task.Delay(1000 * 3);
                    }
#else
                    var body = telemetryGenerator.GenerateTelemetry();

                    var message = new Message(Encoding.UTF8.GetBytes(body));
                    message.ContentType = "application/JSON";
                    message.ContentEncoding = "utf-8";
                    await iotClient.SendEventAsync(message).ConfigureAwait(false);
                    await Task.Delay(1000 * 3);
#endif
                }
                Console.WriteLine("DeviceClient CloseAsync.");
                await iotClient.CloseAsync().ConfigureAwait(false);
            }       
        }
        #endregion
        public async Task StartAsync()
        {
            _sendTelemetry = false;

            await SendTelemetryAsync();
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
