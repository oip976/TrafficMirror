using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using static NativeMethods;

namespace TrafficMirror.Classes
{
    class KeyboardInput
    {

        public static void LeftArrowKey()
        {
            DoKeyboard(KEYEVENTF.VK_LEFT, 0);
        }
        public static void LeftArrowKeyReleased()
        {
            DoKeyboard(KEYEVENTF.VK_LEFT, KEYEVENTF.KEYEVENTF_KEYUP);
        }
        public static void RightArrowKey()
        {
            DoKeyboard(KEYEVENTF.VK_RIGHT, 0);
        }
        public static void RightArrowKeyReleased()
        {
            DoKeyboard(KEYEVENTF.VK_RIGHT, KEYEVENTF.KEYEVENTF_KEYUP);
        }
        public static void UpArrowKey()
        {
            DoKeyboard(KEYEVENTF.VK_UP, 0);
        }
        public static void UpArrowKeyReleased()
        {
            DoKeyboard(KEYEVENTF.VK_UP, KEYEVENTF.KEYEVENTF_KEYUP);
        }
        public static void DownArrowKey()
        {
            DoKeyboard(KEYEVENTF.VK_DOWN, 0);
        }
        public static void DownArrowKeyReleased()
        {
            DoKeyboard(KEYEVENTF.VK_DOWN, KEYEVENTF.KEYEVENTF_KEYUP);
        }
        private static void DoKeyboard(KEYEVENTF flags, KEYEVENTF dwflag)
        {
            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    type = INPUT_KEYBOARD,
                    u = new InputUnion
                    {
                        ki = new KEYBDINPUT
                        {
                            wVk = (short)flags,
                            wScan = 0,
                            dwFlags = dwflag,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero,
                        }
                    }
                }
            };

            int cbSize = Marshal.SizeOf(typeof(INPUT));
            int result = SendInput(inputs.Length, inputs, cbSize);
            if (result == 0)
                Debug.WriteLine(Marshal.GetLastWin32Error());
        }
    }
}
