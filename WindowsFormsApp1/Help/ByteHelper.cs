using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC5.Help
{
    public class ByteHelper
    {
        public static string StrToBase64(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
           // var bts = StrToByte(base64);
            return ByteTostr_Base64(bytes);
        }
        public static byte[] Base64ToByte(string base64)
        {
            return StrToByte(base64);
        }

        public static MemoryStream ByteToStream(byte [] bt)
        {
            MemoryStream ms = new MemoryStream(bt);
            ms.Read(bt, 0, bt.Count());
            return ms;
        }

        public static string ByteTostr_Base64(byte[] bt)
        {
            return Convert.ToBase64String(bt);
        }
        public static string ByteToStr(byte[] bt)
        {
            return System.Text.Encoding.UTF8.GetString(bt);
        }
        public static byte[] StrToByte(string s)
        {
            return System.Text.Encoding.UTF8.GetBytes(s);
        }
        public static byte[] Base64ByteToByte(string base64)
        {
           return Convert.FromBase64String(base64);
        }
    }
}
