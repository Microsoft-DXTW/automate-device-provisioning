﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Samples;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Azure.Devices.Client;
namespace SymmetricKeySample
{
    class Program
    {
        private static IAuthenticationMethod GetAuthenticationMethod(){
            string json = new DataStorage().Read("AuthenticationMethod");
            if(!string.IsNullOrEmpty(json)){
                IAuthenticationMethod auth = JsonConvert.DeserializeObject<DeviceAuthenticationWithRegistrySymmetricKey>(json);
                return auth;
            }else{
                return null;
            }
        }
        
        private static void ReadJsonConfiguration(){
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
        }
        public static int Main(string[] args)
        {
            ReadJsonConfiguration();
            
            var db = new DataStorage();
            var secureConnection = db.Read("ConnectionString");
            DevicePortalInfoModel o = JsonConvert.DeserializeObject<DevicePortalInfoModel>(secureConnection);
            DeviceAuthenticationWithRegistrySymmetricKey auth = (DeviceAuthenticationWithRegistrySymmetricKey)o.Auth;
            string iotHub = (string)o.Host;
            DPSDeviceClient sample = null;
                      
            Console.WriteLine($"Connecting to IoT Hub with AuthenticationMethod:{iotHub}");
            sample = new DPSDeviceClient(db);
            
            sample.StartAsync().GetAwaiter().GetResult();

            Console.WriteLine("Enter any key to exit");
            Console.ReadLine();
            return 0;
        }
    }
}
