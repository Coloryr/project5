using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IoTMcuEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private SocketUtils SocketUtils;
        public MainWindow()
        {
            App.MainWindow_ = this;
            InitializeComponent();
            notifyIcon = new();
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "测试";

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

            SocketUtils = new SocketUtils(SocketClose);

            字体列表.ItemsSource = new List<string>() { "测试", "测试" };
        }
        private void SocketClose()
        { 
            
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            Activate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddFont(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteFont(object sender, RoutedEventArgs e)
        {

        }

        private void AddShow(object sender, RoutedEventArgs e)
        {

        }
        private void ChangeShow(object sender, RoutedEventArgs e)
        {

        }
        private void DeleteShow(object sender, RoutedEventArgs e)
        {

        }
        private void UpShow(object sender, RoutedEventArgs e)
        {

        }
        private void DownShow(object sender, RoutedEventArgs e)
        {


        }
    }
}
