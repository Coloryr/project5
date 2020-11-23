using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace IoTMcu
{
    class ShowSave
    {
        public static readonly string Local = IoTMcuMain.Local + "ShowList/";

        public static Bitmap ShowImg;
        public static Graphics Graphics;

        public readonly Dictionary<int, ShowObj> ShowList = new();

        private Thread UpdateThread;
        private byte[] ShowData;
        private int BULLocal;

        private Color Red;
        private Color Blue;
        private Color Mix;

        bool updata = true;

        private int Bank = 0;
        private int showindex = 0;
        private int showdelay = 0;
        private int XCount;
        private int YCount;

        public ShowSave()
        {
            Red = Color.FromArgb(255, 0, 0);
            Blue = Color.FromArgb(0, 0, 255);
            Mix = Color.FromArgb(0, 255, 0);

            UpdateThread = new Thread(() =>
            {
                for (; ; )
                {
                    IoTMcuMain.IsBoot.WaitOne();
                    //if (ShowList.Count == 0)
                    //{
                    //    Thread.Sleep(1000);
                    //}
                    //else
                    {
                        //var show = ShowList[showindex];
                        if (updata)
                        {
                            Logs.Log("更新显示");
                            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
                            {
                                for (int j = 0; j < IoTMcuMain.Config.Width; j++)
                                {
                                    var temp = ShowImg.GetPixel(j, i);
                                    int bit = j / 8;
                                    int bit_ = j % 8;
                                    if (temp == Red)
                                    {
                                        ShowData[bit + i * XCount] &= (byte)~(1 << bit_);
                                        ShowData[bit + BULLocal + i * XCount] |= (byte)(1 << bit_);
                                    }
                                    else if (temp == Blue)
                                    {
                                        ShowData[bit + i * XCount] |= (byte)(1 << bit_);
                                        ShowData[bit + BULLocal + i * XCount] &= (byte)~(1 << bit_);
                                    }
                                    else if (temp == Mix)
                                    {
                                        ShowData[bit + i * XCount] &= (byte)~(1 << bit_);
                                        ShowData[bit + BULLocal + i * XCount] &= (byte)~(1 << bit_);
                                    }
                                    else
                                    {
                                        ShowData[bit + i * XCount] |= (byte)(1 << bit_);
                                        ShowData[bit + BULLocal + i * XCount] |= (byte)(1 << bit_);
                                    }
                                }
                            }
                            updata = false;
                            string valueString = "";
                            int a = 0;
                            foreach (var item in ShowData)
                            {
                                a++;
                                valueString += Convert.ToString(item, 2).PadLeft(8, '0');
                                if (a == XCount)
                                {
                                    a = 0;
                                    Console.WriteLine(valueString);
                                    valueString = "";
                                }
                            }
                        }
                        Thread.Sleep(100);
                    }
                }
            });
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            var list = new DirectoryInfo(Local);
            foreach (var item in list.GetFiles())
            {
                var temp = JsonSerializer.Deserialize<ShowObj>(File.ReadAllText(item.FullName));
                if (temp != null)
                {
                    ShowList.Add(temp.Index, temp);
                }
            }
            Start();
            UpdateThread.Start();
        }
        public void SetShow(string data)
        {
            List<ShowObj> list = JsonSerializer.Deserialize<List<ShowObj>>(data);
            var list1 = new DirectoryInfo(Local);
            foreach (var item in list1.GetFiles())
            {
                File.Delete(item.FullName);
            }
            ShowList.Clear();
            foreach (var item in list)
            {
                ShowList.Add(item.Index, item);
                var str = JsonSerializer.Serialize(item);
                File.WriteAllText(Local + item.Index + ".json", str);
            }
        }
        public void Start()
        {
            if (Graphics != null)
            {
                Graphics.Dispose();
            }
            if (ShowImg != null)
            {
                ShowImg.Dispose();
            }
            Bank = IoTMcuMain.Config.Height / 16;
            if (Bank != 1 && Bank != 2)
            {
                Logs.Log($"显示高度错误{Bank}");
                return;
            }
            Bank = IoTMcuMain.Config.Width % 8;
            if (Bank != 0)
            {
                Logs.Log($"显示宽度错误{Bank}");
                return;
            }
            ShowImg = new Bitmap(IoTMcuMain.Config.Width, IoTMcuMain.Config.Height);
            Graphics = Graphics.FromImage(ShowImg);
            Graphics.FillRectangle(Brushes.White,
                new Rectangle(0, 0, IoTMcuMain.Config.Width, IoTMcuMain.Config.Height));
            Graphics.FillRectangle(Brushes.Red,
                new Rectangle(0, 0, 8, 8));
            Graphics.FillRectangle(Brushes.Blue,
                new Rectangle(8, 8, 8, 8));
            Graphics.FillRectangle(Brushes.Lime,
                new Rectangle(24, 0, 8, 8));

            XCount = IoTMcuMain.Config.Width / 8;
            YCount = IoTMcuMain.Config.Height / 8;

            ShowData = new byte[IoTMcuMain.Config.Height * XCount * 2];

            BULLocal = IoTMcuMain.Config.Height * XCount;

            var data = new byte[8];
            UartUtils.BuildPack(data);
            data[5] = 0x01;
            data[6] = (byte)IoTMcuMain.Config.Height;
            data[7] = (byte)IoTMcuMain.Config.Width;
            IoTMcuMain.UartUtils.Write(data);
            updata = true;
        }
    }
}
