using Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IoTMcu
{
    class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 8196;
        public byte[] buffer = new byte[BufferSize];
    }
    class SocketIoT
    {
        private Socket NextSocket;
        private Socket LastSocket;
        private Socket ServerSocket;

        private readonly StateObject LastPackObj = new();
        private readonly StateObject NextPackObj = new();
        private readonly StateObject ServerPackObj = new();

        private Dictionary<string, DownloadFile> DownloadTasks = new();

        public void TaskDone(string task)
        {
            DownloadTasks.Remove(task);
        }

        public static bool Check(byte[] data)
        {
            for (int i = 0; i < 6; i++)
            {
                if (data[i] != SocketPack.ThisPack[i])
                {
                    Logs.Error("区块链数据包错误");
                    return false;
                }
                else
                {
                    data[i] = 0;
                }
            }
            return true;
        }
        public static bool CheckServer(byte[] data)
        {
            for (int i = 0; i < 6; i++)
            {
                if (data[i] != SocketPack.ThisPack[i])
                {
                    Logs.Error("服务器数据包错误");
                    return false;
                }
                else
                {
                    data[i] = 0;
                }
            }
            return true;
        }

        private void LastReceiveCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (Check(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                }
            }
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(LastReceiveCallBack), state);
        }
        private void NextReceiveCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (Check(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                    var obj = JsonConvert.DeserializeObject<IoTPackObj>(temp);
                    var data = Convert.FromBase64String(obj.Data1);
                    switch (obj.Type)
                    {
                        case PackType.AddFont:
                            if (DownloadTasks.ContainsKey(obj.Data))
                            {
                                DownloadTasks[obj.Data].Write(data);
                            }
                            else
                            {
                                DownloadTasks.Add(obj.Data, new DownloadFile
                                {
                                    name = obj.Data,
                                    local = FontSave.Local + obj.Data,
                                    size = obj.Data2,
                                    socket = handler,
                                    type = PackType.AddFont
                                });
                            }
                            break;
                        case PackType.AddShow:
                            if (DownloadTasks.ContainsKey(obj.Data))
                            {
                                DownloadTasks[obj.Data].Write(data);
                            }
                            else
                            {
                                DownloadTasks.Add(obj.Data, new DownloadFile
                                {
                                    name = obj.Data,
                                    local = ShowSave.Local + obj.Data,
                                    size = obj.Data2,
                                    socket = handler,
                                    type = PackType.AddShow
                                });
                            }
                            break;
                    }
                }
            }
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(NextReceiveCallBack), state);
        }
        private void ServerReceiveCallBack(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (CheckServer(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                }
            }
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ServerReceiveCallBack), state);
        }
        public void StartLast()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(IoTMcuMain.Config.LastSocket.IP);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, IoTMcuMain.Config.LastSocket.Port);
                LastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LastSocket.Connect(localEndPoint);
                LastSocket.BeginReceive(LastPackObj.buffer, 0, StateObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(LastReceiveCallBack), LastPackObj);
                Logs.Log("上一个设备已连接");
            }
            catch (Exception e)
            {
                Logs.Error(e, "上一个设备链接失败");
            }
        }
        public void StartNext()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(IoTMcuMain.Config.LastSocket.IP);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, IoTMcuMain.Config.LastSocket.Port);
                NextSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                NextSocket.Bind(localEndPoint);
                NextSocket.Listen(5);
                NextSocket.BeginReceive(NextPackObj.buffer, 0, StateObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(NextReceiveCallBack), NextPackObj);
                Logs.Log("下一个设备已准备");
            }
            catch (Exception e)
            {
                Logs.Error(e, "下一个设备准备失败");
            }
        }
        public void StartServer()
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse(IoTMcuMain.Config.LastSocket.IP);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, IoTMcuMain.Config.LastSocket.Port);
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Connect(localEndPoint);
                ServerSocket.BeginReceive(ServerPackObj.buffer, 0, StateObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(LastReceiveCallBack), ServerPackObj);
                Logs.Log("IoT服务器已连接");
            }
            catch (Exception e)
            {
                Logs.Error(e, "服务器链接失败");
            }
        }
        public SocketIoT()
        {
            StartLast();
            StartNext();
            StartServer();
        }

        public void SendNext(string data)
        {
            var pack = Encoding.UTF8.GetBytes("      " + data);
            for (int i = 0; i < 6; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            NextSocket?.Send(pack);
        }
        public void SendServer(string data)
        {
            var pack = Encoding.UTF8.GetBytes("      " + data);
            for (int i = 0; i < 6; i++)
            {
                pack[i] = SocketPack.ReadPack[i];
            }
            ServerSocket?.Send(pack);
        }
        public void SendLast(string data)
        {
            var pack = Encoding.UTF8.GetBytes("      " + data);
            for (int i = 0; i < 6; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            LastSocket?.Send(pack);
        }
    }
}
