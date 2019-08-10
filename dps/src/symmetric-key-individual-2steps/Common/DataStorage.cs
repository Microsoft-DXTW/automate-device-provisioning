using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class DataStorage {
        private readonly string _dbFile = "../../conn.txt";
            
        public string Read(string key){
            switch(key){
                case "ConnectionString":
                    return System.IO.File.ReadAllText(_dbFile);
            }
            return null;
        }
    }
}