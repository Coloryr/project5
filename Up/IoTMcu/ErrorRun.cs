
using Iot.Device.ServoMotor;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Threading;

namespace IoTMcu
{
    class ErrorRun
    {
        public ErrorRun()
        {
            GpioController controller = new GpioController();
            bool goingUp = true;
            int width = 0;
            controller.OpenPin(17, PinMode.Output);

            PwmChannel pwmChannel = PwmChannel.Create(0, 1, 50);
            ServoMotor servoMotor = new ServoMotor(
                pwmChannel,
                180,
                700,
                2200);

            for (; ; )
            {
                if (goingUp)
                {
                    width += 5;
                    if (width > 1000) goingUp = !goingUp;
                }
                else
                {
                    width -= 5;
                    if (width < 10) goingUp = !goingUp;
                }

                servoMotor.WritePulseWidth(width);
                Thread.Sleep(50);
            }
        }
    }
}
