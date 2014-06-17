using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace WhoWasI
{
    public class WhoWasI
    {

        public static Boolean ImpersonateAndExecute(Int32 processID, String command)
        {
            Boolean _Errored = false;
            IntPtr _TokenHandle = IntPtr.Zero;
            IntPtr _DuplicateTokenHandle = IntPtr.Zero;

            IntPtr _ProcessID = Process.GetProcessById(processID).Handle;

            //have we obtained a valid handle?
            if (_ProcessID == IntPtr.Zero)
            {
                   Console.WriteLine("## ERROR ## - Unable To Get Handle For Process ID '{0}', Aborting..", processID);
                return false;
            }//end of if


            if (Win32API.OpenProcessToken(_ProcessID, Win32API.TOKEN_DUPLICATE | Win32API.TOKEN_IMPERSONATE | Win32API.TOKEN_QUERY, out _TokenHandle) == false)
            {
                Console.WriteLine("## ERROR ##  - Trying To Open Process ID '{0}' Handle..\nError '{1}'", processID, Marshal.GetLastWin32Error());
                return false;
            }//end of if           

            if (_TokenHandle == IntPtr.Zero)
            {
                Console.WriteLine("## ERROR ## - Opened Token For Process ID '{0}' However Handle Is Invalid, Aborting..", processID);
                return false;
            }

            //Instantiate the process handle so we can resolve the account context
            WindowsIdentity _ProcessAccountID = new WindowsIdentity(_TokenHandle);
            Console.WriteLine("[+] Sucessfully Opened Process ID '{0}' Handle", processID);

            try
            {
                Win32API.SECURITY_ATTRIBUTES _SecuirtyAttributes = new Win32API.SECURITY_ATTRIBUTES();
                _SecuirtyAttributes.nLength = Marshal.SizeOf(_SecuirtyAttributes);

                //we need to duplicate the primary token
                if (Win32API.DuplicateTokenEx(_TokenHandle, Win32API.TOKEN_ALL_ACCESS, ref _SecuirtyAttributes, Win32API.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, Win32API.TOKEN_TYPE.TokenPrimary, out _DuplicateTokenHandle) == false)
                {
                    Console.WriteLine("## ERROR ## - Attempting To Duplicate Process Token..\nWin32 Error: {0}", Marshal.GetLastWin32Error());
                    return false;
                }//end of if


                //do we have a valid token
                if (_DuplicateTokenHandle == IntPtr.Zero)
                {
                    Console.WriteLine("## ERROR ## - Token Duplication Failed!\nWin32 Error: {0}", Marshal.GetLastWin32Error());
                    return false;
                }//end of if

                //Who Was I Again? :D
                WindowsImpersonationContext _ImpersonationContext = _ProcessAccountID.Impersonate();

                Console.WriteLine("[+] Impersonation Successfull!\n[+] Account Token ID Is '{0}', Impersonated Account Is '{1}'", WindowsIdentity.GetCurrent().Token, WindowsIdentity.GetCurrent().Name);

                return ExecuteCommand(_DuplicateTokenHandle, command);
            }
            catch (Exception ex)
            {
                Console.WriteLine("## ERROR ## - Something Went Wrong Duplicating The Process Token!\nError: '{0}'", ex.Message);
                _Errored = true;
            }
            finally
            {
                //clean up after ourselves
                if (_TokenHandle != IntPtr.Zero) { Win32API.CloseHandle(_TokenHandle); }                
            }//end of try-catch-finally

            //catch it here because i always want the finally clase to execute
            //you cannot jump out of the finally block
            if (_Errored) { return false; }


            //default catchall
            return false;
        }//end of public Boolean ImpersonateAndExecute(Int32 processID, String command)


        private static Boolean ExecuteCommand(IntPtr userAccountHandle, String command)
        {
            Win32API.PROCESS_INFORMATION _ProcessInfo = new Win32API.PROCESS_INFORMATION();
            Win32API.SECURITY_ATTRIBUTES _ProcesSecurityAttributes = new Win32API.SECURITY_ATTRIBUTES();
            Win32API.SECURITY_ATTRIBUTES _ThreadSecurityAttributes = new Win32API.SECURITY_ATTRIBUTES();

            _ProcesSecurityAttributes.nLength = Marshal.SizeOf(_ProcesSecurityAttributes);
            _ThreadSecurityAttributes.nLength = Marshal.SizeOf(_ThreadSecurityAttributes);

            Win32API.STARTUP_INFO _AppStartupInfo = new Win32API.STARTUP_INFO();
            _AppStartupInfo.cb = Marshal.SizeOf(_AppStartupInfo);

            //todo: is there a better method of selecting
            _AppStartupInfo.lpDesktop = @"WinSta0\Default"; //we want the defalt desktop
            _AppStartupInfo.dwFlags = Win32API.STARTF_USESHOWWINDOW | Win32API.STARTF_FORCEONFEEDBACK;
            _AppStartupInfo.wShowWindow = Win32API.SW_SHOW;


            //todo: load the user profile so i can access MyDocuments and other profile information

            //execute a new process with the token
            if (Win32API.CreateProcessWithTokenW(userAccountHandle, Win32API.LOGON_FLAGS.LOGON_NETCREDENTIALS_ONLY, null, command, Win32API.CREATION_FLAGS.CREATE_NEW_CONSOLE, IntPtr.Zero, null, ref _AppStartupInfo, out _ProcessInfo))
            {
                Console.WriteLine("[+] Sucessfully Executed Command '{0}' With Assigned Process ID '{1}", command, _ProcessInfo.dwProcessId);
                return true;
            }

            Console.WriteLine("## ERROR ## - Problem Executing Command!\nWin32 Error: '{0}'", Marshal.GetLastWin32Error());
            return false;
        }

        #region Process & Account Listing Methods

                /// <summary>
                /// Gets The First Process For The Listed Account
                /// </summary>
                /// <param name="account">Account Name To Search For</param>
                /// <returns>Process Entry Containing The Process ID</returns>
                public static ProcessEntry GetProcessIDForAccount(String account)
                {
                    account = account.ToUpper().Trim();

                    switch (account)
                    {
                        case "SYSTEM": account = @"NT AUTHORITY\SYSTEM"; break;
                        case "NETWORK SERVICE": account = @"NT AUTHORITY\NETWORK SERVICE"; break;
                        case "LOCAL SERVICE": account = @"NT AUTHORITY\LOCAL SERVICE"; break;   
                    }

                    foreach (KeyValuePair<String, List<ProcessEntry>> _Account in ListProcessAccounts)
                    {
                        //have we found the correct user?
                        if(_Account.Key.Equals(account))
                        {
                            //return the first PID
                            return _Account.Value[0];
                        }// if(_Account.Key.Equals(account))


                    }//end of foreach (KeyValuePair<String, List<Int32>> _Account in ListProcessAccounts)

                    return new ProcessEntry
                    {
                        Name = String.Empty,
                        PID = -1
                    };
                }//end of public ProcessEntry GetProcessIDForAccount(String account)


                /// <summary>
                /// List All Accounts Processes Are Running As.
                /// </summary>
                /// <returns>List of Account Names & Running Processes Under The Context of The Account.</returns>
                public static Dictionary<String, List<ProcessEntry>> ListProcessAccounts
                {
                    get
                    {
                        Dictionary<String, List<ProcessEntry>> _ActiveAccounts = new Dictionary<String, List<ProcessEntry>>();

                        foreach (var _Process in Process.GetProcesses())
                        {
                            IntPtr _ProcessHandle = IntPtr.Zero;

                            try
                            {
                                //obtain a handle to the process.
                                if (Win32API.OpenProcessToken(_Process.Handle, Win32API.TOKEN_QUERY, out _ProcessHandle))
                                {
                                    //do we have a valid handle?
                                    if (_ProcessHandle != IntPtr.Zero)
                                    {
                                        //lets get the identity of the handle.
                                        WindowsIdentity _AccountIdentity = new WindowsIdentity(_ProcessHandle);

                                        //do we have th account stored already?
                                        List<ProcessEntry> _ProcessIDs;
                                        if (_ActiveAccounts.TryGetValue(_AccountIdentity.Name, out _ProcessIDs))
                                        {
                                            _ProcessIDs.Add(
                                                            new ProcessEntry
                                                            {
                                                                Name = _Process.ProcessName,
                                                                PID = _Process.Id
                                                            }
                                                        );
                                        }
                                        else
                                        {
                                            _ActiveAccounts.Add(_AccountIdentity.Name, new List<ProcessEntry>
                                            {
                                                new ProcessEntry
                                                {
                                                    Name = _Process.ProcessName,
                                                    PID = _Process.Id
                                                }
                                            });
                                        } //end of if-else

                                    } //end of if (_ProcessHandle != IntPtr.Zero)

                                } //end of ifWin32API.OpenProcessToken
                            }
                            catch
                            {
                                //It Is not possible to open some processes and so there is always a few
                                //access denied errors, as such i want to silently disguard them.
                            }
                            finally
                            {
                                //if the handle is not null/zero then we need to close it.
                                if (_ProcessHandle != IntPtr.Zero) { Win32API.CloseHandle(_ProcessHandle); }
                            }//end of try-finally

                        }//end of foreach

                        return _ActiveAccounts;

                    }//end of get
                }//end of public static Dictionary<String, List<Int32>> ListProcessAccounts


                public static void PrintActiveAccountsToConsole()
                {
                    Console.WriteLine(" - Listing Accounts Of Active Processes...");

                    foreach (KeyValuePair<String, List<ProcessEntry>> _RunningAccount in ListProcessAccounts)
                    {
                        Console.WriteLine("\t[+] {0} - {1} Running Processes", _RunningAccount.Key, _RunningAccount.Value.Count);
                    }//end offoreach (KeyValuePair<String, List<Int32>> _RunningAccount in ListProcessAccounts)


                }//end of  public static void PrintActiveAccountsToConsole()
        #endregion


        #region Utility Methods

                public static Boolean IsAdmin
                {
                    get
                    {
                        WindowsIdentity _Identity = WindowsIdentity.GetCurrent();
                        if (_Identity == null) { return false; }

                        WindowsPrincipal _Principal = new WindowsPrincipal(_Identity);

                        return _Principal.IsInRole(WindowsBuiltInRole.Administrator);
                    }
                }//end of public static Boolean IsAdmin
        
        #endregion

    }//end of class WhoWasI

    public struct ProcessEntry
    {
        public String Name;
        public Int32 PID;
    }//end of Struct

}//end of namespace
