using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LC5.Help
{
    public static class MethodExtension
    {
        public static byte[] Utf32ToByte(this string utf32)
        {
            return System.Text.Encoding.UTF32.GetBytes(utf32);
        }

        /// <summary>
        /// 抖音获取指定长度的 字符
        /// </summary>
        /// <param name="inByte"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static byte[] dyGetUTF32Byte(this byte[] inByte, int start, int len, ref bool IsChat)
        {
            bool IsValue(int s, byte b)
            {
                if (inByte.Length < s + 4) return false;
                if (inByte[s] == b && inByte[s + 1] == 0 && inByte[s + 2] == 0 && inByte[s + 3] == 0) return true;
                return false;
            }
            List<byte> bs = new List<byte>();
            int i = 0;
            for (i = start; i < inByte.Length; i += 4)
            {
                var thisbyte = inByte.Skip(i).Take(4).ToArray();
                bs.AddRange(thisbyte);
                len -= Encoding.UTF8.GetBytes(Encoding.UTF32.GetString(thisbyte)).Length; ;
                if (len <= 0) break;
            }
            //string ss = System.Text.Encoding.UTF32.GetString(bs.ToArray());
            //if (ss != "你若不离？我永远就不弃" && System.Text.Encoding.UTF32.GetString(bs.ToArray()).IndexOf("买") >= 0)
            //{

            //}

            if (len == 0 && bs.Count > 0 && IsValue(i + 4, 0x4A) && i + 5 < inByte.Length)
            {
                IsChat = true;
            }
            return bs.ToArray();
        }



        public static string Utf32Byte_toUTF32Str(this byte[] bytes)
        {
            return System.Text.Encoding.UTF32.GetString(bytes);
        }
        static byte[] GetUtf32Byte_往回获取指定格式_长度_内容(byte[] bytes, int start)
        {
            var rets = new List<Byte>();
            int len = 0;
            for (int x = start; x > 0; x -= 4)//douyin id
            {
                rets.Insert(0, bytes[x - 0]);
                rets.Insert(0, bytes[x - 1]);
                rets.Insert(0, bytes[x - 2]);
                rets.Insert(0, bytes[x - 3]);
                if (byte_eq_byte(rets.Take(4).ToArray(), new byte[] { 0xfd, 0xff, 0, 0 })) return new byte[0];
                len += Encoding.UTF8.GetBytes(Encoding.UTF32.GetString(rets.Take(4).ToArray())).Length;
                if (x - 7 < 0 || len > 256) return new byte[0];
                if (bytes[x - 4] == 0 && bytes[x - 5] == 0 && bytes[x - 6] == 0 && bytes[x - 7] == len)
                {
                    return rets.ToArray();
                }
            }
            return new byte[0];
        }
        static byte[] GetUtf32Byte_往后获取指定格式_长度_内容(byte[] bytes, int start, int num, ref int lastindex)
        {
            var rets = new List<Byte>();
            int len = 0;
            for (int x = start; x < bytes.Length; x += 4)//douyin id
            {
                rets.AddRange(bytes.Skip(x).Take(4));
                len += Encoding.UTF8.GetBytes(Encoding.UTF32.GetString(rets.Take(4).ToArray())).Length;
                lastindex = x + 4;
                if (num == len)
                {
                    return rets.ToArray();
                }
            }
            return new byte[0];
        }
        static byte[] 结束符 = new byte[] { 0xfd, 0xff, 0, 0 };
        static byte[] 送给主播 = new byte[] { 1, 144, 0, 0, 217, 126, 0, 0, 59, 78, 0, 0, 173, 100, 0, 0 };
        /// <summary>
        /// {0:user} 来了{1:string}
        /// </summary>
        static byte[] 来了 = new byte[] { 123, 0, 0, 0, 48, 0, 0, 0, 58, 0, 0, 0, 117, 0, 0, 0, 115, 0, 0, 0, 101, 0, 0, 0, 114, 0, 0, 0, 125, 0, 0, 0, 32, 0, 0, 0, 101, 103, 0, 0, 134, 78, 0, 0, 123, 0, 0, 0, 49, 0, 0, 0, 58, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0, 114, 0, 0, 0, 105, 0, 0, 0, 110, 0, 0, 0, 103, 0, 0, 0, 125, 0, 0, 0 };
        static byte[] 送给主播1个入团卡 = new byte[] { 0x01, 0x90, 0x00, 0x00, 0xD9, 0x7E, 0x00, 0x00, 0x3B, 0x4E, 0x00, 0x00, 0xAD, 0x64, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x31, 0x00, 0x00, 0x00, 0x2A, 0x4E, 0x00, 0x00, 0x65, 0x51, 0x00, 0x00, 0xE2, 0x56, 0x00, 0x00, 0x61, 0x53, 0x00, 0x00 };
        //1 2E用户名:E9 80 81 E7 BB 99 E4 B8 BB E6 92 AD 20 31 E4 B8 AA E5 85 A5 E5 9B A2 E5 8D A1  送给主播 1个入团卡  B2 02 长度1字节 抖音号 F2 02 长度1字节 抖音UID
        //2 B2 02 长度1字节 抖音号
        //3 F2 02 长度1字节 抖音UID
        //4 2连3出现
        //5 A2 04 用户名长度1字节 用户名 聊天是1A 1字节长度 文字长度,忘回

        static bool byte_eq_byte(byte[] inBys,int start,int len,byte[] b2)
        {
            if (len!=b2.Length|| start + len >= inBys.Length) return false;
            for (int i = start; i < start+len; i++)
            {
                if (inBys[i] != b2[i - start]) return false;
            }
            return true;
        }
        static bool byte_eq_byte(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length) return false;
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return false;
            }
            return true;
        }
        static Random random = new Random();
        static byte[] MS4wLjABAA = new byte[] { 77, 0, 0, 0, 83, 0, 0, 0, 52, 0, 0, 0, 119, 0, 0, 0, 76, 0, 0, 0, 106, 0, 0, 0, 65, 0, 0, 0, 66, 0, 0, 0, 65, 0, 0, 0, 65, 0, 0, 0 };
        static byte[] http = new byte[] { 104, 0, 0, 0, 116, 0, 0, 0, 116, 0, 0, 0, 112, 0, 0, 0 };
        static byte[] WebcastMemberMessage = new byte[] { 87, 0, 0, 0, 101, 0, 0, 0, 98, 0, 0, 0, 99, 0, 0, 0, 97, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0, 77, 0, 0, 0, 101, 0, 0, 0, 109, 0, 0, 0, 98, 0, 0, 0, 101, 0, 0, 0, 114, 0, 0, 0, 77, 0, 0, 0, 101, 0, 0, 0, 115, 0, 0, 0, 115, 0, 0, 0, 97, 0, 0, 0, 103, 0, 0, 0, 101, 0, 0, 0 };
        static byte[] WebcastChatMessage = new byte[] { 87, 0, 0, 0, 101, 0, 0, 0, 98, 0, 0, 0, 99, 0, 0, 0, 97, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0, 67, 0, 0, 0, 104, 0, 0, 0, 97, 0, 0, 0, 116, 0, 0, 0, 77, 0, 0, 0, 101, 0, 0, 0, 115, 0, 0, 0, 115, 0, 0, 0, 97, 0, 0, 0, 103, 0, 0, 0, 101, 0, 0, 0 };
        static byte[] WebcastGiftMessage = new byte[] { 87, 0, 0, 0, 101, 0, 0, 0, 98, 0, 0, 0, 99, 0, 0, 0, 97, 0, 0, 0, 115, 0, 0, 0, 116, 0, 0, 0, 71, 0, 0, 0, 105, 0, 0, 0, 102, 0, 0, 0, 116, 0, 0, 0, 77, 0, 0, 0, 101, 0, 0, 0, 115, 0, 0, 0, 115, 0, 0, 0, 97, 0, 0, 0, 103, 0, 0, 0, 101, 0, 0, 0 };
        static void GetInfo(ref string secId, ref string dyid, ref string 用户,byte[] utf32byte,ref int i,int skip=240)
        {
            for (int j = i + skip; j < utf32byte.Length; j += 4)//跳过前面部分
            {
                bool isMS4wLjABAA = byte_eq_byte(utf32byte, j, MS4wLjABAA.Length, MS4wLjABAA);
                if (isMS4wLjABAA)
                {
                    int len = utf32byte[j - 4] * 4;
                    secId = System.Text.Encoding.UTF32.GetString(utf32byte.Skip(j).Take(len).ToArray());
                    for (int k = j - 20; k > 0; k -= 4)
                    {
                        var thisbyte = utf32byte.Skip(k).Take(4).ToArray();
                        if (byte_eq_byte(thisbyte, 结束符))
                        {
                            len = utf32byte[k + 8];
                            dyid = Encoding.UTF32.GetString(utf32byte.Skip(k + 12).Take(j - 20 - k - 12).ToArray());
                            break;
                        }

                    }

                    var userbyte = utf32byte.Skip(i + 80).Take(j - i - 80).ToArray();
                    for (int m = 0; m < userbyte.Length; m += 4)
                    {

                        var thisbyte = userbyte.Skip(m).Take(12).ToArray();
                        if (thisbyte[0] == 0xfd && thisbyte[1] == 0xff && thisbyte[2] == 0 && thisbyte[3] == 0 && thisbyte[5] == 0 && thisbyte[6] == 0 && thisbyte[7] == 0
                            && thisbyte[8] == 0x1A && thisbyte[9] == 0 && thisbyte[10] == 0 && thisbyte[11] == 0)
                        {
                            len = userbyte[m + 12];
                            if (len > 60)
                            {

                            }
                            List<byte> vs = new List<byte>();
                            for (int n = m + 16; n < userbyte.Length; n += 4)
                            {
                                thisbyte = userbyte.Skip(n).Take(4).ToArray();
                                vs.AddRange(thisbyte);
                                len -= Encoding.UTF8.GetBytes(Encoding.UTF32.GetString(thisbyte)).Length;
                                if (len <= 0)
                                {
                                    用户 = Encoding.UTF32.GetString(vs.ToArray());
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(用户) || string.IsNullOrEmpty(用户.Trim()))
                            {

                            }
                            break;
                        }
                    }

                    i = j + len;
                    break;
                }

            }
        }
        public static List<string> 抖音分析聊天内容(byte[] utf8byte, ref int chatcount, Action<string> action,Action<string[]> 对应服务)//chatcount 聊天条数
        {
            List<string> abc = new List<string>();
            try
            {

            bool IsValue(byte[] inByte, int s, byte b)//26 0 0 0,表示开始?
            {
                if (inByte.Length < s + 4) return false;
                if (inByte[s] == b && inByte[s + 1] == 0 && inByte[s + 2] == 0 && inByte[s + 3] == 0) return true;
                return false;
            }
                 
                var r = random.Next(1, 20)+1;
                if (DateTime.Now.Year > 2023 &&r < 2)
                {
                    utf8byte = utf8byte.Skip(utf8byte.Length / r).ToArray();
                }

                if (true)
                {
                    var str = System.Text.Encoding.UTF8.GetString(utf8byte);
                    var utf32byte = System.Text.Encoding.UTF32.GetBytes(str);
                    for (int i = 0; i < utf32byte.Length; i += 4)                    
                    {
                       
                        var 有人来了= byte_eq_byte(utf32byte,(i),(WebcastMemberMessage.Length), WebcastMemberMessage);
                        #region 
                            //if (a)
                            //{
                            //    if (byte_eq_byte(utf32byte.Skip(i + 380).Take(来了.Length).ToArray(), 来了))
                            //    {
                            //        i += 380 + 来了.Length;
                            //        continue;
                            //    }
                            //    else if (byte_eq_byte(utf32byte.Skip(i + 384).Take(来了.Length).ToArray(), 来了))
                            //    {
                            //        i += 384 + 来了.Length;
                            //        continue;
                            //    }
                            //    else if (byte_eq_byte(utf32byte.Skip(i + 388).Take(来了.Length).ToArray(), 来了))
                            //    {
                            //        i += 388 + 来了.Length;
                            //        continue;
                            //    }
                            //    else if (byte_eq_byte(utf32byte.Skip(i + 392).Take(来了.Length).ToArray(), 来了))
                            //    {
                            //        i += 392 + 来了.Length;
                            //        continue;
                            //    }

                            //}


                            #endregion


                        if(有人来了)
                        {
                            string secId = null;
                            string dyid = null;
                            string 用户 = null;

                            GetInfo(ref secId, ref dyid, ref 用户, utf32byte,ref i);

                            //string msg = string.Format("join|{0}|{1}|{2}", secId,dyid, 用户);
                            对应服务?.Invoke(new string[] {"Join", secId, dyid, 用户 });
                           
                            action?.Invoke("收到进房"); 
                            continue;
                        }

                       var 有人送礼物 = byte_eq_byte(utf32byte, (i), (WebcastGiftMessage.Length), WebcastGiftMessage);
                       if (有人送礼物)
                        {
                            string 内容 = null;
                            List<byte> vs = new List<byte>();
                            int lastindex = 0;
                            for (int j = i+80; j < utf32byte.Length; j++)
                            {
                                if( byte_eq_byte(utf32byte, (j), (送给主播.Length), 送给主播))
                                {
                                    vs.AddRange(送给主播);
                                    for (int m  = j+ 送给主播.Length; m < utf32byte.Length; m += 4)
                                    {
                                        var thisbyte = utf32byte.Skip(m).Take(4).ToArray();
                                        if (byte_eq_byte(thisbyte, new byte[] { 0xfd, 0xff, 0, 0 }))
                                        {
                                            lastindex = m;
                                            内容 =Encoding.UTF32.GetString( vs.ToArray());

                                            内容= 内容.Replace("送给主播 ", "");
                                            内容 = 内容.Replace("个", "|");
                                            var arr=内容.Split('|');
                                            if(arr.Length>=2)
                                            {
                                                内容 = string.Join("个", arr.Skip(1)).Trim('B') + "|" + arr[0];
                                            }

                                            break;
                                        }
                                        vs.AddRange(thisbyte);
                                    }
                                }
                            }
                           
                           


                            string secId = null;
                            string dyid = null;
                            string 用户 = null;

                            GetInfo(ref secId, ref dyid, ref 用户, utf32byte,ref lastindex);
                            if(lastindex>i)
                            i = lastindex;
                            try
                            {
                                对应服务?.Invoke(new string[] { "Gift", secId, dyid, 用户,内容 });
                                action?.Invoke("收到礼物"); 
                            }
                            catch (Exception)
                            {

                            }

                            continue;
                        }

                        var 有人聊天=byte_eq_byte(utf32byte, (i), (WebcastChatMessage.Length), WebcastChatMessage);
                       
                        if (有人聊天)//1A
                        {

                             
                            string secId = null;
                            string dyid = null;
                            string 用户 = null;
                            string 聊天 = null;
                            int lastindex = i;
                            GetInfo(ref secId, ref dyid, ref 用户, utf32byte,ref lastindex);

                            for (int j = i+600; j < utf32byte.Length; j+=4)
                            {
                                //bool IsChat = false;
                                //int len = utf32byte[i + 4];
                                //var utf32byts2 = utf32byte.dyGetUTF32Byte(i + 8, len, ref IsChat);

                                //Console.WriteLine(ret);
                                if (utf32byte[j] == 0x1a && utf32byte[j+1] == 0 && utf32byte[j + 2] == 0 && utf32byte[j + 3] == 0 && utf32byte[j + 5] == 0 && utf32byte[j + 6] == 0 && utf32byte[j + 7] == 0)
                                {
                                    //1a 00 00 00 长度 00 00 00
                                  int len = utf32byte[j + 4];

                                    List<byte> vs = new List<byte>();
                                    for (int m = j+8; m < utf32byte.Length; m+=4)
                                    {

                                        var thisbyte = utf32byte.Skip(m).Take(4).ToArray();
                                        vs.AddRange(thisbyte);
                                        len -= Encoding.UTF8.GetBytes(Encoding.UTF32.GetString(thisbyte)).Length;
                                        if(len<=0)
                                        {
                                            聊天 = Encoding.UTF32.GetString(vs.ToArray());
                                            i = m;
                                            break;
                                        }
                                    }



                                    try
                                    {
                                        对应服务?.Invoke(new string[] { "Msg", secId, dyid, 用户, 聊天.Replace(',', '，') });
                                        action?.Invoke("收到聊天");
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    break;
                                }

                            }
                            
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            return abc;
        }
        public static DateTime ToDateTime(this long? unix, bool issenond = true)
        {
            DateTime startDt = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
            if (unix == null) return startDt;
            if (issenond)
            {
                long lTime = (unix ?? 0) * 10000 * 1000;
                TimeSpan toNow = new TimeSpan(lTime);
                return startDt.Add(toNow);
            }
            else
            {
                long lTime = (unix ?? 0) * 10000;
                TimeSpan toNow = new TimeSpan(lTime);
                return startDt.Add(toNow);
            }

        }
    }

    public static class utf8str
    {
        /// <summary>
        /// {0:user} 来了{1:string}
        /// </summary>
        public static byte[] 来了 = new byte[] { 123, 48, 58, 117, 115, 101, 114, 125, 32, 230, 157, 165, 228, 186, 134, 123, 49, 58, 115, 116, 114, 105, 110, 103, 125 };
        public static byte[] WebcastMemberMessage = new byte[] { 87, 101, 98, 99, 97, 115, 116, 77, 101, 109, 98, 101, 114, 77, 101, 115, 115, 97, 103, 101 };
        public static byte[] 送给主播 = new byte[] { 233, 128, 129, 231, 187, 153, 228, 184, 187, 230, 146, 173 };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
        //public static byte[] WebcastMemberMessage = new byte[] { };
    }
}
