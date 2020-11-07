using System;
using System.Collections.Generic;
using System.Device.Gpio;

namespace IoTMcu
{
    class IoTMcuMain
    {
        public static readonly GpioController GpioController = new GpioController();

        public static string Local;
        public static bool IsBoot;

        public static ConfigObj Config;
        public static SocketIoT SocketIoT;
        public static ShowSave Show;
        public static HC138 HC138;
        public static HC595 HC595;
        public static Font Font;
        static void Main(string[] args)
        {
            Logs.Log("Mcu Start");
            Local = AppDomain.CurrentDomain.BaseDirectory;
            Logs.Log("Read Config");
            Config = ConfigRead.Read("config.json", new ConfigObj
            {
                FontList = new List<FontSave>(),
                LastSocket = new SocketSave
                {
                    IP = "192.168.1.1",
                    Port = 600
                },
                NextSocket = new SocketSave
                {
                    IP = "0.0.0.0",
                    Port = 600
                },
                ServerSocket = new SocketSave
                {
                    IP = "192.168.1.2",
                    Port = 25555
                },
                NeedServer = false,
                Height = 32,
                Withe = 16,
                Name = "LED1"
            });
            Logs.Log("Start Hardway");
            HC138 = new HC138();
            HC595 = new HC595();
            Logs.Log("Start Socket");
            SocketIoT = new SocketIoT();
            Logs.Log("Start Font");
            Font = new Font();
            Logs.Log("Start Show");
            Show = new ShowSave();
        }
    }
}
