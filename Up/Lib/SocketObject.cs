using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Lib
{
    class SocketObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 65535;
        public byte[] buffer = new byte[BufferSize];
    }
}
