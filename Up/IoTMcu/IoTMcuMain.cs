using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading;

namespace IoTMcu
{
    class IoTMcuMain
    {
        public static readonly GpioController GpioController = new GpioController();

        public static string Local;
        public static ManualResetEvent IsBoot = new ManualResetEvent(false);

        public static ConfigObj Config;
        public static SocketIoT SocketIoT;
        public static ShowSave Show;
        public static FontSave Font;
        static void Main()
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
                Width = 16,
                Name = "LCD1"
            });
            Logs.Log("Start Hardway");
            new HC138();
            new HC595();
            Logs.Log("Start Socket");
            SocketIoT = new();
            Logs.Log("Start Show");
            Show = new();
            Logs.Log("Start Font");
            Font = new();
            Logs.Log("Start!!!");
        }
    }
}
