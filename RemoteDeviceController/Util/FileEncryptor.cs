using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RemoteDeviceController.Util
{
    public class FileEncryptor
    {
        public static string encryptedFileExtension = ".cooae";

        /// <summary>
        /// 16位十六进制字符
        /// </summary>
        private static string _aesKey = "fce3bad14f2abac3";
        /// <summary>
        /// 十六个元素，每个元素为2位十六进制整形
        /// </summary>
        private static byte[] _aesIV = { 0x01, 0xDA, 0x20, 0xBA, 0xA0, 0xBF, 0x3E, 0x20, 0x72, 0x5A, 0xBA, 0x7A, 0x20, 0x0B, 0x21, 0xAF };

        public static void Encrypt(string filePath, string savePath)
        {
            FileStream result;
            FileStream fs = new FileStream(filePath, FileMode.Open);
            using (MemoryStream ms = new MemoryStream())
            {
                //TODO: optimize: not using memory stream
                byte[] bs = new byte[1024];
                int length = 0;
                while((length = fs.Read(bs, 0, bs.Length)) != 0)
                {
                    ms.Write(bs, 0, length);
                }
                string b64 = Convert.ToBase64String(ms.ToArray());
                string cipher = TextEncryptor.Encrypt_Aes(b64, _aesKey, _aesIV);
                using (result = new FileStream(savePath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(result))
                    {
                        sw.Write(cipher);
                    }
                }
            }
        }

        public static byte[] Encrypt(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            //TODO: may not be able to read the entire file content
            byte[] bs = new byte[fs.Length];
            fs.Read(bs, 0, (int)fs.Length);
            string b64 = Convert.ToBase64String(bs);
            string cipher = TextEncryptor.Encrypt_Aes(b64, _aesKey, _aesIV);
            bs = Convert.FromBase64String(cipher);
            return bs;
        }

        public static byte[] Encrypt(byte[] file)
        {
            byte[] bs = file;
            string b64 = Convert.ToBase64String(bs);
            string cipher = TextEncryptor.Encrypt_Aes(b64, _aesKey, _aesIV);
            bs = Convert.FromBase64String(cipher);
            return bs;
        }

        public static void Decrypt(string filePath, string savePath)
        {
            FileStream result;
            using (StreamReader sr = new StreamReader(filePath))
            {
                //TODO: readtoend() may cause out of memory exception if the file is too large, 50MB around
                string b64 = sr.ReadToEnd();
                b64 = TextEncryptor.Decrypt_Aes(b64, _aesKey, _aesIV);
                byte[] bs = Convert.FromBase64String(b64);
                using (result = new FileStream(savePath, FileMode.Create))
                {
                    result.Write(bs, 0, bs.Length);
                }
            }
        }

        public static byte[] Decrypt(string filePath)
        {
            byte[] bs;
            using (StreamReader sr = new StreamReader(filePath))
            {
                //TODO: readtoend() may cause out of memory exception if the file is too large, 50MB around
                string b64 = sr.ReadToEnd();
                b64 = TextEncryptor.Decrypt_Aes(b64, _aesKey, _aesIV);
                bs = Convert.FromBase64String(b64);
            }
            return bs;
        }

        public static void Decrypt(byte[] bs, string savePath)
        {
            FileStream result;
            //TODO: readtoend() may cause out of memory exception if the file is too large, 50MB around
            string b64 = Convert.ToBase64String(bs);
            b64 = TextEncryptor.Decrypt_Aes(b64, _aesKey, _aesIV);
            byte[] bs2 = Convert.FromBase64String(b64);
            using (result = new FileStream(savePath, FileMode.Create))
            {
                result.Write(bs2, 0, bs2.Length);
            }
        }

        public static byte[] Decrypt(byte[] bs)
        {
            //TODO: readtoend() may cause out of memory exception if the file is too large, 50MB around
            string b64 = Convert.ToBase64String(bs);
            b64 = TextEncryptor.Decrypt_Aes(b64, _aesKey, _aesIV);
            byte[] bs2 = Convert.FromBase64String(b64);
            return bs2;
        }

    }
}
