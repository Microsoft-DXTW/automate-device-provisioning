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
                    device.ID_Scope = _config.GetSection("DPS_Scope").Value;
                    var result = DPSHelper.ProvisionDeviceAsync(device).GetAwaiter().GetResult();
                    FileStorage.WriteProvisioningRecords(result.Device);
                    FileStorage.WriteDeviceProvisioningInfo(JsonConvert.SerializeObject(result));
                }
                catch(Exception exp)
                {
                    return View(new ErrorViewModel{ErrorMessage = exp.Message});
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