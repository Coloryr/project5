using Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IoTMcu
{
    class SocketIoT
    {
        private Socket NextSocket;
        private Socket LastSocket;
        private Socket ServerSocket;

        private Thread ServerThread;

        private bool RunFlag;

        private Dictionary<string, DownloadFile> DownloadTasks = new();

        public void TaskDone(string task)
        {
            DownloadTasks.Remove(task);
        }

        public static bool Check(byte[] data)
        {
            for (int i = 0; i < 5; i++)
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
            for (int i = 0; i < 5; i++)
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
            SocketObject state = (SocketObject)ar.AsyncState;
            Socket ThisSocket = state.workSocket;
            ThisSocket.BeginReceive(state.buffer, 0, SocketObject.BufferSize, 0, new AsyncCallback(LastReceiveCallBack), state);
            int bytesRead = ThisSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (Check(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                }
            }
        }
        private void NextReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                SocketObject state = (SocketObject)ar.AsyncState;
                Socket ThisSocket = state.workSocket;
                var nextpack = new SocketObject
                {
                    workSocket = ThisSocket
                };
                ThisSocket.BeginReceive(nextpack.buffer, 0, SocketObject.BufferSize, 0, new AsyncCallback(NextReceiveCallBack), nextpack);
                int bytesRead = ThisSocket.EndReceive(ar);
                if (bytesRead > 0)
                {
                    if (Check(state.buffer))
                    {
                        string temp = Encoding.UTF8.GetString(state.buffer);
                        temp = temp[5..];
                        var obj = JsonConvert.DeserializeObject<IoTPackObj>(temp);
                        var pack = new IoTPackObj();

                        switch (obj.Type)
                        {
                            case PackType.AddFont:
                                if (DownloadTasks.ContainsKey(obj.Data))
                                {
                                    var data = Convert.FromBase64String(obj.Data1);
                                    DownloadTasks[obj.Data].Write(data);
                                }
                                else
                                {
                                    var Task = new DownloadFile
                                    {
                                        name = obj.Data,
                                        local = FontSave.Local + obj.Data,
                                        size = obj.Data3,
                                        socket = ThisSocket,
                                        type = PackType.AddFont
                                    };
                                    DownloadTasks.Add(obj.Data, Task);
                                    Task.Start();
                                }
                                break;
                            case PackType.SetShow:
                                IoTMcuMain.Show.SetShow(obj.Data);
                                SendNext(new IoTPackObj()
                                {
                                    Type = PackType.SetShow
                                }, ThisSocket);
                                break;
                            case PackType.DeleteFont:
                                IoTMcuMain.Font.RemoveFont(obj.Data, ThisSocket);
                                pack.Type = PackType.DeleteFont;
                                pack.Data = obj.Data;
                                SendNext(pack, ThisSocket);
                                break;
                            case PackType.Info:
                                pack.Type = PackType.Info;
                                pack.Data = IoTMcuMain.Config.Name;
                                pack.Data3 = IoTMcuMain.Config.Width;
                                pack.Data4 = IoTMcuMain.Config.Height;
                                var list = IoTMcuMain.Font.FontFiles;
                                var list1 = IoTMcuMain.Show.ShowList.Values;
                                pack.Data1 = JsonConvert.SerializeObject(list);
                                pack.Data2 = JsonConvert.SerializeObject(list1);
                                SendNext(pack, ThisSocket);
                                break;
                            case PackType.SetInfo:
                                IoTMcuMain.IsBoot.Reset();
                                IoTMcuMain.Config.Name = obj.Data;
                                IoTMcuMain.Config.Width = obj.Data3;
                                IoTMcuMain.Config.Height = obj.Data4;
                                ConfigRead.Write(IoTMcuMain.Config, IoTMcuMain.ConfigName);
                                IoTMcuMain.Show.Start();
                                IoTMcuMain.IsBoot.Set();
                                break;
                            case PackType.ListFont:
                                pack.Type = PackType.ListFont;
                                list = IoTMcuMain.Font.FontFiles;
                                pack.Data = JsonConvert.SerializeObject(list);
                                SendNext(pack, ThisSocket);
                                break;
                            case PackType.ListShow:
                                pack.Type = PackType.ListShow;
                                list1 = IoTMcuMain.Show.ShowList.Values;
                                pack.Data = JsonConvert.SerializeObject(list1);
                                SendNext(pack, ThisSocket);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.Error(e);
                foreach (var item in DownloadTasks)
                {
                    item.Value.Close();
                }
                DownloadTasks.Clear();
            }
        }
        private void ServerReceiveCallBack(IAsyncResult ar)
        {
            SocketObject state = (SocketObject)ar.AsyncState;
            Socket ThisSocket = state.workSocket;
            ThisSocket.BeginReceive(state.buffer, 0, SocketObject.BufferSize, 0, new AsyncCallback(ServerReceiveCallBack), state);
            int bytesRead = ThisSocket.EndReceive(ar);
            if (bytesRead > 0)
            {
                if (CheckServer(state.buffer))
                {
                    string temp = Encoding.UTF8.GetString(state.buffer);
                    temp = temp.Trim();
                }
            }
        }
        public void StartLast()
        {
            try
            {
                //IPAddress ipAddress = IPAddress.Parse(IoTMcuMain.Config.LastSocket.IP);
                //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, IoTMcuMain.Config.LastSocket.Port);
                LastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LastSocket.Connect(IoTMcuMain.Config.LastSocket.IP, IoTMcuMain.Config.LastSocket.Port);
                var LastPackObj = new SocketObject
                {
                    workSocket = LastSocket
                };
                LastSocket.BeginReceive(LastPackObj.buffer, 0, SocketObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(LastReceiveCallBack), LastPackObj);
                Logs.Log("上一个设备已连接");
            }
            catch (Exception e)
            {
                Logs.Error(e, "上一个设备链接失败");
            }
        }
        private void NextListenClientConnect()
        {
            try
            {
                while (RunFlag)
                {
                    Socket clientScoket = NextSocket.Accept();
                    var NextPackObj = new SocketObject
                    {
                        workSocket = clientScoket
                    };
                    clientScoket.BeginReceive(NextPackObj.buffer, 0, SocketObject.BufferSize,
                    SocketFlags.None, new AsyncCallback(NextReceiveCallBack), NextPackObj);
                }
            }
            catch (Exception e)
            {
                Logs.Error(e);
                return;
            }
        }
        public void StartNext()
        {
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, IoTMcuMain.Config.LastSocket.Port);
                NextSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                NextSocket.Bind(localEndPoint);
                NextSocket.Listen(5);

                ServerThread = new Thread(NextListenClientConnect);
                ServerThread.Start();

                RunFlag = true;

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
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.None, IoTMcuMain.Config.LastSocket.Port);
                ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Connect(localEndPoint);
                var ServerPackObj = new SocketObject
                {
                    workSocket = ServerSocket
                };
                ServerSocket.BeginReceive(ServerPackObj.buffer, 0, SocketObject.BufferSize,
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
            //StartLast();
            StartNext();
            //StartServer();
        }

        public void Stop()
        {
            RunFlag = false;
            if (NextSocket != null)
            {
                NextSocket.Dispose();
            }
        }

        public static void SendNext(object obj, Socket socket)
        {
            var data = JsonConvert.SerializeObject(obj);
            var pack = Encoding.UTF8.GetBytes("     " + data);
            for (int i = 0; i < 5; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            socket.Send(pack);
        }
        public void SendServer(string data)
        {
            var pack = Encoding.UTF8.GetBytes("     " + data);
            for (int i = 0; i < 5; i++)
            {
                pack[i] = SocketPack.ReadPack[i];
            }
            ServerSocket?.Send(pack);
        }
        public void SendLast(string data)
        {
            var pack = Encoding.UTF8.GetBytes("     " + data);
            for (int i = 0; i < 5; i++)
            {
                pack[i] = SocketPack.ThisPack[i];
            }
            LastSocket?.Send(pack);
        }
    }
}
