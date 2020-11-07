using System;
using System.IO;
using System.Text;

namespace IoTMcu
{
    class Logs
    {
        private static string local = IoTMcuMain.Local + "logs.log";
        private static object Lock = new object();
        public Logs()
        {
            if (!File.Exists(local))
            {
                File.Create(local).Close();
            }
        }
        private static void Write(string data)
        {
            lock (Lock)
            {
                try
                {
                    var date = DateTime.Now;
                    string year = date.ToShortDateString().ToString();
                    string time = date.ToLongTimeString().ToString();
                    string write = "[" + year + "]" + "[" + time + "]" + data;
                    File.AppendAllText(local, write + Environment.NewLine, Encoding.UTF8);
                    Console.WriteLine(write);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
        public static void Log(string data)
        {
            Write("[INFO]" + data);
        }
        public static void Error(Exception e, string data)
        {
            Write("[ERROR]" + data + "\n" + e.ToString());
        }
        public static void Error(string data)
        {
            Write("[ERROR]" + data);
        }
        public static void Error(Exception data)
        {
            Write("[ERROR]" + data.ToString());
        }
    }
}
