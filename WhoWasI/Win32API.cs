using System;
using System.Runtime.InteropServices;

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

        //http://www.pinvoke.net/default.aspx/Structures/CreateProcessWithTokenW.html
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CreateProcessWithTokenW(IntPtr hToken, LOGON_FLAGS dwLogonFlags, string lpApplicationName, string lpCommandLine, CREATION_FLAGS dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUP_INFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        //http://www.pinvoke.net/default.aspx/advapi32.gettokeninformation
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(IntPtr TokenHandle, TOKEN_INFORMATION_CLASS TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);

        //http://www.pinvoke.net/default.aspx/advapi32.lookupprivilegename
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeName(string lpSystemName, ref LUID lpLuid, System.Text.StringBuilder lpName, ref int cchName);
        
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue(string lpSystemName, string lpName, out LUID lpLuid);

        #region GetTokenInformation Constants/Structs
                    //http://www.pinvoke.net/default.aspx/Enums/TOKEN_INFORMATION_CLASS.html
                    public enum TOKEN_INFORMATION_CLASS
                    {
                        /// <summary>
                        /// The buffer receives a TOKEN_USER structure that contains the user account of the token.
                        /// </summary>
                        TokenUser = 1,

                        /// <summary>
                        /// The buffer receives a TOKEN_GROUPS structure that contains the group accounts associated with the token.
                        /// </summary>
                        TokenGroups,

                        /// <summary>
                        /// The buffer receives a TOKEN_PRIVILEGES structure that contains the privileges of the token.
                        /// </summary>
                        TokenPrivileges,

                        /// <summary>
                        /// The buffer receives a TOKEN_OWNER structure that contains the default owner security identifier (SID) for newly created objects.
                        /// </summary>
                        TokenOwner,

                        /// <summary>
                        /// The buffer receives a TOKEN_PRIMARY_GROUP structure that contains the default primary group SID for newly created objects.
                        /// </summary>
                        TokenPrimaryGroup,

                        /// <summary>
                        /// The buffer receives a TOKEN_DEFAULT_DACL structure that contains the default DACL for newly created objects.
                        /// </summary>
                        TokenDefaultDacl,

                        /// <summary>
                        /// The buffer receives a TOKEN_SOURCE structure that contains the source of the token. TOKEN_QUERY_SOURCE access is needed to retrieve this information.
                        /// </summary>
                        TokenSource,

                        /// <summary>
                        /// The buffer receives a TOKEN_TYPE value that indicates whether the token is a primary or impersonation token.
                        /// </summary>
                        TokenType,

                        /// <summary>
                        /// The buffer receives a SECURITY_IMPERSONATION_LEVEL value that indicates the impersonation level of the token. If the access token is not an impersonation token, the function fails.
                        /// </summary>
                        TokenImpersonationLevel,

                        /// <summary>
                        /// The buffer receives a TOKEN_STATISTICS structure that contains various token statistics.
                        /// </summary>
                        TokenStatistics,

                        /// <summary>
                        /// The buffer receives a TOKEN_GROUPS structure that contains the list of restricting SIDs in a restricted token.
                        /// </summary>
                        TokenRestrictedSids,

                        /// <summary>
                        /// The buffer receives a DWORD value that indicates the Terminal Services session identifier that is associated with the token. 
                        /// </summary>
                        TokenSessionId,

                        /// <summary>
                        /// The buffer receives a TOKEN_GROUPS_AND_PRIVILEGES structure that contains the user SID, the group accounts, the restricted SIDs, and the authentication ID associated with the token.
                        /// </summary>
                        TokenGroupsAndPrivileges,

                        /// <summary>
                        /// Reserved.
                        /// </summary>
                        TokenSessionReference,

                        /// <summary>
                        /// The buffer receives a DWORD value that is nonzero if the token includes the SANDBOX_INERT flag.
                        /// </summary>
                        TokenSandBoxInert,

                        /// <summary>
                        /// Reserved.
                        /// </summary>
                        TokenAuditPolicy,

                        /// <summary>
                        /// The buffer receives a TOKEN_ORIGIN value. 
                        /// </summary>
                        TokenOrigin,

                        /// <summary>
                        /// The buffer receives a TOKEN_ELEVATION_TYPE value that specifies the elevation level of the token.
                        /// </summary>
                        TokenElevationType,

                        /// <summary>
                        /// The buffer receives a TOKEN_LINKED_TOKEN structure that contains a handle to another token that is linked to this token.
                        /// </summary>
                        TokenLinkedToken,

                        /// <summary>
                        /// The buffer receives a TOKEN_ELEVATION structure that specifies whether the token is elevated.
                        /// </summary>
                        TokenElevation,

                        /// <summary>
                        /// The buffer receives a DWORD value that is nonzero if the token has ever been filtered.
                        /// </summary>
                        TokenHasRestrictions,

                        /// <summary>
                        /// The buffer receives a TOKEN_ACCESS_INFORMATION structure that specifies security information contained in the token.
                        /// </summary>
                        TokenAccessInformation,

                        /// <summary>
                        /// The buffer receives a DWORD value that is nonzero if virtualization is allowed for the token.
                        /// </summary>
                        TokenVirtualizationAllowed,

                        /// <summary>
                        /// The buffer receives a DWORD value that is nonzero if virtualization is enabled for the token.
                        /// </summary>
                        TokenVirtualizationEnabled,

                        /// <summary>
                        /// The buffer receives a TOKEN_MANDATORY_LABEL structure that specifies the token's integrity level. 
                        /// </summary>
                        TokenIntegrityLevel,

                        /// <summary>
                        /// The buffer receives a DWORD value that is nonzero if the token has the UIAccess flag set.
                        /// </summary>
                        TokenUIAccess,

                        /// <summary>
                        /// The buffer receives a TOKEN_MANDATORY_POLICY structure that specifies the token's mandatory integrity policy.
                        /// </summary>
                        TokenMandatoryPolicy,

                        /// <summary>
                        /// The buffer receives the token's logon security identifier (SID).
                        /// </summary>
                        TokenLogonSid,

                        /// <summary>
                        /// The maximum value for this enumeration
                        /// </summary>
                        MaxTokenInfoClass
                    }                       
        #endregion

        #region LookupPrivilegeName/LookupPrivilegeValue Constants/Structs
                    //http://www.pinvoke.net/default.aspx/Structures/TOKEN_PRIVILEGES.html
                    public const UInt32 SE_PRIVILEGE_ENABLED_BY_DEFAULT = 0x00000001;
                    public const UInt32 SE_PRIVILEGE_ENABLED = 0x00000002;
                    public const UInt32 SE_PRIVILEGE_REMOVED = 0x00000004;
                    public const UInt32 SE_PRIVILEGE_USED_FOR_ACCESS = 0x80000000;

                    //http://www.pinvoke.net/default.aspx/Structures/TOKEN_PRIVILEGES.html
                    [StructLayout(LayoutKind.Sequential)]
                    public struct TOKEN_PRIVILEGES
                    {
                        public UInt32 PrivilegeCount;
                        public LUID Luid;
                        public UInt32 Attributes;
                    }

                    //http://www.pinvoke.net/default.aspx/Structures/LUID_AND_ATTRIBUTES.html
                    [StructLayout(LayoutKind.Sequential)]
                    public struct LUID_AND_ATTRIBUTES
                    {
                        public LUID Luid;
                        public UInt32 Attributes;
                    }

                    //http://www.pinvoke.net/default.aspx/Structures/LUID.html
                    [StructLayout(LayoutKind.Sequential)]
                    public struct LUID
                    {
                        public uint LowPart;
                        public int HighPart;
                    }

                    //http://msdn.microsoft.com/en-gb/library/windows/desktop/bb530716(v=vs.85).aspx
                    public enum Privilege
                    {
                        SeAssignPrimaryTokenPrivilege,
                        SeAuditPrivilege,
                        SeBackupPrivilege,
                        SeChangeNotifyPrivilege,
                        SeCreateGlobalPrivilege,
                        SeCreatePagefilePrivilege,
                        SeCreatePermanentPrivilege,
                        SeCreateSymbolicLinkPrivilege,
                        SeCreateTokenPrivilege,
                        SeDebugPrivilege,
                        SeEnableDelegationPrivilege,
                        SeImpersonatePrivilege,
                        SeIncreaseBasePriorityPrivilege,
                        SeIncreaseQuotaPrivilege,
                        SeIncreaseWorkingSetPrivilege,
                        SeLoadDriverPrivilege,
                        SeLockMemoryPrivilege,
                        SeMachineAccountPrivilege,
                        SeManageVolumePrivilege,
                        SeProfileSingleProcessPrivilege,
                        SeRelabelPrivilege,
                        SeRemoteShutdownPrivilege,
                        SeRestorePrivilege,
                        SeSecurityPrivilege,
                        SeShutdownPrivilege,
                        SeSyncAgentPrivilege,
                        SeSystemEnvironmentPrivilege,
                        SeSystemProfilePrivilege,
                        SeSystemtimePrivilege,
                        SeTakeOwnershipPrivilege,
                        SeTcbPrivilege,
                        SeTimeZonePrivilege,
                        SeTrustedCredManAccessPrivilege,
                        SeUndockPrivilege,
                        SeUnsolicitedInputPrivilege
                    }

        #endregion

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
