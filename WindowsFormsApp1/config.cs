﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
   public class config
    {
        public string smtp;//smtp.qq.com  
        public string mail;//
        public string pwd;//
        public int jiange;//分钟
        public int 报警值;//
        public void Save()
        {
            try
            {
                System.IO.File.WriteAllText("config.json", Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
            catch (Exception ex)
            {

            }
        }

        public static config Read()
        {
            try
            {

                string json = System.IO.File.ReadAllText("config.json");
                config cfg = Newtonsoft.Json.JsonConvert.DeserializeObject<config>(json);
                if (cfg != null)
                {
                    return cfg;
                }
            }
            catch (Exception ex)
            {

            }

            return new config();
        }
    }
}
