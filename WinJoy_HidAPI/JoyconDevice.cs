using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

    public class JoyconDevice
    {
        //properties that stay the same for every device
        public byte[] mapping = new byte[50];
        public OutputState xOutput;
        public string Name { get; set; }
        public bool Enabled { get; set; }

        private byte[] Color = new byte[3];
        private int[] Center = new int[4];
        private int[] Analogs = new int[4]; //LX, LY, RX, RY
        private bool[] Buttons = new bool[24];  //Y, X, B, A, SR_right, SL_right, R, ZR, Minus, Plus, R3, L3, Home, Capture, -, -, Down, Up, Right, Left, SR_left, SL_left, L, ZL
        private byte[] report = new byte[65];

        //properties that are implemented differently for different devices
        private USBDevice Device;
        private int VendorID;
        private int ProductID;
        private byte Battery; //0: empty, 2: critical, 4: low, 6: medium, 8: full

        delegate byte input(byte subType, byte num);

        public JoyconDevice(ushort VID, ushort PID, string name)
        {
            Enabled = true;
            Device = new USBDevice(VID, PID, null, true, 65);
            VendorID = VID;
            ProductID = PID;
            Name = name;

            //temporary standard mapping
            for (int i = 0; i < 17; i++)
            {
                mapping[i * 2] = (byte)0 << 4;
            }
            mapping[1] = 0;
            mapping[3] = 1;
            mapping[5] = 2;
            mapping[7] = 3;
            mapping[9] = 6;
            mapping[11] = 7;
            mapping[13] = 8;
            mapping[15] = 9;
            mapping[17] = 10;
            mapping[19] = 11;
            mapping[21] = 12;
            mapping[23] = 16;   //dpad down
            mapping[25] = 17;
            mapping[27] = 18;
            mapping[29] = 19;
            mapping[31] = 22;
            mapping[33] = 23;

            for (int i = 17; i < 21; i++)
            {
                mapping[i * 2] = (1 << 4) | 0;
                mapping[(i * 2) + 1] = (byte)(i - 17);
            }
            for (int i = 21; i < 25; i++)
            {
                mapping[i * 2] = (1 << 4) | 1;
                mapping[(i * 2) + 1] = (byte)(i - 21);
            }
        }

        public void PushReport(byte[] buf, int PID)
        {
            report = buf;
        }

        public int[] GetAnalogs()
        {
            return (int[])Analogs.Clone();
        }

        public void SetAnalogs(int left_x_value, int left_y_value, int right_x_value, int right_y_value)
        {
            Analogs[0] = left_x_value;
            Analogs[1] = left_y_value;
            Analogs[2] = right_x_value;
            Analogs[3] = right_y_value;
        }

        public USBDevice GetDevice()
        {
            return Device;
        }

        public void SetDevice(USBDevice dev)
        {
            Device = dev;
        }

        public int GetVendor()
        {
            return VendorID;
        }

        public void SetVendor(int vendor)
        {
            VendorID = vendor;
        }

        public int GetProduct()
        {
            return ProductID;
        }

        public void SetProduct(int product)
        {
            ProductID = product;
        }

        public byte GetBattery()
        {
            return Battery;
        }

        public void SetBattery(byte level)
        {
            Battery = level;
        }

        public void SetLEDs(byte config)
        {
            byte[] buf = new byte[65];
            buf[0] = 0x01;
            buf[10] = 0x30; //LED subcommand
            buf[11] = config;
            try
            {
                Device.Write(buf);
            }
            catch { }
        }

        public void SetColor(byte color, int rgb)
        {
            Color[rgb] = color;
        }

        public byte[] GetColor()
        {
            return Color;
        }

        public void ReadColor() //offsets in SPI data seem to vary depending on current input
        {
            Thread.Sleep(500);      //give the joycon some time to give the correct reply
            byte[] buf = new byte[0x100];
            buf[0] = 0x01;  //send subcommand
            buf[10] = 0x10; //read SPI subcommand
            buf[11] = 0x50; //SPI adress 0x6050 for R
            buf[12] = 0x60;
            buf[15] = 0x18;

            Device.Write(buf);

            buf = new byte[0x100];
            buf[0] = 0x01;  //send subcommand
            buf[10] = 0x10; //read SPI subcommand
            buf[11] = 0x51; //SPI adress 0x6050 for G
            buf[12] = 0x60;
            buf[15] = 0x18;

            Device.Write(buf);

            buf = new byte[0x100];
            buf[0] = 0x01;  //send subcommand
            buf[10] = 0x10; //read SPI subcommand
            buf[11] = 0x52; //SPI adress 0x6052 for B
            buf[12] = 0x60;
            buf[15] = 0x18;

            Device.Write(buf);
        }

        public void CalibrateSticks()
        {
            try
            {
                if (report[0] == 33 || report[0] == 48)
                {
                    switch (ProductID)
                    {
                        case ID.Joycon_L:
                            Center[0] = report[6] | ((report[7] & 0xF) << 8);   //horizontal left
                            Center[1] = (report[2] >> 4) | (report[8] << 4);    //vertical left
                            break;
                        case ID.Joycon_R:
                            Center[2] = report[9] | ((report[10] & 0xF) << 8);   //horizontal right
                            Center[3] = (report[5] >> 4) | (report[11] << 4);    //vertical right
                            break;
                    }
                }
            }
            catch { }
        }


        public byte[] GetOutput()
        {
            UpdateInput();

            byte[] Report = new byte[64];
            Report[1] = 0x02;
            Report[2] = 0x05;
            Report[3] = 0x12;

            Report[10] = (byte)(
                ((xOutput.Back ? 1 : 0) << 0) |
                ((xOutput.L3 ? 1 : 0) << 1) |
                ((xOutput.R3 ? 1 : 0) << 2) |
                ((xOutput.Start ? 1 : 0) << 3) |
                ((xOutput.DpadUp ? 1 : 0) << 4) |
                ((xOutput.DpadRight ? 1 : 0) << 5) |
                ((xOutput.DpadDown ? 1 : 0) << 6) |
                ((xOutput.DpadLeft ? 1 : 0) << 7));

            Report[11] = (byte)(
                ((xOutput.L1 ? 1 : 0) << 2) |
                ((xOutput.R1 ? 1 : 0) << 3) |
                ((xOutput.Y ? 1 : 0) << 4) |
                ((xOutput.B ? 1 : 0) << 5) |
                ((xOutput.A ? 1 : 0) << 6) |
                ((xOutput.X ? 1 : 0) << 7));

            //Guide
            Report[12] = (byte)(xOutput.Home ? 0xFF : 0x00);


            Report[14] = xOutput.LX; //Left Stick X


            Report[15] = xOutput.LY; //Left Stick Y


            Report[16] = xOutput.RX; //Right Stick X


            Report[17] = xOutput.RY; //Right Stick Y

            Report[26] = xOutput.R2;
            Report[27] = xOutput.L2;

            return Report;
        }

        byte Button(byte subType, byte num)
        {
            int i = Convert.ToByte(Buttons[num]) * 127;
            return (byte)i;
        }

        byte Analog(byte subType, byte num)
        {
            int p = Analogs[num];
            int c = Center[num];
            int m;
            //Console.WriteLine("{0}: {1}", num, p-c);
            switch (subType)
            {
                case 0: //up
                    m = ((p - c) * 127) / 1000;
                    if (m < 0)
                    {
                        m = 0;
                    }
                    else if (m > 127)
                    {
                        m = 127;
                    }
                    return (byte)m;
                case 1: //down
                    m = ((p - c) * 127) / 1000;
                    if (-m < 0)
                    {
                        m = 0;
                    }
                    else if (-m > 127)
                    {
                        m = -127;
                    }
                    return (byte)-m;
            }
            return 0;
        }

        private void UpdateInput()
        {

            try
            {
                if (report[0] == 0x30)
                {
                    switch (ProductID)
                    {
                        case ID.Joycon_L:
                            Analogs[0] = report[6] | ((report[7] & 0xF) << 8);   //horizontal left
                            Analogs[1] = (report[2] >> 4) | (report[8] << 4);    //vertical left
                            UpdateButtons(report[5]);
                            UpdateSButtons(report[4]);
                            break;
                        case ID.Joycon_R:
                            Analogs[2] = report[9] | ((report[10] & 0xF) << 8);   //horizontal right
                            Analogs[3] = (report[5] >> 4) | (report[11] << 4);    //vertical right
                            UpdateButtons(report[3]);
                            UpdateSButtons(report[4]);
                            break;
                    }
                    Battery = (byte)((report[2] & 0xF0) >> 4);
                }
            }
            catch { }

            input funcButton = Button;
            input funcAnalog = Analog;

            input[] funcArray = new input[] { funcButton, funcAnalog };

            byte[] output = new byte[25];
            for (int i = 0; i < 25; i++)
            {
                byte subtype = (byte)(mapping[i * 2] & 0x0F);
                byte type = (byte)((mapping[i * 2] & 0xF0) >> 4);
                byte num = mapping[(i * 2) + 1];
                output[i] = funcArray[type](subtype, num);
            }

            xOutput.Y = output[0] != 0;
            xOutput.X = output[1] != 0;
            xOutput.B = output[2] != 0;
            xOutput.A = output[3] != 0;

            xOutput.R1 = output[4] != 0;
            xOutput.R2 = (byte)(output[5] * 2);

            xOutput.Back = output[6] != 0;
            xOutput.Start = output[7] != 0;

            xOutput.R3 = output[8] != 0;
            xOutput.L3 = output[9] != 0;

            xOutput.Home = output[10] != 0; ;

            xOutput.DpadDown = output[11] != 0;
            xOutput.DpadUp = output[12] != 0;
            xOutput.DpadRight = output[13] != 0;
            xOutput.DpadLeft = output[14] != 0;

            xOutput.L1 = output[15] != 0;
            xOutput.L2 = (byte)(output[16] * 2);

            xOutput.LX = (byte)(127 + output[17] - output[21]);
            xOutput.LY = (byte)(127 - output[18] + output[22]); //for some reason inverted
            xOutput.RX = (byte)(127 + output[19] - output[23]);
            xOutput.RY = (byte)(127 - output[20] + output[24]);
        }

        private void UpdateButtons(byte data)
        {
            int offset = new int();
            switch (ProductID)
            {
                case ID.Joycon_L:
                    offset = 16;
                    break;
                case ID.Joycon_R:
                    offset = 0;
                    break;
            }
            for (int i = 0; i < 8; i++)
            {
                Buttons[i + offset] = ((data >> i) & 0x01) != 0;
            }
        }
        private void UpdateSButtons(byte data)
        {
            switch (ProductID)
            {
                case ID.Joycon_L:
                    Buttons[8] = (data & 0x01) != 0;
                    Buttons[11] = ((data >> 3) & 0x01) != 0;
                    Buttons[13] = ((data >> 5) & 0x01) != 0;
                    break;
                case ID.Joycon_R:
                    Buttons[9] = ((data >> 1) & 0x01) != 0;
                    Buttons[10] = ((data >> 2) & 0x01) != 0;
                    Buttons[12] = ((data >> 4) & 0x01) != 0;
                    break;
            }
        }
    }
}
