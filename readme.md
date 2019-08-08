## Device Provisioning Flow

This sample demonstrate how to automate device provisioning process.

(Can't see below diagram ? [Provision Flow](docs/img/provisioning-flow.jpg))

```mermaid
sequenceDiagram
    participant CRM administrator
    participant CRM
    participant DPS API Layer
    participant DPS
    participant IoT Hub
    participant Logic App
    participant User
    participant Device
    
    CRM administrator->>CRM: Create device enrollment records
    CRM ->> DPS API Layer: Invoke API
    DPS API Layer ->> DPS: Create Individual Enrollment record in DPS
    DPS ->> IoT Hub: Create device entry
    DPS ->> DPS API Layer: Return keys
    DPS API Layer ->> CRM: Return keys
    CRM -->> User: Send keys
    Note right of CRM: via email
    
    User ->> Device: Input keys via Device portal
    Device ->> DPS: Provision itself
    DPS ->> IoT Hub: Assign device to IoT Hub
    DPS ->> Device: Return connection info
    Device ->> Logic App: Report Provision Status
    Device ->> Device: Store connection info
    Logic App ->> CRM: Update device provision status
    Device ->> IoT Hub: Send telemetry
```

## How to run this application

#### Backend application

-   Download source codes

-   [Setup Device Provisioning Service](docs/dps.md) to create DPS and link IoT Hub to it.

-   [Setup Device Provisioning Service Middleware API](docs/dps_api.md) for backend applications to communicate with DPS.

-   Create crm/src/web/web/appsettings.json file with below content.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DPS_Scope": "<Your DPS ID_SCOPE>",
  "DPS_GlobalEndpoint": "global.azure-devices-provisioning.net",
  "API_URL": "<YOUR Device Provisioning Service Middleware API URL>"
}
```

-   To run mock CRM web portal

```bash
cd crm/src/web/web
dotnet run
```

#### Device Setup

-   If you are using Raspberry Pi, see [Setup Raspberry Pi](docs/raspberry-pi.md)

-   Create src/device_portal/web/appsettings.json with below content

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "DPS_Scope": "<DPS ID_SCOPE>",
  "DPS_GlobalEndpoint": "global.azure-devices-provisioning.net"
}
```

-   Run device poral

```bash
cd dps/src/device_portal/web
dotnet run
```

-   Update dps/src/data/random_telemetry.json

```json
{"data":"sample 1"}
{"data":"sample 2"}
{"data":"sample 3"}
```
-   Run device

```bash
cd dps/src/symmetric-key-individual-2steps
dotnet run
```