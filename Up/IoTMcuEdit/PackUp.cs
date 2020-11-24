using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace IoTMcuEdit
{
    class PackUp
    {
        private string rootPath = AppDomain.CurrentDomain.BaseDirectory;

        #region 压缩

        /// <summary>
        /// 递归压缩文件夹的内部方法
        /// </summary>
        /// <param name="folderToZip">要压缩的文件夹路径</param>
        /// <param name="zipStream">压缩输出流</param>
        /// <returns></returns>
        private bool ZipDirectory(string folderToZip, ZipOutputStream zipStream)
        {
            bool result = true;
            FileStream fs = null;
            Crc32 crc = new Crc32();
            ZipEntry ent;

            foreach (string file in Directory.GetFiles(folderToZip))
            {
                try
                {
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    ent = new ZipEntry(Path.GetFileName(file));
                    ent.DateTime = DateTime.Now;
                    ent.Size = fs.Length;

                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    ent.Crc = crc.Value;
                    zipStream.PutNextEntry(ent);
                    zipStream.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    
                }
            }


            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
            }
            GC.Collect();
            GC.Collect(1);
            return result;
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="folderToZip">要压缩的文件夹路径</param>
        /// <param name="zipedFile">压缩文件完整路径</param>
        /// <returns>是否压缩成功</returns>
        public bool ZipDirectory(string folderToZip, string zipedFile)
        {
            var test = true;
            if (!Directory.Exists(folderToZip))
                return test;

            ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipedFile));
            zipStream.SetLevel(6);

            test = ZipDirectory(folderToZip, zipStream);

            zipStream.Finish();
            zipStream.Close();

            return test;
        }
        #endregion
    }
}
