namespace Lib
{
    public enum PackType
    {
        AddShow, DeleteShow, ChangeShow, Test, AddFont, DeleteFont, ListShow, ListFont
    }
    class IoTPackObj
    {
        public PackType Type { get; set; }
        public string Data { get; set; }
        public string Data1 { get; set; }
        public int Data2 { get; set; }
        public int Data3 { get; set; }
        public int Data4 { get; set; }
    }
}
