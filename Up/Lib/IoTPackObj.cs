namespace Lib
{
    public enum PackType
    {
        Start, Close, Send, Change, Data, Test, Download
    }
    class IoTPackObj
    {
        public string Name { get; set; }
        public PackType Type { get; set; }
        public string Text { get; set; }
        public string Data { get; set; }
        public int Data1 { get; set; }
    }
}
