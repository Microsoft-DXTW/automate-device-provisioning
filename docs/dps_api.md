## Overview

DPS API provides a REST interface for backend applications to interact with Device Provisioning Service so that backend application does not directly talk to DPS. This allows us to implement business logic in this layer as well as decouple IoT service and backend services.

The API is to deployed to Azure Functions App with Http Trigger.

#### [Source Codes](./src/dps_svc_api/DPS_API/DPS_CRM_API/run.csx)

#### Deploy to Azure

-   You must first create a storage account in your subscription which has same location with your to be deployed Function App.

```bash
## Create Function App if not already
az functionapp create -n azp-msiotacc-func-dpsapi-pl -g azp-rg-msiotacc-pl --runtime dotnet -s azpmsiotaccstorazfuncpl --consumption-plan-location australiacentral

## Publish Function App to Azure
func azure functionapp publish azp-msiotacc-func-dpsapi-pl --csx --publish-local-settings -i 
```

#### Configure Function App

-   The API requires below configuration settings

    -   DPS_CONNECTIONSTRING: Connection String to Device Provision Service