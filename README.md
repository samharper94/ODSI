# ODSI

 Operational Decision Support Interface for Unity. Made in C#.

# Requirements
Software and Hardware:
- Unity 2021.3.2f1

Optional:
- Xbox One/Series S|X controller
- Ethernet connection or secondary WiFi connection to local network to allow for external connectivity with Tello
- Webcam(s)
- Clearpath Husky
- Boston Dynamics Spot
- Clearpath Jackal
- DJI Tello
- SparkFun RFID Kit
- Onboard WiFi for Tello connection

Builds for:
- Windows 10 x86_64 and Mac Mojave (Intel)

# How to Run
- Open the Unity project in 2020.3.11f1 (no earlier version supported)
- Connect SparkFun RFID reader to a USB port. Check the COM port in the Windows Device Manager or in Terminal (Mac). If it's not /dev/tty.usbserial-A10KLPXO, the Port Name public variable on the SerialController GameObject will need to be changed to the correct COM port.
- Configure RFID access, to do this, open a serial reader (Arduino IDE for example), scan an RFID card and copy the output. Paste this output into one of the if statements in RFIDRead.cs, found in the Scripts GameObject.
- Optional: Connect Xbox controller to the PC through either USB or Bluetooth
- Optional: Connect webcam(s) or use built in webcam
- Change WiFi network to the Tello
- Run app

# How to Build
AT THIS TIME, BUILD ONLY SUPPORTED FOR WINDOWS 10, WINDOWS X86_64 STANDALONE APP OR MAC MOJAVE IN INTEL, NOT UWP OR ANDROID

REMOVING ALL REFERENCE TO RFID SHOULD ENABLE BUILD ON ANDROID

This WILL run under Rosetta on Mac.

Necessary steps:
- Ensure API Compatiability Level is on .NET 4.x in Project Settings > Player > Other Settings. This is required for the Serial connection for the RFID.
- Ensure Scripting Backend is Mono. This is required for the Tello video output. NOTE - UWP does not support Mono builds. This means you will need your own method of obtaining the video output from the Tello. Failing that, removing all reference to the Tello video and RFID should enable UWP builds. Similarly for native M1 Mac.
You should now be able to build successfully

# External Libraries used
- Tello for Unity, https://github.com/comoc/TelloForUnity
- Ardity, https://ardity.dwilches.com/
