using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMcu
{
    class ShowSave
    {
        private static readonly List<ShowObj> ShowList = new List<ShowObj>();
        private static readonly List<byte[]> ShowRedTemp = new List<byte[]>();
        private static readonly List<byte[]> ShowBulTemp = new List<byte[]>();
        private static readonly byte[] NullData = new byte[128];

        private static Thread UpdateThread;
        private static Thread ShowThread;
        
        private static int Bank =0 ;
        private static int Number = 0;
        private string local = IoTMcuMain.Local + "ShowList\\";
        public ShowSave()
        {
            if (!Directory.Exists(local))
            {
                Directory.CreateDirectory(local);
            }
            var list = new DirectoryInfo(local);
            foreach (var item in list.GetFiles())
            {
                var temp = ConfigRead.Read<ShowObj>(item.FullName, null);
                if (temp != null)
                {
                    ShowList.Add(temp);
                }
            }
            Bank = IoTMcuMain.Config.Height / 16;
            Number = IoTMcuMain.Config.Withe / 8;
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                ShowBulTemp.Add(new byte[Number]);
                ShowBulTemp.Add(new byte[Number]);
            }
            UpdateThread = new Thread(() =>
            {
                for (; ; )
                {
                    if (!IoTMcuMain.IsBoot)
                    {

                    }
                    Thread.Sleep(100);
                }
            });
            ShowThread = new Thread(StartShow);
            ShowThread.Start();
            UpdateThread.Start();
            for (; ; )
            {
                Thread.Sleep(100);
            }
        }

        public void StartShow()
        {
            IoTMcuMain.HC138.SetEnable(true);
            IoTMcuMain.HC595.SetOut(true);
            int line = 0;
            if (Bank == 0)
            {
                for (; ; )
                {
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], NullData, Number);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], NullData, Number);
                    IoTMcuMain.HC595.SetOut(true);
                    Thread.Sleep(1);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC138.AddPos();
                    line++;
                    if (IoTMcuMain.Config.Withe >= line)
                    {
                        line = 0;
                        IoTMcuMain.HC138.Reset();
                    }
                    
                }
            }
            else if (Bank == 1)
            {
                for (; ; )
                {
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], ShowRedTemp[line + 16], Number);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], ShowBulTemp[line + 16], Number);
                    IoTMcuMain.HC595.SetOut(true);
                    Thread.Sleep(1);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC138.AddPos();
                    line++;
                    if (IoTMcuMain.Config.Withe >= line)
                    {
                        line = 0;
                        IoTMcuMain.HC138.Reset();
                    }
                }
            }
        }
    }
}
