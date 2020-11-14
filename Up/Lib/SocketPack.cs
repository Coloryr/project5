namespace Lib
{
    class SocketPack
    {
        public static readonly byte[] ReadPack = { 0x00, 0xff, 0x5a, 0xa5, 0xff };
        public static readonly byte[] SendPack = { 0xff, 0x56, 0x87, 0x4f, 0x3a };
        public static readonly byte[] ThisPack = { 0xaa, 0x53, 0xea, 0xda, 0x12 };
        public static readonly byte[] ResPack = { 0x55, 0xaa };
    }
}
