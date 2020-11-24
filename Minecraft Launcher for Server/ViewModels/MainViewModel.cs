using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using Minecraft_Launcher_for_Server.Updater;
using Minecraft_Launcher_for_Server.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class ServerInfo
    {
        public string Motd { get; set; }
        public string PlayerCount { get; set; }
        public string Ping { get; private set; }

        public ServerInfo(ServerStatus status)
        {
            Motd = status.Motd;
            PlayerCount = status.PlayersOnline + "/" + status.PlayersMax;
            Ping = status.Ping + "ms";
        }
    }

    class MainViewModel : ParentViewModelBase
    {
        private ErrorMessageObject _errorInfo;
        private string _signalIcon = "SignalCellular1";
        private Visibility _signalIconVisibility = Visibility.Collapsed;
        private bool _showErrorDialog = false;
        private bool _showControlPanel = false;
        private bool _splashAnimation = false;
        private ServerInfo _serverInfo;

        private ICommand _reconnectCommand;
        private ICommand _showSettingCommand;

        public SnackbarMessageQueue SnackMessages { get; private set; }

        public ErrorMessageObject ErrorInfo
        {
            get => _errorInfo;
            set
            {
                _errorInfo = value;
                OnPropertyChanged();
            }
        }

        public bool ShowErrorDialog
        {
            get => _showErrorDialog;
            set
            {
                _showErrorDialog = value;
                OnPropertyChanged();
            }
        }

        public bool ShowControlPanel
        {
            get => _showControlPanel;
            set
            {
                _showControlPanel = value;
                OnPropertyChanged();
            }
        }

        public bool SplashAnimation
        {
            get => _splashAnimation;
            set
            {
                _splashAnimation = value;
                OnPropertyChanged();
            }
        }

        public string SignalIcon
        {
            get => _signalIcon;
            set
            {
                _signalIcon = value;
                OnPropertyChanged();
            }
        }

        public Visibility SignalIconVisibility
        {
            get => _signalIconVisibility;
            set
            {
                _signalIconVisibility = value;
                OnPropertyChanged();
            }
        }

        public ServerInfo ServerInfo
        {
            get => _serverInfo;
            set
            {
                _serverInfo = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReconnectCommand
        {
            get => _reconnectCommand = _reconnectCommand ?? new RelayCommand((a) => App.MainContext.ServerStatus.RetrieveAll());
        }

        public ICommand ShowSettingCommand
        {
            get => _showSettingCommand = _showSettingCommand ?? new RelayCommand((a) => ShowControlPanel = true);
        }

        public MainViewModel()
        {
            // TODO DEBUG: 편의를 위해 임시로 tab을 메인으로
            //CurrentPage = new LoginFormViewModel(this);
            CurrentPage = new TabMainViewModel(this);
            SnackMessages = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            ErrorInfo = new ErrorMessageObject();
            App.MainContext.ServerStatus.OnConnectionStateChanged += ServerStatus_OnConnectionStateChanged;
        }

        private void ServerStatus_OnConnectionStateChanged(object sender, ConnectionState e)
        {
            if(e.State == RetrieveState.Loaded)
            {
                SignalIconVisibility = Visibility.Visible;
                ServerStatus status = App.MainContext.ServerStatus;

                if(status.Ping < 150)
                {
                    SignalIcon = "SignalCellular3";
                }
                else if(status.Ping < 300)
                {
                    SignalIcon = "SignalCellular2";
                }
                else
                {
                    SignalIcon = "SignalCellular1";
                }

                ServerInfo = new ServerInfo(status);
            } 
            else
            {
                SignalIconVisibility = Visibility.Collapsed;
            }
        }

        public void GoToPage(PageViewModelBase nextPage)
        {
            CurrentPage = nextPage;
            SplashAnimation = true;
        }

        public void ShowErrorMessage(Exception e, Action callback)
        {
            ErrorInfo = new ErrorMessageObject()
            {
                Title = "오류 발생",
                Message = e.Message,
                FullMessage = e.ToString(),
                Callback = callback
            };
            ShowErrorDialog = true;
        }

        public void ShowErrorMessage(string title, string message, Action callback)
        {
            ErrorInfo = new ErrorMessageObject()
            {
                Title = title,
                Message = message,
                FullMessage = null,
                Callback = callback
            };
            ShowErrorDialog = true;
        }

        public async Task ThrowIfAPIServerClosed()
        {
            await App.MainContext.ServerStatus.RetrieveAll();
            if (App.MainContext.ServerStatus.ConnectionState.State == RetrieveState.Error)
            {
                ShowErrorMessage("서버가 닫힘", "TEDVENT 서버와 연결할 수 없습니다.\n" +
                            "서버가 닫혀있거나, 인터넷 연결 문제일 수 있으니 다시 시도해보시길 바랍니다.\n" +
                            "확인을 누르면 클라이언트가 종료됩니다.", () => Application.Current.Shutdown(0));
            }
        }

        public void AddErrorSnackbar(string message)
        {
            SnackMessages.Enqueue(message);
        }
    }
}
