Raspberrypi:

Flash sdcard with raspberrypi OS (important: use image WITH Desktop)

Install dotnet core runtime

Copy binaries of SIKOSI.Sample05_IOT-Project to Pi: possibly over smb => https://www.elektronik-kompendium.de/sites/raspberry-pi/2007071.htm

Start webserver.
Set endpoint in appsettings.json

Start backgroundworker with "./SIKOSI.Sample05_IOT" (If required set Permissions with chmod +x SIKOSI.Sample05_IOT)

For GUI:
Install mono => sudo apt install mono-devel
Copy binaries of SIKOSI.Sample05_GUI-Project to pi
Set Display envvariable: export DISPLAY=:0
Start gui with "mono SIKOSI.Sample05_GUI.exe"