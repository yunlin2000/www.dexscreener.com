using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC5.Help
{

    class Gzip
    {
        public static Byte[] D(byte[] bytes)
        {

            if (bytes.Count() > 0)
                try
                {
                    using (var compressStream = new MemoryStream(bytes))
                    {
                        using (var zipStream = new GZipStream(compressStream, System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (var resultStream = new MemoryStream())
                            {
                                zipStream.CopyTo(resultStream);
                                return resultStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            return null;
        }
        private static byte[] ToByteArray(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        ///<summary>
        /// Deflate解压函数
        /// JS:var details = eval('(' + utf8to16(zip_depress(base64decode(hidEnCode.value))) + ')')对应的C#压缩方法
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] DeflateDecompress2(byte[] buffer)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    stream.Flush();
                    int nSize = 1 * 1024 * 1024 + 256;    //假设字符串不会超过16K
                    byte[] decompressBuffer = new byte[nSize];
                    int nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
                    stream.Close();
                    return decompressBuffer.Take(nSizeIncept).ToArray();
                    // return System.Text.Encoding.UTF8.GetString(decompressBuffer, 0, nSizeIncept);   //转换为普通的字符串
                }
            }
        }
        public static byte[] DeflateDecompress(byte[] buffer)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                //using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
                //{undefined
                //    stream.Flush();
                //    int nSize = 16 * 1024 + 256;    //假设字符串不会超过16K
                //    byte[] decompressBuffer = new byte[nSize];
                //    int nSizeIncept = stream.Read(decompressBuffer, 0, nSize);
                //    stream.Close();
                //    return System.Text.Encoding.UTF8.GetString(decompressBuffer, 0, nSizeIncept);   //转换为普通的字符串
                //}
                using (System.IO.Compression.DeflateStream stream = new System.IO.Compression.DeflateStream(ms, System.IO.Compression.CompressionMode.Decompress))
                {
                    stream.Flush();
                    byte[] decompressBuffer = ToByteArray(stream);
                    int nSizeIncept = decompressBuffer.Length;
                    stream.Close();
                    return decompressBuffer;   //转换为普通的字符串
                }
            }
        }

        public static byte[] E(byte[] bytes)
        {

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
                    compressedzipStream.Write(bytes, 0, bytes.Length);
                    compressedzipStream.Close();
                    return ms.ToArray();
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
