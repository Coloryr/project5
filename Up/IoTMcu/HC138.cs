using System;
using System.Device.Gpio;

namespace IoTMcu
{
    public class HC138
    {
        private const int A0 = 32;
        private const int A1 = 33;
        private const int A2 = 25;
        private const int A3 = 16;
        private const int AEN = 26;

        public int Local { get; set; }

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
        }
        public void SetPin()
        {
            IoTMcuMain.GpioController.Write(A0, (Local & 0x01) == 0x01 ? PinValue.High : PinValue.Low);
            IoTMcuMain.GpioController.Write(A1, (Local & 0x02) == 0x01 ? PinValue.High : PinValue.Low);
            IoTMcuMain.GpioController.Write(A2, (Local & 0x04) == 0x01 ? PinValue.High : PinValue.Low);
            IoTMcuMain.GpioController.Write(A3, (Local & 0x08) == 0x01 ? PinValue.High : PinValue.Low);
        }
        public void SetEnable(bool enable)
        {
            IoTMcuMain.GpioController.Write(AEN, enable ? PinValue.Low : PinValue.High);
        }
        public void AddPos()
        {
            Local++;
            SetPin();
        }
        public void SetPos(int pos)
        {
            if (pos > 15)
                return;
            Local = pos;
            SetPin();
        }

        public void Reset()
        {
            Local = 0;
            SetPin();
        }
    }
}
