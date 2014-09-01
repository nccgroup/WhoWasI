using System;
using System.Runtime.InteropServices;
using System.Text;

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
    public static class Privilages
    {
        /// <summary>
        /// Resolves A Specified LUID Value Into The Apropriate Windows Privilege
        /// </summary>
        public static String GetPrivilegeName(Win32API.LUID luid)
        {
            try
            {
                StringBuilder _PrivilegeName = new StringBuilder();

                //hold the length of the LuID Struct
                Int32 _NameLength = 0;

                //first method call is to get the _NameLength so we can allocate a buffer
                Win32API.LookupPrivilegeName(String.Empty, ref luid, _PrivilegeName, ref _NameLength);

                //make sure there is sufficient space in memory
                _PrivilegeName.EnsureCapacity(_NameLength);

                //look up the privilage name
                if (Win32API.LookupPrivilegeName(String.Empty, ref luid, _PrivilegeName, ref _NameLength))
                {
                    return _PrivilegeName.ToString();
                }//if (Win32API.LookupPrivilegeName(String.Empty, ref luid, _PrivilegeName, ref _NameLength))
            }
            catch (Exception)
            {
                Console.WriteLine("## ERROR ## - Problem Getting Privilege Name!\nWin32 Error: '{0}', LUID '{1}'", Marshal.GetLastWin32Error(), luid);
            }//end of try-catch

            //default catch all
            return String.Empty;
        }//end of public static String GetPrivilegeName(Win32API.LUID luid)


        /// <summary>
        /// Gets The System LUID Value for The Specified Privilege
        /// </summary>
        /// <param name="privilege">Specified Privilege</param>
        /// <returns>Associated LUID Value</returns>
        public static Win32API.LUID GetLUIDFromPrivilageName(Win32API.Privilege privilege)
        {
            Win32API.LUID _LUID = new Win32API.LUID();


            if (Win32API.LookupPrivilegeValue(String.Empty, privilege.ToString(), out _LUID) == false)
            {
                Console.WriteLine("## ERROR ## - Problem Resolving Privilage '{0}' To Its LUID!\nWin32 Error: '{1}'", privilege, Marshal.GetLastWin32Error());

                return new Win32API.LUID();
            }

            return _LUID;
        }//end of  public static GetLUIDFromPrivilageName(Win32API.Privilege privilege)

        /// <summary>
        /// Checks Whether The Proccess Has A Specified Privilege
        /// </summary>
        /// <param name="privilege">Required Privilege</param>
        /// <param name="processTokenHandle">Token Handle of Process</param>        
        public static Boolean HasPrivilege(Win32API.Privilege privilege, IntPtr processTokenHandle)
        {
            //Get the nessesary LUID for the required privilage
            Win32API.LUID _PrivLUID = GetLUIDFromPrivilageName(privilege);

            //did we get something valid?
            if (_PrivLUID.HighPart == 0) {  return false; }

            //get a list of privs
            Win32API.LUID_AND_ATTRIBUTES[] _Privs = GetPrivileges(processTokenHandle);

            //cycle through each privilage and dump out to screen
            foreach (var luidAndAttributes in _Privs)
            {
                if (luidAndAttributes.Luid.Equals(_PrivLUID)) { return true; }
            }//end of foreach            

            return false;
        }


        /// <summary>
        /// Get A List of Privilages Assigned To The Specified Process
        /// </summary>
        /// <param name="processTokenHandle">Process Handle</param>
        /// <returns>List of Assigned Privileges</returns>
        public static Win32API.LUID_AND_ATTRIBUTES[] GetPrivileges(IntPtr processTokenHandle)
        {
            //generic try-catch to make sure we gracefully handle any errors!
            try
            {
                // hold the length of TOKEN_PRIVILEGES Struct that is returned on the first call
                Int32 _TokenInformationLength = 0; 

                //_TokenInformationLength variable in the first instance is not required, its the "out" version of this variable we need
                Win32API.GetTokenInformation(processTokenHandle, Win32API.TOKEN_INFORMATION_CLASS.TokenPrivileges, IntPtr.Zero, _TokenInformationLength, out _TokenInformationLength);

                //Allocate a block of memory to hold the required info
                IntPtr _TokenInformation = Marshal.AllocHGlobal(_TokenInformationLength);

                //now we have an allocated block of memory to handle the privilage structre lets grab the list of privilages
                if (Win32API.GetTokenInformation(processTokenHandle, Win32API.TOKEN_INFORMATION_CLASS.TokenPrivileges, _TokenInformation, _TokenInformationLength, out _TokenInformationLength) == false)
                {
                    Console.WriteLine("## ERROR ## - Problem Executing GetTokenInformation on ProcessHandle '{1}'!\nWin32 Error: '{0}'", Marshal.GetLastWin32Error(), processTokenHandle);
                }//end of if (Win32API.GetTokenInformation

                Int32 _PrivilegeCount = Marshal.ReadInt32(_TokenInformation);

                //do we have some privileges to cycle through?
                if (_PrivilegeCount <= 0)
                {
                    Console.WriteLine("## ERROR ## - Privilege Count Aprears To Be Invalid on Process Handle '{0}', Count '{1}'", processTokenHandle, _PrivilegeCount);

                    Marshal.FreeHGlobal(_TokenInformation); //clean up
                    return new Win32API.LUID_AND_ATTRIBUTES[0];
                }//end of if (_PrivilegeCount <= 0)

                Win32API.LUID_AND_ATTRIBUTES[] _TokenPrivileges = new Win32API.LUID_AND_ATTRIBUTES[_PrivilegeCount];

                //pointer to hold the location within memory, take the last pointer plus the size of the last read structure
                IntPtr _ReadPointer = new IntPtr(_TokenInformation.ToInt32() + sizeof(int));

                //cycle through the structure memory and fish out all of the pointer info
                for (Int32 i = 1; i < _PrivilegeCount; i++)
                {
                    //Load the record
                    Win32API.LUID_AND_ATTRIBUTES _TempTokenPrivs = (Win32API.LUID_AND_ATTRIBUTES)Marshal.PtrToStructure(_ReadPointer, typeof(Win32API.LUID_AND_ATTRIBUTES));

                  _ReadPointer = new IntPtr(_ReadPointer.ToInt32() + Marshal.SizeOf(_TempTokenPrivs));
                  _TokenPrivileges[i] = _TempTokenPrivs;
                }//end of for loop

                Marshal.FreeHGlobal(_TokenInformation); //clean up

                //return our list of privilages
                return _TokenPrivileges;
            }
            catch (Exception)
            {
                Console.WriteLine("## ERROR ## - Problem Listing System Privileges!\nWin32 Error: '{0}'", Marshal.GetLastWin32Error());
            }//end of try-catch


            //default catch all, we should not get to this point if everything worked!
            return new Win32API.LUID_AND_ATTRIBUTES[0];
        }//end of public static Boolean ListPrivilages(IntPtr processHandle)


    }
}
