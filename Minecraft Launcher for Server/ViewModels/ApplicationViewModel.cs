using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class ApplicationViewModel : PageViewModel
    {
        private PageViewModel _currentPage;

        public PageViewModel CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ApplicationViewModel()
            :this(null)
        { }

        public ApplicationViewModel(ApplicationViewModel rootViewModel)
            : base(rootViewModel)
        { }
    }
}
