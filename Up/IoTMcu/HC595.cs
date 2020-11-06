﻿using System;
using System.Device.Gpio;
using System.Threading;

namespace IoTMcu
{
    class HC595
    {
        private const int MRES = 10;
        private const int MRCLK = 10;
        private const int MRLOCK = 10;
        private const int MBCLK = 10;
        private const int MBLOCK = 10;
        private const int MOUT = 10;
        private const int M1RED = 10;
        private const int M2RED = 10;
        private const int M1BLU = 10;
        private const int M2BLU = 10;

        public HC595()
        {
            IoTMcuMain.GpioController.OpenPin(MRES, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MRCLK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MRLOCK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MBCLK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MBLOCK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MOUT, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M1RED, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M2RED, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M1BLU, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M2BLU, PinMode.Output);

            IoTMcuMain.GpioController.Write(MRES, PinValue.Low);
            IoTMcuMain.GpioController.Write(MBCLK, PinValue.Low);
            IoTMcuMain.GpioController.Write(MRCLK, PinValue.Low);
            Thread.Sleep(10);
            IoTMcuMain.GpioController.Write(MRES, PinValue.High);
        }

        public void Reset()
        {
            IoTMcuMain.GpioController.Write(MRES, PinValue.Low);
            Thread.Sleep(10);
            IoTMcuMain.GpioController.Write(MRES, PinValue.High);
        }

        public void SetOut(bool val)
        {
            IoTMcuMain.GpioController.Write(MOUT, val ? PinValue.Low : PinValue.High);
        }
        public void SetRDate(byte[] data1, byte[] data2, int size)
        {
            byte i;
            int local;
            byte data_1;
            byte data_2;

            for (local = 0; local < size; local++)
            {
                data_1 = data1[local];
                data_2 = data2[local];
                for (i = 0; i < 8; i++)
                {
                    IoTMcuMain.GpioController.Write(M1BLU, (data_1 & 0x01) == 1 ? PinValue.High : PinValue.Low);
                    IoTMcuMain.GpioController.Write(M2BLU, (data_2 & 0x01) == 1 ? PinValue.High : PinValue.Low);

                    data_1 <<= 1;
                    data_2 <<= 1;

                    IoTMcuMain.GpioController.Write(MRCLK, PinValue.Low);
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    IoTMcuMain.GpioController.Write(MRCLK, PinValue.High);
                }
                IoTMcuMain.GpioController.Write(MRLOCK, PinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                IoTMcuMain.GpioController.Write(MRLOCK, PinValue.Low);
            }
        }
        public void SetBDate(byte[] data1, byte[] data2, int size)
        {
            byte i;
            int local;
            byte data_1;
            byte data_2;

            for (local = 0; local < size; local++)
            {
                data_1 = data1[local];
                data_2 = data2[local];
                for (i = 0; i < 8; i++)
                {
                    IoTMcuMain.GpioController.Write(M1RED, (data_1 & 0x01) == 1 ? PinValue.High : PinValue.Low);
                    IoTMcuMain.GpioController.Write(M2RED, (data_2 & 0x01) == 1 ? PinValue.High : PinValue.Low);

                    data_1 <<= 1;
                    data_2 <<= 1;

                    IoTMcuMain.GpioController.Write(MBCLK, PinValue.Low);
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                    IoTMcuMain.GpioController.Write(MBCLK, PinValue.High);
                }
                IoTMcuMain.GpioController.Write(MBLOCK, PinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                IoTMcuMain.GpioController.Write(MBLOCK, PinValue.Low);
            }
        }
    }
}