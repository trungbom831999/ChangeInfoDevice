using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KAutoHelper;

namespace WinSubTrial
{
    public static class MyAuto
    {
        public static IntPtr ControlGetHandle(IntPtr hwnd, string control = "")
        {
            IntPtr hwndControl = AutoControl.FindHandle(hwnd, null, control);
            if (hwndControl != IntPtr.Zero) return hwndControl;
            try
            {
                return AutoItX.ControlGetHandle(hwnd, control);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlGetHandle: {ex.Message}"); return IntPtr.Zero; }
        }

        public static bool ControlClick(IntPtr hwnd, IntPtr controlHwnd, string button = "left", int numClicks = 1, int x = int.MinValue, int y = int.MinValue)
        {
            try
            {
                if (AutoControl.PostMessage(hwnd, 0xF5, IntPtr.Zero, IntPtr.Zero)) return true;

                return AutoItX.ControlClick(hwnd, controlHwnd, button, numClicks, x, y) == 1;
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlClick: {ex.Message}"); return false; }
        }

        public static bool ControlClick(string hwndTitle, string text, string controlName, string button = "left", int numClicks = 1, int x = int.MinValue, int y = int.MinValue)
        {
            try
            {
                IntPtr hwnd = AutoControl.FindWindowHandle(null, hwndTitle);
                IntPtr hwndControl = AutoControl.FindHandle(hwnd, null, controlName);
                if (AutoControl.PostMessage(hwndControl, 0xF5, IntPtr.Zero, IntPtr.Zero)) return true;

                return AutoItX.ControlClick(hwndTitle, text, controlName, button, numClicks, x, y) == 1;
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlClick: {ex.Message}"); return false; }
        }

        public static string ControlCommand(IntPtr hwnd, IntPtr controlHwnd, string command, string extra = "", int maxLen = 65535)
        {
            try
            {
                return AutoItX.ControlCommand(hwnd, controlHwnd, command, extra, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlCommand: {ex.Message}"); return string.Empty; }
        }

        public static string ControlCommand(string title, string text, string control, string command, string extra = "", int maxLen = 65535)
        {
            try
            {
                return AutoItX.ControlCommand(title, text, control, command, extra, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlCommand: {ex.Message}"); return string.Empty; }
        }

        public static string WinGetTitle(IntPtr hwnd, int maxLen = 65535)
        {
            try
            {
                return AutoItX.WinGetTitle(hwnd, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinGetTitle: {ex.Message}"); return string.Empty; }
        }

        public static string WinGetTitle(string hwndTitle, string text = "", int maxLen = 65535)
        {
            try
            {
                return AutoItX.WinGetTitle(hwndTitle, text, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinGetTitle: {ex.Message}"); return string.Empty; }
        }

        public static string ControlGetText(IntPtr handle, IntPtr hwndStatus, int maxLen = 65535)
        {
            try
            {
                return AutoItX.ControlGetText(handle, hwndStatus, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlGetText: {ex.Message}"); return ""; }
        }

        public static string ControlGetText(string handle, string text, string hwndStatus, int maxLen = 65535)
        {
            try
            {
                return AutoItX.ControlGetText(handle, text, hwndStatus, maxLen);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExControlGetText: {ex.Message}"); return ""; }
        }

        public static bool WinActivate(string hwndTitle)
        {
            try
            {
                return AutoItX.WinActivate(hwndTitle) == 1;
            }
            catch { return false; }
        }
        
        public static bool WinExists(IntPtr handle)
        {
            try
            {
                return AutoItX.WinExists(handle) == 1;
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinExists: {ex.Message}"); return false; }
        }

        public static bool WinExists(string winTitle, string text = "")
        {
            try
            {
                return AutoItX.WinExists(winTitle, text) == 1;
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinExists: {ex.Message}"); return false; }
        }

        public static IntPtr WinGetHandle(string winTitle, string text = "")
        {
            try
            {
                return AutoItX.WinGetHandle(winTitle, text);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinGetHandle: {ex.Message}"); return IntPtr.Zero; }
        }

        public static int WinMove(IntPtr hwnd, int x, int y, int width = -1, int height = -1)
        {
            try
            {
                return AutoItX.WinMove(hwnd, x, y, width, height);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinMove: {ex.Message}"); return 0; }
        }

        public static int WinMove(string hwnd, string text, int x, int y, int width = -1, int height = -1)
        {
            try
            {
                return AutoItX.WinMove(hwnd, text, x, y, width, height);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinMove: {ex.Message}"); return 0; }
        }

        public static Rectangle WinGetPos(string v, string text = "")
        {
            Rectangle rectangle = default;
            try
            {
                rectangle = AutoItX.WinGetPos(v, text);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinGetPos: {ex.Message}"); }
            return rectangle;
        }

        public static Rectangle WinGetPos(IntPtr v)
        {
            Rectangle rectangle = default;
            try
            {
                rectangle = AutoItX.WinGetPos(v);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinGetPos: {ex.Message}"); }
            return rectangle;
        }

        public static int WinSetTitle(IntPtr handle, string winTitle)
        {
            try
            {
                return AutoItX.WinSetTitle(handle, winTitle);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinSetTitle: {ex.Message}"); return 0; }
        }

        public static int WinSetTitle(string handleTitle, string text, string winTitle)
        {
            try
            {
                return AutoItX.WinSetTitle(handleTitle, text, winTitle);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinSetTitle: {ex.Message}"); return 0; }
        }

        public static int WinClose(string v, string text = "")
        {
            try
            {
                return AutoItX.WinClose(v, text);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinClose: {ex.Message}"); return 0; }
        }

        public static int WinClose(IntPtr v)
        {
            try
            {
                return AutoItX.WinClose(v);
            }
            catch (Exception ex) { Utils.Debug.Log($"MyAutoExWinClose: {ex.Message}"); return 0; }
        }
    }
}
