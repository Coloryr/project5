using Lib;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

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
        public PackType type;

        public void Start()
        {
            IoTMcuMain.IsBoot.Set();
            Thread.Sleep(20);
            FileStream = File.OpenWrite(local);
        }

        public async void Write(byte[] data)
        {
            WriteLock.WaitOne();
            int down = data.Length;
            size -= down;
            WriteLock.Set();
            await FileStream.WriteAsync(data.AsMemory(0, down));
            if (size <= 0)
            {
                await FileStream.DisposeAsync();
                IoTMcuMain.SocketIoT.TaskDone(name);
                if (type == PackType.AddFont)
                {
                    IoTMcuMain.Font.Start();
                }
                else if (type == PackType.AddShow)
                {
                    IoTMcuMain.Show.Start();
                }
                IoTMcuMain.IsBoot.Reset();
            }
            socket.Send(SocketPack.ResPack);
            WriteLock.Reset();
        }
    }
}
