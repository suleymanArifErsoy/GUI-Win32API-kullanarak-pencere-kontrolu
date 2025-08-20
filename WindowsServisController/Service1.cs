using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace WindowsServisController
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private readonly string logFilePath = @"C:\ActiveWindowServiceLog.txt";
        private readonly string exePath = @"C:\Users\user\source\repos\WindowsServisController\WindowsControllerConseleApp\bin\Debug\WindowsControllerConseleApp.exe";

        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "ActiveWindowService";
        }

        protected override void OnStart(string[] args)
        {
            Log("Servis başlatıldı.");
            timer = new Timer(KonsolUygulamasiniBaslat, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private void KonsolUygulamasiniBaslat(object state)
        {
            if (!File.Exists(exePath))
            {
                Log($"Exe bulunamadı: {exePath}");
                return;
            }

            bool result = ProcessBaslatici.ProcessBaslat(exePath);

            if (result)
                Log("Konsol uygulaması GUI oturumunda çalıştırıldı.");
            else
                Log("Konsol uygulaması çalıştırılamadı (CreateProcessAsUser).");
        }

        protected override void OnStop()
        {
            Log("Servis durduruluyor...");
            timer?.Dispose();
            Log("Servis durduruldu.");
        }

        private void Log(string message)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch { /* Log hatası loglamaya çalışılmaz */ }
        }
    }

    public static class ProcessBaslatici
    {
        [DllImport("kernel32.dll")]
        private static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("Wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out IntPtr token);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            uint dwDesiredAccess,
            IntPtr lpTokenAttributes,
            int impersonationLevel,
            int tokenType,
            out IntPtr phNewToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            int dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        private const int TOKEN_ALL_ACCESS = 0xF01FF;
        private const int SecurityImpersonation = 2;
        private const int TokenPrimary = 1;
        private const int CREATE_NEW_CONSOLE = 0x00000010;

        public static bool ProcessBaslat(string appPath)
        {
            try
            {
                uint sessionId = WTSGetActiveConsoleSessionId();

                if (!WTSQueryUserToken(sessionId, out IntPtr userToken))
                    return false;

                if (!DuplicateTokenEx(
                    userToken,
                    TOKEN_ALL_ACCESS,
                    IntPtr.Zero,
                    SecurityImpersonation,
                    TokenPrimary,
                    out IntPtr primaryToken))
                    return false;

                STARTUPINFO si = new STARTUPINFO();
                si.cb = Marshal.SizeOf(typeof(STARTUPINFO));
                si.lpDesktop = @"winsta0\default";

                PROCESS_INFORMATION pi;

                bool result = CreateProcessAsUser(
                    primaryToken,
                    null,
                    $"\"{appPath}\"",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    CREATE_NEW_CONSOLE,
                    IntPtr.Zero,
                    null,
                    ref si,
                    out pi);

                return result;
            }
            catch
            {
                return false;
            }
        }
    }
}
