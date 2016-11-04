using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace sys_passport_configurations
{
    static class Program
    {
        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int IsIconic(IntPtr hWnd);

        private static System.Threading.Mutex m;

        private static string GUI
        {
            get
            {
                Assembly a = Assembly.GetExecutingAssembly();
                var attribute = (GuidAttribute)a.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                return attribute.Value.ToString();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool InstanceRunning = false;
            long runningId = 0;

            LookForRunningProcess(out InstanceRunning, out runningId);
            if (!InstanceRunning)
            {
                m = new Mutex(true, GUI + Process.GetCurrentProcess().Id.ToString());

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (string.IsNullOrEmpty(Configurator.GetCfg(Configurator.cfgType.BaseDir)))
                    Application.Run(new Configurations());
                else
                    Application.Run(new f_sys_passport_configs());
            }
            else
            {
                GoToProcById(runningId);
            }
        }

        private static void LookForRunningProcess(out bool InstanceRunning, out long runningId)
        {
            InstanceRunning = false;
            runningId = 0;
            Process[] runningProcesses = Process.GetProcesses();
            foreach (Process p in runningProcesses)
            {
                try
                {
                    bool ok;
                    m = new System.Threading.Mutex(true, GUI + p.Id.ToString(), out ok);
                    if (!ok)
                    {
                        InstanceRunning = true;
                        runningId = p.Id;
                        break;
                    }
                    else
                    {
                        try { m.ReleaseMutex(); }
                        catch { }
                    }
                }
                catch { }
            }
        }

        private static void GoToProcById(long runningId)
        {
            IntPtr winHandle = Process.GetProcessById((int)runningId).MainWindowHandle;
            if (winHandle != IntPtr.Zero)
            {
                const int SW_RESTORE = 9;
                if (IsIconic(winHandle) != 0) ShowWindow(winHandle, SW_RESTORE);
                SetForegroundWindow(winHandle);
            }
            Environment.Exit(0);
        }
    }
}
