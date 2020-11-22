using System.Collections;
using System.Collections.Generic;
using System.Device.Gpio;

namespace IoTMcu
{
    public class HC138
    {
        private const int A0 = 14;
        private const int A1 = 16;
        private const int A2 = 15;
        private const int A3 = 3;
        private const int AEN = 0;

        private static byte Local;

        private static List<PinValue[]> PinValues = new();

        public HC138()
        {
            IoTMcuMain.GpioController.OpenPin(A0, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(A1, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(A2, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(A3, PinMode.Output);
            IoTMcuMain.GpioController.OpenPin(AEN, PinMode.Output);

            IoTMcuMain.GpioController.Write(A0, PinValue.High);
            IoTMcuMain.GpioController.Write(A1, PinValue.High);
            IoTMcuMain.GpioController.Write(A2, PinValue.High);
            IoTMcuMain.GpioController.Write(A3, PinValue.High);
            IoTMcuMain.GpioController.Write(AEN, PinValue.High);

            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.Low, PinValue.Low, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.Low, PinValue.High, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.Low, PinValue.High, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.High, PinValue.Low, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.High, PinValue.Low, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.High, PinValue.High, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.Low, PinValue.High, PinValue.High, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.Low, PinValue.Low, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.Low, PinValue.Low, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.Low, PinValue.High, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.Low, PinValue.High, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.High, PinValue.Low, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.High, PinValue.Low, PinValue.High });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.High, PinValue.High, PinValue.Low });
            PinValues.Add(new PinValue[4] { PinValue.High, PinValue.High, PinValue.High, PinValue.High });
        }
        public static void SetPin()
        {
            IoTMcuMain.GpioController.Write(A0, PinValues[Local][0]);
            IoTMcuMain.GpioController.Write(A1, PinValues[Local][1]);
            IoTMcuMain.GpioController.Write(A2, PinValues[Local][2]);
            IoTMcuMain.GpioController.Write(A3, PinValues[Local][3]);
        }
        public static void SetEnable(bool enable)
        {
            IoTMcuMain.GpioController.Write(AEN, enable ? PinValue.Low : PinValue.High);
        }
        public static void AddPos()
        {
            Local++;
            SetPin();
        }
        public static void SetPos(int pos)
        {
            if (pos > 15)
                return;
            Local = (byte)pos;
            SetPin();
        }

        public static void Reset()
        {
            Local = 0;
            SetPin();
        }
    }
}
