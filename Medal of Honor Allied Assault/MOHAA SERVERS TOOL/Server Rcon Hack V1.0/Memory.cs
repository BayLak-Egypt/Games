using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

public static class Memory
{
    [DllImport("kernel32.dll")] public static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
    [DllImport("kernel32.dll")] public static extern bool ReadProcessMemory(IntPtr hProc, IntPtr baseAddr, byte[] buffer, int size, out int read);
    [DllImport("kernel32.dll")] public static extern bool WriteProcessMemory(IntPtr hProc, IntPtr baseAddr, byte[] buffer, int size, out int written);
    [DllImport("kernel32.dll")] public static extern bool CloseHandle(IntPtr handle);

    // Toolhelp32Snapshot imports
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, int th32ProcessID);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool Module32First(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern bool Module32Next(IntPtr hSnapshot, ref MODULEENTRY32 lpme);

    const uint TH32CS_SNAPMODULE = 0x00000008;
    const uint TH32CS_SNAPMODULE32 = 0x00000010;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct MODULEENTRY32
    {
        public uint dwSize;
        public uint th32ModuleID;
        public uint th32ProcessID;
        public uint GlblcntUsage;
        public uint ProccntUsage;
        public IntPtr modBaseAddr;
        public uint modBaseSize;
        public IntPtr hModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szModule;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExePath;
    }

    // الجديد: GetModuleBase بدون Process.Modules
    public static IntPtr GetModuleBase(Process proc, string moduleName)
    {
        IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPMODULE | TH32CS_SNAPMODULE32, proc.Id);

        MODULEENTRY32 modEntry = new MODULEENTRY32();
        modEntry.dwSize = (uint)Marshal.SizeOf(modEntry);

        if (Module32First(snapshot, ref modEntry))
        {
            do
            {
                if (modEntry.szModule.Equals(moduleName, StringComparison.OrdinalIgnoreCase))
                    return modEntry.modBaseAddr;
            } while (Module32Next(snapshot, ref modEntry));
        }

        return IntPtr.Zero;
    }

    public static Process GetProcess()
    {
        Process[] procs = Process.GetProcessesByName("mohaa");
        return procs.Length > 0 ? procs[0] : null;
    }
}
