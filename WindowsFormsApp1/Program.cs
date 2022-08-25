using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]    //Single Thread Apartment Thread  指定当前线程的ApartmentState属性是true.[STAThread] attribute指示应用程序的 COM 线程模型是单线程单元
        static void Main()
        {
//            string abc = @"
//            {
//                ""a"":1213,
//                   ""b"":
//{
 
//}
//            }";
//            var token = JToken.Parse(abc);
//            if(token["a"].ToObject<long>() == 1213)
//            {

//            }



            Application.EnableVisualStyles();   //激活应用程序的显示风格
            Application.SetCompatibleTextRenderingDefault(false);//在应用程序的默认范围内设置控件的显示方式
            Application.Run(new Form1());
        }
    }
}
