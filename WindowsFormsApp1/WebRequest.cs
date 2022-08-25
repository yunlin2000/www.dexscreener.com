using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LC5
{
   public class myWebRequest
    {

        public static Image Load(string url)
        {
            WebClient webClient = new WebClient();
            var buf = webClient.DownloadData(url);
            Stream stream = new MemoryStream(buf);
            var img=Image.FromStream(stream);
            return img;
        }
        public static string Load(string url,Dictionary<string,CefSharp.Cookie> cookies,string ContentType,string Accept,string UserAgent)
        {
            string cookiestr = string.Join(";", cookies.Values.Select(x => x.Name + ":" + x.Value));
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            myHttpWebRequest.Method = "GET";
            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            myHttpWebRequest.Timeout = 20 * 1000; //连接超时
            myHttpWebRequest.Accept = "*/*";
            myHttpWebRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0;)";
           // myHttpWebRequest.Headers.Add("Cookie", cookiestr);
            myHttpWebRequest.CookieContainer = new CookieContainer();
            foreach (var item in cookies)
            {

                try
                {

                    Cookie myCookie = new Cookie(item.Value.Name, item.Value.Value, item.Value.Path, item.Value.Domain);
                    myCookie.HttpOnly = item.Value.HttpOnly;

                    myCookie.Secure = item.Value.Secure;

                    myCookie.Expires = item.Value.Expires??DateTime.Now;
                    // Add the cookie 
                    myHttpWebRequest.CookieContainer.Add(myCookie);
                }
                catch (Exception ex)
                {
                    
                }
            }

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream stream = myHttpWebResponse.GetResponseStream();
            stream.ReadTimeout = 15 * 1000; //读取超时
            StreamReader sr = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
            string strWebData = sr.ReadToEnd();
            return strWebData;
        }


        public static string Get(
               string strUrl,
               CookieContainer _cookie = null,
               string strHost = "",
               string strRefer = "",
               string strOrigin = "",
               bool blnHttps = true,
               Dictionary<string, string> lstHeads = null,
               bool blnKeepAlive = false,
               string strEncoding = "utf-8",
               string strContentType = "",
               string strCertFile = "",
               string strAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
               string strUserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36",
               bool blnAllowAutoRedirect = true,
               int intTimeout = 1000 * 30, bool isbase64=false)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            request = (HttpWebRequest)WebRequest.Create(strUrl);
            if (blnHttps)
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request.ProtocolVersion = HttpVersion.Version11;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            }
            request.KeepAlive = blnKeepAlive;
            request.Accept = strAccept;
            request.Timeout = intTimeout;
            request.Method = "GET";

            request.Credentials = CredentialCache.DefaultCredentials;
            request.UserAgent = strUserAgent;
            request.AllowAutoRedirect = blnAllowAutoRedirect;
            request.Proxy = null;
            request.CookieContainer = new CookieContainer();
            if (!string.IsNullOrEmpty(strContentType))
            {
                request.ContentType = strContentType;
            }
            if (_cookie != null)
            {
                request.CookieContainer = _cookie;
            }
            if (!string.IsNullOrEmpty(strHost))
            {
                request.Host = strHost;
            }
            if (!string.IsNullOrEmpty(strRefer))
            {
                request.Referer = strRefer;
            }
            if (!string.IsNullOrEmpty(strOrigin))
            {
                request.Headers.Add("Origin", strOrigin);
            }

            if (lstHeads != null && lstHeads.Count > 0)
            {
                foreach (var item in lstHeads)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            response = (HttpWebResponse)request.GetResponse();
            if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"] == "br")
            {
                using (var sxr = response.GetResponseStream())
                {

                    byte[] byts = null;
                    List<byte> bbts = new List<byte>();
                    int count = 0;
                    while (true)
                    {
                        byte[] b = new byte[1024];
                        int i = sxr.Read(b, count++ * 0, 1024);
                        if (i > 0)
                        {
                            bbts.AddRange(b.Take(i).ToArray());
                        }
                        else break;

                    }
                    if (isbase64)
                    {
                        var base64 = LC5.Help.ByteHelper.ByteToStr(bbts.ToArray());
                        byts = LC5.Help.ByteHelper.Base64ByteToByte(base64.Trim('\"'));
                        if (byts == null) return null;
                        try
                        {
                            var nbyts = LC5.Help.Rotli.D(byts.ToArray());
                            var s = LC5.Help.ByteHelper.ByteToStr(nbyts);
                            return s;
                        }
                        catch(Exception xe)
                        {

                        }
                        return LC5.Help.ByteHelper.ByteToStr(byts.ToArray());
                    }
                    else
                    {
                        var nbyts = LC5.Help.Rotli.D(bbts.ToArray());
                        var s = LC5.Help.ByteHelper.ByteToStr(nbyts);
                        return s;

                    }

                
                }
            }
           else if (response.Headers["Content-Encoding"] != null && response.Headers["Content-Encoding"] == "gzip")
            {
                using (var sxr = response.GetResponseStream())
                {

                    byte[] byts = null;
                    List<byte> bbts = new List<byte>();
                    int count = 0;
                    while (true)
                    {
                        byte[] b = new byte[1024];
                        int i = sxr.Read(b, count++ * 0, 1024);
                        if (i > 0)
                        {
                            bbts.AddRange(b.Take(i).ToArray());
                        }
                        else break;

                    }
                    if (isbase64)
                    {
                        var base64 = LC5.Help.ByteHelper.ByteToStr(bbts.ToArray());
                        byts = LC5.Help.ByteHelper.Base64ByteToByte(base64.Trim('\"'));
                        if (byts == null) return null;
                        try
                        {

                            var nbyts = LC5.Help.Gzip.D(byts.ToArray());
                            if (nbyts == null) return null;
                            var s = LC5.Help.ByteHelper.ByteToStr(nbyts);
                            return s;
                        }
                        catch (Exception xe)
                        {

                        }
                        return null;
                    }
                    else
                    {
                        var nbyts = LC5.Help.Rotli.D(bbts.ToArray());
                        var s = LC5.Help.ByteHelper.ByteToStr(nbyts);
                        return s;

                    }


                }
            }
            else
            {

                var sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(strEncoding));
                string strResult = sr.ReadToEnd();
                sr.Close();
                request.Abort();
                response.Close();
                return strResult;
            }

        }
            private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            {
                return true; //总是接受
            }

        }
    
}
