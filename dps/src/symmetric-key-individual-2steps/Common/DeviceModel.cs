﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class DeviceModel
    {
        public string ID_Scope { get; set; }
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public string RegistrationId { get; set; }
        public string DeviceId { get; set; }
    }
}
