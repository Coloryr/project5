using Lib;
using System;
using System.IO.Ports;
using System.Threading;

namespace IoTMcu
{
    class UartUtils
    {
        private SerialPort serialPort;
        private Thread ReadThread;
        private bool IsRun;
        public UartUtils()
        {
            serialPort = new("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
            serialPort.Open();
            IsRun = true;
            ReadThread = new(Read);
            ReadThread.Start();
        }

        private void Read()
        {
            while (IsRun)
            {
                if (serialPort.BytesToRead > 0)
                {
                    var data = new byte[serialPort.BytesToRead];
                    serialPort.Read(data, 0, data.Length);
                    if (SocketIoT.Check(data))
                    {
                        switch (data[5])
                        {
                            case 0x01:
                                if (data[6] == 's' && data[7] == 'e' && data[8] == 't')
                                    Logs.Log("显示配置已更新");
                                else
                                    Logs.Log("串口返回错误");
                                break;
                            case 0x02:
                                if (data[6] == 'o' && data[7] == 'k')
                                    Logs.Log("显示内容已更新");
                                else
                                    Logs.Log("串口返回错误");
                                break;
                        }
                    }
                }
            }
        }

        public static void BuildPack(byte[] data)
        {
            if (data.Length < 5)
            {
                Logs.Log("数据包过短");
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    data[i] = SocketPack.ThisPack[i];
                }
            }
        }

        public void Write(byte[] data)
        {
            var temp = new byte[data.Length + 6];
            BuildPack(temp);
            temp[5] = 0x02;
            Array.Copy(data, 0, temp, 6, data.Length);
            if (serialPort.IsOpen)
                serialPort.Write(temp, 0, temp.Length);
            else
                Logs.Log("串口未打开");
        }

        public void Stop()
        {
            IsRun = false;
        }
    }
}
