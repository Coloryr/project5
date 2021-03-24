using Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brushes = System.Windows.Media.Brushes;

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
        private string local = AppDomain.CurrentDomain.BaseDirectory + "TEMP/";

        private Dictionary<string, DownloadFile> DownloadTasks = new();
        private Dictionary<int, Bitmap> Imgs = new();
        public ObservableCollection<ShowObj> ShowList { get; set; } = new();
        public ShowObj TempShowObj { get; set; } = new();
        public LcdObj LcdObj { get; set; } = new()
        {
            IP = "192.168.0.100",
            Port = 50000,
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

            显示列表.ItemsSource = ShowList;

            ShowList.Add(new ShowObj());

            ShowList[0].Bind(TempShowObj);

            显示列表.SelectedIndex = 0;

            lock1.IsEnabled =
            lock3.IsEnabled =
            lock4.IsEnabled = false;
            连接.Content = "连接";
            StateLable.Content = "未连接";
            StateLed.Fill = Brushes.Red;

            SocketState(true);
        }
        public void TaskDone(string name)
        {
            if (DownloadTasks.ContainsKey(name))
            {
                DownloadTasks.Remove(name);
                Dispatcher.Invoke(() => FontReload_Click(null, null));
            }
        }
        private void SocketData(string data)
        {
            var obj = JsonConvert.DeserializeObject<IoTPackObj>(data);
            switch (obj.Type)
            {
                case PackType.Info:
                    Dispatcher.Invoke(() =>
                    {
                        LcdObj.Name = obj.Data;
                        LcdObj.X = obj.Data3;
                        LcdObj.Y = obj.Data4;
                    });

                    var list = JsonConvert.DeserializeObject<List<ShowObj>>(obj.Data1);
                    Dispatcher.Invoke(() =>
                    {
                        ShowList.Clear();
                        foreach (var item in list)
                        {
                            ShowList.Add(item);
                        }
                    });
                    App.ShowA("屏幕", "配置已读取");
                    break;
                case PackType.ListShow:
                    list = JsonConvert.DeserializeObject<List<ShowObj>>(obj.Data);
                    ShowList.Clear();
                    foreach (var item in list)
                    {
                        ShowList.Add(item);
                    }
                    App.ShowA("屏幕", "显示已读取");
                    break;
                case PackType.SetShow:
                    App.ShowA("显示", "显示内容已设置");
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
                    foreach (var item in DownloadTasks)
                    {
                        item.Value.Close();
                    }
                    DownloadTasks.Clear();
                    lock1.IsEnabled =
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
        private void AddShow(object sender, RoutedEventArgs e)
        {
            index = ShowList.Count;
            ShowList.Add(new ShowObj
            {
                Index = index,
                Name = "Show" + (index + 1)
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
            Bitmap img = null;
            if (Imgs.ContainsKey(TempShowObj.Index))
            {
                Imgs[TempShowObj.Index].Dispose();
                img = Imgs[TempShowObj.Index];
            }
            else if(TempShowObj.Type == ShowType.Normal)
            {
                img = GenShow.Gen(TempShowObj, LcdObj);
                Imgs.Add(TempShowObj.Index, img);
            }
            if (img != null)
            {
                ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                   img.GetHbitmap(),
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
                ImgTest.Source = wpfBitmap;
            }
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
            if (TempShowObj.Type == ShowType.Normal)
            {
                var img = GenShow.Gen(TempShowObj, LcdObj);
                if (Imgs.ContainsKey(TempShowObj.Index))
                {
                    Imgs[TempShowObj.Index].Dispose();
                    Imgs[TempShowObj.Index] = img;
                }
                else
                {
                    Imgs.Add(TempShowObj.Index, img);
                }
            }
            TempShowObj.Bind(ShowList[index]);
            App.ShowA(TempShowObj.Name, "设置成功");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SocketUtils.Close();
            notifyIcon.Dispose();
        }

        private void LcdSet_Click(object sender, RoutedEventArgs e)
        {
            if (LcdObj.X % 8 != 0 || LcdObj.Y % 8 != 0)
            {
                App.ShowB("屏幕", "尺寸错误");
                return;
            }
            var pack = new IoTPackObj
            {
                Type = PackType.SetInfo,
                Data = LcdObj.Name,
                Data3 = LcdObj.X,
                Data4 = LcdObj.Y
            };
            SocketUtils.SendNext(pack);
            App.ShowA("屏幕", "参数已设置");
        }

        private void FontReload_Click(object sender, RoutedEventArgs e)
        {
            var pack = new IoTPackObj
            {
                Type = PackType.ListFont
            };
            SocketUtils.SendNext(pack);
            App.ShowA("屏幕", "刷新字体");
        }

        private void ShowReload_Click(object sender, RoutedEventArgs e)
        {
            var pack = new IoTPackObj
            {
                Type = PackType.ListShow
            };
            SocketUtils.SendNext(pack);
            App.ShowA("屏幕", "刷新显示");
        }

        private void SetShow_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(local))
            {
                Directory.CreateDirectory(local);
            }
            foreach (var item in Directory.GetFiles(local))
            {
                File.Delete(item);
            }
            foreach (var item in Imgs)
            {
                item.Value.Save(local + item.Key + ".jpg");
            }
            var packup = new PackUp();
            var temp = packup.ZipDirectory(local, local + "pack.zip");
            if (!temp)
            {
                App.ShowB("显示", "显示内容设置失败");
                return;
            }
            var pack = new IoTPackObj
            {
                Type = PackType.SetShow,
                Data = JsonConvert.SerializeObject(ShowList),
                Data1 = Convert.ToBase64String(File.ReadAllBytes(local + "pack.zip"))
            };
            SocketUtils.SendNext(pack);
            App.ShowA("显示", "正在设置显示内容");
        }
        private void ShowRe_Click(object sender, RoutedEventArgs e)
        {
            var img = GenShow.Gen(TempShowObj, LcdObj);
            if (Imgs.ContainsKey(TempShowObj.Index))
            {
                Imgs[TempShowObj.Index].Dispose();
                Imgs[TempShowObj.Index] = img;
            }
            else
            {
                Imgs.Add(TempShowObj.Index, img);
            }
            TempShowObj.Type = ShowType.Normal;
            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
               img.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            ImgTest.Source = wpfBitmap;
        }

        private void Font_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog fontDialog = new();
            if (fontDialog.ShowDialog() != System.Windows.Forms.DialogResult.Cancel)
            {
                TempShowObj.FontType = fontDialog.Font.FontFamily.Name;
                TempShowObj.Size = (int)fontDialog.Font.Size;
            }
        }

        private void Pic_Click(object sender, RoutedEventArgs eve)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Title = "选择图片";
            openFileDialog1.Filter = "图片|*.jpg;*.png;*.jpeg;*.bmp";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Bitmap bitmap = new(openFileDialog1.FileName);
                    var img = GenShow.Gen(TempShowObj, LcdObj, bitmap);
                    if (Imgs.ContainsKey(TempShowObj.Index))
                    {
                        Imgs[TempShowObj.Index].Dispose();
                        Imgs[TempShowObj.Index] = img;
                    }
                    else
                    {
                        Imgs.Add(TempShowObj.Index, img);
                    }
                    TempShowObj.Type = ShowType.Pic;
                    TempShowObj.Text = "";
                    ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                    img.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                    ImgTest.Source = wpfBitmap;
                }
                catch (Exception e)
                {
                    App.ShowB("打开图片失败", e.ToString());
                }
            }
        }
    }
}
