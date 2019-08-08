using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace web.Helpers
{
    public class DPSRequest
    {
        public string RegistrationID { get; set; } = string.Empty;
        public Dictionary<string, string> DesiredProperties { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Tags { get; set; } = new Dictionary<string, string>();
        public override string ToString()
        {
            var o = new
            {
                registrationId = RegistrationID,
                desiredProperties = new System.Dynamic.ExpandoObject(),
                tags = new System.Dynamic.ExpandoObject(),
            };
            foreach(var p in DesiredProperties.Keys)
            {
                o.desiredProperties.TryAdd(p,DesiredProperties[p]);
            }
            foreach (var t in Tags.Keys)
            {
                o.tags.TryAdd(t, Tags[t]);
            }
            return JsonConvert.SerializeObject(
                    o
                );
        }
    }
    public class DPSHelper
    {
        public DPSHelper(string url){
            API_URL = url;
        }
        private string API_URL = "";
        public async Task<string> DeleteDeviceRegistration(DPSRequest request)
        {
            var req = HttpWebRequest.Create(API_URL) as HttpWebRequest;
            req.Method = "DELETE";
            req.ContentType = "application/JSON";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(request.ToString());
                sw.Close();
            }
            using (var resp = req.GetResponse())
            {
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var body = await sr.ReadToEndAsync();
                    return body;
                }
            }
        }
        public async Task<string> CreateDeviceRegistration(DPSRequest request)
        {
            var req = HttpWebRequest.Create(API_URL) as HttpWebRequest;
            req.Method = "POST";
            req.ContentType = "application/JSON";
            using (var sw = new StreamWriter(req.GetRequestStream()))
            {
                sw.Write(request.ToString());
                sw.Close();
            }
            using (var resp = req.GetResponse())
            {
                using (var sr = new StreamReader(resp.GetResponseStream()))
                {
                    var body = await sr.ReadToEndAsync();
                    return body;
                }
            }
        }
    }
}
