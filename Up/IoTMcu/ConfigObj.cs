using Lib;
using System.Collections.Generic;

namespace IoTMcu
{
    public class SocketSave
    {
        public string IP { get; set; }
        public int Port { get; set; }
    }
    public class ConfigObj
    {
        public string Name { get; set; }
        public SocketSave NextSocket { get; set; }
        public SocketSave LastSocket { get; set; }
        public SocketSave ServerSocket { get; set; }
        public bool NeedServer { get; set; }
        public List<FontSave> FontList { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class ShowObj
    {
        public string Name { get; set; }
        public int Index { get; set; }
        //public ShowType Type { get; set; }
        public string FontType { get; set; }
        public FontSelfSize Size { get; set; }
        public FontSelfColor Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Time { get; set; }
        public string Text { get; set; }
    }
}
