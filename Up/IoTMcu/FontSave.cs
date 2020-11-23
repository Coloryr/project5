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

        public readonly Dictionary<string, Font> FontList = new();
        public FontSave()
        {
            Start();
        }
        public void Start()
        {
            PrivateFontCollection pfc = new();
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            var list = new DirectoryInfo(Local);
            int index = 0;
            if (FontList.Count != 0)
            {
                foreach (var item in FontList.Values)
                {
                    item.Dispose();
                }
            }
            FontList.Clear();
            foreach (var item in list.GetFiles())
            {
                pfc.AddFontFile(item.FullName);
                Font f12 = new Font(pfc.Families[index], 12);
                Font f16 = new Font(pfc.Families[index], 16);
                Font f24 = new Font(pfc.Families[index], 24);
                Font f32 = new Font(pfc.Families[index], 32);

                FontList.Add(item.Name + "12", f12);
                FontList.Add(item.Name + "16", f16);
                FontList.Add(item.Name + "24", f24);
                FontList.Add(item.Name + "32", f32);

                index++;
            }
            pfc.Dispose();
        }

        public void GenShow(Graphics Graphics, string data, string FontName, int x, int y, FontSelfSize size, FontSelfColor color)
        {
            if (FontList.ContainsKey(FontName + size))
            {
                Graphics.DrawString(data, FontList[FontName + size], FontData.BrushesSave[color], new PointF(x, y));
            }
        }

        public void RemoveFont(string data, Socket socket)
        {
            if (FontList.ContainsKey(data))
            {
                FontList[data].Dispose();
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
