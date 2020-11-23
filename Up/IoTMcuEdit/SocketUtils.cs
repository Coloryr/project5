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
        public bool IsConnect { get; private set; }
        private Socket socket = new(SocketType.Stream, ProtocolType.Tcp);
        private delegate void CallEvent(bool state);
        private delegate void DataEvent(string data);
        private CallEvent ConnectCall;
        private DataEvent DataCall;

        public SocketUtils(Action<bool> action, Action<string> action1)
        {
            ConnectCall = new CallEvent(action);
            DataCall = new DataEvent(action1);
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
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
            socket = null;
        }
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                SocketObject state = (SocketObject)ar.AsyncState;
                Socket ThisSocket = state.workSocket;
                var pack = new SocketObject
                {
                    workSocket = ThisSocket
                };
                ThisSocket.BeginReceive(pack.buffer, 0, SocketObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallBack), pack);
                int bytesRead = ThisSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    if (Check(state.buffer))
                    {
                        string temp = Encoding.UTF8.GetString(state.buffer);
                        temp = temp[5..];
                        DataCall.Invoke(temp);
                    }
                }
            }
            catch
            { 
                
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
                    if(socket == null)
                        socket = new(SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ip, port);
                    var LastPackObj = new SocketObject
                    {
                        workSocket = socket
                    };
                    socket.BeginReceive(LastPackObj.buffer, 0, SocketObject.BufferSize,
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
