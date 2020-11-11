using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class ParentViewModelBase : PageViewModelBase
    {
        private PageViewModelBase _currentPage;

        public PageViewModelBase CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        public ParentViewModelBase()
            :this(null)
        { }

        public ParentViewModelBase(ParentViewModelBase rootViewModel)
            : base(rootViewModel)
        { }
    }
}
