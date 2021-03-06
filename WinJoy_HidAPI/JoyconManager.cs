﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using USBInterface;

namespace WinJoy_HidAPI
{
    class JoyconManager : ScpDevice
    {
        private class ContData
        {
            public byte[] parsedData = new byte[28];
            public byte[] output = new byte[8];
        }

        public event EventHandler DeviceInfoChanged;

        public JoyconDevice[] joycons = new JoyconDevice[2];
        public bool isActive = new bool();
        public bool joined = new bool();

        public const String BUS_CLASS_GUID = "{F679F562-3164-42CE-A4DB-E7DDBE723909}";
        private Control handle;
        private ContData[] processingData = new ContData[3];

        public JoyconManager(Control _handle)
            : base(BUS_CLASS_GUID)
        {
            isActive = false;
            joined = false;
            handle = _handle;
            processingData[0] = new ContData();
            processingData[1] = new ContData();
            processingData[2] = new ContData();
        }

        public override Boolean Open(int Instance = 0)
        {
            return base.Open(Instance);
        }

        public override Boolean Open(String DevicePath)
        {
            m_Path = DevicePath;
            m_WinUsbHandle = (IntPtr)INVALID_HANDLE_VALUE;

            if (GetDeviceHandle(m_Path))
            {

                m_IsActive = true;

            }
            return true;
        }

        public override bool Start()
        {
            Open();

            byte[] buf = new byte[65];
            buf[0] = 0x01;
            buf[10] = 0x03;
            buf[11] = 0x30; //set device to standard 

            if (!joined)
            {
                foreach (int i in Enumerable.Range(0, 2))
                {
                    if (joycons[i] != null)
                    {
                        Plugin(i + 1);
                        joycons[i].GetDevice().Write(buf);
                    }
                }
            } else
            {
                Plugin(1);
                foreach (int i in Enumerable.Range(0, 2))
                {
                    if (joycons[i] != null)
                    {
                        joycons[i].GetDevice().Write(buf);
                    }
                }
            }
            isActive = true;
            return isActive;
        }

        public override bool Stop()
        {
            for (int i = 0; i < 2; i++)
            {
                if (joycons[i] != null)
                {
                    Unplug(i + 1);
                    byte[] buf = new byte[65];
                    buf[0] = 0x01;
                    buf[10] = 0x03;
                    buf[11] = 0x3F; //set device to inactive mode

                    joycons[i].GetDevice().Write(buf);
                }
            }
            isActive = false;
            return base.Stop();
        }

        public void Calibrate()
        {
            for (int i = 0; i < 3; i++)
            {
                if (joycons[i] != null) joycons[i].CalibrateSticks();
            }
        }

        public bool JoinDevices()
        {
            if (joycons[0] != null && joycons[1] != null && !isActive)
            {
                joined = true;
            }
            return joined;
        }

        public void UnjoinDevices()
        {
            if (!isActive)
            {
                joined = false;
            }
        }

        public Boolean Plugin(Int32 Serial)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (Byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (Byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (Byte)((Serial >> 24) & 0xFF);

                return DeviceIoControl(m_FileHandle, 0x2A4000, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }

            return false;
        }

        public Boolean Unplug(Int32 Serial)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;
                Byte[] Buffer = new Byte[16];

                Buffer[0] = 0x10;
                Buffer[1] = 0x00;
                Buffer[2] = 0x00;
                Buffer[3] = 0x00;

                Buffer[4] = (Byte)((Serial >> 0) & 0xFF);
                Buffer[5] = (Byte)((Serial >> 8) & 0xFF);
                Buffer[6] = (Byte)((Serial >> 16) & 0xFF);
                Buffer[7] = (Byte)((Serial >> 24) & 0xFF);

                return DeviceIoControl(m_FileHandle, 0x2A4004, Buffer, Buffer.Length, null, 0, ref Transfered, IntPtr.Zero);
            }
            return false;
        }

        public void ProcessData(int index, ReportEventArgs a, int subindex)
        {
            switch (a.Data[0])
            {
                case 0x30:
                    if (isActive)
                    {
                        int oldBattery = joycons[index].GetBattery();
                        joycons[index].PushReport(a.Data, joycons[index].GetProduct());
                        byte[] data = joycons[index].GetOutput();
                        if (data != null)
                        {
                            data[0] = (byte)index;
                            Parse(data, processingData[index].parsedData);  //input to output
                            if (!joined)
                            {
                                Report(processingData[index].parsedData, processingData[index].output);
                            }
                            else
                            {
                                data[0] = (byte)0;
                                Parse(data, processingData[2].parsedData);
                                // combine processingData 0 + 1 and Report.
                                for (int i = 10; i < 14; i++) processingData[2].parsedData[i] = (byte)(processingData[0].parsedData[i] | processingData[1].parsedData[i]); //buttons
                                for (int i = 14; i < 18; i++) processingData[2].parsedData[i] = processingData[0].parsedData[i];    //left stick
                                for (int i = 18; i < 22; i++) processingData[2].parsedData[i] = processingData[1].parsedData[i];    //right stick
                                Report(processingData[2].parsedData, processingData[2].output);
                            }
                            if (joycons[index].GetBattery() != oldBattery)
                            {
                                SetBatteryLED(joycons[index], subindex, joycons[index].GetBattery());
                                DeviceInfoChanged(this, new EventArgs());
                            }
                        }
                    }
                    break;

                case 0x21:
                    if (a.Data[14] == 0x10)
                    {
                        if (a.Data[15] == 0x50 && a.Data[16] == 0x60) joycons[index].SetColor(a.Data[20], 0);
                        else if (a.Data[15] == 0x51 && a.Data[16] == 0x60) joycons[index].SetColor(a.Data[20], 1);
                        else if (a.Data[15] == 0x52 && a.Data[16] == 0x60) joycons[index].SetColor(a.Data[20], 2);
                        else break;
                        DeviceInfoChanged(this, new EventArgs());
                    }
                    break;
            }
        }

