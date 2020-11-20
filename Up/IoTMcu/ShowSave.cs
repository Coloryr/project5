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

        public readonly Dictionary<int, ShowObj> ShowList = new();
        private readonly List<PinValue[]> ShowRedTemp = new();
        private readonly List<PinValue[]> ShowBulTemp = new();

        private Thread UpdateThread;
        private Thread ShowThread;

        private Color Red;
        private Color Blue;
        private Color Mix;

        bool updata = true;

        private int Bank = 0;
        private int showindex = 0;
        private int showdelay = 0;

        public ShowSave()
        {
            Red = Color.Red;
            Blue = Color.Blue;
            Mix = Color.Orange;

            UpdateThread = new Thread(() =>
            {
                for (; ; )
                {
                    IoTMcuMain.IsBoot.WaitOne();
                    var show = IoTMcuMain.Show.ShowList[showindex];
                    if (updata)
                    {
                        for (int i = 0; i < IoTMcuMain.Config.Height; i++)
                        {
                            for (int j = 0; j < IoTMcuMain.Config.Width; j++)
                            {
                                var temp = ShowImg.GetPixel(j, i);
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
            ShowThread = new Thread(StartShow);
            ShowThread.Start();
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
            if (ShowImg != null)
            {
                ShowImg.Dispose();
            }
            ShowImg = new Bitmap(IoTMcuMain.Config.Width, IoTMcuMain.Config.Height);
            Bank = IoTMcuMain.Config.Height / 16;
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                ShowRedTemp.Add(new PinValue[IoTMcuMain.Config.Width]);
                ShowBulTemp.Add(new PinValue[IoTMcuMain.Config.Width]);
            }
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                for (int j = 0; j < IoTMcuMain.Config.Width; j++)
                {
                    ShowRedTemp[i][j] = PinValue.High;
                    ShowBulTemp[i][j] = PinValue.High;
                }
            }
        }

        public void StartShow()
        {
            HC138.SetEnable(true);
            HC595.SetOut(true);
            int line = 0;
            if (Bank == 0)
            {
                for (; ; )
                {
                    IoTMcuMain.IsBoot.WaitOne();
                    Thread.Sleep(10);
                    HC595.SetDate(ShowRedTemp[line], null,
                        ShowBulTemp[line], null, IoTMcuMain.Config.Width, false);
                    HC595.SetOut(false);
                    HC138.AddPos();
                    HC595.Unlock();
                    HC595.SetOut(true);
                    line++;
                    if (IoTMcuMain.Config.Width >= line)
                    {
                        line = 0;
                        HC138.Reset();
                    }
                }
            }
            else if (Bank == 1)
            {
                for (; ; )
                {
                    IoTMcuMain.IsBoot.WaitOne();
                    Thread.Sleep(10);
                    HC595.SetDate(ShowRedTemp[line], ShowRedTemp[line + 16],
                        ShowBulTemp[line], ShowBulTemp[line + 16], IoTMcuMain.Config.Width);
                    HC595.SetOut(false);
                    HC138.AddPos();
                    HC595.Unlock();
                    HC595.SetOut(true);
                    line++;
                    if (IoTMcuMain.Config.Width >= line)
                    {
                        line = 0;
                        HC138.Reset();
                    }
                }
            }
        }
    }
}
