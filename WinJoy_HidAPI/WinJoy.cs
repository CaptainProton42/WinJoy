using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
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
            //reset gui
            labelNameL.Text = "Disconnected.";
            labelNameR.Text = "Disconnected.";

            labelBatteryL.Text = "";
            labelBatteryR.Text = "";

            labelLatL.Text = "";
            labelLatR.Text = "";

            pictureBoxL.Image = Properties.Resources.joycon_l_inactive;
            pictureBoxR.Image = Properties.Resources.joycon_r_inactive;

            groupBoxL.Enabled = false;
            groupBoxR.Enabled = false;

            //setup events
            joyconManager = new JoyconManager(this);
            scanner_L.DeviceArrived += DeviceArrived;
            scanner_L.DeviceRemoved += DeviceRemoved;
            scanner_R.DeviceArrived += DeviceArrived;
            scanner_R.DeviceRemoved += DeviceRemoved;

            scanner_L.StartAsyncScan();
            scanner_R.StartAsyncScan();

            joyconManager.DeviceInfoChanged += DeviceInfoChanged;
        }

        private void WinJoy_Shown(object sender, EventArgs e)
        {

        }

        private void DeviceArrived(object s, EventArgs a)
        {
            if (s.Equals(scanner_L))
            {
                joyconManager.joycons[0] = new JoyconDevice(ID.Vendor, ID.Joycon_L, "JoyCon (L)");
                joyconManager.joycons[0].GetDevice().InputReportArrivedEvent += (object o, ReportEventArgs e) => joyconManager.ProcessData(0, e, 0);
                joyconManager.joycons[0].GetDevice().StartAsyncRead();
                joyconManager.joycons[0].ReadColor();
            }
            else
            {
                joyconManager.joycons[1] = new JoyconDevice(ID.Vendor, ID.Joycon_R, "JoyCon (R)");
                joyconManager.joycons[1].GetDevice().InputReportArrivedEvent += (object o, ReportEventArgs e) => joyconManager.ProcessData(1, e, 0);
                joyconManager.joycons[1].GetDevice().StartAsyncRead();
                joyconManager.joycons[1].ReadColor();
            }


            BeginInvoke(new Action(delegate { UpdateInfo(joyconManager.joycons); } ));
        }

        private void DeviceRemoved(object s, EventArgs a)
        {
            if (s.Equals(scanner_L))
            {
                joyconManager.joycons[0].GetDevice().StopAsyncRead();
                joyconManager.joycons[0] = null;
            }
            else
            {
                joyconManager.joycons[1].GetDevice().StopAsyncRead();
                joyconManager.joycons[1] = null;
            }

            BeginInvoke(new Action(delegate { UpdateInfo(joyconManager.joycons); }));
        }

        private void DeviceInfoChanged(object s, EventArgs a)
        {
            Console.WriteLine("Device info changed, updating GUI...");
            BeginInvoke(new Action(delegate { UpdateInfo(joyconManager.joycons); }));
        }

        public void UpdateInfo(JoyconDevice[] joycons)
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
                labelBatteryL.Text = "";
                pictureBoxL.Image = Properties.Resources.joycon_l_inactive;
                groupBoxL.Enabled = false;
            } else
            {
                labelNameL.Text = joycons[0].Name;
                if (joyconManager.isActive) labelBatteryL.Text = "Battery: " + ((float)joycons[0].GetBattery() / 8) * 100 + " %";
                else labelBatteryL.Text = "";
                pictureBoxL.Image = Joycon_Image(0, Color.FromArgb(joycons[0].GetColor()[0], joycons[0].GetColor()[1], joycons[0].GetColor()[2]));
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
                labelBatteryR.Text = "";
                pictureBoxR.Image = Properties.Resources.joycon_r_inactive;
                groupBoxR.Enabled = false;
            } else
            {
                labelNameR.Text = joycons[1].Name;
                if (joyconManager.isActive) labelBatteryR.Text = "Battery: " + ((float)joycons[1].GetBattery() / 8) * 100 + " %";
                else labelBatteryR.Text = "";
                pictureBoxR.Image = Joycon_Image(1, Color.FromArgb(joycons[1].GetColor()[0], joycons[1].GetColor()[1], joycons[1].GetColor()[2]));
                groupBoxR.Enabled = true;
            }

            if (joycons[0] == null || joycons[1] == null || joyconManager.isActive) button2.Enabled = false;
            else button2.Enabled = true;
        }

        private Image Joycon_Image(int index, Color color)
        {
            Bitmap bmp;
            switch (index)
            {
                case 0:
                    bmp = Properties.Resources.joycon_l;
                    break;
                case 1:
                    bmp = Properties.Resources.joycon_r;
                    break;
                default:
                    bmp = Properties.Resources.joycon_c;
                    break;
            }

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    if (bmp.GetPixel(x, y) == Color.FromArgb(255, 0, 255)) bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
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
                if (joyconManager.JoinDevices()) button2.Text = "Separate";
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
            if (joyconManager.IsActive) joyconManager.Stop();

            scanner_L.StopAsyncScan();
            scanner_R.StopAsyncScan();

            if (joyconManager.joycons[0] != null) joyconManager.joycons[0].GetDevice().StopAsyncRead();
            if (joyconManager.joycons[0] != null) joyconManager.joycons[1].GetDevice().StopAsyncRead();
        }
    }
}
