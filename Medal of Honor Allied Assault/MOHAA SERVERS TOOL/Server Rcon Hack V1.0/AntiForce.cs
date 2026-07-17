using System;
using System.Diagnostics;
using System.Threading;

public static class AntiForce
{
    private static bool _active = false;
    private static Thread _workerThread;

    public static void Toggle(bool status)
    {
        _active = status;

        if (status)
        {
            if (_workerThread == null || !_workerThread.IsAlive)
            {
                _workerThread = new Thread(WorkerLoop)
                {
                    IsBackground = true
                };
                _workerThread.Start();
            }
        }
    }

    private static void WorkerLoop()
    {
        var proc = Memory.GetProcess();
        if (proc == null) return;

        IntPtr hProc = Memory.OpenProcess(0x1F0FFF, false, proc.Id);
        if (hProc == IntPtr.Zero) return;

        IntPtr exeBase = Memory.GetModuleBase(proc, "MOHAA.exe");
        if (exeBase == IntPtr.Zero)
        {
            Memory.CloseHandle(hProc);
            return;
        }

        IntPtr ptrAddr = exeBase + 0x00AC67C8;
        byte[] ptrBuf = new byte[4];
        byte[] value = BitConverter.GetBytes(1); // القيمة المطلوبة

        while (_active)
        {
            // اقرأ الـ pointer
            if (Memory.ReadProcessMemory(hProc, ptrAddr, ptrBuf, 4, out _))
            {
                int basePtr = BitConverter.ToInt32(ptrBuf, 0);
                if (basePtr != 0)
                {
                    IntPtr finalAddr = new IntPtr(basePtr + 0xFF0);

                    // كتابة مستمرة (Freeze)
                    Memory.WriteProcessMemory(
                        hProc,
                        finalAddr,
                        value,
                        4,
                        out _
                    );
                }
            }

            Thread.Sleep(5);
        }

        Memory.CloseHandle(hProc);
    }
}
