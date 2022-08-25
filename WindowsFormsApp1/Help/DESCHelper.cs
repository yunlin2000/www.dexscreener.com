using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Gcashface.EDCode
{
   public class DESCHelper
    {
        //密钥
        public static byte[] _KEY = new byte[] { 0x64, 0x66, 0x70, 0x74, 0x5F, 0x49, 0x37, 0x63 };
        //向量
        public static byte[] _IV = new byte[] { 0x26, 0x67, 0x58, 0x57, 0x75, 0x4E, 0x2F, 0x45 };

        /// <summary>
        /// DES加密操作
        /// </summary>
        /// <param name="normalTxt"></param>
        /// <returns></returns>
        public string DesEncrypt(string normalTxt)
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(_KEY, _IV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(normalTxt);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();

            string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            return strRet;
        }

        /// <summary>
        /// DES解密操作
        /// </summary>
        /// <param name="securityTxt">加密字符串</param>
        /// <returns></returns>
        public string DesDecrypt(string securityTxt)//解密  
        {
            //byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
            //byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
            byte[] byEnc;
            try
            {
                securityTxt.Replace("_%_", "/");
                securityTxt.Replace("-%-", "#");
                byEnc = Convert.FromBase64String(securityTxt);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(_KEY, _IV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }

        //DES加密 ECB 
        public static string Encrypt(string encryptString, string sKey)
        {
            try
            {
                string key;
                //密钥为8位
                if (sKey.Length <= 8)
                {
                    key = sKey.PadRight(8, '0');
                }
                else
                {
                    key = sKey.Substring(0, 8);
                }

                byte[] keyBytes = Encoding.Default.GetBytes(key);
                byte[] keyIV = keyBytes;
                byte[] encryptBytes = Encoding.Default.GetBytes(encryptString);

                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                // 使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                // PKCS7padding
                desProvider.Padding = PaddingMode.PKCS7;
                MemoryStream memStream = new MemoryStream();
                //CreateEncryptor(keyBytes, keyIV)类似于OpenSSL中的密钥置换
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                crypStream.Write(encryptBytes, 0, encryptBytes.Length);
                crypStream.FlushFinalBlock();
                byte[] cipherBytes = memStream.ToArray();

                string strRet = Convert.ToBase64String(cipherBytes, 0, (int)cipherBytes.Length);
                return strRet;
            }
            catch
            {
                return encryptString;
            }
        }


        //DEC解密 ECB
        public static string Decrypt(string decryptString, string sKey)
        {
            //key为8位
            string key;
            if (sKey.Length <= 8)
            {
                key = sKey.PadRight(8, '0');
            }
            else
            {
                key = sKey.Substring(0, 8);
            }
            byte[] keyBytes = Encoding.Default.GetBytes(key);
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            desProvider.Mode = CipherMode.ECB;
            MemoryStream memStream = new MemoryStream();
            CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            crypStream.Write(inputByteArray, 0, inputByteArray.Length);
            crypStream.FlushFinalBlock();

            return Encoding.Default.GetString(memStream.ToArray());
        }

    }
}
