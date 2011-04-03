
namespace KinecTh.common
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 
    /// </summary>
    public class Win32
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out RECT rect);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        public static IntPtr FindWindow(string name)
        {
            Process[] procs = Process.GetProcesses();

            foreach (Process proc in procs)
            {
                if (proc.ProcessName == name)
                {
                    return proc.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }
}
