using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZIP工具.Help
{
  public static  class  StringExtention
    {
        public static Byte[] HexToByte(this string hex)
        {
            hex = hex.Replace("\r", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace( " ", "");
            List<byte> bs = new List<byte>();
            for (int i = 0; i < hex.Length; i += 2)
            {
                if (i >= hex.Length) break;
                var ret = Int32.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                byte b = (byte)(ret % 256);
                bs.Add(b);
            }
            return bs.ToArray();
        }
        public static Byte[] toBytes(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }


        public static string GetPhone(this string nickname)
        {
            if (string.IsNullOrEmpty(nickname)) return null;
            List<string> vs = new List<string>();
            if (string.IsNullOrEmpty(nickname) == false)
            {
                var nums = System.Text.RegularExpressions.Regex.Replace(nickname, "[^0-9]+", ",");
                StringBuilder rsSb = new StringBuilder(",");

                string[] numAry = nums.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in numAry)
                {
                    if (item.Length != 11) continue;
                    if (item[0] == '1' && item[1] >= '3') vs.Add(item);

                }
            }
            return string.Join(",", vs);
        }
    }
}