        private Int32 Scale(Int32 Value, Boolean Flip)  //returns the actual value to be sent to the device
        {
            Value -= 0x80;

            if (Value == -128) Value = -127;
            if (Flip) Value *= -1;

            return (Int32)((float)Value * 258.00787401574803149606299212599f);
        }

        public Boolean Report(Byte[] Input, Byte[] Output)
        {
            if (IsActive)
            {
                Int32 Transfered = 0;


                bool result = DeviceIoControl(m_FileHandle, 0x2A400C, Input, Input.Length, Output, Output.Length, ref Transfered, IntPtr.Zero) && Transfered > 0;   //send the actual data to the XInput device
                int deviceInd = Input[4] - 1;
                return true;

            }
            return false;
        }

        public void Parse(Byte[] Input, Byte[] Output)
        {
            Byte Serial = (Byte)(Input[0] + 1);

            for (Int32 Index = 0; Index < 28; Index++) Output[Index] = 0x00;

            Output[0] = 0x1C;
            Output[4] = (Byte)(Input[0] + 1);
            Output[9] = 0x14;

            if (true)//Input[1] == 0x02) // Pad is active
            {

                UInt32 Buttons = (UInt32)((Input[10] << 0) | (Input[11] << 8) | (Input[12] << 16) | (Input[13] << 24));

                if ((Buttons & (0x1 << 0)) > 0) Output[10] |= (Byte)(1 << 5); // Back
                if ((Buttons & (0x1 << 1)) > 0) Output[10] |= (Byte)(1 << 6); // Left  Thumb
                if ((Buttons & (0x1 << 2)) > 0) Output[10] |= (Byte)(1 << 7); // Right Thumb
                if ((Buttons & (0x1 << 3)) > 0) Output[10] |= (Byte)(1 << 4); // Start

                if ((Buttons & (0x1 << 4)) > 0) Output[10] |= (Byte)(1 << 0); // Up
                if ((Buttons & (0x1 << 5)) > 0) Output[10] |= (Byte)(1 << 3); // Down
                if ((Buttons & (0x1 << 6)) > 0) Output[10] |= (Byte)(1 << 1); // Right
                if ((Buttons & (0x1 << 7)) > 0) Output[10] |= (Byte)(1 << 2); // Left

                if ((Buttons & (0x1 << 10)) > 0) Output[11] |= (Byte)(1 << 0); // Left  Shoulder
                if ((Buttons & (0x1 << 11)) > 0) Output[11] |= (Byte)(1 << 1); // Right Shoulder

                if ((Buttons & (0x1 << 12)) > 0) Output[11] |= (Byte)(1 << 7); // Y
                if ((Buttons & (0x1 << 13)) > 0) Output[11] |= (Byte)(1 << 5); // B
                if ((Buttons & (0x1 << 14)) > 0) Output[11] |= (Byte)(1 << 4); // A
                if ((Buttons & (0x1 << 15)) > 0) Output[11] |= (Byte)(1 << 6); // X

                if ((Buttons & (0x1 << 16)) > 16) Output[11] |= (Byte)(1 << 2); // Guide     

                Output[12] = Input[26]; // Left Trigger
                Output[13] = Input[27]; // Right Trigger

                Int32 ThumbLX = Scale(Input[14], false);
                Int32 ThumbLY = -Scale(Input[15], false);
                Int32 ThumbRX = Scale(Input[16], false);
                Int32 ThumbRY = -Scale(Input[17], false);

                Output[14] = (Byte)((ThumbLX >> 0) & 0xFF); // LX
                Output[15] = (Byte)((ThumbLX >> 8) & 0xFF);

                Output[16] = (Byte)((ThumbLY >> 0) & 0xFF); // LY
                Output[17] = (Byte)((ThumbLY >> 8) & 0xFF);

                Output[18] = (Byte)((ThumbRX >> 0) & 0xFF); // RX
                Output[19] = (Byte)((ThumbRX >> 8) & 0xFF);

                Output[20] = (Byte)((ThumbRY >> 0) & 0xFF); // RY
                Output[21] = (Byte)((ThumbRY >> 8) & 0xFF);
            }
        }

        private void SetBatteryLED(JoyconDevice dev, int subindex, byte batlvl)
        {
            switch (batlvl)
            {
                case 8:
                    dev.SetLEDs(0x8 | 0x4 | 0x2 | 0x1);
                    break;
                case 6:
                    dev.SetLEDs(0x0 | 0x4 | 0x2 | 0x1);
                    break;
                case 4:
                    dev.SetLEDs(0x0 | 0x0 | 0x2 | 0x1);
                    break;
                case 2:
                    dev.SetLEDs(0x0 | 0x0 | 0x0 | 0x1);
                    break;
                case 0:
                    dev.SetLEDs(0x0 | 0x0 | 0x0 | 0x10);
                    break;
            }
        }
    }
}
