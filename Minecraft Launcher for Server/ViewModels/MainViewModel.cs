using MaterialDesignThemes.Wpf;
using Minecraft_Launcher_for_Server.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class MainViewModel : ParentViewModelBase
    {
        private ErrorMessageObject _errorInfo;
        private bool _errorVisible = false;

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

        public bool ErrorVisible
        {
            get => _errorVisible;
            set
            {
                _errorVisible = value;
                OnPropertyChanged();
            }
        }

        public void ThrowIfAPIServerClosed()
        {
            if (!ServerStatus.IsActiveAPIServer())
            {
                ShowErrorMessage("서버가 닫힘", "TEDVENT 서버와 연결할 수 없습니다.\n" +
                            "서버가 닫혀있거나, 인터넷 연결 문제일 수 있으니 다시 시도해보시길 바랍니다.\n" +
                            "확인을 누르면 클라이언트가 종료됩니다.", null);
            }
        }

        public MainViewModel()
        {
            CurrentPage = new LoginFormViewModel(this);
            SnackMessages = new SnackbarMessageQueue(TimeSpan.FromSeconds(2));
            ErrorInfo = new ErrorMessageObject();
        }

        public void ShowErrorMessage(string title, string message, Action callback)
        {
            ErrorInfo = new ErrorMessageObject()
            {
                Title = title,
                Message = message,
                Callback = callback
            };
            ErrorVisible = true;
        }

        public void AddErrorSnackbar(string message)
        {
            SnackMessages.Enqueue(message);
        }
    }
}
