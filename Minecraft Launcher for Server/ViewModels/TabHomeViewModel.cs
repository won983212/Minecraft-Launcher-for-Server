using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class TabHomeViewModel : PageViewModelBase
    {
        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                if (_startCommand == null)
                    _startCommand = new RelayCommand(OnStart, CanStart);
                return _startCommand;
            }
        }

        public TabHomeViewModel(ParentViewModelBase parent)
            : base(parent)
        { }

        private void OnStart(object arg)
        {
            TabMainViewModel mainVModel = RootViewModel as TabMainViewModel;
            mainVModel.StartDownload();
        }

        private bool CanStart(object arg)
        {
            return (RootViewModel as TabMainViewModel).CanStart();
        }
    }
}
