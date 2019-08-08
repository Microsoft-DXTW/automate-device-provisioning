using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using web.Models;

namespace web.Helpers
{
    public class FileStorage
    {
        private static string _connFile = @"..\..\..\conn.txt";
        private static string _file = @"..\web\app_data\device_provision_list.json";
        public static string ReadConnectionString()
        {
            return File.ReadAllText(_connFile);
        }
        public static void WriteConnectionString(string connString)
        {
            if (File.Exists(_connFile))
            {
                File.Delete(_connFile);
            }
            using (var sw = File.CreateText(_connFile))
            {
                sw.Write(connString);
            }
        }
        public static string ReadProvisioningRecods()
        {
            return File.ReadAllText(_file);
        }
        public static void WriteProvisioningRecords(DeviceModel device)
        {
            if (File.Exists(_file))
            {
                File.Delete(_file);
            }
            using (var sw = File.CreateText(_file))
            {
                sw.Write(JsonConvert.SerializeObject(new DeviceModel[] { device }));
            }
        }
    }
}
