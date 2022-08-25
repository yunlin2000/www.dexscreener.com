using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Gcashface.EDCode
{
   public class AESHelper
    {
        public static string ToAesEncrypt(string toEncrypt, byte[] keyArray, byte[] ivArray)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return "";
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            using (var rDel = new RijndaelManaged())
            {

                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.PKCS7;

                var resultArray = rDel.CreateEncryptor().TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);

            }

        }

        public static string ToAesDecrypt(string toDecrypt, byte[] keyArray, byte[] ivArray)
        {

            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            using (var rDel = new RijndaelManaged())
            {

                rDel.Key = keyArray;
                rDel.IV = ivArray;
                rDel.Mode = CipherMode.CBC;
                rDel.Padding = PaddingMode.PKCS7;

                var resultArray = rDel.CreateDecryptor().TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }

        }

        /// <summary>
        /// HMD5签名
        /// </summary>
        /// <param name="source">要签名的内容</param>
        /// <param name="key">签名的秘钥</param>
        /// <returns></returns>
        public static string HmacMD5(string source, string key)
        {
            HMACMD5 hmaCmd = new HMACMD5(Encoding.Default.GetBytes(key));
            byte[] byteArray = hmaCmd.ComputeHash(Encoding.Default.GetBytes(source));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < byteArray.Length; i++)
            {
                sb.Append(byteArray[i].ToString("X2"));
            }
            hmaCmd.Clear();
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="secret">加密内容</param>
        /// <param name="signKey">秘钥</param>
        /// <returns></returns>
        public static string HmacSHA256(string secret, string signKey)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(Encoding.UTF8.GetBytes(signKey)))
            {
                byte[] hash = mac.ComputeHash(Encoding.UTF8.GetBytes(secret));
                signRet = Convert.ToBase64String(hash);
                // signRet = ToHexString(hash); 
            }
            return signRet;
        }
        /// <returns></returns>
        public static string HmacSHA256(byte[] secret, byte[] signKey)
        {
            string signRet = string.Empty;
            using (HMACSHA256 mac = new HMACSHA256(signKey))
            {
                byte[] hash = mac.ComputeHash(secret);
                signRet = Convert.ToBase64String(hash);
                // signRet = ToHexString(hash); 
            }
            return signRet;
        }
        public static string HMACSHA256(string signature,string bs64key)
        {
            var shaKeyBytes = Convert.FromBase64String(bs64key);
            using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(shaKeyBytes))
            {
                var signatureBytes = System.Text.Encoding.UTF8.GetBytes(signature);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                //var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2"))).ToLower();

                var signRet = Convert.ToBase64String(signatureHashBytes);
                return signRet;

            }
        }
        public static string HMACSHA256_bs64_bs64(string signature64, string bs64key)
        {
            var shaKeyBytes = Convert.FromBase64String(bs64key);
            using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(shaKeyBytes))
            {
                var signatureBytes = Convert.FromBase64String(signature64);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                //var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2"))).ToLower();

                var signRet = Convert.ToBase64String(signatureHashBytes);
                return signRet;

            }
        }
        public static string HMACSHA256_bs64_str(string signature64, string key)
        {
            var shaKeyBytes =UTF8Encoding.Default.GetBytes(key);
            using (var shaAlgorithm = new System.Security.Cryptography.HMACSHA256(shaKeyBytes))
            {
                var signatureBytes = Convert.FromBase64String(signature64);
                var signatureHashBytes = shaAlgorithm.ComputeHash(signatureBytes);
                //var signatureHashHex = string.Concat(Array.ConvertAll(signatureHashBytes, b => b.ToString("X2"))).ToLower();

                var signRet = Convert.ToBase64String(signatureHashBytes);
                return signRet;

            }
        }
    }
}
