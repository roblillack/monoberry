

# MONOBERRY #


_Mono for the BlackBerry 10 platform._

##### Summary #####

* Website: http://burningsoda.com/software/monoberry/
* GitHub: http://github.com/roblillack/monoberry
* License: MIT X11 license

##### About #####

MonoBerry is Mono for the BlackBerry PlayBook OS and the future BlackBerry 10
platform. The project consists of three parts:

* **Runtime:** The actual port of the Mono .NET runtime to the QNX based
               platform for the ARM & x86 architectures (devices/simulators).
* **Libraries:**
  * libblackberry—a C# library to access native functions (screen, camera,
    sensors, …) of BlackBerry devices.
  * Ports of existing libraries (OpenTK, MonoGame, …) to make them work with
    MonoBerry and the BlackBerry devices—based on libblackberry.
  * _Future:_ Cascades#, a BlackBerry Cascades (Qt-based GUI toolkit) wrapper
    for .NET. **(BB10 platform only!)**
* **Tooling:** Functionality to package a .NET assembly to a BlackBerry Archive
               (`.bar` file) and integration with tools of the BlackBerry NDK.

##### Building from source #####

###### Mac OS X ######

* Install XCode and the commandline tools
* Install the BlackBerry 10 Native SDK
* Install Mono
* Run `make && sudo make install`

###### Linux ######

These notes are specific to Ubuntu 12.04 but should easily be
adaptable to other distributions.

* Install the BlackBerry 10 Native SDK
* Install some build requirements, including Mono:
  `sudo apt-get install build-essential mono-complete autoconf libtool gettext openjdk-7-jre`
* Run `make && sudo make install`

##### References #####

MonoBerry is standing on the shoulders of the following giants:

* [Mono](http://mono-project.com/): Open Source CLR implementation and SDK
* [OpenTK](http://www.opentk.com/): C# OpenGL, OpenCL, and OpenAL wrapper
* [Nini](http://nini.sourceforge.net/): .NET Configuration Library
* to be continued …

##### License #####

Copyright &copy; 2012&ndash;2013 [Robert Lillack](http://roblillack.net/).

Licensed under the terms of the MIT X11 license. Please see
[LICENSE](https://github.com/roblillack/monoberry/blob/master/LICENSE)
for more information.
