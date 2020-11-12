using System.Collections.Generic;
using System.Device.Gpio;
using System.Drawing;
using System.IO;
using System.Threading;

namespace IoTMcu
{
    class ShowSave
    {
        private readonly List<ShowObj> ShowList = new List<ShowObj>();
        private readonly List<PinValue[]> ShowRedTemp = new List<PinValue[]>();
        private readonly List<PinValue[]> ShowBulTemp = new List<PinValue[]>();

        private Thread UpdateThread;
        private Thread ShowThread;

        public Bitmap ShowImg;

        private Color Red;
        private Color Blue;
        private Color Mix;

        private int Bank =0 ;
        private int Number = 0;
        private string Local = IoTMcuMain.Local + "ShowList\\";
        public ShowSave()
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

            var ColorConverter = new ColorConverter();
            Red = (Color)ColorConverter.ConvertFromString(Brushes.Red.ToString());
            Blue = (Color)ColorConverter.ConvertFromString(Brushes.Blue.ToString());
            Mix = (Color)ColorConverter.ConvertFromString(Brushes.Orange.ToString());

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
            UpdateThread = new Thread(() =>
            {
                bool updata = true;
                for (; ; )
                {
                    if (!IoTMcuMain.IsBoot)
                    {
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
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], null, Number, false);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], null, Number, false);
                    IoTMcuMain.HC595.SetOut(true);
                    Thread.Sleep(1);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC138.AddPos();
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
                    IoTMcuMain.HC595.SetRDate(ShowRedTemp[line], ShowRedTemp[line + 16], Number);
                    IoTMcuMain.HC595.SetBDate(ShowBulTemp[line], ShowBulTemp[line + 16], Number);
                    IoTMcuMain.HC595.SetOut(true);
                    Thread.Sleep(1);
                    IoTMcuMain.HC595.SetOut(false);
                    IoTMcuMain.HC138.AddPos();
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
