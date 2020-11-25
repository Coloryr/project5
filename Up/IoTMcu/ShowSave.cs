using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace IoTMcu
{
    class ShowSave
    {
        public static readonly string Local = IoTMcuMain.Local + "ShowList/";

        public static Bitmap ShowImg;

        public readonly Dictionary<int, ShowObj> ShowList = new();
        public readonly Dictionary<ShowObj, Bitmap> ShowDataList = new();

        private Thread UpdateThread;
        private byte[] ShowData;
        private int BULLocal;

        private Color Red;
        private Color Blue;
        private Color Mix;

        private BitArray RedBitArray;
        private BitArray BulBitArray;

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

            UpdateThread = new Thread(Update);
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            Start();
            UpdateThread.Start();
        }

        private void Update()
        {
            for (; ; )
            {
                IoTMcuMain.IsBoot.WaitOne();
                if (ShowList.Count == 0)
                {
                    updata = false;
                    Thread.Sleep(1000);
                }
                else
                {
                    if (updata)
                    {
                        if (showindex >= ShowList.Count)
                            showindex = 0;
                        var show = ShowList[showindex];
                        showdelay = show.Time;
                        ShowImg = ShowDataList[show];
                        UpdateShow();
                        IoTMcuMain.UartUtils.Write(ShowData, 0x02);
                        updata = false;
                    }
                    Thread.Sleep(100);
                    showdelay -= 100;
                    if (showdelay <= 0)
                    {
                        updata = true;
                        showindex++;
                    }
                }
            }
        }

        private void UpdateShow()
        {
            Logs.Log("更新显示");
            for (int i = 0; i < IoTMcuMain.Config.Height; i++)
            {
                for (int j = 0; j < IoTMcuMain.Config.Width; j++)
                {
                    var temp = ShowImg.GetPixel(j, i);

                    if (temp == Red)
                    {
                        RedBitArray[i * IoTMcuMain.Config.Width + j] = false;
                        BulBitArray[i * IoTMcuMain.Config.Width + j] = true;
                    }
                    else if (temp == Blue)
                    {
                        RedBitArray[i * IoTMcuMain.Config.Width + j] = true;
                        BulBitArray[i * IoTMcuMain.Config.Width + j] = false;
                    }
                    else if (temp == Mix)
                    {
                        RedBitArray[i * IoTMcuMain.Config.Width + j] = false;
                        BulBitArray[i * IoTMcuMain.Config.Width + j] = false;
                    }
                    else
                    {
                        RedBitArray[i * IoTMcuMain.Config.Width + j] = true;
                        BulBitArray[i * IoTMcuMain.Config.Width + j] = true;
                    }
                }
            }
            string valueString = "";
            int a = 0;
            int index = 0;
            for (int i = 0; i < IoTMcuMain.Config.Width * IoTMcuMain.Config.Height; i++)
            {
                a++;
                valueString += RedBitArray[i] ? "1" : "0";
                if (a == 8)
                {
                    ShowData[index] = Convert.ToByte(valueString, 2);
                    index++;
                    valueString = "";
                    a = 0;
                }
            }
            index = 0;
            a = 0;
            for (int i = 0; i < IoTMcuMain.Config.Width * IoTMcuMain.Config.Height; i++)
            {
                a++;
                valueString += BulBitArray[i] ? "1" : "0";
                if (a == 8)
                {
                    ShowData[index + IoTMcuMain.Config.Height * XCount] = Convert.ToByte(valueString, 2);
                    index++;
                    valueString = "";
                    a = 0;
                }
            }
            valueString = "";
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
        public void SetShow(string data, string data1)
        {
            IoTMcuMain.IsBoot.Reset();
            List<ShowObj> list = JsonConvert.DeserializeObject<List<ShowObj>>(data);
            var list1 = new DirectoryInfo(Local);
            foreach (var item in list1.GetFiles())
            {
                File.Delete(item.FullName);
            }
            foreach (var item in list)
            {
                var str = JsonConvert.SerializeObject(item);
                File.WriteAllText(Local + item.Index + ".json", str);
            }
            var temp = Convert.FromBase64String(data1);
            PackDown PackDown = new();
            MemoryStream stream = new MemoryStream(temp);
            PackDown.UnZip(Local, stream);
            GetShow();
            updata = true;
            IoTMcuMain.IsBoot.Set();
        }
        private void GetShow()
        {
            foreach (var item in ShowDataList)
            {
                item.Value.Dispose();
            }
            ShowList.Clear();
            ShowDataList.Clear();
            var list = new DirectoryInfo(Local);
            foreach (var item in list.GetFiles())
            {
                if (item.FullName.EndsWith(".json"))
                {
                    var temp = JsonConvert.DeserializeObject<ShowObj>(File.ReadAllText(item.FullName));
                    if (temp != null)
                    {
                        ShowList.Add(temp.Index, temp);
                        ShowDataList.Add(temp, new Bitmap(Local + temp.Index + ".jpg"));
                    }
                }
            }
        }
        public void Start()
        {
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

            GetShow();

            XCount = IoTMcuMain.Config.Width / 8;
            YCount = IoTMcuMain.Config.Height / 8;

            RedBitArray = new(IoTMcuMain.Config.Height * IoTMcuMain.Config.Width);
            BulBitArray = new(IoTMcuMain.Config.Height * IoTMcuMain.Config.Width);

            ShowData = new byte[IoTMcuMain.Config.Height * XCount * 2];

            BULLocal = IoTMcuMain.Config.Height * IoTMcuMain.Config.Width;

            Logs.Log("设置屏幕");

            var data = new byte[2];
            data[0] = (byte)IoTMcuMain.Config.Height;
            data[1] = (byte)IoTMcuMain.Config.Width;
            IoTMcuMain.UartUtils.Write(data, 0x01);
            updata = true;
        }
    }
}
