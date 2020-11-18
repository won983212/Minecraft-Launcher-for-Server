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
            (RootViewModel as TabMainViewModel).DownloadCompleted += (s, a) => UpdateStartButton();
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

        private void OnStartClick(object arg)
        {
            TabMainViewModel mainVModel = RootViewModel as TabMainViewModel;
            if (_launchState != LauncherState.CanStart)
            {
                mainVModel.StartDownload();
            }
            else
            {
                mainVModel.StartMinecraft();
            }
        }

        private bool CanStart(object arg)
        {
            return (RootViewModel as TabMainViewModel).CanStart();
        }
    }
}
