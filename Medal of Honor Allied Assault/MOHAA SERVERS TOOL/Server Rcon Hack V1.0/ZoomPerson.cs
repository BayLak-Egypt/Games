using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public static class ZoomPerson
{
    private static bool _active = false;       // السكريبت مفعل أم لا
    private static Thread _hotkeyThread;
    private static short _currentValue = 0;    // القيمة الحالية للفريز
    private static bool _initialized = false;  // هل بدأ السكريبت بعد أول ضغط 9

    // دالة فحص أزرار الكيبورد
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    // العنوان الرئيسي للعبة
    private const int TARGET_ADDRESS = 0x0110A30E;
    private const short VALUE_ENABLE = 17160;
    private const short VALUE_DISABLE = 17056;

    // Pointer التحكم
    private const string CONTROL_MODULE = "gamex86.dll";
    private const int CONTROL_BASE_OFFSET = 0x0000EE58;
    private const int CONTROL_OFFSET = 0xAD4;

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
        bool last9State = false;

        // البداية بالقيمة ENABLE
        _currentValue = VALUE_ENABLE;

        Console.WriteLine("[*] Zoom script idle. Press 9 to start...");

        while (_active)
        {
            bool current9State = (GetAsyncKeyState((int)Keys.D9) & 0x8000) != 0;

            if (current9State && !last9State)
            {
                if (!_initialized)
                {
                    _initialized = true;
                    Console.WriteLine("[*] Zoom script activated!");
                }
                else
                {
                    // تبديل القيمة بين ENABLE / DISABLE
                    _currentValue = (_currentValue == VALUE_ENABLE) ? VALUE_DISABLE : VALUE_ENABLE;
                    Console.WriteLine($"[*] Zoom freeze value changed: {_currentValue}");
                }
            }

            last9State = current9State;

            // إذا السكريبت بدأ → فريز مستمر للقيمة الحالية طالما Pointer التحكم = 1
            if (_initialized)
            {
                ApplyZoomFreeze();
            }

            Thread.Sleep(5);
        }
    }

    private static void ApplyZoomFreeze()
    {
        var proc = Memory.GetProcess();
        if (proc == null) return;

        IntPtr hProc = Memory.OpenProcess(0x1F0FFF, false, proc.Id);
        if (hProc == IntPtr.Zero) return;

        IntPtr modBase = Memory.GetModuleBase(proc, CONTROL_MODULE);
        if (modBase != IntPtr.Zero)
        {
            IntPtr ptrAddr = modBase + CONTROL_BASE_OFFSET;
            byte[] ptrBuf = new byte[4];

            if (Memory.ReadProcessMemory(hProc, ptrAddr, ptrBuf, 4, out _))
            {
                int basePtr = BitConverter.ToInt32(ptrBuf, 0);
                if (basePtr != 0)
                {
                    IntPtr controlFinalAddr = new IntPtr(basePtr + CONTROL_OFFSET);
                    byte[] controlBuf = new byte[4];

                    if (Memory.ReadProcessMemory(hProc, controlFinalAddr, controlBuf, 4, out _))
                    {
                        int controlValue = BitConverter.ToInt32(controlBuf, 0);

                        if (controlValue == 1)
                        {
                            byte[] zoomBuf = BitConverter.GetBytes(_currentValue);
                            Memory.WriteProcessMemory(hProc, (IntPtr)TARGET_ADDRESS, zoomBuf, 2, out _);
                        }
                        else
                        {
                            Console.WriteLine("[!] Pointer control = 0 → script paused.");
                        }
                    }
                }
            }
        }

        Memory.CloseHandle(hProc);
    }
}
