using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
namespace web.Models
{
    public class DevicePortalInfoModel
    {
        public string Host { get; set; }
        public IAuthenticationMethod Auth { get; set; }
        public DeviceModel Device { get; set; }
        public string ConnectionString { get; set; }
    }
}
