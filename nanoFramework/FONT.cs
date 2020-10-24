using nanoFramework.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NFApp1
{
    class FontType
    {
        public string Name { get; set; }
        public long px12 { get; set; }
        public long px16 { get; set; }
        public long px24 { get; set; }
        public long px32 { get; set; }
    }
    public class FONT
    {
        private Stack data;
        public void Init(StorageFile save)
        {
            data = (Stack)JsonConvert.DeserializeObject(DataReader.FromBuffer(FileIO.ReadBuffer(save)), data.GetType());
            if (data.Count == 0)
            {

            }
        }

        public void Create(StorageFile save)
        {
            int add = 0;
            var list = Program.FileSystem.StorageFolder.GetFolder("D:\\FONT").GetFolders();
            if (list.Length == 0)
            {
                Debug.WriteLine("没有字库文件");
                new ErrorRun();
            }
            foreach (var item in list)
            {
                var list1 = item.GetFiles();
                if (list1.Length == 0)
                    continue;
                var obj = new FontType
                {
                    Name = item.Name
                };
                foreach (var item2 in list1)
                {
                    if (item2.Name == "GBK12")
                    {
                        obj.px12 = add * 1024 * 1024;
                        WriteFont(DataReader.FromBuffer(FileIO.ReadBuffer(item2)), obj.px12);
                    }
                }
                add++;
            }
            if (add == 0)
            {
                Debug.WriteLine("没有字库文件");
                new ErrorRun();
            }
        }

        private void WriteFont(DataReader buffer, long Addr)
        {
            if (buffer.UnconsumedBufferLength < 4096)
            {
                var temp = new byte[buffer.UnconsumedBufferLength];
                buffer.ReadBytes(temp);
                Program.FLASH.Write(temp, buffer.UnconsumedBufferLength);
            }
            else
            {
                uint now = 0;//偏移
                var temp = new byte[4096];
                for (; ; )
                {
                    if (now > buffer.UnconsumedBufferLength)
                    {
                        uint size = now - buffer.UnconsumedBufferLength;
                        temp = new byte[size];
                        buffer.ReadBytes(temp);
                        Program.FLASH.Write(temp, Addr + now - 4096);
                        break;
                    }
                    else
                    {
                        buffer.ReadBytes(temp);
                        Program.FLASH.Write(temp, Addr + now);
                    }
                    now += 4096;
                }
            }
        }
    }
}
