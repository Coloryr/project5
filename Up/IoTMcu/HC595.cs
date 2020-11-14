using System;
using System.Device.Gpio;
using System.Threading;

namespace IoTMcu
{
    class HC595
    {
        private const int MRES = 10;
        private const int MCLK = 10;
        private const int MLOCK = 10;
        private const int MOUT = 10;
        private const int M1RED = 10;
        private const int M2RED = 10;
        private const int M1BLU = 10;
        private const int M2BLU = 10;

        public HC595()
        {
            IoTMcuMain.GpioController.OpenPin(MRES, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MCLK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MLOCK, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(MOUT, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M1RED, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M2RED, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M1BLU, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(M2BLU, PinMode.Output);

            IoTMcuMain.GpioController.Write(MRES, PinValue.Low);
            IoTMcuMain.GpioController.Write(MCLK, PinValue.Low);
            Thread.Sleep(10);
            IoTMcuMain.GpioController.Write(MRES, PinValue.High);
        }

        public static void Reset()
        {
            IoTMcuMain.GpioController.Write(MRES, PinValue.Low);
            Thread.Sleep(10);
            IoTMcuMain.GpioController.Write(MRES, PinValue.High);
        }

        public static void SetOut(bool val)
        {
            IoTMcuMain.GpioController.Write(MOUT, val ? PinValue.Low : PinValue.High);
        }
        public static void SetDate(PinValue[] data1, PinValue[] data2,
            PinValue[] data3, PinValue[] data4, int size, bool both = true)
        {
            int local;

            for (local = 0; local < size; local++)
            {
                IoTMcuMain.GpioController.Write(M1BLU, data1[local]);
                IoTMcuMain.GpioController.Write(M1RED, data3[local]);
                if (both)
                {
                    IoTMcuMain.GpioController.Write(M2BLU, data2[local]);
                    IoTMcuMain.GpioController.Write(M2RED, data4[local]);
                }

                IoTMcuMain.GpioController.Write(MCLK, PinValue.Low);
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
                IoTMcuMain.GpioController.Write(MCLK, PinValue.High);
            }
        }
        public static void Unlock()
        {
            IoTMcuMain.GpioController.Write(MLOCK, PinValue.High);
            Thread.Sleep(TimeSpan.FromMilliseconds(10));
            IoTMcuMain.GpioController.Write(MLOCK, PinValue.Low);
        }
    }
}