using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GZIP工具.Help
{
   public class Aes
    {
       public static string SecretKeySpec(string needEncode,string key)
        {
            System.Text.UTF8Encoding uTF8 = new UTF8Encoding();
            AesManaged tdes = new AesManaged();
            tdes.Key = uTF8.GetBytes(key);//key
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] plain = Encoding.UTF8.GetBytes(needEncode);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            var enc = Convert.ToBase64String(cipher);
            return enc;
        }
        public static string SecretKeySpec1(string base64needEncode, string base64key,string base64iv)
        {



            System.Text.UTF8Encoding uTF8 = new UTF8Encoding();
            AesManaged tdes = new AesManaged();

            tdes.Key = Convert.FromBase64String(base64key);
            if( string.IsNullOrEmpty( base64iv)==false)
                tdes.IV=Convert.FromBase64String(base64iv);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] plain = Convert.FromBase64String(base64needEncode);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            var enc = Convert.ToBase64String(cipher);
            return enc;
        }
        public static string SecretKeySpec2(string base64needEncode, string hexKey,string hexIv)
        {

            AesManaged tdes = new AesManaged();
            var bs64key= Convert.ToBase64String(hexKey.HexToByte());
            tdes.Key = Convert.FromBase64String(bs64key);
            if (string.IsNullOrEmpty(hexIv) == false)
                tdes.IV = hexIv.HexToByte();
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateEncryptor();
           var  bs64str = Convert.ToBase64String(base64needEncode.HexToByte());
            byte[] plain = Convert.FromBase64String(bs64str);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            var enc = Convert.ToBase64String(cipher);
            return enc;
        }

        public static string SecreKeyDec(string base64Ret,string bs64key,string base64Iv)
        {

            AesManaged tdes = new AesManaged();;
            tdes.Key = Convert.FromBase64String(bs64key);
            if (string.IsNullOrEmpty(base64Iv) == false)
                tdes.IV = Convert.FromBase64String(base64Iv);
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;
            ICryptoTransform crypt = tdes.CreateDecryptor();
            byte[] plain = Convert.FromBase64String(base64Ret);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            var enc = Convert.ToBase64String(cipher);
            return enc;
        }
        public static string SecretKeyDec2(string base64needEncode, string hexKey, string hexIv)
        {

            AesManaged tdes = new AesManaged();
            tdes.Key = hexKey.HexToByte();
            if (string.IsNullOrEmpty(hexIv) == false)
                tdes.IV = hexIv.HexToByte();
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypt = tdes.CreateDecryptor();
            byte[] plain = Convert.FromBase64String(base64needEncode);
            byte[] cipher = crypt.TransformFinalBlock(plain, 0, plain.Length);
            var enc = Convert.ToBase64String(cipher);
            return enc;
        }
    }
}
