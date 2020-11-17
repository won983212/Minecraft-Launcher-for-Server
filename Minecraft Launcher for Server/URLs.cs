using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server
{
    public static class URLs
    {
        private static Properties.Settings settings = Properties.Settings.Default;

        public static string InfoFile
        {
            get
            {
                return settings.APIServerLocation + "/info.json";
            }
        }

        public static string IndexFile
        {
            get
            {
                return settings.APIServerLocation + "/indexes.json";
            }
        }

        public static string PatchFolder
        {
            get
            {
                return settings.APIServerLocation + "/resources";
            }
        }

        public static string LauncherConfig
        {
            get
            {
                return settings.APIServerLocation + "/launch-config.json";
            }
        }

        public static readonly string AssetsURL = "http://resources.download.minecraft.net/";
    }
}
