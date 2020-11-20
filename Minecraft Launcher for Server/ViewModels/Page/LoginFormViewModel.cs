using Minecraft_Launcher_for_Server.Pages;
using Minecraft_Launcher_for_Server.Updater;
using Minecraft_Launcher_for_Server.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels.Page
{
    class LoginFormViewModel : PageViewModelBase
    {
        private ICommand _reconnectCommand;

        public bool CanLogin
        {
            get => ConnectionState == RetrieveState.Loaded;
        }

        public string ConnectionErrorMessage
        {
            get
            {
                string message = App.MainContext.ServerStatus.ConnectionState.ErrorMessage;
                if (string.IsNullOrEmpty(message))
                {
                    return "연결 성공";
                } 
                else
                {
                    return message;
                }
            }
        }

        public RetrieveState ConnectionState
        {
            get => App.MainContext.ServerStatus.ConnectionState.State;
        }

        public ICommand ReconnectCommand
        {
            get => _reconnectCommand = _reconnectCommand ?? new RelayCommand((a) => App.MainContext.ServerStatus.RetrieveAll());
        }

        public LoginFormViewModel(ParentViewModelBase root)
            : base(root)
        {
            App.MainContext.ServerStatus.OnConnectionStateChanged += MainContext_OnConnectionStateChanged;
        }

        private void MainContext_OnConnectionStateChanged(object sender, ConnectionState e)
        {
            OnPropertyChanged("ConnectionErrorMessage");
            OnPropertyChanged("ConnectionState");
        }

        public void LoginSuccess()
        {
            ((MainViewModel)RootViewModel).GoToPage(new TabMainViewModel(RootViewModel));
        }
    }
}
