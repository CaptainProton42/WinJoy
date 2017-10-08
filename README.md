# WinJoy
A small XInput wrapper for JoyCons written in C# using [temach's](https://github.com/temach/HIDInterface) version of HIDInterface as a wrapper for HIDAPI.
ScpDrviver is used as the backend.

This is a very early release I did as a project in my free time in order to learn and improve my programming.
There will be bugs, there will be missing features.

Currently known issues:
* Application won't always shut down correctly (make sure you kill the task)
* There can be input lag.
* Support for custom control bindings is not yet implemented.
* Crashs might occur.

I was able to reuse some of the code from my fork of [XOutput](https://github.com/CaptainProton42/XOutput). Also huge thanks to all the contributors over at the [Switch Reverse Engineering](https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering) repo.

# Setup
1. If you're on Windows 7, install the [XBox 360 Controller Driver](http://www.microsoft.com/hardware/en-us/d/xbox-360-controller-for-windows) first.
2. Run `ScpDriver.exe` from the folder `ScpDriver`.
3. Start `WinJoy.exe`.
