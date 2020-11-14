using Lib;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMcu
{
    class DownloadFile
    {
        private ManualResetEvent WriteLock = new(false);
        private FileStream FileStream;

        public string name;
        public string local;
        public Socket socket;
        public long size;

        public void Start()
        {
            IoTMcuMain.IsBoot.Set();
            Thread.Sleep(20);
            FileStream = File.OpenWrite(local);
        }

        public void Write(byte[] data)
        {
            Task.Run(async () =>
            {
                WriteLock.WaitOne();
                int down = data.Length;
                size -= down;
                WriteLock.Set();
                await FileStream.WriteAsync(data, 0, down);
                if (size <= 0)
                {
                    await FileStream.DisposeAsync();
                    IoTMcuMain.SocketIoT.TaskDone(name);
                    IoTMcuMain.IsBoot.Reset();
                }
                socket.Send(SocketPack.ResPack);
                WriteLock.Reset();
            });
        }
    }
}
