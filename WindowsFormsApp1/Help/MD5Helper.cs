using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Gcashface.EDCode
{
   public  class MD5Helper
    {
        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string encrypt(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }
        /// <summary>
        /// MD5加密(16位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string encrypt16(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string encrypt(byte[] btys )
        {
            MD5 md5 = MD5.Create();
            string pwd = "";
            byte[] s = md5.ComputeHash(btys);
            for (int i = 0; i < s.Length; i++)
            {
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }
        /// <summary>
        /// MD5加密(16位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string encrypt16(byte[] btys)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(btys), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }
    }
}
