using nanoFramework.Hardware.Esp32;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Devices;

namespace NFApp1
{
    public class FileSystem
    {
        public bool IsRun { get; set; } = false;
        public StorageFolder StorageFolder;
        public FileSystem()
        {
            try
            {
                Configuration.SetPinFunction(5, DeviceFunction.SPI1_CLOCK);
                Configuration.SetPinFunction(21, DeviceFunction.SPI1_MISO);
                Configuration.SetPinFunction(22, DeviceFunction.SPI1_MOSI);

                SDCard.MountSpi("SPI1", 13);
                StorageFolder = KnownFolders.RemovableDevices;
                IsRun = true;
                Debug.WriteLine("SDcard Ready!");

                foreach (var item in StorageFolder.GetFolders())
                {
                    Debug.WriteLine("文件夹：" + item.Name);
                    foreach (var item1 in item.GetFiles())
                    {
                        Debug.WriteLine("文件：" + item1.Name);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to mount SDCard :-{ex.Message}");
            }
        }
    }
}