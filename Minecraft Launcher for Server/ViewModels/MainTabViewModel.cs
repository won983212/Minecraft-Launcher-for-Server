using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.ViewModels
{
    class TabItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public PageViewModel ViewModel { get; set; }
    }

    class MainTabViewModel : ApplicationViewModel
    {
        private int _currentPageIndex;

        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                UpdatePage(value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TabItem> TabItems { get; set; }

        public MainTabViewModel(ApplicationViewModel root)
            : base(root)
        {
            TabItems = new ObservableCollection<TabItem>();
            TabItems.Add(new TabItem() { Title = "홈", Icon = "Home", ViewModel = new HomeTabViewModel(this) });
            TabItems.Add(new TabItem() { Title = "공지사항", Icon = "Notifications", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "이벤트", Icon = "CardGiftcard", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "패치내역", Icon = "ContentPaste", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "가이드", Icon = "ImportContacts", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "프로필", Icon = "Account", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "설정", Icon = "Settings", ViewModel = null });
            UpdatePage(0);
        }

        private void UpdatePage(int pageIdx)
        {
            _currentPageIndex = pageIdx;
            CurrentPage = TabItems[pageIdx].ViewModel;
        }
    }
}
