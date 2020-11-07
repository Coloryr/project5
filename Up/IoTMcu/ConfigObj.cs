using System.Collections.Generic;

namespace IoTMcu
{
    public enum ShowType
    { 
        Normal, 
    }
    public class FontSave
    {
        public string Name { get; set; }
        public string Local { get; set; }
        public string px12 { get; set; }
        public string px16 { get; set; }
        public string px24 { get; set; }
        public string px32 { get; set; }
    }
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
        public int Withe { get; set; }
        public int Height { get; set; }
    }
    public class ShowObj
    { 
        public ShowType Type { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public int PosX { get; set; }
        public int PoxY { get; set; }
        public int Times { get; set; }

    }
}
