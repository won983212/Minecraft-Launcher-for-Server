using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.Updater
{
    public class AssetsDownloader
    {
        private string _assetsPath;
        private string _assetsIndexUrl;
        private int count = 0;
        private int total = 0;

        public AssetsDownloader(string assetsPath, string indexUrl)
        {
            _assetsPath = assetsPath;
            _assetsIndexUrl = indexUrl;
        }

        public Task DownloadTask()
        {
            return Task.Factory.StartNew(() => DownloadAssets());
        }

        private void DownloadAssets()
        {
            string indexData = new WebClient().DownloadString(_assetsIndexUrl);
            string indexesFolder = Path.Combine(_assetsPath, "indexes");
            string objectsFolder = Path.Combine(_assetsPath, "objects");

            if (!Directory.Exists(indexesFolder))
                Directory.CreateDirectory(indexesFolder);

            if (!Directory.Exists(objectsFolder))
                Directory.CreateDirectory(objectsFolder);

            File.WriteAllText(Path.Combine(indexesFolder, Path.GetFileName(_assetsIndexUrl)), indexData);

            JObject assetsData = JObject.Parse(indexData);
            List<string> hash = new List<string>();

            foreach (var token in assetsData["objects"])
            {
                JProperty p = token as JProperty;
                if(p != null)
                {
                    hash.Add((string)p.Value["hash"]);
                }
            }

            Interlocked.Exchange(ref total, hash.Count);
            Parallel.ForEach(hash, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (h) => DownloadFile(objectsFolder, h));
        }

        private void DownloadFile(string path, string hash)
        {
            Console.WriteLine("Start download " + hash);

            string url = hash.Substring(0, 2) + "/" + hash;
            path = Path.Combine(path, url);

            DirectoryInfo parent = Directory.GetParent(path);
            if (!parent.Exists)
                Directory.CreateDirectory(parent.FullName);

            var sr = new BinaryReader(WebRequest.Create("http://resources.download.minecraft.net/" + url)
                .GetResponse().GetResponseStream());
            var sw = new BinaryWriter(new FileStream(path, FileMode.Create));

            byte[] buf = new byte[1024];
            int len = 0;
            while((len = sr.Read(buf, 0, buf.Length)) > 0)
                sw.Write(buf, 0, len);

            sr.Close();
            sw.Close();
            Interlocked.Increment(ref count);

            Console.WriteLine((count * 100.0 / total) + "% --- End download " + hash);
        }
    }
}
