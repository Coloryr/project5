using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Devices;

namespace NFApp1
{
    public class FileSystem
    {
        public bool IsRun = false;
        public StorageFolder externalDevices;
        public FileSystem()
        {
            try
            {
                SDCard.MountSpi("SPI2", 17);
                externalDevices = KnownFolders.RemovableDevices;
                IsRun = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to mount SDCard :-{ex.Message}");
            }
        }
    }
}