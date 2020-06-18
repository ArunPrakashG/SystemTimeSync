# SystemTimeSync
SystemTimeSync is a windows service which starts up with windows and run few functions which forcefully syncs with the windows time service (time.windows.com)
Last windows update introduced a bug which desyncs windows time. This should hopefully act as a workaround for that until Microsoft issues a patch.

## Installation

* Download the zip file from here [Release](https://github.com/ArunPrakashG/SystemTimeSync/releases/download/1.0.0.0/SystemTimeSync.zip)
* Extract the zip file to an HDD. (just that you shouldn't delete the contents of the zip file, if u do so, it wont work)
* right click on installer.bat file and press on Run as Adminstrator.
* Wait for few moments for the script to complete its execution.
* Once its done, u can see a confirmation message stating if the service is successfully installed or not. If it fails, you might have a corrupt download or the directory isn't correct.
* Thats it!

## Uninstallation

* Just run the uninstall.bat as adminstrator!

## Troubleshooting

* If you get System.IO.FileLoadException which states that the program failed to load essential files during startup and the whole process fails, then it is due to windows blocking the download file. the `SystemTimeSync.exe` file to precise.
Simply, right click on the file, go to properties, tick the unblock option and press apply. it should fix it.
