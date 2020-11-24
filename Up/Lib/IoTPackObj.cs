namespace Lib
{
    public enum PackType
    {
        Info, SetInfo, SetShow, Test, ListShow, ListFont
    }
    public class IoTPackObj
    {
        public PackType Type { get; set; }
        public string Data { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }
        public int Data5 { get; set; }
    }
}
