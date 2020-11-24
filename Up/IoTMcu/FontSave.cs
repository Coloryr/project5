using Lib;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Net.Sockets;

namespace IoTMcu
{
    public class FontData
    {
        public static readonly Dictionary<FontSelfColor, Brush> BrushesSave = new()
        {
            { FontSelfColor.RED, Brushes.Red },
            { FontSelfColor.BULE, Brushes.Blue },
            { FontSelfColor.MIX, Brushes.Lime }
        };
    }
    public class FontSave
    {
        public static readonly string Local = IoTMcuMain.Local + "Font/";

        private readonly Dictionary<string, int> FontList = new();
        private PrivateFontCollection pfc = new();

        public readonly List<string> FontFiles = new();
        public FontSave()
        {
            Start();
        }
        public void Start()
        {

            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            var list = new DirectoryInfo(Local);
            int index = 0;
            if (pfc != null)
            {
                pfc.Dispose();
            }
            pfc = new();
            FontList.Clear();
            FontFiles.Clear();
            foreach (var item in list.GetFiles())
            {
                pfc.AddFontFile(item.FullName);
                FontList.Add(item.Name, index);
                FontFiles.Add(item.Name);

                index++;
            }
            foreach (var item in FontList.Keys)
            {
                Logs.Log(item);
            }
        }

        public void GenShow(Graphics Graphics, ShowObj show)
        {
            Logs.Log(show.FontType);
            if (FontList.ContainsKey(show.FontType))
            {
                Logs.Log("正在写字");
                Graphics.DrawString(show.Text, new Font(pfc.Families[FontList[show.FontType]], show.Size),
                    FontData.BrushesSave[show.Color], show.X, show.Y);
            }
        }

        public void RemoveFont(string data, Socket socket)
        {
            if (FontList.ContainsKey(data))
            {
                pfc.Families[FontList[data]].Dispose();
                FontList.Remove(data);
                SocketIoT.SendNext(new IoTPackObj
                {
                    Type = PackType.DeleteFont,
                    Data = "ok"
                }, socket);
            }
            else
            {
                SocketIoT.SendNext(new IoTPackObj
                {
                    Type = PackType.DeleteFont,
                    Data = "no"
                }, socket);
            }
        }
    }
}
