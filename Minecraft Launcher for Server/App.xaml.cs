using Minecraft_Launcher_for_Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Minecraft_Launcher_for_Server
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private MinecraftLauncherMain _main = new MinecraftLauncherMain();

        public static MinecraftLauncherMain GetContext()
        {
            return ((App)Current)._main;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MainViewModel vmodel = Current.MainWindow.DataContext as MainViewModel;
            vmodel.ShowErrorMessage("오류 발생", e.Exception.Message, () => { });
            e.Handled = true;
        }
    }
}
