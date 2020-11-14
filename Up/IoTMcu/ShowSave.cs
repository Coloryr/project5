using Lib;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Drawing;
using System.IO;
using System.Threading;

namespace IoTMcu
{
    class ShowSave
    {
        public static readonly string Local = IoTMcuMain.Local + "ShowList\\";

        public static Bitmap ShowImg;

        private readonly List<ShowObj> ShowList = new List<ShowObj>();
        private readonly List<PinValue[]> ShowRedTemp = new List<PinValue[]>();
        private readonly List<PinValue[]> ShowBulTemp = new List<PinValue[]>();

        private Thread UpdateThread;
        private Thread ShowThread;

        private Color Red;
        private Color Blue;
        private Color Mix;

        private int Bank = 0;
        private int Number = 0;

        public ShowSave()
        {
            var ColorConverter = new ColorConverter();
            Red = (Color)ColorConverter.ConvertFromString(Brushes.Red.ToString());
            Blue = (Color)ColorConverter.ConvertFromString(Brushes.Blue.ToString());
            Mix = (Color)ColorConverter.ConvertFromString(Brushes.Orange.ToString());

            UpdateThread = new Thread(() =>
            {
                bool updata = true;
                int showindex = 0;
                int showdelay = 0;
                for (; ; )
                {
                    IoTMcuMain.IsBoot.WaitOne();
                    var show = IoTMcuMain.Show.ShowList[showindex];
                    if (updata)
                    {
                        for (int i = 0; i < IoTMcuMain.Config.Width; i++)
                        {
                            for (int j = 0; j < IoTMcuMain.Config.Height; j++)
                            {
                                var temp = ShowImg.GetPixel(i, j);
                                if (temp == Red)
                                {
                                    ShowRedTemp[i][j] = PinValue.Low;
                                    ShowBulTemp[i][j] = PinValue.High;
                                }
                                else if (temp == Blue)
                                {
                                    ShowRedTemp[i][j] = PinValue.Low;
                                    ShowBulTemp[i][j] = PinValue.High;
                                }
                                else if (temp == Mix)
                                {
                                    ShowRedTemp[i][j] = PinValue.Low;
                                    ShowBulTemp[i][j] = PinValue.Low;
                                }
                                else
                                {
                                    ShowRedTemp[i][j] = PinValue.High;
                                    ShowBulTemp[i][j] = PinValue.High;
                                }
                            }
                        }
                        updata = false;
                    }
                    Thread.Sleep(100);
                }
            });
            Start();
            ShowThread = new Thread(StartShow);
            ShowThread.Start();
            UpdateThread.Start();
        }

        public void Start()
        {
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            var list = new DirectoryInfo(Local);
            foreach (var item in list.GetFiles())
            {
                var temp = ConfigRead.Read<ShowObj>(item.FullName, null);
                if (temp != null)
                {
                    ShowList.Add(temp);
                }
            }
            if (ShowImg != null)
            {
                ShowImg.Dispose();
            }
            ShowImg = new Bitmap(IoTMcuMain.Config.Width, IoTMcuMain.Config.Height);
            Bank = IoTMcuMain.Config.Height / 16;
            Number = IoTMcuMain.Config.Width / 8;
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                ShowRedTemp.Add(new PinValue[IoTMcuMain.Config.Width]);
                ShowBulTemp.Add(new PinValue[IoTMcuMain.Config.Width]);
            }
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                for (int j = 0; j < IoTMcuMain.Config.Width; i++)
                {
                    ShowRedTemp[i][j] = PinValue.High;
                    ShowBulTemp[i][j] = PinValue.High;
                }
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
                    IoTMcuMain.IsBoot.WaitOne();
                    Thread.Sleep(10);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], null, Number, false);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], null, Number, false);
                    IoTMcuMain.HC138.AddPos();
                    IoTMcuMain.HC595.Unlock();
                    IoTMcuMain.HC595.SetOut(true);
                    line++;
                    if (IoTMcuMain.Config.Width >= line)
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
                    IoTMcuMain.IsBoot.WaitOne();
                    Thread.Sleep(10);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], ShowRedTemp[line + 16], Number);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], ShowBulTemp[line + 16], Number);
                    IoTMcuMain.HC138.AddPos();
                    IoTMcuMain.HC595.Unlock();
                    IoTMcuMain.HC595.SetOut(true);
                    line++;
                    if (IoTMcuMain.Config.Width >= line)
                    {
                        line = 0;
                        IoTMcuMain.HC138.Reset();
                    }
                }
            }
        }
    }
}
