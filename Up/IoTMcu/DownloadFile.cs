using System.IO;
using System.Net.Sockets;

namespace IoTMcu
{
    class DownloadFile
    {
        private FileStream FileStream;
        public string local { get; set; }
        public Socket socket { get; set; }
        public long size { get; set; }

        public async void Start()
        {
            IoTMcuMain.IsBoot = true;
            long now = 0;
            int down;
            byte[] re = new byte[2] { 0x55, 0xaa };
            byte[] temp = new byte[8196];
            FileStream = File.OpenWrite(local);
            while (now < size)
            {
                down = socket.Receive(temp, 8196, SocketFlags.None);
                now += down;
                size -= down;
                await FileStream.WriteAsync(temp, 0, down);
                socket.Send(re);
            }
            await FileStream.DisposeAsync();
            IoTMcuMain.IsBoot = false;
        }
    }
}
