using System;
using System.Threading;
using Windows.Devices.Gpio;

namespace NFApp1
{
    class HC595
    {
        private GpioPin MRES;
        private GpioPin MRCLK;
        private GpioPin MRLOCK;
        private GpioPin MBCLK;
        private GpioPin MBLOCK;
        private GpioPin MOUT;
        private GpioPin M1RED;
        private GpioPin M2RED;
        private GpioPin M3RED;
        private GpioPin M4RED;
        private GpioPin M1BLU;
        private GpioPin M2BLU;
        private GpioPin M3BLU;
        private GpioPin M4BLU;

        public HC595()
        {
            MRES = GpioController.GetDefault().OpenPin(32);
            MRCLK = GpioController.GetDefault().OpenPin(32);
            MRLOCK = GpioController.GetDefault().OpenPin(32);
            MBCLK = GpioController.GetDefault().OpenPin(32);
            MBLOCK = GpioController.GetDefault().OpenPin(32);
            MOUT = GpioController.GetDefault().OpenPin(32);
            M1RED = GpioController.GetDefault().OpenPin(32);
            M2RED = GpioController.GetDefault().OpenPin(32);
            M3RED = GpioController.GetDefault().OpenPin(32);
            M4RED = GpioController.GetDefault().OpenPin(32);
            M1BLU = GpioController.GetDefault().OpenPin(32);
            M2BLU = GpioController.GetDefault().OpenPin(32);
            M3BLU = GpioController.GetDefault().OpenPin(32);
            M4BLU = GpioController.GetDefault().OpenPin(32);

            MRES.SetDriveMode(GpioPinDriveMode.Output);
            MRCLK.SetDriveMode(GpioPinDriveMode.Output);
            MRLOCK.SetDriveMode(GpioPinDriveMode.Output);
            MBCLK.SetDriveMode(GpioPinDriveMode.Output);
            MBLOCK.SetDriveMode(GpioPinDriveMode.Output);
            MOUT.SetDriveMode(GpioPinDriveMode.Output);
            M1RED.SetDriveMode(GpioPinDriveMode.Output);
            M2RED.SetDriveMode(GpioPinDriveMode.Output);
            M3RED.SetDriveMode(GpioPinDriveMode.Output);
            M4RED.SetDriveMode(GpioPinDriveMode.Output);
            M1BLU.SetDriveMode(GpioPinDriveMode.Output);
            M2BLU.SetDriveMode(GpioPinDriveMode.Output);
            M3BLU.SetDriveMode(GpioPinDriveMode.Output);
            M4BLU.SetDriveMode(GpioPinDriveMode.Output);

            MRES.Write(GpioPinValue.Low);
            MBCLK.Write(GpioPinValue.Low);
            MRCLK.Write(GpioPinValue.Low);
            Thread.Sleep(10);
            MRES.Write(GpioPinValue.High);
        }

        public void Reset()
        {
            MRES.Write(GpioPinValue.Low);
            Thread.Sleep(10);
            MRES.Write(GpioPinValue.High);
        }

        public void SetOut(bool val)
        {
            MOUT.Write(val ? GpioPinValue.Low : GpioPinValue.High);
        }
        public void SetRDate(byte[] data1, byte[] data2, byte[] data3, byte[] data4, int size)
        {
            byte i;
            int local;
            byte data_1;
            byte data_2;
            byte data_3;
            byte data_4;

            for (local = 0; local < size; local++)
            {
                data_1 = data1[local];
                data_2 = data2[local];
                data_3 = data3[local];
                data_4 = data4[local];
                for (i = 0; i < 8; i++)
                {
                    M1RED.Write((data_1 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M2RED.Write((data_2 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M3RED.Write((data_3 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M4RED.Write((data_4 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);

                    data_1 <<= 1;
                    data_2 <<= 1;
                    data_3 <<= 1;
                    data_4 <<= 1;

                    MRCLK.Write(GpioPinValue.Low);
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    MRCLK.Write(GpioPinValue.High);
                }
                MRLOCK.Write(GpioPinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                MRLOCK.Write(GpioPinValue.Low);
            }
        }
        public void SetBDate(byte[] data1, byte[] data2, byte[] data3, byte[] data4, int size)
        {
            byte i;
            int local;
            byte data_1;
            byte data_2;
            byte data_3;
            byte data_4;

            for (local = 0; local < size; local++)
            {
                data_1 = data1[local];
                data_2 = data2[local];
                data_3 = data3[local];
                data_4 = data4[local];
                for (i = 0; i < 8; i++)
                {
                    M1BLU.Write((data_1 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M2BLU.Write((data_2 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M3BLU.Write((data_3 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);
                    M4BLU.Write((data_4 & 0x01) == 1 ? GpioPinValue.High : GpioPinValue.Low);

                    data_1 <<= 1;
                    data_2 <<= 1;
                    data_3 <<= 1;
                    data_4 <<= 1;

                    MBCLK.Write(GpioPinValue.Low);
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    MBCLK.Write(GpioPinValue.High);
                }
                MBLOCK.Write(GpioPinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                MBLOCK.Write(GpioPinValue.Low);
            }
        }
    }
}
