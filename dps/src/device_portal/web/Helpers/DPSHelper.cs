using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using web.Models;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Client;

namespace web.Helpers
{
    public class DPSRequest
    {
        public string RegistrationID { get; set; } = string.Empty;
        public Dictionary<string, string> DesiredProperties { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public override string ToString()
        {
            var o = new
            {
                registrationId = RegistrationID,
                desiredProperties = new System.Dynamic.ExpandoObject(),
                tags = new System.Dynamic.ExpandoObject(),
            };
            foreach(var p in DesiredProperties.Keys)
            {
                o.desiredProperties.TryAdd(p,DesiredProperties[p]);
            }
            foreach (var t in Tags.Keys)
            {
                o.tags.TryAdd(t, Tags[t]);
            }
            return JsonConvert.SerializeObject(
                    o
                );
        }
    }
    public class DPSHelper
    {
        private static string GlobalDeviceEndpoint = "global.azure-devices-provisioning.net";

        private static IAuthenticationMethod GetAuthenticationMethod(DeviceRegistrationResult result, SecurityProvider security)
        {
            IAuthenticationMethod auth;
            if (security is SecurityProviderTpm)
            {
                Console.WriteLine("Creating TPM DeviceClient authentication.");
                auth = new DeviceAuthenticationWithTpm(result.DeviceId, security as SecurityProviderTpm);
            }
            else if (security is SecurityProviderX509)
            {
                Console.WriteLine("Creating X509 DeviceClient authentication.");
                auth = new DeviceAuthenticationWithX509Certificate(result.DeviceId, (security as SecurityProviderX509).GetAuthenticationCertificate());
            }
            else if (security is SecurityProviderSymmetricKey)
            {
                Console.WriteLine("Creating Symmetric Key DeviceClient authenication");
                auth = new DeviceAuthenticationWithRegistrySymmetricKey(result.DeviceId, (security as SecurityProviderSymmetricKey).GetPrimaryKey());
            }
            else
            {
                throw new NotSupportedException("Unknown authentication type.");
            }
            return auth;
        }
        public static async Task<string> ProvisionDeviceAsync(DeviceModel device)
        {
            try
            {

                using (var security = new SecurityProviderSymmetricKey(device.RegistrationId, device.PrimaryKey, device.SecondaryKey))
                {
                    using (var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
                    {
                        ProvisioningDeviceClient provClient =
                            ProvisioningDeviceClient.Create(GlobalDeviceEndpoint, device.ID_Scope, security, transport);
                        var result = await provClient.RegisterAsync();

                        if (result.Status != ProvisioningRegistrationStatusType.Assigned)
                            return null;
                        IAuthenticationMethod auth = GetAuthenticationMethod(result, security);
                        var host = $"{result.AssignedHub}";
                        var builder = IotHubConnectionStringBuilder.Create(host, auth);
                        device.PrimaryKey = security.GetPrimaryKey();
                        device.SecondaryKey = security.GetSecondaryKey();
                        var o = new
                        {
                            Host = host,
                            Auth = JsonConvert.SerializeObject(auth),
                            Device = JsonConvert.SerializeObject(device),
                            ConnectionString = builder.ToString()
                        };
                        return JsonConvert.SerializeObject(o);
                    }
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}
