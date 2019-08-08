## [Download and Install DotNet Core 2.2](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-2.2.101-linux-arm32-binaries)

-   [Publish dotnet program]

Run dotnet publish to pack program with corresponding [runtime Id](https://docs.microsoft.com/zh-tw/dotnet/core/rid-catalog).

```bash
dotnet publish -o C:\nsw\client --runtime linux-arm
```

-   [Download dotnet core SDK](https://dotnet.microsoft.com/download/dotnet-core/2.2#sdk-2.2.401)

-   Run below commands to isntall dotnet SDK on Raspberry Pi

```bash
mkdir -p $HOME/dotnet && tar zxf dotnet-sdk-2.2.101-linux-arm.tar.gz -C $HOME/dotnet
export DOTNET_ROOT=$HOME/dotnet
export PATH=$PATH:$HOME/dotnet
```

-   Update environment variables permanently 

    -   Bash as login shell will load /etc/profile, ~/.bash_profile, ~/.bash_login, ~/.profile in the order
    -   Bash as non-login interactive shell will load ~/.bashrc
    -   Bash as non-login non-interactive shell will load the configuration specified in environment variable $BASH_ENV

## Certificates Sample

```bash
```shell
# Install IoT Extension
az extension add --name azure-cli-iot-ext

# Create Resource Group
az group create --name michi-dps-20190726-rg --location eastasia

# Create IoT Hub
az iot hub create --name michi-dps-eastasia-001 --resource-group michi-dps-20190726-rg --location eastasia

# Create DPS
az iot dps create --name michi-dps-20190726 --resource-group michi-dps-20190726-rg --location eastasia

# Get Connection String
FOR /F "tokens=*" %a in ('az iot hub show-connection-string --name michi-iothub-dps-sea-001 --key primary --query connectionString -o tsv') do SET connString=%a
echo %connString%

# Associate IoT Hub with DPS
az iot dps linked-hub create --dps-name michi-dps-20190726 --resource-group michi-dps-20190726-rg --connection-string %connString% --location eastasia

# Verify
az iot dps show --name michi-dps-20190726
```

#### Configure Enrollment Group

- Device Provisioning Service use Enrollment Group to manage devices which has same configuration. These devices, when boot up, register themselves to DPS with their certificate singed by a root CA. DPS determine if that certificate is valid and assign connected device to corresponding IoT Hub.

- Execute below commands to setup Enrollment Group

```bash
# Clone IoT C SDK
git clone https://github.com/Azure/azure-iot-sdk-c.git

cd tools/CACertificates
chmod 700 certGen.sh
chmod 700 certGenEx.sh

# Create Root and Intermediate CA
./certGen.sh create_root_and_intermediate

## Chain
# Concat root and intermediate certificate
cat certs/azure-iot-test-only.intermediate.cert.pem certs/azure-iot-test-only.root.ca.cert.pem > certs/ca-full-chain.cert.pem

# Upload ca-full-chain.cert.pem to Azure DPS
## >> [Below command gave me an error, so I manually upload ca-full-chain.cert.pem to Portal]
## >> az iot dps certificate create --dps-name michi-dps-20190726 --resource-group michi-dps-20190726-rg --name dps20190726chain --path ./certs/ca-full-chain.cert.pem

# Manually upload ca-ful-chain.cert.pem to DPS and name it dps20190726chain

# Get Latest ETAG
az iot dps certificate show -n dps20190726chain --dps-name michi-dps-20190726 -g michi-dps-20190726-rg

# Generate Validation Code for Chain CA (2EF194C10D480915EBB4DAD7B3A18CCDEECF793D1FFEAFB6)
az iot dps certificate generate-verification-code -g michi-dps-20190726-rg --dps-name michi-dps-20190726 -n dps20190726chain -e <ETAG>

# Create verification certificate
./certGenEx.sh  create_intermediate_verification_certificate <VERIFICATION_CODE>

# Get Latest ETAG (AAAAAAD8c2k=)
az iot dps certificate show -n dps20190726chain --dps-name michi-dps-20190726 -g michi-dps-20190726-rg

# Verify Chain CA
az iot dps certificate verify --dps-name michi-dps-20190726 -g michi-dps-rg --name dps20190726chain --path ./certs/verification-code.cert.pem -e <ETAG>

## Create Enrollment Group with GeoLatency allocation policy
az iot dps enrollment-group create --dps-name michi-dps-20190726 -g michi-dps-20190726-rg --enrollment-id dpseastasia --root-ca-name dps20190726chain --ap geolatency
```

- Now that we have our Root Certificate and Intermediate Certificate ready. We use them to generate device certificates

```bash
# Create Device CA
rm ./certs/new-device.cert.pem
./certGen.sh create_device_certificate deviceid001

# Retrieve required information
az iot dps show -n michi-dps-20190726 -g michi-dps-20190726-rg

# A Json document should be returned, note down idScope value and deviceProvisioningHostName value
{
  "etag": "xxxxxxxxxxxxxx",
  "id": "/subscriptions/xxxxxxxxxxxxx/resourceGroups/michi-dps-20190726-rg/providers/Microsoft.Devices/provisioningServices/michi-dps-20190726",
  "location": "eastasia",
  "name": "michi-dps-20190726",
  "properties": {
    "allocationPolicy": "Hashed",
    "authorizationPolicies": null,
    "deviceProvisioningHostName": "global.azure-devices-provisioning.net",
    "idScope": "0neXXXXXXXX",
    "iotHubs": [
      {
        "allocationWeight": null,
        "applyAllocationPolicy": null,
        "connectionString": "HostName=michi-dps-eastasia-001.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=xxxxxxxxxxxxxxxxxxxxxxx",
        "location": "eastasia",
        "name": "michi-dps-eastasia-001.azure-devices.net"
      }
    ],
    "provisioningState": null,
    "serviceOperationsHostName": "michi-dps-20190726.azure-devices-provisioning.net",
    "state": "Active"
  },
  "resourcegroup": "michi-dps-20190726-rg",
  "sku": {
    "capacity": 1,
    "name": "S1",
    "tier": "Standard"
  },
  "subscriptionid": "xxxxxxxxxxxxxxxx",
  "tags": {},
  "type": "Microsoft.Devices/provisioningServices"
```

-  Provide Device certificates, its password (if any), ID_Scope and deviceProvisioningHostName retrieved from above to hardware manufacturer

## Hardware manufacturer Steps

As a hardware manufacturer, we need to safely stored device certificates to our device hardware for connectivity. This certificate also need to be updated periodically to ensure security.

In this tutorial, we will be using a simulator to simulate the device.

#### Codes

- Get [this sample code](https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/master/provisioning/Samples/device/X509Sample)

- Update Program.cs, replace default ID_Scope, passowrd, certificate file name and deviceProvisioningHostName value

```csharp
private static string s_idScope = "0ne1234567";
//...
private static string s_certificateFileName = "new-device.cert.pfx";
//...
private static X509Certificate2 LoadProvisioningCertificate()
        {
            string certificatePassword = "1234"
//...
```

- Compile and run

```cshart
dotnet restore
dotnet build
dotnet run
```

- Device should provisioned successfully

```bash
Found certificate: AA7AE75A2BAD769079F021D388F58A9A40EA3C74 CN=deviceid001; PrivateKey: True
Using certificate AA7AE75A2BAD769079F021D388F58A9A40EA3C74 CN=deviceid001
RegistrationID = deviceid001
ProvisioningClient RegisterAsync . . . Assigned
ProvisioningClient AssignedHub: michi-dps-eastasia-001.azure-devices.net; DeviceID: deviceid001
Creating X509 DeviceClient authentication.
DeviceClient OpenAsync.
DeviceClient SendEventAsync.
DeviceClient CloseAsync.
```

## Custom Assign Policy

You may also want to have your own logic to assign device to IoT Hub. DPS allows you to write an Azure Function to assign device by custom logic.

A sample function can be found [here](./src/custom-allocation/readme.md)
