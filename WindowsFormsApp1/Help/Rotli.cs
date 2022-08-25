using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LC5.Help
{
    public class Rotli
    {

        public static byte[] E(byte[] bytes)
        {
            try
            {
                return BrotliSharpLib.Brotli.CompressBuffer(bytes.ToArray(), 0, bytes.Count());

            }
            catch (Exception ex)
            {

            }
            return null;
        }
        public static int bytes2Int(byte[] bytes)
        {
            int num = 0;
            foreach (var item in bytes)
            {
                if (item >= 48 && item <= 57)
                {
                    num = num * 16 + item - '0';
                }
                else
                    if (item >= 'A' && item <= 'F')
                {
                    num = num * 16 + item - 'A' + 10;
                }
                if (item >= 'a' && item <= 'f')
                {
                    num = num * 16 + item - 'a' + 10;
                }
            }

            return num;
        }
        public static byte[] D(byte[] bytes)
        {
            try
            {

                return BrotliSharpLib.Brotli.DecompressBuffer(bytes, 0, bytes.Length);
            }
            catch (Exception)
            {

            }
            return null;
        }
        public static byte[] D1(byte[] bytes)
        {
            try
            {

                int starindex = 0;
                string hex = "";
                for (int i = 0; i < bytes.Count(); i++)
                {
                    if (i + 1 < bytes.Count() && bytes[i] == 13 && bytes[i + 1] == 10)
                    {
                        starindex = i + 2;
                        break;
                    }

                }

                int len = bytes2Int(bytes.Take(starindex - 2).ToArray());
                return BrotliSharpLib.Brotli.DecompressBuffer(bytes, starindex, len);

            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static byte[] readBrBody(byte[] bytes)
        {
            List<byte> bs = new List<byte>();
            int start = 0;

            while (start < bytes.Length)
            {
                int starindex = start;
                for (int i = start; i < bytes.Count(); i++)
                {
                    if (i + 1 < bytes.Count() && bytes[i] == 13 && bytes[i + 1] == 10)
                    {
                        starindex = i + 2;
                        break;
                    }
                }
                int len = bytes2Int(bytes.Skip(start).Take(starindex - 2 - start).ToArray());
                if (len == 0) break;
                bs.AddRange(bytes.Skip(starindex).Take(len));
                start = starindex + len + 2;
            }
            return bs.ToArray();
        }
    }
}
