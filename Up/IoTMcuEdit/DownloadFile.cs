using Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMcuEdit
{
    class DownloadFile
    {
        private readonly ManualResetEvent WriteLock = new(false);
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
            WriteLock.Set();
            var data = new byte[8196];
            if (size > 8196)
            {
                down = 8196;
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
            WriteLock.Reset();
        }
    }
}
