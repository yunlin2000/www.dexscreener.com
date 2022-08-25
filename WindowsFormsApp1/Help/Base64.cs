using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZIP工具.Help
{
  public  class Base64
    {
        public static string Es(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        public static string Eb(string code_type, Byte[] bytes)
        {
            string encode = "";
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
            }
            return encode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code_type">utf-8</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string Ds(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                if (code_type == "Default")
                {
                    decode = Encoding.Default.GetString(bytes);
                }else

                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch( Exception ex)
            {
                decode = code;
            }
            return decode;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code_type">utf-8</param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] Db(string code_type, string code)
        {
            byte[] bytes = Convert.FromBase64String(code);          
            return bytes;
        }
    }
}
