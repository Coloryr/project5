using Lib;
using System;
using System.IO;
using System.Threading;

namespace IoTMcuEdit
{
    class DownloadFile
    {
        private readonly ManualResetEvent WriteLock = new(true);
        private IoTPackObj Pack;
        private byte[] Data;
        private int size;
        private int now = 0;
        private int down = 0;

        public SocketUtils SocketUtils;
        public string name;
        public string local;
        public PackType type;

        public void Start()
        {
            Data = File.ReadAllBytes(local);
            size = Data.Length;
            Pack = new()
            {
                Type = type,
                Data = name,
                Data3 = size
            };
            if (!SocketUtils.SendNext(Pack))
            {
                App.ShowB("字体", "字体" + name + "上传失败");
                App.MainWindow_.TaskDone(name);
            }
        }

        public void Send()
        {
            WriteLock.WaitOne();
            WriteLock.Reset();
            var data = new byte[800];
            if (size > 800)
            {
                down = 800;
            }
            else
            {
                down = size - now;
                App.MainWindow_.TaskDone(name);
            }
            Array.Copy(Data, now, data, 0, down);
            Pack.Data1 = Convert.ToBase64String(data, 0, down);
            if (!SocketUtils.SendNext(Pack))
            {
                App.ShowB("字体", "字体" + name + "上传失败");
                App.MainWindow_.TaskDone(name);
            }
            size -= down;
            now += 800;
            WriteLock.Set();
        }

        public void Close()
        {
            WriteLock.Dispose();
        }
    }
}
