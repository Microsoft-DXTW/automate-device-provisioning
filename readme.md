## Device Provisioning Flow Overview

In this demo, there are following participants involved in device provisioning process. We will also be adding notification mechanism to this solution, details please refer to [this section](#notification-mechanism)

- CRM user
  
  Who is in charge of managing devices from asset perspective. When a device is delivered to the solution provider by manufacturer, he inputs device information to the solution provider's backend system. And record to which customer this device is sold to.

- End user

  When end user receives a device, a key information will be sent to the user from solution provider. The end user needs to register his device to the cloud.

In order to better architect this solution, we devided this solution into several parts

- Azure Device Provisioning Service

  Azure DPS manages device registration records, authenticate device with attestation mechanism of choice, assign a device to proper IoT Hub based on allocation policy configured in the cloud.

  Device Provisioning Service provides different provisioning mechanism, for simplicity, we are using Individual enrollment with symmetric key attestation.
   
- Azure IoT Hub

  Azure IoT Hub is the front line service which has direct connection to devices. A device will be sending telemetry to its associated IoT Hub.
  
  IoT Hub can also issue cloud-to-device commands, or request devices to update themselves by updating Desired Properties.

- DPS API Middleware

  Instead of having backend solution talk to DPS directly, an DPS API middle tier is created to decouple DPS and backend cunsumers. The API layer should maintain a list of device registration records as well as mapping between device and IoT Hub, so that its backend consumer can query through each device when required.

- Device

  A Device portal is installed in the device, when user receives provision key from solution provider. He needs to connect to this portal and input provided information to provision his device.

  Note:

  >In this sample, we seperate device portal and device telemetry sender programs since generally speaking, provisioning and sending telemetry are different process.

- Logic App

  Finally, a Logic App is created to orchestrate device provisioning process and email notification to end users.

- Full Device provisioning process is illustrated below.

<img src="docs/img/provisioning-flow.jpg"  />

##  Deploy

Follow this [instruction](docs/deploy.md) to deploy resourcecs.

## How to run this application

Follow this [instruction](docs/run.md) to run this application.

## Demo Script

- Launch Mock CRM and navigate to Mock CRM
```bash
dotnet run
```

- Create a new device via IoT Device Management page

Note
>You should receives an email once created device.

<img src="docs/img/mock-crm-new-device.jpg" style="width:300px;height:400px"/>

- Copy Registration Id, Key 1 and Key 2 from your email

- Launch Device Portal, go to IoT Device Management page. Input required information and click "Create"

```bash
dotnet run
```

- Launch Device

```bash
dotnet run

##  You should see Device Id matches to newly created device
# Connecting to IoT Hub with ConnectionString:HostName=michi-dps-20190809.azure-devices.net;DeviceId=michi-20190809-006;SharedAccessKey=xxxxxxxxx;X509Cert=False

```


## Notification Mechanism

[Notification Setup](notification/readme.md)