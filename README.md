# SolarTools #

Tools for working with Enphase solar arrays and Shelly switches.

### Projects ###

1. **SolarPOC** Proof of concept for working with Enphase Envoy gateways. A CLI tool to reporting on the gateway.
1. **ShellyPOC** Proof of concept for working with Shelly switches. A CLI tool to reporting on a list of switches.
1. **SolarLiveStatusPanel** A status panel showing current generation information and the ability to switch a shelly switch on and off, this could be used to control the hot water tank.
1. **Support** Support files including prebuild versions of **SolarLiveStatusPanel**

![Screen](support/screenshots/1.png)

### Running  SolarLiveStatusPanel ###

There is a prebuilt version of the status panel app in the Support folder. Its intended to be used on a 64 bit version of Windows.

After copying the EXE, edit the config file, a specimen file is provided.

```
{
  "shelly": {
    "devices": [
      {
        "address": "192.168.0.999",
        "type":  "1"
      }
    ]
  },
  "enphase": {
    "credentials" : {
      "user": "configure your user",
      "password": "configure your password"
    },
    "gateway": {
      "address": "192.168.0.999"
    }
  }
}
```

You need to provide IP addresses for the Envoy gateway and the Shelly switch. Also cedentials for Enphase are needed to generate the token to access the gateway. The token lasts a year, once generated you can remove your credentials until the token need to be regenerated.

### Building the projects ###

The projects are all built using Visual Studio (I used VS 2022), and written in C#.

