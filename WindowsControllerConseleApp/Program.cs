using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    const int SW_MINIMIZE = 6;
    const int SW_MAXIMIZE = 3;
    const uint WM_CLOSE = 0x0010;

    static void Main()
    {
        IntPtr hWnd = GetForegroundWindow();

        if (hWnd != IntPtr.Zero)
        {
            StringBuilder buff = new StringBuilder(256);
            GetWindowText(hWnd, buff, 256);
            Console.WriteLine("Aktif pencere: " + buff.ToString());

            Console.WriteLine("Küçültülüyor...");
            ShowWindow(hWnd, SW_MINIMIZE);
            Thread.Sleep(1000);

            Console.WriteLine("Büyütülüyor...");
            ShowWindow(hWnd, SW_MAXIMIZE);
            Thread.Sleep(1000);

            Console.WriteLine("Kapatılıyor...");
            PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
        else
        {
            Console.WriteLine("Aktif pencere bulunamadı.");
        }
    }
}
