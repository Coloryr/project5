using System.Threading;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using nanoFramework.Hardware.Esp32;
using Windows.Storage;
using System;
using nanoFramework.Json;
using System.Collections;
using Windows.Storage.Streams;
using System.Reflection;

namespace NFApp1
{

    public class Program
    {
        public static FONT FONT;
        public static FLASH FLASH;
        public static HC138 HC138;
        public static FileSystem FileSystem;
        public static void Main()
        {
            Debug.WriteLine(">> " + " free memory: " + nanoFramework.Runtime.Native.GC.Run(true) + " bytes");
            FontType[] list = new FontType[2];
            FontType item = new FontType
            {
                Name = "Name",
                px12 = 120
            };

            list[0] = item;

            item = new FontType
            {
                Name = "Item1",
                px12 = 150
            };

            list[1] = item;

            Thread.Sleep(50);
            FLASH = new FLASH();
            FileSystem = new FileSystem();
            HC138 = new HC138();

            var id = FLASH.GetID();
            if (id == 0xffff)
            {
                new ErrorRun();
            }
            string data = id.ToString("X4");
            Debug.WriteLine("FLASH:" + data);

            var temp = FLASH.Read(0, 1);
            if (temp[0] != 0x65)
            {
                FLASH.PageWrite(new byte[] { 0x65 }, 0);
            }

            if (!FileSystem.IsRun)
            {
                new ErrorRun();
            }

            FONT = new FONT();

            StorageFile StongeFile;

            string json = JsonConvert.SerializeObject(list);

            //StongeFile = StorageFile.GetFileFromPath("D:\\System.Net.dll");
            //var ass = Assembly.Load("D:\\System.Net.dll");

            //var type = ass.GetTypes()[0];
            //MethodInfo MethodInfo = type.GetMethods()[0];
            //MethodInfo.Invoke(ass, null);

            //object obj = type.InvokeMember(null, BindingFlags.Public, null, null, null);


            //type.InvokeMember("test", BindingFlags.Public, null, obj, new object[] { });

            var removableDevices = FileSystem.StorageFolder.GetFolders();
            StorageFolder device = removableDevices[0];

            try
            {
                StongeFile = StorageFile.GetFileFromPath("D:\\save.json");
                FONT.Init(StongeFile);
            }
            catch
            {
                var folderNew = device.CreateFolder("ColoryrMCU", CreationCollisionOption.ReplaceExisting);
                StongeFile = folderNew.CreateFile("save.json", CreationCollisionOption.ReplaceExisting);
                FileIO.WriteText(StongeFile, json);
                //FONT.Create(StongeFile);
            }

            Debug.WriteLine(">> " + " free memory: " + nanoFramework.Runtime.Native.GC.Run(true) + " bytes");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
