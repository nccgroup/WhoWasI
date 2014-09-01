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

[List All Available Accounts]
WhoWasI.exe -la

[Impersonate & Execute Command as Another User]
usage: WhoWasI.exe {account} {command}

The following command will spawn a command shell as the SYSTEM user:

WhoWasI.exe system cmd.exe

Dependencies 
-------------
* .NET (2.0) Framework (Present On All Supported Windows Operating Systems)
