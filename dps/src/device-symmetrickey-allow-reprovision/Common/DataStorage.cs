using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class DataStorage {
        public const string DeviceConnectionString = "ConnectionString";
        private readonly string _conn = "../../secure_conn.json";
        public string Read(string key){
            switch(key){
                case DeviceConnectionString:
                    return System.IO.File.ReadAllText(_conn);
            }
            return null;
        }

        public void Write(string key, string content){
            switch(key){
                case DeviceConnectionString:
                    if(File.Exists(_conn)){
                        File.Delete(_conn);
                    }
                    File.WriteAllText(_conn, content);
                    break;
            }
        }
    }
}