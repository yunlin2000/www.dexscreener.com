using CefSharp;
using CefSharp.Handler;
using CefSharp.Structs;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app
{
    public class CefLifeSpanHandler : CefSharp.ILifeSpanHandler
    {

        public CefLifeSpanHandler()
        {

        }

        public bool DoClose(IWebBrowser browserControl, CefSharp.IBrowser browser)
        {
            if (browser.IsDisposed || browser.IsPopup)
            {
                return false;
            }

            return true;
        }

        public void OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            var _chromiumWebBrowser = (Chrome)chromiumWebBrowser;

            _chromiumWebBrowser.Invoke(new Action(() =>
            {
                NewWindowEventArgs e = new NewWindowEventArgs(windowInfo, targetUrl);
                _chromiumWebBrowser.OnNewWindow(e);
            }));

            newBrowser = null;
            return true;
        }

    }

    public class MenuHandler : IContextMenuHandler
    {
        void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            //主要修改代码在此处;如果需要完完全全重新添加菜单项,首先执行model.Clear()清空菜单列表即可.
            //需要自定义菜单项的,可以在这里添加按钮;
            if (model.Count > 0)
            {
                model.AddSeparator();//添加分隔符;
            }
            model.AddItem((CefMenuCommand)26501, "Show DevTools");
            model.AddItem((CefMenuCommand)26502, "Close DevTools");
            model.AddItem((CefMenuCommand)26503, "Find");
        }

        bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            //命令的执行,点击菜单做什么事写在这里.
            if (commandId == (CefMenuCommand)26501)
            {
                browser.GetHost().ShowDevTools();
                return true;
            }
            if (commandId == (CefMenuCommand)26502)
            {
                browser.GetHost().CloseDevTools();
                return true;
            }
            if (commandId == (CefMenuCommand)26503)
            {
                browser.GetHost().ShowDevTools(null, parameters.XCoord, parameters.YCoord);
                return true;
            }

            return false;
        }

        void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            var webBrowser = (ChromiumWebBrowser)browserControl;
            Action setContextAction = delegate ()
            {
                webBrowser.ContextMenu = null;
            };
            webBrowser.Invoke(setContextAction);
        }

        bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            //return false 才可以弹出;
            return false;
        }

        //下面这个官网Example的Fun,读取已有菜单项列表时候,实现的IEnumerable,如果不需要,完全可以注释掉;不属于IContextMenuHandler接口规定的
        private static IEnumerable<Tuple<string, CefMenuCommand, bool>> GetMenuItems(IMenuModel model)
        {
            for (var i = 0; i < model.Count; i++)
            {
                var header = model.GetLabelAt(i);
                var commandId = model.GetCommandIdAt(i);
                var isEnabled = model.IsEnabledAt(i);
                yield return new Tuple<string, CefMenuCommand, bool>(header, commandId, isEnabled);
            }
        }
    }
    public class rqh:CefSharp.Handler.RequestContextHandler
    {

    }
    public class MyResourceRequestHandler : ResourceRequestHandler
    {
        //    protected override IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        //    {

        //        //if (request.Url.EndsWith("test1.js") || request.Url.EndsWith("test1.css"))
        //        //{
        //        //    MessageBox.Show($@"资源拦截：{request.Url}");

        //        //    string type = request.Url.EndsWith(".js") ? "js" : "css"; // 这里简单判断js还是css，不过多编写
        //        //    string fileName = null;
        //        //    using (OpenFileDialog openFileDialog = new OpenFileDialog())
        //        //    {
        //        //        openFileDialog.Filter = $@"{type}文件|*.{type}"; // 过滤
        //        //        openFileDialog.Multiselect = true;
        //        //        if (openFileDialog.ShowDialog() == DialogResult.OK)
        //        //        {
        //        //            fileName = openFileDialog.FileName;
        //        //        }
        //        //    }

        //        //    if (string.IsNullOrWhiteSpace(fileName))
        //        //    {
        //        //        // 没有选择文件，还是走默认的Handler
        //        //        return base.GetResourceHandler(chromiumWebBrowser, browser, frame, request);
        //        //    }
        //        //    // 否则使用选择的资源返回
        //        //    return new ResourceHandler(fileName);
        //        //}

        //        return base.GetResourceHandler(chromiumWebBrowser, browser, frame, request);
        //    }
        protected override CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            return CefReturnValue.Continue;
            if (request.ResourceType == ResourceType.Image)
            {
                if (request.Url.IndexOf("catpcha.byteimg.com") >= 0)
                {
                    return CefReturnValue.Continue;
                }
                if (request.Url.StartsWith("https://verify.snssdk.com/captcha/verify") || request.Url.StartsWith("https://catpcha.byteimg.com"))
                {
                    return CefReturnValue.Continue;
                }
                return CefReturnValue.Cancel; ;
            }
            if (request.ResourceType == ResourceType.Media)
            {
                return CefReturnValue.Cancel;
            }
            if (request.Url.StartsWith("https://www.douyin.com/aweme/v1/web/emoji"))
            {
                return CefReturnValue.Cancel;
            }
            return CefReturnValue.Continue;

        }
    }


    public class RQHandle : IRequestHandler
    {
        MyResourceRequestHandler handle= new MyResourceRequestHandler();
        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            return true;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {

            #region 过滤资源代码
            if (request.ResourceType == ResourceType.Image)
            {
                if(request.Url.IndexOf("catpcha.byteimg.com")>=0)
                {
                    return null;
                }
                if (request.Url.StartsWith("https://verify.snssdk.com/captcha/verify") || request.Url.StartsWith("https://catpcha.byteimg.com"))
                {
                    return null;
                }
                return handle;
            }
            if (request.ResourceType == ResourceType.Media)
            {
                return handle;
            }
            if (request.Url.StartsWith("https://www.douyin.com/aweme/v1/web/emoji"))
            {
                return handle;
            }

            #endregion
            return null;

        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {

        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {

        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return true;
        }
    }
    public class CookieVisitor : CefSharp.ICookieVisitor
    {
        public event Action<CefSharp.Cookie> SendCookie;

        public void Dispose()
        {

        }

        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            if (SendCookie != null)
            {
                SendCookie(cookie);
            }

            return true;
        }
      
    }
    public class Chrome : ChromiumWebBrowser
    {
        public Chrome()
            : base()
        {


            this.LifeSpanHandler = new CefLifeSpanHandler();
            this.RequestHandler = new RQHandle();
            
            this.MenuHandler = new MenuHandler();
            
        }

        public Chrome(string url)
            : base(url)
        {



            this.LifeSpanHandler = new CefLifeSpanHandler();
            this.RequestHandler = new RQHandle();
            this.MenuHandler = new MenuHandler();

            
        }

        /// <summary>
        /// 用户隔离
        /// </summary>
        /// <param name="url"></param>
        /// <param name="context"></param>
        /// <param name="accountName"></param>
        public Chrome(string url, RequestContext context)
            : base(url, context)
        {
            this.LifeSpanHandler = new CefLifeSpanHandler();
            this.RequestHandler = new RQHandle();
            this.MenuHandler = new MenuHandler();
        }
        public event EventHandler<NewWindowEventArgs> StartNewWindow;


        public void OnNewWindow(NewWindowEventArgs e)
        {
            if (StartNewWindow != null)
            {
                StartNewWindow(this, e);
            }
        }
    }
    public class NewWindowEventArgs : EventArgs
    {
        private IWindowInfo _windowInfo;
        public IWindowInfo WindowInfo
        {
            get { return _windowInfo; }
            set { value = _windowInfo; }
        }


        public string url { get; set; }

        public NewWindowEventArgs(IWindowInfo windowInfo, string url)

        {

            _windowInfo = windowInfo;

            this.url = url;

        }
    }

    public class ChromeAction
    {
        public CefSharp.CefState defaultDoldImg { get; set; } = CefState.Enabled;
        public IntPtr Handle()
        {
            if (null == chrome) return IntPtr.Zero;
            return chrome.Handle;
        }
        Chrome chrome = null;
        private bool binit = false;

        internal bool IsLoadding()
        {
            if (chrome != null)
            {
                try
                {

                    return chrome.GetBrowser().IsLoading;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }

        public void Init(string accountName = "default", string proxyServer = null)
        {
            try
            {
 
                string path = Application.StartupPath + "\\AppCaches\\" + accountName;

              if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var setting = new CefSharp.WinForms.CefSettings
                {
                    Locale = "zh-CN"
                };
                setting.CachePath = path;
                setting.CefCommandLineArgs.Add("disable-gpu", "0");
                if (!string.IsNullOrEmpty(proxyServer))
                    setting.CefCommandLineArgs.Add("proxy-server", proxyServer);
                setting.PersistSessionCookies = true;
                
                setting.CefCommandLineArgs.Add("disable-application-cache", "1");
                setting.CefCommandLineArgs.Add("disable-session-storage", "1");
                //必须进行初始化，否则就出来页面啦。
                setting.CefCommandLineArgs.Add("autoplay-policy", "no-user-gesture-required");
                CefSharp.Cef.Initialize(setting);
                binit = true;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    
        public ICookieManager GetCookieManager()
        {
           return chrome.GetCookieManager();
        }
        public void Cretate(Control control, int itype, EventHandler<NewWindowEventArgs> StartNewWindow, EventHandler<FrameLoadEndEventArgs> FrameLoadEnd)
        {
            if (chrome == null)
            {

                chrome = new Chrome();// 
                chrome.BrowserSettings.ImageLoading = defaultDoldImg;
                if (StartNewWindow != null)
                    chrome.StartNewWindow += StartNewWindow;
                if (FrameLoadEnd != null)
                    if (itype == 1)
                    {
                        chrome.Dock = DockStyle.Fill;
                    }
                    else if (itype == 2)
                    {
                        chrome.Width = 0;
                        chrome.Height = 0;
                    }
                control.Controls.Add(chrome);

            }
        }
        public void Cretate(Control control, int itype, EventHandler<NewWindowEventArgs> StartNewWindow, EventHandler<FrameLoadEndEventArgs> FrameLoadEnd, string url, string accountName)
        {
            if (chrome == null)
            {
                if (!System.IO.Directory.Exists("AppCaches/" + accountName))
                    System.IO.Directory.CreateDirectory("AppCaches/" + accountName);
                var cp = Application.StartupPath + "\\AppCaches\\" + accountName;
                RequestContext context = new RequestContext(new RequestContextSettings()
                {
                    CachePath = cp,
                    PersistSessionCookies = true,//保存session cookie
                    PersistUserPreferences = false,//保留用户首选项                    
                });

                chrome = new Chrome(url, context);// 

                chrome.BrowserSettings.ImageLoading = defaultDoldImg;
                if (StartNewWindow != null)
                    chrome.StartNewWindow += StartNewWindow;
                if (FrameLoadEnd != null)
                    chrome.FrameLoadEnd += FrameLoadEnd;
                if (itype == 1)
                {
                    chrome.Dock = DockStyle.Fill;
                }
                else if (itype == 2)
                {
                    chrome.Width = 0;
                    chrome.Height = 0;
                }


                control.Controls.Add(chrome);

            }
        }



        public void ReCretate(Control control, int itype, EventHandler<NewWindowEventArgs> StartNewWindow, EventHandler<FrameLoadEndEventArgs> FrameLoadEnd)
        {
            try
            {

                chrome.Dispose();
            }
            catch (Exception)
            {
                
            }
            chrome = null;
            Cretate(control,  itype,  StartNewWindow,FrameLoadEnd);
        }
        public void ReCretate(Control control, int itype, EventHandler<NewWindowEventArgs> StartNewWindow, EventHandler<FrameLoadEndEventArgs> FrameLoadEnd, string url, string accountName)
        {
            try
            {

                chrome.Dispose();
            }
            catch (Exception)
            {

            }
            chrome = null;

            Cretate(control, itype, StartNewWindow, FrameLoadEnd, url, accountName);
        }
        public void Input(string tgname, string pname, string value, string input, int frameIndex = -1)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;


                StringBuilder sb = new StringBuilder();
                //  sb.Append(File.ReadAllText("jquery.min.js"));
                sb.AppendLine("function Input() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                //sb.AppendLine(" $(\"["+pname+" = '"+value+"']\").value=\""+input+"\";");
                sb.AppendLine("  var tgn= document.getElementsByTagName('" + tgname + "');");
                sb.AppendLine("  for(var tg in tgn)");
                sb.AppendLine("  {");
                sb.AppendLine("    if(tgn[tg]['" + pname + "']=='" + value + "'){tgn[tg].value='" + input + "';return true}");
                sb.AppendLine("  ");
                sb.AppendLine("  ");
                sb.AppendLine("  ");
                sb.AppendLine("  }");

                sb.AppendLine("}");
                sb.AppendLine("Input();");


                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                }
                            }
                        }
                    });
                }
            }
        }

        internal void ShutDowm()
        {
            if(chrome!=null)
            {
                Cef.Shutdown();
            }
        }

        public void Load(string url)
        {
            if (null != chrome)
                chrome.Load(url);
        }
        public List<string> GetFramesName()
        {
            return chrome.GetBrowser().GetFrameNames();
        }
        public void GetHtml(Action<string> action, int frameIndex)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function GetHtml() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine("return document.getElementsByTagName('html')[0];");
                sb.AppendLine("}");
                sb.AppendLine("GetHtml();");


                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }
        public string GetHtml()
        {
            return onJs("function gethtml(){ return document.querySelector('body').innerHTML;}  gethtml();").Result;
        }
        public void GetHtml(Action<string> action)
        {
            onJs(action, "function gethtml(){ return document.querySelector('body').innerHTML;}  gethtml();");
        }
        public void onJs(Action<string> action, string js)
        {
            if (chrome != null)
            {

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(js);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }

            }
        }

        public async Task<string> onJs(string js)
        {
            if (chrome != null)
            {
                string result = null;
                DateTime starttime = DateTime.Now;
                int timeout = 30;
                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    var frm = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]);
                  
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(js);
                    task01.ContinueWith(t =>
                     {
                         if (!t.IsFaulted)
                         {
                             var response = t.Result;
                             if (response.Success == true)
                             {
                                 if (response.Result != null)
                                 {
                                     if (response?.Result != null && response.Result.GetType().Name == "ExpandoObject")
                                     {
                                         result = Newtonsoft.Json.JsonConvert.SerializeObject(response.Result);
                                         ;
                                     }
                                     else
                                     {
                                         string resultStr = response.Result.ToString();
                                         result = resultStr;
                                     }
                                 }
                             }
                         }
                     }).Wait();
                    if (string.IsNullOrEmpty(result) == false)
                    {
                        break;
                    }
                }

                //while ((DateTime.Now - starttime).TotalSeconds > timeout || string.IsNullOrEmpty(result)) { };
                return result;
            }
            return null;
        }

        /// <summary>
        /// 获取部分
        /// </summary>
        public void GetChildHtml(Action<string> action, string tagName, string attName, String value, int frameIndex)
        {

            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function GetHtml() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");
                sb.AppendLine("      return tgn[t].innerHTML;");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("GetHtml();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }

            }
        }
        /// <summary>
        /// 指定某个frameIndex,默认不指定
        /// </summary>
        /// <param name="add"></param>
        /// <param name="tagName"></param>
        /// <param name="frameIndex">指定某个frameIndex,默认不指定</param>
        /// <returns></returns>
        public Rect GetXY(string tagName)
        {
            string js = "function getmyxy(){var tgn= document.querySelector(\"" + tagName + "\");var cr= tgn.getBoundingClientRect(); return cr.x+','+cr.y+','+cr.width+','+cr.height;} getmyxy();";
            var xy = onJs(js).Result;
            if (xy == null)
            {
                return new Rect();
            }
            string[] xys = xy.Split(',');
            double[] r = new double[4];
            if (xys.Length == 4)
            {
                for (int i = 0; i < xys.Length; i++)
                {
                    var item = xys[i]; double d = 0;
                    if (double.TryParse(item, out d) == false)
                    {
                        return new Rect();
                    }
                    r[i] = d;
                }
                return new Rect((int)r[0], (int)r[1], (int)r[2], (int)r[3]);
            }
            return new Rect();
        }

        public void ClickUp(Action<string> action, string tagName, string attName, String value, int frameIndex, int buynum, string otherNode = "")
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");

                sb.AppendLine("      for(var i=0;i<" + (buynum - 1) + ";i++)");
                sb.AppendLine("      if(tgn[t].parentNode.parentNode.previousSibling.value<" + buynum + ")");
                sb.AppendLine("      tgn[t]" + otherNode + ".click();");

                sb.AppendLine("      return 'click';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);                  
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }

        public string getRemote(string url)
        {
            var js = @"function ajaxx(opts) {
  var xhr = new XMLHttpRequest(),
    type = opts.type || 'GET',
    url = opts.url,
    params = opts.data,
    dataType = opts.dataType || 'json';

  type = type.toUpperCase();

  if (type === 'GET') {
    params = (function(obj){
      var str = '';

      for(var prop in obj){
        str += prop + '=' + obj[prop] + '&'
      }
      str = str.slice(0, str.length - 1);
      return str;
    })(opts.data);
    url += url.indexOf('?') === -1 ? '?' + params : '&' + params;
  }

  xhr.open(type, url,false);

  if (opts.contentType) {
    xhr.setRequestHeader('Content-type', opts.contentType);
  }

  xhr.send(params ? params : null);
 var result;
        try {
          result = JSON.parse(xhr.response);
        } catch (e) {
          result = xhr.response;
        }
    
 return result;
}
var a={url:'" + url + "'};ajaxx(a);";
            return onJs(js).Result;
        }


        public string getRemote2(string url)
        {
            var js = @"var a={url:'" + url + "'};ajaxx(a);";
            return onJs(js).Result;
        }
    

        public void ClickDown(Action<string> action, string tagName, string attName, String value, int frameIndex, int buynum, string otherNode = "")
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");

                sb.AppendLine("      for(var i=0;i<" + (buynum - 1) + ";i++)");
                sb.AppendLine("      if(tgn[t].parentNode.parentNode.nextSibling.value>" + buynum + ")");
                sb.AppendLine("      tgn[t]" + otherNode + ".click();");

                sb.AppendLine("      return 'click';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }

        internal string GetUrl()
        {
            try
            {

                return chrome.GetBrowser().MainFrame.Url;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public void GetEditValue(Action<string> action, string tagName, string attName, String value, int frameIndex, int buynum, string otherNode = "")
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");


                sb.AppendLine("      return tgn[t].parentNode.parentNode.nextSibling.value;");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }

        public void GetSelectAsync(Action<string> action, string selector, string attName)
        {
            if (chrome != null)
            {

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.querySelector(\"" + selector + "\");");


                sb.AppendLine(" if(tgn){");
                sb.AppendLine("    return tgn." + attName + "; ");
                sb.AppendLine("     ");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    string resultStr = "";
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }

        public async Task<string> GetSelectAsync(string selector, string attName)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return null;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.querySelector(\"" + selector + "\");");


                sb.AppendLine(" if(tgn){");
                sb.AppendLine("    return tgn." + attName + "; ");
                sb.AppendLine("     ");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");
                DateTime start = DateTime.Now;
                int timeout = 5;
                string resultStr = "";
                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    resultStr = response.Result.ToString();

                                }
                            }
                        }
                    }).Wait();

                    if (!String.IsNullOrEmpty(resultStr))
                    {
                        break;
                    }

                }

                return resultStr;
            }
            return null;
        }

        public async Task<string> ClickAsync(string selector)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return null;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.querySelector(\"" + selector + "\");");


                sb.AppendLine(" if(tgn){");
                sb.AppendLine("    tgn.click()");
                sb.AppendLine("    return 'click'; ");
                sb.AppendLine("  }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    string resultStr = null;
                    await task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    resultStr = response.Result.ToString();

                                }
                            }
                        }
                    });
                    if (!string.IsNullOrEmpty(resultStr) && resultStr == "click")
                    {
                        return "click";
                    }
                }
            }
            return null;
        }
        public async Task<string> ClickAsync(string tagName, string attName, String value, int frameIndex, string otherNode = "")
        {

            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return null;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");
                sb.AppendLine("      tgn[t]" + otherNode + ".click();");
                sb.AppendLine("      return 'click';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    string resultStr = null;
                    await task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    resultStr = response.Result.ToString();
                                }
                            }
                        }
                    });
                    if (resultStr == "click")
                    {
                        return "click";
                    }
                }
            }
            return null;

        }
        public void Click(Action<string> action, string tagName, string attName, String value, int frameIndex, string otherNode = "")
        {

            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");
                sb.AppendLine("      tgn[t]" + otherNode + ".click();");
                sb.AppendLine("      return 'click';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    //  Console.WriteLine(i);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }

        }
        public void Submit(Action<string> action, string tagName, string attName, String value, int frameIndex)
        {

            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function Clicktg() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");
                sb.AppendLine("      tgn[t].submit;");
                sb.AppendLine("      return 'click';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("Clicktg();");

                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    //var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                    Console.WriteLine(i);
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }

        }
        public void Command(Action<string> action, string tagName, string attName, String value, string command)
        {

            if (chrome == null)
            {
                if (!chrome.IsBrowserInitialized) return;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function comm_and() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 
                sb.AppendLine(" var tgn= document.getElementsByTagName('" + tagName + "');");
                sb.AppendLine(" for(var t in tgn)");
                sb.AppendLine(" {");
                sb.AppendLine("   if(tgn[t]." + attName + "=='" + value + "')");
                sb.AppendLine("    {");
                sb.AppendLine("      " + command + ";");
                sb.AppendLine("      return 'command';");
                sb.AppendLine("    }");
                sb.AppendLine("");
                sb.AppendLine(" }");
                sb.AppendLine("}");
                sb.AppendLine("comm_and();");
                var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[0]).EvaluateScriptAsync(sb.ToString());
                task01.ContinueWith(t =>
                {
                    if (!t.IsFaulted)
                    {
                        var response = t.Result;
                        if (response.Success == true)
                        {
                            if (response.Result != null)
                            {
                                string resultStr = response.Result.ToString();
                                action?.Invoke(resultStr);
                            }
                        }
                    }
                });
            }

        }


        /// <summary>
        /// 按层获取HTML
        /// </summary>
        /// <param name="action"></param>
        /// <param name="tuples"></param>
        /// <param name="frameIndex"></param>
        public void SliceGetHtml(Action<string> action, List<Tuple<string, string, string>> tuples, int frameIndex = -1)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;


                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function GetHtml() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 

                sb.AppendLine(" function getsubitem(parent,item1,item2,item3) {");
                sb.AppendLine("   var tgn= parent.getElementsByTagName(item1);");
                sb.AppendLine("   for(var t in tgn)");
                sb.AppendLine("   {");
                sb.AppendLine("     if(tgn[t][item2]==item3)");
                sb.AppendLine("      {");
                sb.AppendLine("        return tgn[t][item2];");
                sb.AppendLine("      }");
                sb.AppendLine("   }");
                sb.AppendLine(" }");

                sb.AppendLine(" var item0=getsubitem(document,'" + tuples[0].Item1 + "','" + tuples[0].Item2 + "','" + tuples[0].Item3 + "');");

                for (int i = 1; i < tuples.Count; i++)
                {

                    sb.AppendLine(" var item" + i + "=getsubitem(item" + (i - 1) + ",'" + tuples[i].Item1 + "','" + tuples[i].Item2 + "','" + tuples[i].Item3 + "');");
                }
                sb.AppendLine(" return item" + (tuples.Count - 1) + ".innerHTML;");
                sb.AppendLine("}");
                sb.AppendLine("GetHtml();");


                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }


        /// <summary>
        /// 按层点击目标
        /// </summary>
        /// <param name="action"></param>
        /// <param name="tuples"></param>
        /// <param name="frameIndex"></param>
        public void SliceClick(Action<string> action, List<Tuple<string, string, string>> tuples, int frameIndex = -1)
        {
            if (chrome != null)
            {
                if (!chrome.IsBrowserInitialized) return;


                StringBuilder sb = new StringBuilder();
                sb.AppendLine("function GetHtml() {");
                //sb.AppendLine(" return document.body.innerHTML; "); 

                sb.AppendLine(" function getsubitem(parent,item1,item2,item3) {");
                sb.AppendLine("   var tgn= parent.getElementsByTagName(item1);");
                sb.AppendLine("   for(var t in tgn)");
                sb.AppendLine("   {");
                sb.AppendLine("     if(tgn[t][item2]==item3)");
                sb.AppendLine("      {");
                sb.AppendLine("        return tgn[t][item2];");
                sb.AppendLine("      }");
                sb.AppendLine("   }");
                sb.AppendLine(" }");

                sb.AppendLine(" var item0=getsubitem(document,'" + tuples[0].Item1 + "','" + tuples[0].Item2 + "',\"" + tuples[0].Item3 + "\");");

                for (int i = 1; i < tuples.Count; i++)
                {
                    sb.AppendLine(" var item" + i + "=getsubitem(item" + (i - 1) + ",'" + tuples[i].Item1 + "','" + tuples[i].Item2 + "',\"" + tuples[i].Item3 + "\");");
                }
                sb.AppendLine(" item" + (tuples.Count - 1) + ".click();");
                sb.AppendLine(" return 'click';");
                sb.AppendLine("}");
                sb.AppendLine("GetHtml();");


                for (int i = 0; i < chrome.GetBrowser().GetFrameNames().Count; i++)
                {
                    if (frameIndex != i && frameIndex != -1) continue;
                    var task01 = chrome.GetBrowser().GetFrame(chrome.GetBrowser().GetFrameNames()[i]).EvaluateScriptAsync(sb.ToString());
                    task01.ContinueWith(t =>
                    {
                        if (!t.IsFaulted)
                        {
                            var response = t.Result;
                            if (response.Success == true)
                            {
                                if (response.Result != null)
                                {
                                    string resultStr = response.Result.ToString();
                                    action?.Invoke(resultStr);
                                }
                            }
                        }
                    });
                }
            }
        }


      
    }
}
