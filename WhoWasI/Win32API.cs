using System;
using System.Runtime.InteropServices;

namespace WhoWasI
{
    public static class Win32API
    {
        //http://www.pinvoke.net/default.aspx/advapi32.openprocesstoken
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        //http://www.pinvoke.net/default.aspx/kernel32.closehandle
        [DllImport("kernel32.dll", SetLastError=true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CloseHandle(IntPtr hObject);

        //http://www.pinvoke.net/default.aspx/advapi32.duplicatetokenex
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static Boolean DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, ref SECURITY_ATTRIBUTES lpTokenAttributes, SECURITY_IMPERSONATION_LEVEL ImpersonationLevel, TOKEN_TYPE TokenType, out IntPtr phNewToken);

        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CreateProcessWithTokenW(IntPtr hToken, LOGON_FLAGS dwLogonFlags, string lpApplicationName, string lpCommandLine, CREATION_FLAGS dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUP_INFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);


        #region DuplicateTokenEx Constants/Structs
                public enum SECURITY_IMPERSONATION_LEVEL
                {
                    SecurityAnonymous,
                    SecurityIdentification,
                    SecurityImpersonation,
                    SecurityDelegation
                }

                public enum TOKEN_TYPE
                {
                    TokenPrimary = 1,
                    TokenImpersonation
                }
        #endregion

        #region OpenProcessToken Constants/Structs
                public const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
                public const UInt32 STANDARD_RIGHTS_READ = 0x00020000;
                public const UInt32 TOKEN_ASSIGN_PRIMARY = 0x0001;
                public const UInt32 TOKEN_DUPLICATE = 0x0002;
                public const UInt32 TOKEN_IMPERSONATE = 0x0004;
                public const UInt32 TOKEN_QUERY = 0x0008;
                public const UInt32 TOKEN_QUERY_SOURCE = 0x0010;
                public const UInt32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
                public const UInt32 TOKEN_ADJUST_GROUPS = 0x0040;
                public const UInt32 TOKEN_ADJUST_DEFAULT = 0x0080;
                public const UInt32 TOKEN_ADJUST_SESSIONID = 0x0100;
                public const UInt32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
                public const UInt32 TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
                    TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
                    TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
                    TOKEN_ADJUST_SESSIONID);
        #endregion

        #region CreateProcessWithTokenW Constants/Structs
                [Flags]
                public enum CREATION_FLAGS
                {
                    CREATE_SUSPENDED = 0x00000004,
                    CREATE_NEW_CONSOLE = 0x00000010,
                    CREATE_NEW_PROCESS_GROUP = 0x00000200,
                    CREATE_UNICODE_ENVIRONMENT = 0x00000400,
                    CREATE_SEPARATE_WOW_VDM = 0x00000800,
                    CREATE_DEFAULT_ERROR_MODE = 0x04000000,
                }

                [Flags]
                public enum LOGON_FLAGS
                {
                    LOGON_WITH_PROFILE = 0x00000001,
                    LOGON_NETCREDENTIALS_ONLY = 0x00000002
                }

        #endregion

        #region Generic / All-Round Constants / Structs

                //todo: where are the references for this?
                public const short SW_SHOW = 5;
                public const int STARTF_USESHOWWINDOW = 0x00000001;
                public const int STARTF_FORCEONFEEDBACK = 0x00000040;

                //http://www.pinvoke.net/default.aspx/Structures/SECURITY_ATTRIBUTES.html
                [StructLayout(LayoutKind.Sequential)]
                public struct SECURITY_ATTRIBUTES
                {
                    public int nLength;
                    public IntPtr lpSecurityDescriptor;
                    public int bInheritHandle;
                }

                //http://www.pinvoke.net/default.aspx/Structures/PROCESS_INFORMATION.html
                [StructLayout(LayoutKind.Sequential)]
                public struct PROCESS_INFORMATION
                {
                    public IntPtr hProcess;
                    public IntPtr hThread;
                    public int dwProcessId;
                    public int dwThreadId;
                }

                //http://www.pinvoke.net/default.aspx/Structures/STARTUPINFO.html
                [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
                public struct STARTUP_INFO
                {
                    public Int32 cb;
                    public string lpReserved;
                    public string lpDesktop;
                    public string lpTitle;
                    public Int32 dwX;
                    public Int32 dwY;
                    public Int32 dwXSize;
                    public Int32 dwYSize;
                    public Int32 dwXCountChars;
                    public Int32 dwYCountChars;
                    public Int32 dwFillAttribute;
                    public Int32 dwFlags;
                    public Int16 wShowWindow;
                    public Int16 cbReserved2;
                    public IntPtr lpReserved2;
                    public IntPtr hStdInput;
                    public IntPtr hStdOutput;
                    public IntPtr hStdError;
                }
        #endregion

    }
}
