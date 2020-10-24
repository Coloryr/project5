using System.Threading;
using Windows.Devices.Gpio;

namespace NFApp1
{
    public class Run
    {
        public void test()
        {
            bool goingUp = true;
            bool isRun = true;
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

            for (; ; )
            {
                if (isRun)
                {
                    dummyPad.Write(GpioPinValue.High);
                    Thread.Sleep(500);
                    dummyPad.Write(GpioPinValue.Low);
                    Thread.Sleep(500);
                }
                Thread.Sleep(50);
            }
        }
    }
}
