using System;
/*
 * 
 *      Released as open source by NCC Group Plc - http://www.nccgroup.com/
 *
 *      Developed by Chris Thomas, chris dot thomas at nccgroup dot com
 *
 *      https://github.com/nccgroup/WhoWasI
 *
 *      Released under AGPL see LICENSE for more information
 */

namespace WhoWasI
{
    class Program
    {
        private const String VERSION_NUMBER = "1.1";

        static void Main(String[] args)
        {
            PrintBanner();

            if (args.Length == 0)
            {
                PrintUseage();              
                return;                
            }

            if (WhoWasI.IsAdmin == false)
            {
                Console.WriteLine("\n## ERROR ## - You Are Not An Admin.. You need Admin Privs!");
                PrintUseage();
                return;
            }



            //we already have the last two arguments
            for (Int32 i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower().Trim())
                {
                    case "-la": WhoWasI.PrintActiveAccountsToConsole();                       
                                return;                        
                    case "-pp":
                                Int32 _PID;

                                //try and parse the next argument, this should be a process ID and so numerical
                                if (!Int32.TryParse(args[i + 1], out _PID))
                                {
                                    Console.WriteLine("## ERROR ## - Invalid Process ID '{0}' Specified, Unable To Dump Process Privilages...\n");
                                    PrintUseage();
                                    return;
                                }//end of  if (!UInt32.TryParse(args[i + 1], out _PID))

                                WhoWasI.PrintProcessPrivsToConsole(_PID);
                                Environment.Exit(0);

                        return;
                }//end of switch
            }//end of  for (Int32 i = 0; i < args.Length -2; i++)


            if (args.Length < 2)
            {
                Console.WriteLine("## ERROR ## - Not Enough Command Line Args, Aborting..n");
                return;                
            }

            //We want the penultimate paramiter as the account
            String _InputtedAccount = args[args.Length - 2];
            
            
            Int32 _InputtedProcessID;
            Boolean _UseProcessIDAsAccountHandle = false;

            //the following checks are used to determine whether a string (account) or number (processID) has been specified
            if (Int32.TryParse(_InputtedAccount, out _InputtedProcessID)) { _UseProcessIDAsAccountHandle = true; }


            //Last paramiter is the command to run
            String _CommandToRun = args[args.Length - 1];

            if (String.IsNullOrEmpty(_InputtedAccount))
            {
                Console.WriteLine("## ERROR ## - Account Name Is NULL/Empty, Aborting..n");
                PrintUseage();
                return;
            }//end of if (String.IsNullOrEmpty(_InputtedAccount))

            if (String.IsNullOrEmpty(_CommandToRun))
            {
                Console.WriteLine("## ERROR ## - Command Is NULL/Empty, Aborting..\n");
                PrintUseage();
                return;
            }//end of if (String.IsNullOrEmpty(_CommandToRun))

            ProcessEntry _ProcessIdToUse;


            if (_UseProcessIDAsAccountHandle)
            {
                _ProcessIdToUse = new ProcessEntry()
                {
                    Name = "ASDF",
                    PID = _InputtedProcessID
                };
            }
            else
            {
                _ProcessIdToUse = WhoWasI.GetProcessIDForAccount(_InputtedAccount);
            }
            
            //do we have a valid process?
            if (_ProcessIdToUse.PID == -1)
            {
                Console.WriteLine("## ERROR ## - No Processes Found For Account '{0}', Aborting..\n", _InputtedAccount);
                PrintUseage();
                return;
            }//end of if (_ProcessIdToUse == -1)

            Console.WriteLine("[+] Using Process '[{0}] - {1}' With Owner '{2}'..", _ProcessIdToUse.Name, _ProcessIdToUse.PID, _InputtedAccount);

            WhoWasI.ImpersonateAndExecute(_ProcessIdToUse.PID, _CommandToRun);
        }



        private static void PrintBanner()
        {
            Console.WriteLine("WhoWasI v{0} - Written by Chris Thomas (chris.thomas@nccgroup.com)", VERSION_NUMBER);
            Console.WriteLine("==================================================================");
        }


        private static void PrintUseage()
        {
            Console.WriteLine("Useage: WhoWasI.exe [Options] {Run As User/PID} {Command}");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-la\t\t-\tList Available Users");
            Console.WriteLine("\t-pp <pid>\t-\tList Process Privilages (By PID)");
            Console.WriteLine("\nExample:");
            Console.WriteLine("\tWhoWasI.exe system cmd.exe\t (Run With SYSTEM Permissions)");
            Console.WriteLine("\tWhoWasI.exe 1234 cmd.exe\t (Run With PID 1234 Permissions)");
        }
    }
}
