using Minecraft_Launcher_for_Server.Pages;
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
    class LoginFormViewModel : PageViewModelBase
    {
        private RetrieveState _connectionState = RetrieveState.Processing;
        private string _connectionErrorMessage = "";
        private ICommand _reconnectCommand;

        // TODO TEST: 매번 서버열기 귀찮으니 잠시 비활성화
        public bool CanLogin
        {
            //get => _connectionState == RetrieveState.Loaded;
            get => true;
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

        public void LoginSuccess()
        {
            ((MainViewModel)RootViewModel).GoToPage(new TabMainViewModel(RootViewModel));
        }

        private async Task TestIfServerClosed()
        {
            ConnectionState = RetrieveState.Processing;
            ServerStatus status = App.GetContext().ServerStatus;
            string messagePrefix = "API 서버 연결중 오류: ";

            try
            {
                bool isActive = await Task.Factory.StartNew(ServerStatus.IsActiveAPIServer);
                if (!isActive)
                    throw new Exception("API 서버와 연결할 수 없습니다.");
                await status.RetrieveAPIServerVersion();

                messagePrefix = "마인크래프트 서버 연결중 오류: ";
                await status.RetrieveServerStatus();
                ConnectionState = RetrieveState.Loaded;
            }
            catch (Exception e)
            {
                Logger.Debug(e);
                ConnectionState = RetrieveState.Error;
                ConnectionErrorMessage = messagePrefix + e.Message;
            }
        }
    }
}
