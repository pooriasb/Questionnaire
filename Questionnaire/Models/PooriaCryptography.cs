using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Tejarat.Models.Tools
{
    public class PooriaCryptography
    {
        public string encrypt(string encryptString)
        {
            string EncryptionKey = "PooriaZ13740056010SajadPooria005558CVzxcbWW42";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }
        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "PooriaZ13740056010SajadPooria005558CVzxcbWW42";

            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
        public string ToBase64ForUrl(string str) {

            ///encode
            ///System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainTextBytes));
            ///decode 
            ///System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64EncodedData));
           return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        }
        public string FromBase64UrlToString(string str) {


            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(str));
        }
        /// <summary>
        /// generate random password based on GUID with substring 1,8 and replace - to $PO
        /// </summary>
        /// <returns>return a string </returns>
        public string GeneratePassword() {
            return Guid.NewGuid().ToString().Substring(1,8).Replace("-","$PO");
        }

    }
}