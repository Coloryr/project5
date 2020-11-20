using Lib;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace IoTMcu
{
    class DownloadFile
    {
        private readonly ManualResetEvent WriteLock = new(true);
        private FileStream FileStream;
        private IoTPackObj Pack;

        public string name;
        public string local;
        public Socket socket;
        public long size;
        public PackType type;

        public void Start()
        {
            IoTMcuMain.IsBoot.Reset();
            Thread.Sleep(20);
            FileStream = File.OpenWrite(local);
            Pack = new()
            {
                Type = type,
                Data = name
            };
            SocketIoT.SendNext(Pack, socket);
            Logs.Log($"开始写字体{name}");
        }

        public async void Write(byte[] data)
        {
            WriteLock.WaitOne();
            int down = data.Length;
            size -= down;
            WriteLock.Reset();
            await FileStream.WriteAsync(data.AsMemory(0, down));
            if (size <= 0)
            {
                await FileStream.DisposeAsync();
                IoTMcuMain.SocketIoT.TaskDone(name);
                IoTMcuMain.Font.Start();
                IoTMcuMain.IsBoot.Set();
            }
            SocketIoT.SendNext(Pack, socket);
            WriteLock.Set();
        }

        public void Close()
        {
            FileStream.Close();
            WriteLock.Dispose();
        }
    }
}
