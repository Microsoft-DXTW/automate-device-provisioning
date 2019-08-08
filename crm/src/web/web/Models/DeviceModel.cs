using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class DeviceModel
    {
        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; }
        public bool Provisioned { get; set; }
        public string RegistrationId { get; set; }
        public string DeviceId { get; set; }
        public string AllowedDevices { get; set; }
        public string DeniedDevices { get; set; }
        public string Tags { get; set; }
    }
}
