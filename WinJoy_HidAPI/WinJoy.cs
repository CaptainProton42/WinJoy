using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using USBInterface;

namespace WinJoy_HidAPI
{
    public partial class WinJoy : Form
    {
        private JoyconManager joyconManager;

        DeviceScanner scanner_L = new DeviceScanner(ID.Vendor, ID.Joycon_L);
        DeviceScanner scanner_R = new DeviceScanner(ID.Vendor, ID.Joycon_R);

        public WinJoy()
        {
            InitializeComponent();
        }

        private void WinJoy_Load(object sender, EventArgs e)
        {
            joyconManager = new JoyconManager(this);
            scanner_L.DeviceArrived += DeviceArrived;
            scanner_L.DeviceRemoved += DeviceRemoved;
            scanner_R.DeviceArrived += DeviceArrived;
            scanner_R.DeviceRemoved += DeviceRemoved;

            scanner_L.StartAsyncScan();
            scanner_R.StartAsyncScan();
        }

        private void WinJoy_Shown(object sender, EventArgs e)
        {

        }

        private void DeviceArrived(object s, EventArgs a)
        {
            if (s.Equals(scanner_L)) joyconManager.joycons[0] = new JoyconDevice(ID.Vendor, ID.Joycon_L, "JoyCon (L)");
            else joyconManager.joycons[1] = new JoyconDevice(ID.Vendor, ID.Joycon_R, "JoyCon (R)");

            BeginInvoke(new Action(delegate { UpdateInfo(joyconManager.joycons); } ));
        }

        private void DeviceRemoved(object s, EventArgs a)
        {
            if (s.Equals(scanner_L)) joyconManager.joycons[0] = null;
            else joyconManager.joycons[1] = null;

            BeginInvoke(new Action(delegate { UpdateInfo(joyconManager.joycons); }));
        }

        private void UpdateInfo(IDevice[] joycons)
        {
            if (joycons[0] == null)
            {
                labelNameL.Text = "Disconnected.";
                labelBatteryL.Text = "";
                pictureBoxL.Image = Properties.Resources.joycon_l_inactive;
                groupBoxL.Enabled = false;
            } else if (!joycons[0].Enabled)
            {
                labelNameL.Text = "Disabled.";
                labelBatteryL.Text = "Battery: " + joycons[0].GetBattery() / 8 * 100 + " %";
                pictureBoxL.Image = Properties.Resources.joycon_l_inactive;
                groupBoxL.Enabled = false;
            } else
            {
                labelNameL.Text = joycons[0].Name;
                labelBatteryL.Text = "Battery: " + joycons[0].GetBattery() / 8 * 100 + " %";
                pictureBoxL.Image = Properties.Resources.joycon_l;
                groupBoxL.Enabled = true;
            }

            if (joycons[1] == null)
            {
                labelNameR.Text = "Disconnected.";
                labelBatteryR.Text = "";
                pictureBoxR.Image = Properties.Resources.joycon_r_inactive;
                groupBoxR.Enabled = false;
            }
            else if (!joycons[1].Enabled)
            {
                labelNameR.Text = "Disabled.";
                labelBatteryR.Text = "Battery: " + joycons[1].GetBattery() / 8 * 100 + " %";
                pictureBoxR.Image = Properties.Resources.joycon_r_inactive;
                groupBoxR.Enabled = false;
            } else
            {
                labelNameR.Text = joycons[1].Name;
                labelBatteryR.Text = "Battery: " + joycons[1].GetBattery() / 8 * 100 + " %";
                pictureBoxR.Image = Properties.Resources.joycon_r;
                groupBoxR.Enabled = true;
            }

            if (joycons[2] == null)
            {
                labelNameC.Text = "Disconnected.";
                pictureBoxC.Image = Properties.Resources.joycon_c_inactive;
                groupBoxC.Enabled = false;
            }
            else if (!joycons[2].Enabled)
            {
                labelNameC.Text = "Disabled.";
                pictureBoxC.Image = Properties.Resources.joycon_c_inactive;
                groupBoxC.Enabled = false;
            } else
            {
                labelNameC.Text = joycons[2].Name;
                pictureBoxC.Image = Properties.Resources.joycon_c;
                groupBoxC.Enabled = true;
            }

            if (joycons[0] == null || joycons[1] == null || joyconManager.isActive) button2.Enabled = false;
            else button2.Enabled = true;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (buttonStart.Text == "Start")
            {
                joyconManager.Start();
                buttonStart.Text = "Stop";
                button2.Enabled = false;
            }
            else
            {
                joyconManager.Stop();
                buttonStart.Text = "Start";
                button2.Enabled = true;
            }
        }

        private void buttonCal_Click(object sender, EventArgs e)
        {
            joyconManager.Calibrate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text == "Combine")
            {
                joyconManager.JoinDevices();
                UpdateInfo(joyconManager.joycons);
                button2.Text = "Separate";
            } else if (button2.Text == "Separate")
            {
                joyconManager.UnjoinDevices();
                UpdateInfo(joyconManager.joycons);
                button2.Text = "Combine";
            }
        }

        private void buttonCalL_Click(object sender, EventArgs e)
        {
            joyconManager.joycons[0].CalibrateSticks();
        }

        private void buttonCalR_Click(object sender, EventArgs e)
        {
            joyconManager.joycons[1].CalibrateSticks();
        }

        private void buttonCalC_Click(object sender, EventArgs e)
        {
            joyconManager.joycons[2].CalibrateSticks();
        }

        private void WinJoy_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (joyconManager.IsActive)
            {
                joyconManager.Stop();
            }
            scanner_L.StopAsyncScan();
            scanner_R.StopAsyncScan();
        }
    }
}
