using System.Net.Sockets;

namespace Lib
{
    class SocketObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 12287;
        public byte[] buffer = new byte[BufferSize];
    }
}
