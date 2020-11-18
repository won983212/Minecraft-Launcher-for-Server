using Minecraft_Launcher_for_Server.Updater;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    public enum LauncherState
    {
        NeedInstall, NeedUpdate, CanStart
    }

    public class MinecraftLauncherMain
    {
        public AuthResponse AuthInfo { get; set; }

        public ServerStatus ServerStatus { get; private set; } = new ServerStatus();

        public string PatchVersion { get; private set; }


        public MinecraftLauncherMain()
        {
            string filePath = Path.Combine(Properties.Settings.Default.MinecraftDir, "version");
            if (File.Exists(filePath))
            {
                PatchVersion = File.ReadAllText(filePath);
            } 
            else
            {
                PatchVersion = "Unknown";
            }
        }

        public LauncherState GetLauncherState()
        {
            if (PatchVersion == "Unknown")
            {
                return LauncherState.NeedInstall;
            }
            else if(PatchVersion != ServerStatus.PatchVersion)
            {
                return LauncherState.NeedUpdate;
            }

            return LauncherState.CanStart;
        }
    }
}
