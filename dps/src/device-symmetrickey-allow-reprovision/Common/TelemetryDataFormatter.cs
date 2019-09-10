// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Microsoft.Azure.Devices.Provisioning.Client.Samples
{
    public interface IDataFormatter
    {
        string Format(string msg);
    }

    public class TelemetryDataFormatter:IDataFormatter
    {
        public string Format(string msg){
            return msg.Replace("$UTC_TIME$", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        }
    }
}