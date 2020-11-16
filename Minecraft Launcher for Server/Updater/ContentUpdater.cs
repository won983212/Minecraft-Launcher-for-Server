using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.Updater
{
    public class ContentUpdater
    {
        private static Properties.Settings settings = Properties.Settings.Default;

        // TODO APP SAVEPATH를 기본설정 추가
        public event EventHandler<ProgressArgs> OnProgress;

        public async Task BeginDownload()
        {
            UpdateProgress(0, "Launch Config 가져오는중..");
            string assetsURL = await RetrieveAssetsIndex();

            HashDownloader assetsDownloader = new HashDownloader(Path.Combine(settings.SavePath, "assets"), 
                assetsURL, 
                "http://resources.download.minecraft.net/");

            HashDownloader patchDownloader = new HashDownloader(settings.SavePath,
                settings.RestServerLocation + "/indexes.json",
                settings.RestServerLocation + "/resources");

            assetsDownloader.OnProgress += (s, a) => { UpdateProgress(a.Progress / 2 + 10, "에셋파일: " + a.Status); };
            assetsDownloader.UseHashPath = true;
            await assetsDownloader.DownloadTask();

            patchDownloader.OnProgress += (s, a) => { UpdateProgress(a.Progress * 0.4 + 60, "패치파일: " + a.Status); };
            await patchDownloader.DownloadTask();

            UpdateProgress(100, "설치완료");
        }

        private async Task<string> RetrieveAssetsIndex()
        {
            WebClient client = new WebClient();
            client.DownloadProgressChanged += (s, e) =>
            {
                UpdateProgress(0.1 * e.ProgressPercentage, "Launch Config 가져오는중");
            };

            string cfg = await client.DownloadStringTaskAsync(settings.RestServerLocation + "/launch-config.json");

            // save cfg file
            Directory.CreateDirectory(settings.SavePath);
            File.WriteAllText(Path.Combine(settings.SavePath, "launch-config.json"), cfg);

            JObject cfgJson = JObject.Parse(cfg);
            return (string)cfgJson["assetIndex"]["url"];
        }

        private void UpdateProgress(double progress, string status)
        {
            OnProgress?.Invoke(this, new ProgressArgs(progress, status));
        }
    }
}
