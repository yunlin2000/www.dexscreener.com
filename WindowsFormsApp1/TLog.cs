using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC5
{

    public static class TLog
    {
        static bool isRun = false;
        static List<string> los = new List<string>();
        public static void WriteLog(this string log)
        {
            lock (los)
            {
                if (isRun == false)
                {
                    isRun = true;
                    Task.Run(new Action(run));
                }
                los.Add(string.Format("{0}>>{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log));
            }
        }
        public static int LogCount => los.Count;
        static async void run()
        {
            if (!System.IO.Directory.Exists("Log")) System.IO.Directory.CreateDirectory("Log");
            string fn = "Log/{0}.txt";
            string[] vs = null;
            while (true)
            {
                try
                {
                    if (vs != null && vs.Length > 0)
                    {
                        System.IO.File.AppendAllLines(string.Format(fn, DateTime.Now.ToString("yyyyMMdd")), vs);
                        vs = null;
                    }
                    if (los.Count == 0) await Task.Delay(2000);

                    lock (los)
                    {
                        vs = los.ToArray();
                        los.Clear();
                    }
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            }
        }
    }
}
