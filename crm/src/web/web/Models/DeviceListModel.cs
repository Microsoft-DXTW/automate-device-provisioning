using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace web.Models
{
    public class DeviceListModel:IEnumerable<DeviceModel>
    {
        private readonly IConfiguration _config;
        private DeviceModel[] _devices = null;

        public DeviceListModel(IConfiguration config)
        {
            _config = config;
        }
        public DeviceListModel()
        {
        }
        public DeviceListModel(string json)
        {
            var list = JsonConvert.DeserializeObject<DeviceModel[]>(json);
            Devices = list.Where(d => d != null).ToArray();
        }
        public DeviceModel[] Devices { get => _devices; set => _devices = value; }

        public IEnumerator<DeviceModel> GetEnumerator()
        {
            return ((IEnumerable<DeviceModel>)Devices).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DeviceModel>)Devices).GetEnumerator();
        }
    }
}

