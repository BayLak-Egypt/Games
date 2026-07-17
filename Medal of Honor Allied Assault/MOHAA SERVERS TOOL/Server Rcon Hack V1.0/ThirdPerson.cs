using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public static class ThirdPerson
{
    private static bool _active = false;
    private static Thread _hotkeyThread;
    private static bool _isThirdPersonOn = false; // لحفظ الحالة الحالية (هل هو شغال أم لا)

    // استيراد دالة فحص أزرار الكيبورد
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static void Toggle(bool status)
    {
        _active = status;
        if (status)
        {
            if (_hotkeyThread == null || !_hotkeyThread.IsAlive)
            {
                _hotkeyThread = new Thread(HotkeyLoop) { IsBackground = true };
                _hotkeyThread.Start();
            }
        }
    }

    private static void HotkeyLoop()
    {
        bool last8State = false;

        while (_active)
        {
            // فحص هل زر "8" مضغوط حالياً
            bool current8State = (GetAsyncKeyState((int)Keys.D8) & 0x8000) != 0;

            // إذا تم ضغط الزر (ولم يكن مضغوطاً في الدورة السابقة) - منع التكرار السريع
            if (current8State && !last8State)
            {
                ApplyMemoryChange();
            }

            last8State = current8State;
            Thread.Sleep(20); // تقليل الضغط على المعالج
        }
    }

    private static void ApplyMemoryChange()
    {
        var proc = Memory.GetProcess();
        if (proc == null) return;

        IntPtr hProc = Memory.OpenProcess(0x1F0FFF, false, proc.Id);
        IntPtr modBase = Memory.GetModuleBase(proc, "cgamex86.dll");

        if (modBase != IntPtr.Zero && hProc != IntPtr.Zero)
        {
            IntPtr ptrAddr = modBase + 0x002B7DA4;
            byte[] buf = new byte[4];

            if (Memory.ReadProcessMemory(hProc, ptrAddr, buf, 4, out _))
            {
                int basePtr = BitConverter.ToInt32(buf, 0);
                if (basePtr != 0)
                {
                    IntPtr finalAddr = new IntPtr(basePtr + 0x78);

                    // عكس الحالة: إذا كان 1 خليه 0، وإذا كان 0 خليه 1
                    _isThirdPersonOn = !_isThirdPersonOn;
                    byte[] val = BitConverter.GetBytes(_isThirdPersonOn ? 1 : 0);

                    Memory.WriteProcessMemory(hProc, finalAddr, val, 4, out _);
                }
            }
        }
        Memory.CloseHandle(hProc);
    }
}