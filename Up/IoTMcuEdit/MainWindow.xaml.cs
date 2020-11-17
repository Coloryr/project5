using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lib;
using System.Collections.Generic;

namespace IoTMcuEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private SocketUtils SocketUtils;
        private int index;
        public ObservableCollection<string> FontList { get; set; } = new();
        public ObservableCollection<ShowObj> ShowList { get; set; } = new();
        public ShowObj TempShowObj { get; set; } = new();
        public LcdObj LcdObj { get; set; } = new()
        {
            IP = "192.168.0.100",
            Port = 25555,
            Name = "LCD1",
            X = 32,
            Y = 16
        };
        public MainWindow()
        {
            App.MainWindow_ = this;
            InitializeComponent();
            notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "测试";

            DataContext = this;

            BitmapSource m = (BitmapSource)Icon;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(m.PixelWidth, m.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(
            new System.Drawing.Rectangle(System.Drawing.Point.Empty, bmp.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            m.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);

            IntPtr iconHandle = bmp.GetHicon();
            System.Drawing.Icon icon = System.Drawing.Icon.FromHandle(iconHandle);

            notifyIcon.Icon = icon;
            notifyIcon.Click += NotifyIcon_Click;

            SocketUtils = new SocketUtils(SocketState, SocketData);

            字体列表.ItemsSource = FontList;
            显示列表.ItemsSource = ShowList;

            ShowList.Add(new ShowObj());

            ShowList[0].Bind(TempShowObj);

            显示列表.SelectedIndex = 0;

            lock1.IsEnabled =
            lock2.IsEnabled =
            lock3.IsEnabled =
            lock4.IsEnabled = false;
            连接.Content = "连接";
            StateLable.Content = "未连接";
            StateLed.Fill = Brushes.Red;

            SocketState(true);
        }
        private void SocketData(string data)
        {
            var obj = JsonSerializer.Deserialize<IoTPackObj>(data);
            switch (obj.Type)
            {
                case PackType.Info:
                    LcdObj.Name = obj.Data;
                    LcdObj.X = obj.Data3;
                    LcdObj.Y = obj.Data4;

                    var list = JsonSerializer.Deserialize<List<string>>(obj.Data1);
                    FontList.Clear();
                    foreach (var item in list)
                    {
                        FontList.Add(item);
                    }
                    var list1 = JsonSerializer.Deserialize<List<ShowObj>>(obj.Data1);
                    ShowList.Clear();
                    foreach (var item in list1)
                    {
                        ShowList.Add(item);
                    }
                    App.ShowA("屏幕", "配置已读取");
                    break;
            }
        }
        private void SocketState(bool state)
        {
            Dispatcher.Invoke(() =>
            {
                if (state)
                {
                    lock1.IsEnabled =
                    lock2.IsEnabled =
                    lock3.IsEnabled =
                    lock4.IsEnabled = true;
                    连接.Content = "断开";
                    StateLable.Content = "已连接";
                    StateLed.Fill = Brushes.LawnGreen;
                    IP.IsEnabled =
                    Port.IsEnabled = false;
                }
                else
                {
                    lock1.IsEnabled =
                    lock2.IsEnabled =
                    lock3.IsEnabled =
                    lock4.IsEnabled = false;
                    连接.Content = "连接";
                    StateLable.Content = "未连接";
                    StateLed.Fill = Brushes.Red;
                    IP.IsEnabled =
                    Port.IsEnabled = true;
                }
            });
        }
        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Activate();
        }
        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (SocketUtils.IsConnect)
            {
                App.ShowA("屏幕", "正在断开");
                SocketUtils.Close();
            }
            else
            {
                App.ShowA("屏幕", "正在连接");
                SocketUtils.SetConnect(LcdObj.IP, LcdObj.Port);
            }
        }
        private void AddFont(object sender, RoutedEventArgs e)
        {

        }
        private void DeleteFont(object sender, RoutedEventArgs e)
        {

        }
        private void AddShow(object sender, RoutedEventArgs e)
        {
            index = ShowList.Count;
            ShowList.Add(new ShowObj
            {
                Index = index
            });
            ShowList[index].Bind(TempShowObj);
            显示列表.SelectedIndex = index;
            App.ShowA("显示列表", "已添加");
        }
        private void ChangeShow(object sender, RoutedEventArgs e)
        {
            if (显示列表.SelectedItem == null)
                return;
            index = 显示列表.SelectedIndex;
            var data = (ShowObj)显示列表.SelectedItem;
            data.Bind(TempShowObj);
            App.ShowA(data.Name, "正在修改");
        }
        private void DeleteShow(object sender, RoutedEventArgs e)
        {
            if (显示列表.SelectedItem == null)
                return;
            index = 显示列表.SelectedIndex;
            ShowList.RemoveAt(index);
            for (int a = index; a < ShowList.Count; a++)
            {
                ShowList[a].Index--;
            }
            显示列表.SelectedIndex = index - 1;
            App.ShowA("显示列表", "已删除");
        }
        private void UpShow(object sender, RoutedEventArgs e)
        {
            if (显示列表.SelectedItem == null)
                return;
            index = 显示列表.SelectedIndex;
            if (index == 0)
                return;
            int up = index - 1;
            var temp = ShowList[index];
            var temp1 = ShowList[up];
            temp.Index--;
            temp1.Index++;
            ShowList[index] = temp1;
            ShowList[up] = temp;
            显示列表.SelectedIndex = index;
            App.ShowA(temp.Name, "已上移");
        }
        private void DownShow(object sender, RoutedEventArgs e)
        {
            if (显示列表.SelectedItem == null)
                return;
            index = 显示列表.SelectedIndex;
            if (index == ShowList.Count - 1)
                return;
            int down = index + 1;
            var temp = ShowList[index];
            var temp1 = ShowList[down];
            temp.Index++;
            temp1.Index--;
            ShowList[index] = temp1;
            ShowList[down] = temp;
            显示列表.SelectedIndex = index;
            App.ShowA(temp.Name, "已下移");
        }
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            TempShowObj.Bind(ShowList[index]);
            App.ShowA(TempShowObj.Name, "设置成功");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SocketUtils.Close();
            notifyIcon.Dispose();
        }
    }
}
