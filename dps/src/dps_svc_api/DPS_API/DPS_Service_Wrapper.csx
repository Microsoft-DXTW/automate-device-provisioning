using System;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Provisioning.Service;
using Microsoft.Azure.Devices.Shared;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace DPS_API_V2{
public class DPSAPIWrapper{
    private ProvisioningServiceClient _provisioning = null;
    private ILogger _logger = null;
    public DPSAPIWrapper(string connString, ILogger log){
        _provisioning = ProvisioningServiceClient.CreateFromConnectionString(connString);
        _logger = log;
    }

    public async Task DeleteIndividualEnrollmentAsync(string registrationId){
        try
        {
            await _provisioning.DeleteIndividualEnrollmentAsync(registrationId);
        }
        catch(Exception exp){
            Console.WriteLine($"Exception::{exp.Message}");
            if(exp.Message.IndexOf("Not Found") < 0){
                throw exp;
            }else{
                return ;
            }
        }
    }

    public async Task<IndividualEnrollment> GetExistingIndividualEnrollmentAsync(string registrationId){
        try{
            var existing = await _provisioning.GetIndividualEnrollmentAsync(registrationId);
            Console.WriteLine($"Existing:{JsonConvert.SerializeObject(existing)}");

            return existing;
        }catch{
            return null;
        }
    }
    
    public async Task<IndividualEnrollment> CreateSymmetricKeyInidividualEnrollmentAsync(string registrationId, object desiredProperties, object tags){
        var existing = await GetExistingIndividualEnrollmentAsync(registrationId);
        if(existing == null){
            string pk = "", sk = "";
            Attestation attestation = new SymmetricKeyAttestation(pk, sk);
            IndividualEnrollment individualEnrollment =
                new IndividualEnrollment(
                    registrationId,
                    attestation);
            individualEnrollment.ProvisioningStatus = ProvisioningStatus.Enabled;
            individualEnrollment.DeviceId = registrationId;
            individualEnrollment.AllocationPolicy = AllocationPolicy.GeoLatency;
            individualEnrollment.InitialTwinState = new TwinState(
                desiredProperties: desiredProperties != null ? new TwinCollection(JsonConvert.SerializeObject(desiredProperties)): null ,
                tags: tags != null ? new TwinCollection(JsonConvert.SerializeObject(tags)): null
            );
            IndividualEnrollment individualEnrollmentResult = await _provisioning.CreateOrUpdateIndividualEnrollmentAsync(
                individualEnrollment
            );
            
            return individualEnrollmentResult;
        }else{
            return existing;
        }
    }

    public async Task<IndividualEnrollment> UpdateSymmetricKeyInidividualEnrollmentAsync(string registrationId, object desiredProperties, object tags){
        var existing = await GetExistingIndividualEnrollmentAsync(registrationId);

        if(existing != null){
            existing.InitialTwinState = existing.InitialTwinState?? new TwinState(new TwinCollection(JsonConvert.SerializeObject(new{})), new TwinCollection(JsonConvert.SerializeObject(new{})) );
            existing.InitialTwinState.DesiredProperties = existing.InitialTwinState.DesiredProperties?? new TwinCollection(JsonConvert.SerializeObject(new{}));
            existing.InitialTwinState.Tags = existing.InitialTwinState.Tags?? new TwinCollection(JsonConvert.SerializeObject(new{}));
            if(desiredProperties != null){
                foreach(var o in ((JObject)desiredProperties)){
                    existing.InitialTwinState.DesiredProperties[o.Key] = o.Value;
                }
            }
            if(tags != null){
                foreach(var o in ((JObject)tags)){
                    existing.InitialTwinState.Tags[o.Key] = o.Value;
                }
            }
            
            IndividualEnrollment individualEnrollmentResult = await _provisioning.CreateOrUpdateIndividualEnrollmentAsync(
                existing
            );
            return existing;
        }else{
            throw new ArgumentNullException($"Registration Id [{registrationId}] not found");
        }
    }
}
}