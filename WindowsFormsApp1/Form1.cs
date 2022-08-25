using app;
using CefSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using 抖音数据筛选;
using LC5;
using Newtonsoft.Json.Linq;
using LC5.Email;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public static ChromeAction chromeAction = new ChromeAction();
        public static Fiddler2 fiddler = new Fiddler2();
        MailHandler mail = new MailHandler();
        /// <summary>
        /// 找到的列表，要对此列表进行循环检测
        /// </summary>
        List<currency> currencies = new List<currency>();

        Dictionary<string, bool> 报警记录 = new Dictionary<string, bool>();

        config cfg = null;

        public Form1()
        {
            InitializeComponent();
            cfg = config.Read();
            try
            {
                tb_jiange.Text = cfg.jiange.ToString();
                tb_mail.Text = cfg.mail;

                tb_pwd.Text= cfg.pwd ;
               tb_stmp.Text= cfg.smtp;
                tb_warring.Text = cfg.报警值.ToString();

            }
            catch (Exception)
            {

            }

            #region 启动fiddler 监听端口，所有经过fiddler的数据，都可以处理
            Random random = new Random();
            int port = random.Next(3000, 65535);
            while (!fiddler.Start(port))
            {
                Console.WriteLine($"{port}被占用");
                port = random.Next(3000, 65530);
            }
            #endregion




            // chromeAction.defaultDoldImg = CefState.Disabled;
            chromeAction.Init("default", $"127.0.0.1:{port}");

            //fiddler.OnResponse = abc;
            // fiddler.OnResponse = new Action<string, string>(abc);

            ///拦截数据包
            //fiddler.OnResponse = new Action<string, string>((url, json)=> {
              //  MessageBox.Show(url);
                //url.WriteLog();
            //});
                fiddler.OnResponse = new Action<string, string>((url, json) => {


                    if (string.IsNullOrEmpty(json)) return;
                    url.WriteLog();// TLog.WriteLog(url);
                    json.WriteLog();
                    //tb_log.Invoke(new Action(() =>
                    //{
                    //    tb_log.AppendText( url + "\r\n");
                    //    tb_log.AppendText(json + "\r\n");
                    //}));

                    if (json.StartsWith("42/u/ws/screener/custom/h"))
                    {
                        var newjson = json.Substring(json.IndexOf('['));
                       // newjson.WriteLog();
                        
                        //var abc = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JArray>(newjson);
                        //var abc2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JToken[]>(newjson);
                        var array = JArray.Parse(newjson);
                        var newarr = array[1]["pairs"].ToObject<JArray>();
                        List<currency> currencies_tmp = new List<currency>();


                        foreach (var item in newarr)
                        {
                            currency c = new currency()
                            {
                                bd_qian = item["baseToken"]["symbol"].ToString(),
                                bd_hou = item["quoteTokenSymbol"].ToString(),
                                sc = item["platformId"].ToString(),
                                add = item["pairAddress"].ToString()
                            };
                            currencies_tmp.Add(c);
                        }
                        currencies.AddRange(currencies_tmp);
                        return;
                    }

                    return;
                    if (url.StartsWith("io.dexscreener.com/u/trading-history/recent/bsc"))
                    {
                        var jtoken = Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(json);
                        var str = jtoken["tradingHistory"].ToString();
                        var resps = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponsData[]>(str);
                        if (resps != null)
                            foreach (var item in resps)
                            {
                                decimal vol = 0;
                                if (decimal.TryParse(item.volumeUsd, out vol))
                                {
                                    if (vol > 1000)
                                    {
                                        WriteLog(item.volumeUsd);
                                    }
                                }

                            }
                    }
                    // 拦截数据处理(url, json);
                });
            //List<int> lasdjf = new List<int>();
            //var x = lasdjf.Where(x => x.Name == "abc");
            //foreach (var item in x)
            //{
            //    //item.Name
            //}
            //创建一个chrome的浏览器  StartNewWindow=运行一个跳出的新窗口前调用   FrameLoadEnd =frame加载完毕激活
            chromeAction.Cretate(panel1, 1, StartNewWindow, FrameLoadEnd, $"https://dexscreener.com/", "default");

           
            Task.Run(new Action(运行一个新线程));

        }
        void WriteLog(string log)
        {
            tb_log.Invoke(new Action(() =>
            { 
                tb_log.AppendText(log + "\r\n");
            }));

        }
        void  运行一个新线程()
        {
            DateTime 当前任务完成时间 = DateTime.MinValue;
            DateTime 最新加载时间 = DateTime.MinValue;
           
            while (true)
            {
                try
                {
                    if (DateTime.Now > 当前任务完成时间.AddMinutes(cfg.jiange))
                    {
                        if (currencies.Count() > 0)
                        {
                            var datas = currencies.GroupBy(x => x.add);
                            foreach (var _item in datas)
                            {
                                //foreach (var item2 in _item)
                                //{

                                //}
                                var item = _item.Last();

                                //https://io.dexscreener.com/u/trading-history/recent/bsc/0x16b9a82891338f9bA80E2D6970FddA79D1eb0daE
                                var ret = chromeAction.getRemote($"https://io.dexscreener.com/u/trading-history/recent/{item.sc}/{item.add.ToLower()}");
                                var token = JToken.Parse(ret);
                                var array = token["tradingHistory"].ToArray();
                                int warringCount = 0;//符合报警的条数
                                foreach (var item1 in array)
                                {
                                    string key = $"{item1["txnHash"]}_{item.bd_qian}_{item.bd_hou}";
                                    lock (报警记录)
                                    {
                                        if (报警记录.ContainsKey(key) == true) continue;
                                        报警记录.Add(key, false);
                                    }
                                    if (item1["volumeUsd"] != null && item1["volumeUsd"].ToObject<decimal>() > cfg.报警值)
                                    {
                                        warringCount++;
                                    }
                                }
                                if (warringCount > 0)
                                    sendEmail($"{item.bd_qian}/{item.bd_hou}\thttps://dexscreener.com/{item.sc}/{item.add}\t{warringCount}");


                            }
                            if (currencies.Count() > 0)
                                当前任务完成时间 = DateTime.Now;
                        }
                        else if((DateTime.Now-最新加载时间).TotalSeconds>10)
                        {
                            最新加载时间 = DateTime.Now;
                            currencies.Clear();
                            chromeAction.Load("https://dexscreener.com/new-pairs");
                        }

                    }
                }
                catch (Exception ex)
                {
                    ex.Message.WriteLog();
                }
                System.Threading.Thread.Sleep(1000);
            }

        }

        private void FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            textBox1.Invoke(new Action(()=> {

                textBox1.Text = e.Url;
            }));
        }
        void sendEmail(string title)
        {
            var rtn=mail.SendMail(title, title, cfg.mail, cfg.smtp, cfg.mail, cfg.pwd);
            WriteLog(title);
        }
        private void StartNewWindow(object sender, NewWindowEventArgs e)
        {


        }

        private void 拦截数据处理(string url, string json)
        {


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chromeAction.Load(textBox1.Text);
        }

        private async void button2_Click(object sender, EventArgs e)
        {


          
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {

                cfg.jiange = Convert.ToInt32(tb_jiange.Text);
                cfg.mail = (tb_mail.Text);
                cfg.pwd = (tb_pwd    .Text);
                cfg.smtp = (tb_stmp.Text);
                cfg.报警值 = Convert.ToInt32(tb_warring.Text);
                cfg.Save();
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    class currency
    {
        public string bd_qian;
        public string bd_hou;
        public string sc;//所属平台
        public string add;
        public DateTime intime;

    }
     
   
}
