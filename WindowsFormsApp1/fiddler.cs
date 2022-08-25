using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LC5.Help;
using LC5;
namespace 抖音数据筛选
{
    public class Fiddler2
    {
        //https代理
        Proxy oSecureEndpoint;
        string sSecureEndpointHostname = "localhost";
       // public int iSecureEndpointPort { get; set; } = 6512;
       // public int ProxyPoint { get; set; } = 6511;
        public  Action<string> websocketaction;
        public Action<string,string> OnResponse;
        public Action<string, byte[]> OnResponseByte;
        public static List<string> filterUrlEnd = new List<string>();
        public static List<string> filterUrlStart = new List<string>();
        public static List<Tuple<string,string>> filterResponseStart = new List<Tuple<string, string>>();
        public static List<Tuple<string, string>> filterResponseEnd = new List<Tuple<string, string>>();
        public static bool Dycatpcha = false;
        public static bool IsRequeryBody = false;
        public static bool IsResponseBody = false;
        public static bool IsResponseHeaders = false;
        public static bool IsRequestHeaders = false;
        public int CoutCount = 0;
        /// <summary>
        /// 安装、获取证书
        /// </summary>
        static void InstallCertificate()
        {
            //生成证书
          CertMaker.createRootCert();

        //   var oRootcert = CertMaker.FindCert("www.fiddler2.com");
            //获取证书
            X509Certificate2 oRootcert = CertMaker.GetRootCertificate();

            //把证书安装到受信任的根证书颁发机构
            X509Store certStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            certStore.Open(OpenFlags.ReadWrite);

            try
            {
                certStore.Add(oRootcert);
            }
            finally
            {

                certStore.Close();
            }
            //证书赋值
            FiddlerApplication.oDefaultClientCertificate = oRootcert;
            //在解密HTTPS通信时，控制服务器证书错误是否被忽略。
            CONFIG.IgnoreServerCertErrors = true;
        }
        /// <summary>
        /// 启动解密
        /// </summary>
        /// <returns></returns>
        public bool Start(int ProxyPoint)
        {

            try
            {

                //Control.CheckForIllegalCrossThreadCalls = false;
                //设置别名
                Fiddler.FiddlerApplication.SetAppDisplayName("FiddlerCoreDemoApp");

                //日志
                FiddlerApplication.Log.OnLogString += (sender, msgevt) =>
                {
                    Console.WriteLine(msgevt.LogString);
                };



                //启动方式
                FiddlerCoreStartupFlags oFCSF = FiddlerCoreStartupFlags.DecryptSSL | FiddlerCoreStartupFlags.Default;

                //定义http代理端口
                //int ProxyPoint = 8877;
                //启动代理程序，开始监听http请求
                //端口,是否使用windows系统代理（如果为true，系统所有的http访问都会使用该代理）我使用的是

                FiddlerApplication.Prefs.SetBoolPref("fiddler.network.streaming.abortifclientaborts", true);
                InstallCertificate();
                Fiddler.FiddlerApplication.Startup(ProxyPoint, false, true, true);//第二个参数开启全局代理,所有连接都通过此上网

                // 我们还将创建一个HTTPS监听器，当FiddlerCore被伪装成HTTPS服务器有用
                // 而不是作为一个正常的CERN样式代理服务器。
                // oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(iSecureEndpointPort, true, sSecureEndpointHostname);


                List<Fiddler.Session> oAllSessions = new List<Fiddler.Session>();

                ////请求出错时处理
                //Fiddler.FiddlerApplication.BeforeReturningError += FiddlerApplication_BeforeReturningError;

                //在发送请求之前执行的操作
                Fiddler.FiddlerApplication.BeforeRequest += delegate (Fiddler.Session oS)
                {
                //请求的全路径
                // Console.WriteLine("Before request for:\t" + oS.fullUrl);
                // 为了使反应篡改，必须使用缓冲模式
                // 被启用。这允许FiddlerCore以允许修改
                // 在BeforeResponse处理程序中的反应，而不是流
                // 响应给客户机作为响应进来。
                // string rbody = System.Text.Encoding.Default.GetString(oS.ResponseBody);
                // Console.WriteLine(rbody);
                oS.bBufferResponse = true;
                    Monitor.Enter(oAllSessions);
                    oAllSessions.Add(oS);
                    Monitor.Exit(oAllSessions);

                    oS["X-AutoAuth"] = "(default)";


                //"显示" heads responseboy url
                string showstr = oS.fullUrl + "\r\n";

                    if (IsRequestHeaders && oS.RequestHeaders.Count() > 0)
                    {
                        string headsreq = string.Join(",", oS.RequestHeaders.Select(x => x.Name + "=" + x.Value));
                        showstr += "requestheads:[" + headsreq + "]\r\n-------------------------\r\n";
                    }
                    if (IsResponseHeaders && oS.ResponseHeaders.Count() > 0)
                    {
                        string headsrsp = string.Join(",", oS.ResponseHeaders.Select(x => x.Name + "=" + x.Value));
                        showstr += "responseheads:[" + headsrsp + "]\r\n-------------------------\r\n";
                    }
                    if (IsRequeryBody && oS.RequestBody.Length > 0)
                    {
                        showstr += "reqbody:[" + oS.GetRequestBodyAsString() + "]\r\n-------------------------\r\n";
                    }
                    if (IsResponseBody && oS.ResponseBody.Length > 0)
                    {
                        showstr += "rsqbody:[" + oS.GetRequestBodyAsString() + "]\r\n-------------------------\r\n";
                    }
                    Console.WriteLine(showstr + "-----------------\r\n");
                /* 如果请求是要我们的安全的端点，我们将回显应答。
                
                注：此BeforeRequest是越来越要求我们两个主隧道代理和我们的安全的端点，
                让我们来看看它的Fiddler端口连接到（pipeClient.LocalPort）客户端，以确定是否该请求
                被发送到安全端点，或为了达到**安全端点被仅仅发送到主代理隧道（例如，一个CONNECT）。
                因此，如果你运行演示和参观的https：//本地主机：7777在浏览器中，你会看到
                Session list contains...
                 
                    1 CONNECT http://localhost:7777
                    200                                         <-- CONNECT tunnel sent to the main proxy tunnel, port 8877
                    2 GET https://localhost:7777/
                    200 text/html                               <-- GET request decrypted on the main proxy tunnel, port 8877
                    3 GET https://localhost:7777/               
                    200 text/html                               <-- GET request received by the secure endpoint, port 7777
                */

                //oS.utilCreateResponseAndBypassServer();
                //oS.oResponse.headers.SetStatus(200, "Ok");
                //string str = oS.GetResponseBodyAsString();
                //oS.utilSetResponseBody(str + "aaaaaaaaaaaaaaaaaaaaa");

                //if ((oS.oRequest.pipeClient.LocalPort == iSecureEndpointPort) && (oS.hostname == sSecureEndpointHostname))
                //{
                //    oS.utilCreateResponseAndBypassServer();
                //    oS.oResponse.headers.SetStatus(200, "Ok");
                //    oS.oResponse["Content-Type"] = "text/html; charset=UTF-8";
                //    oS.oResponse["Cache-Control"] = "private, max-age=0";
                //    oS.utilSetResponseBody("<html><body>Request for httpS://" + sSecureEndpointHostname + ":" + iSecureEndpointPort.ToString() + " received. Your request was:<br /><plaintext>" + oS.oRequest.headers.ToString());
                //}
                //if ((oS.oRequest.pipeClient.LocalPort == 8877) && (oS.hostname == "www.baidu.com"))
                //{
                //    string url = oS.fullUrl;
                //    oS.utilCreateResponseAndBypassServer();
                //    oS.oResponse.headers.SetStatus(200, "Ok");
                //    oS.oResponse["Content-Type"] = "text/html; charset=UTF-8";
                //    oS.oResponse["Cache-Control"] = "private, max-age=0";
                //    oS.utilSetResponseBody("<html><body>Request for httpS://" + sSecureEndpointHostname + ":" + iSecureEndpointPort.ToString() + " received. Your request was:<br /><plaintext>" + oS.oRequest.headers.ToString());
                //}
            };

                /*
                    // 下面的事件，您可以检查由Fiddler阅读每一响应缓冲区。  
                 *     请注意，这不是为绝大多数应用非常有用，因为原始缓冲区几乎是无用的;它没有解压，它包括标题和正文字节数等。
                    //
                    // 本次仅适用于极少数的应用程序这就需要一个原始的，未经处理的字节流获取有用
                    Fiddler.FiddlerApplication.OnReadResponseBuffer += new EventHandler<RawReadEventArgs>(FiddlerApplication_OnReadResponseBuffer);
                */
                Fiddler.FiddlerApplication.OnWebSocketMessage += (sender, msgevn) =>
                {
                   
                };


                Fiddler.FiddlerApplication.BeforeResponse += delegate (Fiddler.Session oS)
                {


                    
                    //string s = oS.GetResponseBodyAsString();
                    //OnResponse?.Invoke(oS.url, s);
                    //return;

                    //string  line = $"Content-Encoding:{oS.ResponseHeaders["Content-Encoding"]},responseBodyBytes.Length:{oS.responseBodyBytes.Length },url:{oS.url}";
                    //    line.WriteLog();

                    if (oS.url.IndexOf("catpcha.byteimg.com") >= 0)
                    {
                        Dycatpcha = true;
                        return;
                    }
                    if (oS.url.StartsWith("verify.snssdk.com/captcha/verify"))
                    {
                        Dycatpcha = false;
                        return;
                    }
                 
                    //if (oS.responseBodyBytes.Length == 0) return;
                    if (!Filter(oS.url)) return;
                    if (!Filter(oS.ResponseHeaders)) return;


                    // line = $"Content-Encoding:{oS.ResponseHeaders["Content-Encoding"]},responseBodyBytes.Length:{oS.responseBodyBytes.Length},url:{oS.url}";
                    //line.WriteLog();
                    /*
                    if (oS.url.StartsWith("live.douyin.com"))
                    {
                        try
                        {
                            if (oS.ResponseBody.Count() == 13) return;
                            var bytes = Help.Rotli.D(oS.responseBodyBytes);
                            if(bytes.Count()>0)
                            Console.WriteLine(System.Text.UTF8Encoding.Default.GetString(bytes));
                        }
                        catch (Exception)
                        {

                        }
                        return;
                    }
                    if(oS.url.StartsWith("mcs.snssdk.com/v1/list"))//将收到{"e":0}信息
                    {
                        CoutCount = 10;
                        var bytes = oS.responseBodyBytes;
                        Console.WriteLine("-----------------start\r\n");
                        //var bbs= Help.Gzip.D(bytes);
                        Console.WriteLine(oS.GetResponseBodyAsString()+"="+string.Join(" ", bytes.Select(x => x.ToString("X2"))));
                        // if(bbs!=null)  Console.WriteLine(string.Join(" ",bbs.Select(x=>x.ToString("X2"))));
                        Console.WriteLine("-----------------end\r\n");
                    }
                    */
                    try
                    {
                        if (oS.url.StartsWith("io.dexscreener.com/u/trading-history/recent/bsc"))
                        {
                            Console.WriteLine(oS.url );
                            Console.WriteLine( oS.ResponseHeaders["Content-Encoding"]);
                        }
                        if (oS.ResponseHeaders["Content-Encoding"] == "br")
                        {
                            "BeforeResponse br".WriteLog();
                            var bs = Rotli.readBrBody(oS.responseBodyBytes);
                            if (bs == null || bs.Count() == 0)
                            {
                                bs = oS.responseBodyBytes;
                            }
                            var nbs = Rotli.D(bs);

                         
                            nbs = nbs.Where(x => x != 0).ToArray();
                            string rets = System.Text.Encoding.UTF8.GetString(nbs);
                            string hex = string.Join(" ", nbs.Select(x => x.ToString("X2")));
                            //string rets= WebSocketSharp.Net.HttpUtility.UrlDecode(nbs,System.Text.Encoding.UTF8);


                            OnResponse?.Invoke(oS.url, rets);

                            //string btssd = string.Join(" ", nbs.Select(x => x.ToString("X2")));
                            return;
                        }
                        if(oS.ResponseHeaders["Content-Encoding"] == "gzip")
                        {

                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    string s = oS.GetResponseBodyAsString();
                    OnResponse?.Invoke(oS.url, s);
                // Console.WriteLine(string.Format("Response:{0}", s));
            };
                Fiddler.FiddlerApplication.OnReadResponseBuffer += (sender, msgevn) =>
                {

                    var bytes = msgevn.arrDataBuffer.Take(msgevn.iCountOfBytes).ToArray();
                    //Console.WriteLine(string.Join(" ", bytes.Select(x => x.ToString("X2"))));

                };
                return true;
            }
            catch (Exception)
            {

            }
            return false;

        }
     
        private void FiddlerApplication_OnReadResponseBuffer(object sender, RawReadEventArgs e)
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            if (null != oSecureEndpoint) oSecureEndpoint.Dispose();
            Fiddler.FiddlerApplication.Shutdown();
            return true;
        }
        public static bool Filter(string url)
        {
            foreach (var item in filterUrlStart)
            {
                if (url.StartsWith(item)) return false;
            }
            foreach (var item in filterUrlEnd)
            {
                if (url.EndsWith(item)) return false;
            }
            return true;
        }
        public static bool Filter(HTTPResponseHeaders responseHeaders)
        {
            foreach (var item in filterResponseStart)
            {
                try
                {
                    if (responseHeaders[item.Item1].StartsWith(item.Item2))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    
                }
            }
            foreach (var item in filterResponseEnd)
            {
                try
                {
                    if (responseHeaders[item.Item1].EndsWith(item.Item2))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                }
            }
            return true;
        }
    }
}
