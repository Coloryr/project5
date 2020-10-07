using Windows.Devices.Gpio;

namespace NFApp1
{
    class HC138
    {
        private GpioPin A0;
        private GpioPin A1;
        private GpioPin A2;
        private GpioPin A3;
        private GpioPin AEN;

        public byte Local { get; set; }

        public HC138()
        {
            A0 = GpioController.GetDefault().OpenPin(32);
            A1 = GpioController.GetDefault().OpenPin(33);
            A2 = GpioController.GetDefault().OpenPin(25);
            A3 = GpioController.GetDefault().OpenPin(16);
            AEN = GpioController.GetDefault().OpenPin(26);

            A0.SetDriveMode(GpioPinDriveMode.Output);
            A1.SetDriveMode(GpioPinDriveMode.Output);
            A2.SetDriveMode(GpioPinDriveMode.Output);
            A3.SetDriveMode(GpioPinDriveMode.Output);
            AEN.SetDriveMode(GpioPinDriveMode.Output);

            A0.Write(GpioPinValue.High);
            A1.Write(GpioPinValue.High);
            A2.Write(GpioPinValue.High);
            A3.Write(GpioPinValue.High);
            AEN.Write(GpioPinValue.High);
        }
        public void SetPin()
        {
            A0.Write((Local &0x01) == 0x01? GpioPinValue.High: GpioPinValue.Low);
            A1.Write((Local & 0x02) == 0x01 ? GpioPinValue.High : GpioPinValue.Low);
            A2.Write((Local & 0x04) == 0x01 ? GpioPinValue.High : GpioPinValue.Low);
            A3.Write((Local & 0x08) == 0x01 ? GpioPinValue.High : GpioPinValue.Low);
        }
        public void SetEnable(bool enable)
        {
            AEN.Write(enable ? GpioPinValue.Low : GpioPinValue.High);
        }
        public void AddPos()
        {
            if (Local >= 15)
                Local = 0;
            else
                Local++;
            SetPin();
        }
        public void SetPos(byte pos)
        {
            if (pos > 15)
                return;
            Local = pos;
            SetPin();
        }
    }
}
