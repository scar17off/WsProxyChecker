using System;
using System.Runtime.InteropServices;

namespace WsProxyChecker
{
    internal static class NativeWinAPI
    {
        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        
        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();
    }
} 