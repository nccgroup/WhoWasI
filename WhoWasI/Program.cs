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
        private const String VERSION_NUMBER = "1.0";

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
                }//end of switch
            }//end of  for (Int32 i = 0; i < args.Length -2; i++)

            if (args.Length < 2)
            {
                Console.WriteLine("## ERROR ## - Not Enough Command Line Args, Aborting..n");
                return;                
            }

            //We want the penultimate paramiter as the account
            String _InputtedAccount = args[args.Length - 2];
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


            ProcessEntry _ProcessIdToUse = WhoWasI.GetProcessIDForAccount(_InputtedAccount);
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
            Console.WriteLine("Useage: WhoAmI.exe [Options] {Run As User} {Command}");
            Console.WriteLine("Options:");
            Console.WriteLine("\t-la\t-\tList Available Users\n");
            Console.WriteLine("Example:");
            Console.WriteLine("\tWhoAmI.exe system cmd.exe");
        }
    }
}
