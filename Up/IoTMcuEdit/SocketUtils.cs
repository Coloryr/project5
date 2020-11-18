using Lib;
using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace IoTMcuEdit
{
    class SocketUtils
    {
        class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 8196;
            public byte[] buffer = new byte[BufferSize];
        }
        public bool IsConnect { get; private set; }
        private StateObject LastPackObj = new();
        private readonly Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
        private delegate void CallEvent(bool state);
        private delegate void DataEvent(string data);
        private CallEvent ConnectCall;
        private DataEvent DataCall;

        public SocketUtils(Action<bool> action, Action<string> action1)
        {
            ConnectCall = new CallEvent(action);
            DataCall = new DataEvent(action1);
            LastPackObj.workSocket = socket;
        }
        public static bool Check(byte[] data)
        {
            for (int i = 0; i < 5; i++)
            {
                if (data[i] != SocketPack.ThisPack[i])
                {
                    App.ShowB("错误", "数据包错误");
                    return false;
                }
                else
                {
                    data[i] = 0;
                }
            }
            return true;
        }
        private void ShutDown()
        {
            if(socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket ThisSocket = state.workSocket;
            ThisSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, 
                new AsyncCallback(ReceiveCallBack), state);
            int bytesRead = ThisSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (Check(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                    DataCall.Invoke(temp);
                }
            }
        }
        public async void SetConnect(string ip, int port)
        {
            if (IsConnect && socket != null && socket.Connected)
            {
                ShutDown();
            }
            IsConnect = false;
            await Task.Run(() =>
            {
                try
                {
                    socket.Connect(ip, port);
                    socket.BeginReceive(LastPackObj.buffer, 0, StateObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(ReceiveCallBack), LastPackObj);
                    IsConnect = true;
                    ConnectCall.Invoke(IsConnect);
                    SendNext(new IoTPackObj
                    {
                        Type = PackType.Info
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            });
        }

        public bool SendNext(object obj)
        {
            var data = JsonSerializer.Serialize(obj);
            var pack = Encoding.UTF8.GetBytes("     " + data);
            for (int i = 0; i < 5; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            try
            {
                socket.Send(pack);
                return true;
            }
            catch (Exception e)
            {
                App.ShowB("连接错误", e.ToString());
                Close();
                return false;
            }
        }
        public void Close()
        {
            IsConnect = false;
            ShutDown();
            ConnectCall.Invoke(IsConnect);
        }
        public void Scan()
        {

        }
    }
}
