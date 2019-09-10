// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public class RandomTelemetry {
        private IDataFormatter _formatter = null;
        private readonly string _dataFolder = @"./data";
        
        private readonly string [] _files = new string [] {"random_telemetry.json"};

        private static int _index = 0;

        public RandomTelemetry(){}
        public RandomTelemetry(IDataFormatter formatter){
            _formatter = formatter;
        }
        public string GenerateTelemetry(){
            var telemetry = File.ReadAllText(Path.Combine(_dataFolder, _files[_index % _files.Length]));
            _index ++;

            if(_formatter != null){
                telemetry = _formatter.Format(telemetry);
            }
            return telemetry;
        }
        public string [] GenerateTelemetryLines(){
            var telemetry = File.ReadAllLines(Path.Combine(_dataFolder, _files[_index % _files.Length]));
            _index ++;

            if(_formatter != null){
                var telemetries = from line in telemetry
                                    select _formatter.Format(line);
                telemetry = telemetries.ToArray();
            }

            return telemetry;
        }
    }
}