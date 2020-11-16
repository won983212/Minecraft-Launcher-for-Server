using Minecraft_Launcher_for_Server.Updater;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class MainClass
    {
        MainClass()
        {
            // Assets downloader test
            HashDownloader downloader = new HashDownloader(@"C:\Users\psvm\Desktop\assets", "https://launchermeta.mojang.com/mc/assets/1.12/67e29e024e664064c1f04c728604f83c24cbc218/1.12.json", "http://resources.download.minecraft.net/");
            downloader.UseHashPath = true;
            downloader.OnProgress += (sender, arg) => { Console.WriteLine((int)(arg.Progress * 100) / 100.0 + "%  " + arg.Status); };

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (Console.Read() == 'c')
                    {
                        Console.WriteLine("Canceling...");
                        downloader.Cancel();
                    }
                }
            });

            downloader.DownloadTask().Wait();

            // Download patches
            /*HashDownloader downloader = new HashDownloader(@"C:\Users\psvm\Desktop\minecraft", "http://localhost/indexes.json", "http://localhost/resources/");
            downloader.DownloadTask().Wait();*/

            // timer
            /*Stopwatch timer = new Stopwatch();
            timer.Start();
            timer.Stop();
            Console.WriteLine("Elapsed time: " + timer.ElapsedMilliseconds);*/

            //AddIndexResources(@"C:\Users\psvm\Desktop\webserver\nginx-1.19.4\html\files", @"C:\Users\psvm\Desktop\webserver\nginx-1.19.4\html\resources", @"C:\Users\psvm\Desktop\webserver\nginx-1.19.4\html");
        }

        void AddIndexResources(string srcPath, string dstPath, string indexPath)
        {
            JObject json = new JObject();
            Queue<string> folders = new Queue<string>();
            SHA1Managed sha1 = new SHA1Managed();
            folders.Enqueue(srcPath);

            while (folders.Count > 0)
            {
                string dir = folders.Dequeue();
                foreach (string f in Directory.GetFiles(dir))
                {
                    string hash = SHA1(sha1, f);
                    string name = f.Substring(srcPath.Length + 1).Replace("\\", "/");
                    json.Add(name, new JObject(new JProperty("hash", hash), new JProperty("size", new FileInfo(f).Length)));

                    string parent = Path.Combine(dstPath, hash.Substring(0, 2));
                    if (!Directory.Exists(parent))
                        Directory.CreateDirectory(parent);

                    File.Copy(f, Path.Combine(dstPath, parent, hash));
                    Console.WriteLine(name);
                }
                foreach (string directory in Directory.GetDirectories(dir))
                {
                    folders.Enqueue(directory);
                }
            }

            json = new JObject(new JProperty("objects", json));
            File.WriteAllText(Path.Combine(indexPath, "indexes.json"), json.ToString());
            sha1.Dispose();
        }

        string SHA1(SHA1Managed sha1, string path)
        {
            StringBuilder sb = new StringBuilder(40);
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                byte[] hash = sha1.ComputeHash(fs);
                foreach (byte b in hash)
                    sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        static void Main(string[] args)
        {
            new MainClass();
        }
    }
}
