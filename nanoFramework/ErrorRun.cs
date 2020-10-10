
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;

namespace NFApp1
{
    class ErrorRun
    {
        public ErrorRun()
        {
            bool goingUp = true;
            bool isRun = true;
            float dutyCycle = .00f;
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
