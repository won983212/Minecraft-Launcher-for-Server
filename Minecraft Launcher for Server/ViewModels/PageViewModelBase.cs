using Minecraft_Launcher_for_Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class PageViewModelBase : ObservableObject
    {
        public readonly ParentViewModelBase RootViewModel;

        public PageViewModelBase(ParentViewModelBase rootViewModel)
        {
            RootViewModel = rootViewModel;
        }
    }
}
