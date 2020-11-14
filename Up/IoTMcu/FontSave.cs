using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;

namespace IoTMcu
{
    public class FontData
    {
        public enum FontSize
        {
            px12 = 12,
            px16 = 16,
            px24 = 24,
            px32 = 32
        }
        public enum SelfColor
        {
            RED,
            BULE,
            MIX
        }
        public static readonly Dictionary<SelfColor, Brush> BrushesSave = new Dictionary<SelfColor, Brush>()
        {
            { SelfColor.RED, Brushes.Red},
            { SelfColor.BULE, Brushes.Blue},
            { SelfColor.MIX, Brushes.Orange}
        };
    }
    public class FontSave
    {
        public static readonly string Local = IoTMcuMain.Local + "Font\\";

        public readonly Dictionary<string, Font> FontList = new Dictionary<string, Font>();

        private Graphics Graphics;
        public FontSave()
        {
            Start();
        }
        public void Start()
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            Graphics = Graphics.FromImage(ShowSave.ShowImg);
            Graphics.FillRectangle(Brushes.Black,
                new Rectangle(0, 0, IoTMcuMain.Config.Width, IoTMcuMain.Config.Height));
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
        public void GenShow(string data, string FontName, int x, int y, FontData.FontSize size, FontData.SelfColor color)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Graphics.DrawString(data[i].ToString(),
                    FontList[FontName + size], FontData.BrushesSave[color],
                    new PointF(x + i * (int)size, y));
            }
        }
    }
}
