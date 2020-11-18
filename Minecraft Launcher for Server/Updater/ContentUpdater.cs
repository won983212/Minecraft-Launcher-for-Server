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

        public event EventHandler<ProgressArgs> OnProgress;

        public async Task BeginDownload()
        {
            UpdateProgress(0, "Launch Config 가져오는중..");
            string assetsURL = await RetrieveAssetsIndex();

            HashDownloader assetsDownloader = new HashDownloader(Path.Combine(settings.MinecraftDir, "assets"), assetsURL, URLs.AssetsURL);
            assetsDownloader.OnProgress += (s, a) => { UpdateProgress(a.Progress / 2 + 10, "에셋파일: " + a.Status); };
            assetsDownloader.UseHashPath = true;
            await assetsDownloader.DownloadTask();

            HashDownloader patchDownloader = new HashDownloader(settings.MinecraftDir, URLs.IndexFile, URLs.PatchFolder);
            patchDownloader.OnProgress += (s, a) => { UpdateProgress(a.Progress * 0.4 + 60, "패치파일: " + a.Status); };
            await patchDownloader.DownloadTask();

            UpdateProgress(99, "버전 정보 수정중..");
            await Task.Factory.StartNew(UpdatePatchVersion);

            UpdateProgress(100, "설치완료");
        }

        private void UpdatePatchVersion()
        {
            string patchVersionPath = Path.Combine(settings.MinecraftDir, "version");
            using(WebClient client = new WebClient())
            {
                string data = client.DownloadString(URLs.InfoFile);
                JObject json = JObject.Parse(data);
                File.WriteAllText(patchVersionPath, (string)json["patchVersion"]);
            }
        }

        private async Task<string> RetrieveAssetsIndex()
        {
            WebClient client = new WebClient();
            client.DownloadProgressChanged += (s, e) =>
            {
                UpdateProgress(0.1 * e.ProgressPercentage, "Launch Config 가져오는중");
            };

            string cfg = await client.DownloadStringTaskAsync(URLs.LauncherConfig);
            client.Dispose();

            // save cfg file
            Directory.CreateDirectory(settings.MinecraftDir);
            File.WriteAllText(Path.Combine(settings.MinecraftDir, Path.GetFileName(URLs.LauncherConfig)), cfg);

            JObject cfgJson = JObject.Parse(cfg);
            return (string)cfgJson["assetIndex"]["url"];
        }

        private void UpdateProgress(double progress, string status)
        {
            OnProgress?.Invoke(this, new ProgressArgs(progress, status));
        }
    }
}
