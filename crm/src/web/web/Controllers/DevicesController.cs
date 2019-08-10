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

namespace web.Controllers
{
    public class DevicesController : Controller
    {
        private string _file = @"..\web\app_data\device_provision_list.json";
        private IConfiguration _config = null;
        private string ReadDataStorage()
        {
            string text = System.IO.File.ReadAllText(_file);
            return text;
        }
        private void DeleteFromDataStorage(DeviceModel device)
        {
            string text = System.IO.File.ReadAllText(_file);
            var devices = JsonConvert.DeserializeObject<DeviceModel[]>(text);
            List<DeviceModel> result = new List<DeviceModel>();
            var target = devices.Where(d => d.DeviceId == device.DeviceId).SingleOrDefault();
            if (target != null)
            {
                devices = devices.Where(d => d.DeviceId != device.DeviceId).ToArray();
            }
            
            result.AddRange(devices);
            devices = result.ToArray();

            text = JsonConvert.SerializeObject(devices);
            if (System.IO.File.Exists(_file))
            {
                System.IO.File.Delete(_file);
            }
            System.IO.File.WriteAllText(_file, text);
        }
        private void WriteDataStorage(DeviceModel device)
        {
            string text = System.IO.File.ReadAllText(_file);
            var devices = JsonConvert.DeserializeObject<DeviceModel[]>(text);
            List<DeviceModel> result = new List<DeviceModel>();
            var target = devices.Where(d => d!= null && d.DeviceId == device.DeviceId).SingleOrDefault();
            if(target != null)
            {
                target.RegistrationId = device.RegistrationId;
                target.Tags = device.Tags;
                target.AllowedDevices = device.AllowedDevices;
                target.DeniedDevices = device.DeniedDevices;
                devices = devices.Where(d => d!= null &&  d.DeviceId != device.DeviceId).ToArray();
            }
            else
            {
                result.Add(device);
            }
            result.AddRange(devices);
            target = device;
            devices = result.ToArray();

            text = JsonConvert.SerializeObject(devices);
            if (System.IO.File.Exists(_file))
            {
                System.IO.File.Delete(_file);
            }
            System.IO.File.WriteAllText(_file, text);
        }
        public DevicesController(IConfiguration config)
        {
            _config = config;
        }
        public IActionResult Edit(DeviceModel device)
        {
            WriteDataStorage(device);
            return View(device);
        }
        public IActionResult Details(DeviceModel device)
        {
            return View(device);
        }
        public IActionResult Create(DeviceModel device)
        {
            if (device != null && !string.IsNullOrEmpty(device.RegistrationId))
            {
                try
                {
                    var body = new DPSRequest();
                    body.RegistrationID = device.RegistrationId;
                    if(!string.IsNullOrEmpty(device.Tags))  body.Tags.Add("tags", device.Tags);
                    if(!string.IsNullOrEmpty(device.AllowedDevices))  body.DesiredProperties.Add("allowed", device.AllowedDevices);
                    var dps = new DPSHelper(_config.GetSection("API_URL").Value);
                    var result = dps.CreateDeviceRegistration(body).GetAwaiter().GetResult();
                    dynamic o = JsonConvert.DeserializeObject(result);
                    device.PrimaryKey = (string)o.attestation.symmetricKey.primaryKey;
                    device.SecondaryKey = (string)o.attestation.symmetricKey.secondaryKey;
                    WriteDataStorage(device);
                }
                catch(Exception exp)
                {
                    return View(new ErrorViewModel { ErrorMessage = exp.Message });
                }
            }
            return View(device);
        }
        public IActionResult Delete(DeviceModel device)
        {
            if (device != null && !string.IsNullOrEmpty(device.RegistrationId))
            {
                try
                {
                    var body = new DPSRequest();
                    body.RegistrationID = device.RegistrationId;
                    var dps = new DPSHelper(_config.GetSection("API_URL").Value);
                    var result = dps.DeleteDeviceRegistration(body).GetAwaiter().GetResult();
                    DeleteFromDataStorage(device);
                }
                catch(Exception exp)
                {
                    return View(new ErrorViewModel { ErrorMessage = exp.Message });
       
                }
            }
            return View(device);
        }
        public IActionResult List()
        {
            string text = ReadDataStorage();
            return View(new DeviceListModel(text));
        }
    }
}