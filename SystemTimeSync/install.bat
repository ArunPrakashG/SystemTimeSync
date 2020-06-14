@echo off
set installerFilePath=C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe
set appFilePath=%~d0%~p0\SystemTimeSync.exe
set serviceName=SystemTimeSync

if not exist %installerFilePath% (
    echo %installerFilePath% file doesn't exist.
    echo Check if you have .NET framework 4.7.2 installed on your system.
    pause
)

if not exist %appFilePath% (
    echo %appFilePath% file doesnt exist.
    echo Server core file doesn't exist.
    echo Check if you downloaded the correct package.
    echo If you are not running this script from the download path, then it wont work.
    pause
)

%installerFilePath% -i %appFilePath%

echo Service installed, Starting service...
sc start %serviceName% 
echo Service started, setting service state to automatic...
sc config %serviceName% start= auto
echo Service state configured.

if ERRORLEVEL 1 goto error
echo Script finished.
echo Windows time will be synced automatically on every startup.
pause 
exit
:error
echo Failed to install service.
pause