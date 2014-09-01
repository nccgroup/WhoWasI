WhoWasI
======================
This application is able to impersonate any logged in user and execute commands under the context of the impersonated user.

Released as open source by NCC Group Plc - http://www.nccgroup.com/

Developed By:
* Chris Thomas, chris [dot] Thomas [at] nccgroup [dot] com

https://github.com/nccgroup/WhoWasI

Released under AGPL see LICENSE for more information

Usage
-------------
[List Process Privileges]

WhoWasI.exe -pp <PID>


[List All Available Accounts]

WhoWasI.exe -la


[Impersonate & Execute Command as Another User]

usage: WhoWasI.exe {account} {command}

The following command will spawn a command shell as the SYSTEM user:

WhoWasI.exe system cmd.exe


Dependencies 
-------------
THE SOLUTION WILL COMPILE WITH ALL STOCK VERSIONS OF .NET, NO DEPENDANCIES ARE REQUIRED.

Note: .NET Framework Binaries (.NET 2 is present on all Windows OS’s until Windows 8 / Server 2012 where .NET 4 is present by default).

Change Log
-------------
Version 1.0 - 17th June 2014 
* Initial Release

Version 1.1 - 1st Sept 2014
* Added: Ability To Specify Process ID Instead of Account Name
* Added: The 'SeAssignPrimaryTokenPrivilege' Privilege Is Not Applied When Impersonating
* Added: Able To List A Processes Privileges
* Fixed: Wrong Application Name In Usage Menu
