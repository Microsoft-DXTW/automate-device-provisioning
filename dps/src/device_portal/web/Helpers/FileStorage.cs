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
        private static string _deviceProvisionInfoFile = @"..\..\..\secure_conn.json";
        public static string ReadConnectionString()
        {
            return File.ReadAllText(_connFile);
        }
        public static void WriteDeviceProvisioningInfo(string info){
            EnsureWriteFile(_deviceProvisionInfoFile, info);
        }
        private static void EnsureWriteFile(string file, string content){
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            using (var sw = File.CreateText(file))
            {
                sw.Write(content);
            }
        }
        public static void WriteConnectionString(string connString)
        {
            EnsureWriteFile(_connFile, connString);
        }
        public static string ReadProvisioningRecods()
        {
            return File.ReadAllText(_file);
        }
        public static void WriteProvisioningRecords(DeviceModel device)
        {
            EnsureWriteFile(_file, JsonConvert.SerializeObject(new DeviceModel[]{device}));
            
        }
    }
}
