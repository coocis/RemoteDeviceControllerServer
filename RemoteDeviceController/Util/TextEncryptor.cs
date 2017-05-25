using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace RemoteDeviceController.Util
{
    public class TextEncryptor
    {
        /// <summary>
        /// 16位十六进制字符
        /// </summary>
        private static string _aesKey = "ace3a4d14d39bfce";
        /// <summary>
        /// 十六个元素，每个元素为2位十六进制整形
        /// </summary>
        private static byte[] _aesIV = { 0x01, 0xD5, 0x2F, 0x7A, 0xA0, 0xBA, 0x34, 0x2F, 0x7C, 0x3A, 0x5A, 0x7B, 0xE0, 0x0A, 0x01, 0x0F };

        public static string Encrypt_Aes(string plainText, string aesKey, byte[] aesIV)
        {
            Aes aes = Aes.Create();
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(aesKey);
                aesAlg.IV = aesIV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// 使用AES-128位算法进行加密
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt_Aes(string plainText)
        {
            return Encrypt_Aes(plainText, _aesKey, _aesIV);
        }

        public static string Encrypt_Aes_Hex(string plainText)
        {
            string aes = Encrypt_Aes(plainText);
            string result = "";
            byte[] bs = Convert.FromBase64String(aes);
            foreach (var b in bs)
            {
                result += Convert.ToString(b, 16).PadLeft(2, '0');
            }
            return result;
        }

        public static string Encrypt_Hex(string plainText)
        {
            string result = "";
            byte[] bs = Encoding.UTF8.GetBytes(plainText);
            foreach (var b in bs)
            {
                result += Convert.ToString(b, 16).PadLeft(2, '0');
            }
            return result;
        }

        public static string Decrypt_Aes(string cipherText, string aesKey, byte[] aesIV)
        {

            byte[] cipherTextByte = Convert.FromBase64String(cipherText.Trim());
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(aesKey);
                aesAlg.IV = aesIV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherTextByte))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        public static string Decrypt_Aes(string cipherText)
        {
            return Decrypt_Aes(cipherText, _aesKey, _aesIV);
        }

        public static string Decrypt_Aes_Hex(string cipherText)
        {
            string result = "";
            List<byte> bs = new List<byte>();
            for (int i = 0; i < cipherText.Length; i += 2)
            {
                bs.Add(Convert.ToByte(cipherText.Substring(i, 2), 16));
            }
            result = Decrypt_Aes(Convert.ToBase64String(bs.ToArray()));
            return result;
        }

        public static string Decrypt_Hex(string cipherText)
        {
            string result = "";
            List<byte> bs = new List<byte>();
            for (int i = 0; i < cipherText.Length; i += 2)
            {
                bs.Add(Convert.ToByte(cipherText.Substring(i, 2), 16));
            }
            result = Encoding.UTF8.GetString(bs.ToArray());
            return result;
        }
    }
}
