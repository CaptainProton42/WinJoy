using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBInterface;

namespace WinJoy_HidAPI
{
    class JoinedDevice : IDevice
    {
        //properties that stay the same for every device
        public byte[] mapping = new byte[50];
        public OutputState xOutput;
        public string Name { get; set; }
        public bool Enabled { get; set; }

        private int[] Center = new int[4];
        private int[] Analogs = new int[4]; //LX, LY, RX, RY
        private bool[] Buttons = new bool[24];  //Y, X, B, A, SR_right, SL_right, R, ZR, Minus, Plus, R3, L3, Home, Capture, -, -, Down, Up, Right, Left, SR_left, SL_left, L, ZL
        private byte[][] report = new byte[2][];

        //properties that are implemented differently for different devices, index 0 is left, 1 is right
        private USBDevice[] Device = new USBDevice[2];
        private int[] VendorID = new int[2];
        private int[] ProductID = new int[2];
        private byte[] Battery = new byte[2]; //0: empty, 2: critical, 4: low, 6: medium, 8: full

        delegate byte input(byte subType, byte num);

        public JoinedDevice()
        {
            Enabled = true;
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

        void IDevice.PushReport(byte[] buf, int PID)
        {
            switch (PID)
            {
                case ID.Joycon_L:
                    report[0] = buf;
                    break;
                case ID.Joycon_R:
                    report[1] = buf;
                    break;
            }
        }

        int[] IDevice.GetAnalogs()
        {
            return (int[])Analogs.Clone();
        }
        void IDevice.SetAnalogs(int left_x_value, int left_y_value, int right_x_value, int right_y_value)
        {
            Analogs[0] = left_x_value;
            Analogs[1] = left_y_value;
            Analogs[2] = right_x_value;
            Analogs[3] = right_y_value;
        }

        USBDevice IDevice.GetDevice(int index)
        {
            return Device[index];
        }

        void IDevice.SetDevice(USBDevice dev, int index)
        {
            Device[index] = dev;
        }

        int IDevice.GetVendor(int index)
        {
            return VendorID[index];
        }

        void IDevice.SetVendor(int vendor, int index)
        {
            VendorID[index] = vendor;
        }

        int IDevice.GetProduct(int index)
        {
            return ProductID[index];
        }

        void IDevice.SetProduct(int product, int index)
        {
            ProductID[index] = product;
        }

        byte IDevice.GetBattery(int index)
        {
            return Battery[index];
        }

        void IDevice.SetBattery(byte level, int index)
        {
            Battery[index] = level;
        }

        void IDevice.SetLEDs(byte config, int index)
        {
            byte[] buf = new byte[65];
            buf[0] = 0x01;
            buf[10] = 0x30; //LED subcommand
            buf[11] = config;
            Device[index].Write(buf);
        }

        void IDevice.CalibrateSticks()
        {
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    if (report[i][0] == 33 || report[i][0] == 48)
                    {
                        switch (ProductID[i])
                        {
                            case ID.Joycon_L:
                                Center[0] = report[i][6] | ((report[i][7] & 0xF) << 8);   //horizontal left
                                Center[1] = (report[i][2] >> 4) | (report[i][8] << 4);    //vertical left
                                break;
                            case ID.Joycon_R:
                                Center[2] = report[i][9] | ((report[i][10] & 0xF) << 8);   //horizontal right
                                Center[3] = (report[i][5] >> 4) | (report[i][11] << 4);    //vertical right
                                break;
                        }
                    }
                }
            }
            catch { }
        }


        byte[] IDevice.GetOutput()
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
                for (int i = 0; i < 2; i++)
                {
                    if (report[i][0] == 0x30)
                    {
                        switch (ProductID[i])
                        {
                            case ID.Joycon_L:
                                Analogs[0] = report[i][6] | ((report[i][7] & 0xF) << 8);   //horizontal left
                                Analogs[1] = (report[i][2] >> 4) | (report[i][8] << 4);    //vertical left
                                UpdateButtons(report[i][5], i);
                                UpdateSButtons(report[i][4], i);
                                break;
                            case ID.Joycon_R:
                                Analogs[2] = report[i][9] | ((report[i][10] & 0xF) << 8);   //horizontal right
                                Analogs[3] = (report[i][5] >> 4) | (report[i][11] << 4);    //vertical right
                                UpdateButtons(report[i][3], i);
                                UpdateSButtons(report[i][4], i);
                                break;
                        }
                        Battery[i] = (byte)((report[i][2] & 0xF0) >> 4);
                    }
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
            xOutput.R2 = (byte)(output[16] * 2);    //also twisted for some reason

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
            xOutput.L2 = (byte)(output[5] * 2);

            xOutput.LX = (byte)(127 + output[17] - output[21]);
            xOutput.LY = (byte)(127 - output[18] + output[22]); //for some reason inverted
            xOutput.RX = (byte)(127 + output[19] - output[23]);
            xOutput.RY = (byte)(127 - output[20] + output[24]);
        }

        private void UpdateButtons(byte data, int index)
        {
            int offset = new int();
            switch (ProductID[index])
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
        private void UpdateSButtons(byte data, int index)
        {
            switch (ProductID[index])
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

        byte[] IDevice.GetColor()
        {
            return new byte[3];
        }
    }
}
