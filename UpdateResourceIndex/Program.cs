using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UpdateResourceIndex
{
    class Program
    {
        static void AddIndexResources(string srcPath, string dstPath, string indexPath)
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

        static string SHA1(SHA1Managed sha1, string path)
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
            if (args.Length != 3)
            {
                Console.WriteLine("updateRes <Source resource path> <Output resource path> <Indexes file path>");
            }
            else
            {
                AddIndexResources(args[0], args[1], args[2]);
            }
        }
    }
}
