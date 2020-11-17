using Minecraft_Launcher_for_Server.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    public enum RetrieveState
    {
        Processing, Loaded, Error
    }

    class LoginFormViewModel : PageViewModelBase
    {
        private RetrieveState _connectionState = RetrieveState.Processing;
        private string _connectionErrorMessage = "";
        private ICommand _reconnectCommand;
        private ServerStatus _serverStatus = new ServerStatus();

        public bool CanLogin
        {
            get => _connectionState == RetrieveState.Loaded;
        }

        public string ConnectionErrorMessage
        {
            get => _connectionErrorMessage;
            set
            {
                _connectionErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public RetrieveState ConnectionState
        {
            get => _connectionState;
            set
            {
                _connectionState = value;
                OnPropertyChanged();
            }
        }

        public ICommand ReconnectCommand
        {
            get => _reconnectCommand = _reconnectCommand ?? new RelayCommand((a) => TestIfServerClosed());
        }

        public LoginFormViewModel(ParentViewModelBase root)
            : base(root)
        {
            TestIfServerClosed();
        }

        private async Task TestIfServerClosed()
        {
            ConnectionState = RetrieveState.Processing;
            try
            {
                bool result = await Task.Factory.StartNew(ServerStatus.IsActiveAPIServer);
                if (!result)
                {
                    ConnectionState = RetrieveState.Error;
                    ConnectionErrorMessage = "API서버와 연결할 수 없습니다.";
                }
                else
                {
                    await _serverStatus.RetrieveServerStatus();
                    ConnectionState = RetrieveState.Loaded;
                }
            }
            catch (SocketException e)
            {
                Logger.Debug(e);
                ConnectionState = RetrieveState.Error;
                ConnectionErrorMessage = e.Message;
            }
        }
    }
}
