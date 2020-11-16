using Lib;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IoTMcuEdit
{
    class SocketUtils
    {
        private Socket socket;
        private bool IsConnect;
        private delegate void CloseEvent();
        private CloseEvent CloseCall;

        public SocketUtils(Action action)
        {
            CloseCall = new CloseEvent(action);
        }
        public bool Check(byte[] data)
        {
            for (int i = 0; i < 6; i++)
            {
                if (data[i] != SocketPack.ThisPack[i])
                {
                    App.Show("错误", "数据包错误");
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
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket.Dispose();
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
                    IsConnect = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            });
        }

        public void SendNext(string data)
        {
            var pack = Encoding.UTF8.GetBytes("      " + data);
            for (int i = 0; i < 6; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            try
            {
                socket.Send(pack);
            }
            catch (Exception e)
            {
                App.Show("连接错误", e.ToString());
                Close();
            }
        }
        public void Close()
        {
            IsConnect = false;
            ShutDown();
            CloseCall.Invoke();
        }
        public void Scan()
        {

        }
    }
}
