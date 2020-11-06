using System.Collections.Generic;

namespace IoTMcu
{
    public class FontSave
    { 
        public string Name { get; set; }
        public string Local { get; set; }
    }
    public enum SocketType
    {
        Server, Client
    };
    public class ConfigObj
    {
        public SocketType Type { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public List<FontSave> FontList { get; set; }
    }
}
