using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.Properties
{
	internal sealed partial class Settings
	{
		public Settings()
		{
			SettingsLoaded += Settings_SettingsLoaded;
		}

		private void Settings_SettingsLoaded(object sender, System.Configuration.SettingsLoadedEventArgs e)
		{
			Uri result;
			if (!Uri.TryCreate(Default.APIServerLocation, UriKind.Absolute, out result) || (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps))
			{
				Default.APIServerLocation = "http://localhost";
				Default.Save();
			}

			if (!Directory.Exists(Default.MinecraftDir))
			{
				Default.MinecraftDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "minecraft_tedvent");
				Default.Save();
			}
		}
	}
}
