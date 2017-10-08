using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBInterface;

namespace WinJoy_HidAPI
{
    public struct OutputState
    {
        public byte LX, LY, RX, RY, L2, R2;
        public bool A, B, X, Y, Start, Back, L1, R1, L3, R3, Home;
        public bool DpadUp, DpadRight, DpadDown, DpadLeft;
    }

    public static class ID
    {
        public const int Vendor = 0x057e;
        public const int Joycon_L = 0x2006;
        public const int Joycon_R = 0x2007;
    }

    public interface IDevice
    {
        string Name { get; set; }
        bool Enabled { get; set; }

        void PushReport(byte[] buf, int PID);
        int[] GetAnalogs();
        void SetAnalogs(int left_x_val, int left_y_val, int right_x_val, int right_y_val);
        void CalibrateSticks();
        byte[] GetOutput();
        USBDevice GetDevice(int index = 0);
        void SetDevice(USBDevice dev, int index = 0);
        int GetVendor(int index = 0);
        void SetVendor(int vendor, int index = 0);
        int GetProduct(int index = 0);
        void SetProduct(int product, int index = 0);
        byte GetBattery(int index = 0);
        void SetBattery(byte level, int index = 0);
        void SetLEDs(byte config, int index = 0);
    }
}
