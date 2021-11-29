using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using DInvoke.DynamicInvoke;
using static DInvoke.Data.Native;

namespace ForkDumpSharp
{
    class Help
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate NTSTATUS NtCreateProcessEx(
            out IntPtr threadHandle,
            uint desiredAccess,
            IntPtr objectAttributes,
            IntPtr processHandle,
            bool InheritObjectTable,
            IntPtr SectionHandle,
            IntPtr DebugPort,
            IntPtr ExceptionPort);

        public static NtCreateProcessEx GetNtCreateProcessExAddress()
        {
            IntPtr stub = Generic.GetSyscallStub("NtCreateProcessEx");
            NtCreateProcessEx sysNtCreateProcessEx = (NtCreateProcessEx)Marshal.GetDelegateForFunctionPointer(stub, typeof(NtCreateProcessEx));
            return sysNtCreateProcessEx;
        }

        [DllImport("dbghelp.dll", EntryPoint = "MiniDumpWriteDump", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
        public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint GetProcessIdOfThread(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
             uint processAccess,
             bool bInheritHandle,
             int processId
        );
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
    }
}
