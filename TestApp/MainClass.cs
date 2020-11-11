using Minecraft_Launcher_for_Server.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class MainClass
    {
        static void Main(string[] args)
        {
            AssetsDownloader a = new AssetsDownloader(@"C:\Users\psvm\Desktop\assets", "https://launchermeta.mojang.com/mc/assets/1.12/67e29e024e664064c1f04c728604f83c24cbc218/1.12.json");
            a.DownloadTask().Wait();
        }
    }
}
