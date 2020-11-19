using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    public class AuthInfoStorage
    {
        private static readonly string DataPath = Path.Combine(Properties.Settings.Default.MinecraftDir, "lastlogin");
        private static RijndaelManaged Cipher = new RijndaelManaged();
        private static ICryptoTransform Encryptor;
        private static ICryptoTransform Decryptor;

        public string AccessToken { get; set; } = null;
        public string ClientToken { get; set; } = null;

        static AuthInfoStorage()
        {
            byte[] salt = Encoding.ASCII.GetBytes("s4etDn7GESw3c");
            string pwd = "kh3hggCb2@bMps3FQ$#D4fg2";

            PasswordDeriveBytes secretKey = new PasswordDeriveBytes(pwd, salt);
            Encryptor = Cipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

            secretKey = new PasswordDeriveBytes(pwd, salt);
            Decryptor = Cipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
        }

        public bool HasTokenData()
        {
            return AccessToken != null && ClientToken != null;
        }

        public void Load()
        {
            if (File.Exists(DataPath))
            {
                byte[] data = Convert.FromBase64String(File.ReadAllText(DataPath));
                string decrypted;
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (CryptoStream cStream = new CryptoStream(stream, Decryptor, CryptoStreamMode.Read))
                    {
                        byte[] text = new byte[data.Length];
                        int len = cStream.Read(text, 0, text.Length);
                        decrypted = Encoding.UTF8.GetString(text, 0, len);
                    }
                }

                string[] splited = decrypted.Split('#');
                if (splited.Length == 2)
                {
                    AccessToken = splited[0];
                    ClientToken = splited[1];
                }
            }
        }

        public void Save()
        {
            byte[] text = Encoding.UTF8.GetBytes(AccessToken + '#' + ClientToken);
            string data;
            using (MemoryStream stream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(stream, Encryptor, CryptoStreamMode.Write))
                {
                    cStream.Write(text, 0, text.Length);
                    cStream.FlushFinalBlock();
                    data = Convert.ToBase64String(stream.ToArray());
                }
            }

            DirectoryInfo parent = Directory.GetParent(DataPath);
            if (!parent.Exists)
                parent.Create();

            File.WriteAllText(DataPath, data);
        }

        public void Clear()
        {
            if(File.Exists(DataPath))
                File.Delete(DataPath);
            AccessToken = null;
            ClientToken = null;
        }
    }
}
