using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class TabHomeViewModel : PageViewModelBase
    {
        private Launcher _launcher = new Launcher();
        private string _startText = "게임 시작";
        private ICommand _startCommand;
        private LauncherState _launchState;


        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                    _startCommand = new RelayCommand(OnStartClick, CanStart);
                return _startCommand;
            }
        }

        public string StartText
        {
            get => _startText;
            set
            {
                _startText = value;
                OnPropertyChanged();
            }
        }


        public TabHomeViewModel(ParentViewModelBase parent)
            : base(parent)
        {
            UpdateStartButton();
            _launcher.OnLog += (s, t) => Logger.Log(t);
            _launcher.OnError += (s, t) => Logger.Error(t);
            _launcher.OnExited += (s, t) => Logger.Log(" Exited (code: " + t + ")");
        }

        private void UpdateStartButton()
        {
            _launchState = App.GetContext().GetLauncherState();
            switch (_launchState)
            {
                case LauncherState.CanStart:
                    StartText = "게임 시작";
                    break;
                case LauncherState.NeedInstall:
                    StartText = "설치";
                    break;
                case LauncherState.NeedUpdate:
                    StartText = "업데이트";
                    break;
            }
        }

        private async Task StartMinecraft()
        {
            await _launcher.Start();
            if (!Properties.Settings.Default.UseLogging)
                App.Current.Shutdown(0);
        }

        private void OnStartClick(object arg)
        {
            if (_launchState != LauncherState.CanStart)
            {
                TabMainViewModel mainVModel = RootViewModel as TabMainViewModel;
                mainVModel.StartDownload();
            }
            else
            {
                StartMinecraft();
            }
        }

        private bool CanStart(object arg)
        {
            return (RootViewModel as TabMainViewModel).CanStart() && !_launcher.IsRunning;
        }
    }
}
