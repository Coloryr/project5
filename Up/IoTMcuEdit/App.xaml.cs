using System.Windows;

namespace IoTMcuEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainWindow_;
        public static void ShowA(string title, string data)
        {
            MainWindow_.notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Info);
        }
        public static void ShowB(string title, string data)
        {
            MainWindow_.notifyIcon.ShowBalloonTip(300, title, data, System.Windows.Forms.ToolTipIcon.Error);
        }
    }
}
