using System.Threading;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using nanoFramework.Hardware.Esp32;

namespace NFApp1
{
    public class Program
    {
        //public static byte[,] show = new byte[16,32];
        public static void Main()
        {
            bool goingUp = true;
            bool isRun = true;
            float dutyCycle = .00f;

            Configuration.SetPinFunction(18, DeviceFunction.SPI2_CLOCK);
            Configuration.SetPinFunction(19, DeviceFunction.SPI2_MISO);
            Configuration.SetPinFunction(23, DeviceFunction.SPI2_MOSI);

            var FLASH = new FLASH();

            var HC138 = new HC138();
            //var HC595 = new HC595();
            var id = FLASH.GetID();
            string data = id.ToString("X4");
            Debug.WriteLine("FLASH:" + data);

            var FileSystem = new FileSystem();
            FileSystem.test();

            FLASH.WriteEnable();
            FLASH.PageWrite(new byte[] { 0x65 }, 0);
            var temp = FLASH.Read(0, 1);

            data = temp[0].ToString("X2");
            Debug.WriteLine("DATA:" + data);

            //HC138.SetEnable(true);
            //HC595.SetOut(false);

            //while (true)
            //{
            //    for (int i = 0; i < 16; i++)
            //    {
            //        HC595.SetBDate(show, 32, i);
            //        HC595.SetOut(true);
            //        Thread.Sleep(1);
            //        HC595.SetOut(false);
            //        HC138.AddPos();
            //    }
            //}

            GpioPin dummyPad = GpioController.GetDefault().OpenPin(2);
            GpioPin key = GpioController.GetDefault().OpenPin(0);
            key.SetDriveMode(GpioPinDriveMode.Input);
            key.ValueChanged += (a, e) =>
                {
                    if (e.Edge == GpioPinEdge.FallingEdge)
                    {
                        isRun = !isRun;
                    }
                };
            dummyPad.SetDriveMode(GpioPinDriveMode.Input);

            PwmController pwmController;
            PwmPin pwmPin;
            pwmController = PwmController.FromId("TIM1");
            pwmController.SetDesiredFrequency(5000);

            pwmPin = pwmController.OpenPin(2);
            pwmPin.SetActiveDutyCyclePercentage(dutyCycle);
            pwmPin.Start();

            for (; ; )
            {
                if (isRun)
                {
                    if (goingUp)
                    {
                        dutyCycle += 0.05f;
                        if (dutyCycle > .95) goingUp = !goingUp;
                    }
                    else
                    {
                        dutyCycle -= 0.05f;
                        if (dutyCycle < 0.10) goingUp = !goingUp;
                    }

                    pwmPin.SetActiveDutyCyclePercentage(dutyCycle);
                }

                Thread.Sleep(50);
            }
        }
    }
}
