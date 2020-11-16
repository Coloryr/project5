using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace IoTMcuEdit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MainWindow_;
        public static void Show(string title, string data)
        {
            MainWindow_.notifyIcon.ShowBalloonTip(1000, title, data, System.Windows.Forms.ToolTipIcon.Error);
        }
    }
}
