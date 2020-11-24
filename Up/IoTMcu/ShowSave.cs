﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
                        IoTMcuMain.UartUtils.Write(ShowData);
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
                byte temp1 = 0;
                byte temp2 = 0;
                byte temp3 = 0;
                for (int j = 0; j < IoTMcuMain.Config.Width; j++)
                {
                    var temp = ShowImg.GetPixel(j, i);
                    int bit = j / 8;
                    int bit_ = j % 8;

                    if (temp == Red)
                    {
                        temp2 |= (byte)(1 << bit_);
                    }
                    else if (temp == Blue)
                    {
                        temp1 |= (byte)(1 << bit_);
                    }
                    // if (temp == Mix)
                    else
                    {
                        temp2 |= (byte)(1 << bit_);
                        temp1 |= (byte)(1 << bit_);
                    }
                    if (temp3 == 7)
                    {
                        ShowData[i * XCount + XCount - bit - 1] = temp1;
                        ShowData[BULLocal + i * XCount + XCount - bit - 1] = temp2;
                        temp3 = 0;
                    }
                    else
                        temp3++;
                }
            }
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
        public void SetShow(string data, string data1)
        {
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
