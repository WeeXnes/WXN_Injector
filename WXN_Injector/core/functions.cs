using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WXN_Injector.core
{
    public static class Injector
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
            IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        public static void Inject(string procname, string path)
        {
            string dll = path;
            Process targetProcess = null;
            bool found = false;

            do
            {
                try
                {
                    targetProcess = Process.GetProcessesByName(procname)[0];
                    found = true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not find process!");
                    Thread.Sleep(1000);
                    continue;
                }
            }
            while (found == false);

            Console.WriteLine("Process found. ID: " + targetProcess.Id);

            IntPtr handle = OpenProcess(0x001F0FFF, false, targetProcess.Id); //0x001F0FFF: Access - All
            
            IntPtr LibraryAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            IntPtr AllocatedMemory = VirtualAllocEx(handle, IntPtr.Zero, (uint)dll.Length + 1, 0x00001000, 4); //0x00001000: Memory - commit, 4: Page - Read and Write
            Console.WriteLine("DLL allocated at: " + AllocatedMemory.ToString());
            MessageBox.Show("DLL allocated at: " + AllocatedMemory.ToString(), "Process found. ID: " + targetProcess.Id);

            UIntPtr bytesWritten;
            WriteProcessMemory(handle, AllocatedMemory, Encoding.Default.GetBytes(dll), (uint)dll.Length + 1, out bytesWritten);

            IntPtr threadHandle = CreateRemoteThread(handle, IntPtr.Zero, 0, LibraryAddress, AllocatedMemory, 0, IntPtr.Zero);

            //WaitForSingleObject(handle, 0xFFFFFFFF); //0xFFFFFFFF: Infinite

            
            CloseHandle(threadHandle);
            
            VirtualFreeEx(handle, AllocatedMemory, dll.Length + 1, 0x8000); //0x8000: Memory - release
            CloseHandle(handle);
        }
    }

    public static class experimental
    {
        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }
        public static string CheckGraphicsApi(string processName)
        {
            string returnVal = null;
            Process process = Process.GetProcessesByName(processName).FirstOrDefault();
            ProcessModuleCollection modules = process.Modules;
            foreach (ProcessModule module in modules)
            {
                //Console.WriteLine(module.FileName);
                if (Contains(module.ModuleName, "d3d9", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses DirectX 9\n" + module.FileName + " found";
                    break;
                }

                if (Contains(module.ModuleName, "d3d10", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses DirectX 10\n" + module.FileName + " found";
                    break;
                }

                if (Contains(module.ModuleName, "d3d11", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses DirectX 11\n" + module.FileName + " found";
                    break;
                }

                if (Contains(module.ModuleName, "d3d12", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses DirectX 12\n" + module.FileName + " found";
                    break;
                }

                if (Contains(module.ModuleName, "vulcan", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses Vulcan\n" + module.FileName + " found";
                    break;
                }

                if (Contains(module.ModuleName, "opengl", StringComparison.OrdinalIgnoreCase))
                {
                    returnVal = "-> " + processName + ".exe uses OpenGL\n" + module.FileName + " found";
                    break;
                }
            }

            return returnVal;
        }
    }
}