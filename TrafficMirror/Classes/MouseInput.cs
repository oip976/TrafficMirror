using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using static NativeMethods;

namespace TrafficMirror.Classes
{
    public class MouseInput
    {
        public static void LeftClick()
        {
            DoMouse(MOUSEEVENTF.LEFTDOWN, new Point(0, 0));
            DoMouse(MOUSEEVENTF.LEFTUP, new Point(0, 0));
        }

        public static void LeftClick(int x, int y)
        {
            DoMouse(MOUSEEVENTF.LEFTDOWN, new Point(x, y));
            DoMouse(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE, new Point(x, y));
            DoMouse(MOUSEEVENTF.LEFTUP, new Point(x, y));
        }

        public static void ClickBoundingRectangleByPercentage(int xPercentage, int yPercentage, Rectangle bounds)
        {
            double additional = 0.0;
            if (xPercentage == 99)
                additional = 0.5;
            int xPixel = Convert.ToInt32(bounds.Left + bounds.Width * (xPercentage + additional) / 100);
            int yPixel = Convert.ToInt32(bounds.Top + bounds.Height * (yPercentage) / 100);
            LeftClick(xPixel, yPixel);
        }

        public static void RightClick()
        {
            DoMouse(MOUSEEVENTF.RIGHTDOWN, new Point(0, 0));
            DoMouse(MOUSEEVENTF.RIGHTUP, new Point(0, 0));
        }

        public static void RightClick(int x, int y)
        {
            DoMouse(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE, new Point(x, y));
            DoMouse(MOUSEEVENTF.RIGHTDOWN, new Point(x, y));
            DoMouse(MOUSEEVENTF.RIGHTUP, new Point(x, y));
        }

        public static void MoveMouse(Point p)
        {
            MoveMouse(p.X, p.Y);
        }

        public static void MoveMouse(System.Windows.Point p)
        {
            MoveMouse(Convert.ToInt32(p.X), Convert.ToInt32(p.Y));
        }

        public static void MoveMouse(int x, int y)
        {
            DoMouse(MOUSEEVENTF.MOVE | MOUSEEVENTF.ABSOLUTE, new Point(x, y));
        }

        public static void SetCursorPosition(Point newPoint)
        {
            Cursor.Position = newPoint;
        }

        public static void ScrollWheel(int scrollSize)
        {
            DoMouse(MOUSEEVENTF.WHEEL, new Point(0, 0), scrollSize);
        }

        private static void DoMouse(MOUSEEVENTF flags, Point newPoint, int scrollSize = 0)
        {
            INPUT[] inputs = new INPUT[]
            {
                new INPUT
                {
                    type = INPUT_MOUSE,
                    u = new InputUnion
                    {
                        mi = new MOUSEINPUT
                        {
                            // mouse co-ords: top left is (0,0), bottom right is (65535, 65535)
                            // convert screen co-ord to mouse co-ords...
                            dx = newPoint.X * 65535 / Screen.PrimaryScreen.Bounds.Width,
                            dy = newPoint.Y * 65535 / Screen.PrimaryScreen.Bounds.Height,
                            mouseData = scrollSize * 120,
                            dwFlags = flags,
                            time = 0,
                            dwExtraInfo = IntPtr.Zero,
                        }
                    }
                }
            };

            
            int cbSize = Marshal.SizeOf(typeof(INPUT));
            int result = SendInput(1, inputs, cbSize);
            if (result == 0)
                Debug.WriteLine(Marshal.GetLastWin32Error());
        }
    }
}