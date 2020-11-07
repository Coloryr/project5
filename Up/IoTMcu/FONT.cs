using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace IoTMcu
{
    public enum FontSize
    {
        px12, px16, px24, px32
    }
    public class FontType
    {
        public string Name { get; set; }
        public byte[] px12 { get; set; }
        public byte[] px16 { get; set; }
        public byte[] px24 { get; set; }
        public byte[] px32 { get; set; }
    }
    public class Font
    {
        //先只支持一种字体
        //public static readonly Dictionary<string, FontType> FontList = new Dictionary<string, FontType>();
        private FontType FontSave;
        private string Local = IoTMcuMain.Local + "Font/";
        private Stack data;
        public Font()
        {
            if (!Directory.Exists(Local))
            {
                Directory.CreateDirectory(Local);
            }
            FontSave = new FontType();
            if (File.Exists(Local + "px12.font"))
            {
                FontSave.px12 = File.ReadAllBytes(Local + "px12.font");
                Logs.Log("发现px12字体");
            }
            if (File.Exists(Local + "px16.font"))
            {
                FontSave.px16 = File.ReadAllBytes(Local + "px16.font");
                Logs.Log("发现px16字体");
            }
            if (File.Exists(Local + "px24.font"))
            {
                FontSave.px24 = File.ReadAllBytes(Local + "px24.font");
                Logs.Log("发现px24字体");
            }
            if (File.Exists(Local + "px32.font"))
            {
                FontSave.px32 = File.ReadAllBytes(Local + "px32.font");
                Logs.Log("发现px32字体");
            }
        }
        public void WriteFont(byte[] buffer, string name)
        {
            File.WriteAllBytes(Local + name, buffer);
        }

        public void GenShow(char data, List<byte[]> show, int start, FontSize size)
        {

        }
    }
}
