using Minecraft_Launcher_for_Server.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class PageViewModel : ObservableObject
    {
        public readonly ApplicationViewModel RootViewModel;

        public PageViewModel(ApplicationViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
        }
    }
}
