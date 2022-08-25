using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gcashface.EDCode
{
  public class UrlHelper
    {
        /// <summary>
        /// 对字符进行UrlEncode编码
        /// string转Encoding格式
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encod">编码格式</param>
        /// <param name="cap">是否输出大写字母</param>
        /// <returns></returns>
        public static string UrlEncode(string text, Encoding encod, bool cap = true)
        {
            if (cap)
            {
                StringBuilder builder = new StringBuilder();
                foreach (char c in text)
                {
                    if (System.Web.HttpUtility.UrlEncode(c.ToString(), encod).Length > 1)
                    {
                        builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString(), encod).ToUpper());
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
                return builder.ToString();
            }
            else
            {
                string encodString = System.Web.HttpUtility.UrlEncode(text, encod);
                return encodString;
            }
        }

        /// <summary>
        /// 对字符进行UrlDecode解码
        /// Encoding转string格式
        /// </summary>
        /// <param name="encodString"></param>
        /// <param name="encod">编码格式</param>
        /// <returns></returns>
        public static string UrlDecode(string encodString, Encoding encod)
        {
            string text = System.Web.HttpUtility.UrlDecode(encodString, encod);
            return text;
        }
    }
}
