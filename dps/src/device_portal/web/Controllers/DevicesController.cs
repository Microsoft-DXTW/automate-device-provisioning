using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using web.Models;
using System.IO;
using web.Helpers;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;

namespace web.Controllers
{
    public class DevicesController : Controller
    {
        private IConfiguration _config = null;
        
        public DevicesController(IConfiguration config)
        {
            _config = config;
        }
        
        public IActionResult Details(DeviceModel device)
        {
            return View(device);
        }
        //  User input device key infor.
        public IActionResult Create(DeviceModel device)
        {
            if (device != null && !string.IsNullOrEmpty(device.RegistrationId))
            {
                try
                {
                    var result = DPSHelper.ProvisionDeviceAsync(device).GetAwaiter().GetResult();
                    dynamic o = JsonConvert.DeserializeObject(result);
                    FileStorage.WriteProvisioningRecords(JsonConvert.DeserializeObject<DeviceModel>((string)o.Device));
                    FileStorage.WriteConnectionString((string)o.ConnectionString);
                    /*
                     * new
                    {
                        Host = host,
                        Auth = JsonConvert.SerializeObject(auth),
                        Device = device,
                        ConnectionString = builder.ToString()
                    };
                     * */
                }
                catch
                {
                    return View();
                }
            }
            return View(device);
        }
        
        public IActionResult List()
        {
            string text = FileStorage.ReadProvisioningRecods();
            return View(new DeviceListModel(text));
        }
    }
}