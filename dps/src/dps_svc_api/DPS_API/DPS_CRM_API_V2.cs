using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DPS_API_V2
{

    public static class DeviceProvisioningAPI
    {

        public static bool IsPropertyExist(Newtonsoft.Json.Linq.JObject settings, string name)
        {
            return settings[name] != null;
        }

        ///=========================================================================
        /// POST:
        ///     Request Body:
        ///         {
        ///             registrationId:"id",
        ///             desiredProperties:{
        ///                 property1:value1
        ///             },
        ///             tags:{
        ///                 tag1:value1
        ///             }
        ///         }
        ///=========================================================================
        [FunctionName("DeviceProvisioningAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "patch", "delete", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request:{req.Method}");

            string requestBody = null;
            string DPS_CONNECTIONSTRING = Environment.GetEnvironmentVariable("DPS_CONNECTIONSTRING");
            var wrapper = new DPSAPIWrapper(DPS_CONNECTIONSTRING, log);
            string response = string.Empty;
            dynamic data;
            object result = null, desiredProperties = null, tags = null;

            switch (req.Method)
            {
                case "DELETE":
                    requestBody = new StreamReader(req.Body).ReadToEndAsync().GetAwaiter().GetResult();
                    data = JsonConvert.DeserializeObject(requestBody);
                    log.LogInformation($"Deleting...{requestBody}");
                    await wrapper.DeleteIndividualEnrollmentAsync((string)data.registrationId);
                    result = data;
                    break;
                case "GET":
                    //  Get Existing individual enrollment by registration id
                    requestBody = req.Query["registrationId"];
                    result = await wrapper.GetExistingIndividualEnrollmentAsync(requestBody);
                    break;
                case "POST":
                    //  Create a new individual enrollment
                    requestBody = new StreamReader(req.Body).ReadToEndAsync().GetAwaiter().GetResult();
                    data = JsonConvert.DeserializeObject(requestBody);
                    desiredProperties = (object)data.desiredProperties;
                    tags = (object)data.tags;
                    result = await wrapper.CreateSymmetricKeyInidividualEnrollmentAsync((string)data.registrationId, desiredProperties, tags);
                    log.LogInformation($"CreateSymmetricKeyInidividualEnrollment::\r\n{JsonConvert.SerializeObject(result)}");
                    break;
                case "PATCH":
                    requestBody = new StreamReader(req.Body).ReadToEndAsync().GetAwaiter().GetResult();
                    data = JsonConvert.DeserializeObject(requestBody);
                    if (IsPropertyExist(data, "desiredProperties"))
                    {
                        desiredProperties = (object)data.desiredProperties;
                    }
                    if (IsPropertyExist(data, "tags"))
                    {
                        tags = (object)data.tags;
                    }
                    result = await wrapper.UpdateSymmetricKeyInidividualEnrollmentAsync((string)data.registrationId, desiredProperties, tags);
                    break;
            }

            return result != null
                ? (ActionResult)new OkObjectResult(result)
                : new BadRequestObjectResult(new { message = "No result found" });
        }
    }
}
