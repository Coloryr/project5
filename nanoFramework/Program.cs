using System.Threading;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.Devices.Pwm;
using nanoFramework.Hardware.Esp32;
using Windows.Storage;

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

            var StongeFile = StorageFile.GetFileFromPath("D:\\save.json");
            if (!StongeFile.IsAvailable)
            {
                StongeFile = FileSystem.externalDevices.CreateFile("D:\\save.json");
                FONT.Create(StongeFile);
            }
            

            for (; ; )
            {

            }
        }
    }
}
