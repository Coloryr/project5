using System;
using System.Device.Gpio;

namespace IoTMcu
{
    class IoTMcuMain
    {
        public static GpioController GpioController = new GpioController();
        public static ConfigObj Config;

        private static HC138 HC138;
        private static HC595 HC595;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Mcu Start");
            Console.WriteLine("Read Config");
            Config = ConfigRead.Read("config.json", new ConfigObj 
            { 
                
            });
            Console.WriteLine("Start Hardway");
            HC138 = new HC138();
            HC595 = new HC595();
            Console.WriteLine("Start Socket");

        }
    }
}
