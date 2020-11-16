using Minecraft_Launcher_for_Server.Updater;
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
        public PageViewModelBase ViewModel { get; set; }
    }

    class TabMainViewModel : ParentViewModelBase
    {
        private ContentUpdater _updater = new ContentUpdater();

        private int _currentPageIndex;
        private bool _isShowDownloadStatus = false;
        private string _downloadStatus = "";

        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            set
            {
                UpdatePage(value);
                OnPropertyChanged();
            }
        }

        public bool IsShowDownloadStatus
        {
            get => _isShowDownloadStatus;
            private set
            {
                _isShowDownloadStatus = value;
                OnPropertyChanged();
            }
        }

        public string DownloadStatus
        {
            get => _downloadStatus;
            private set
            {
                _downloadStatus = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TabItem> TabItems { get; set; }

        public TabMainViewModel(ParentViewModelBase root)
            : base(root)
        {
            TabItems = new ObservableCollection<TabItem>();
            TabItems.Add(new TabItem() { Title = "홈", Icon = "Home", ViewModel = new TabHomeViewModel(this) });
            TabItems.Add(new TabItem() { Title = "공지사항", Icon = "Notifications", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "이벤트", Icon = "CardGiftcard", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "패치내역", Icon = "ContentPaste", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "가이드", Icon = "ImportContacts", ViewModel = null });
            TabItems.Add(new TabItem() { Title = "프로필", Icon = "Account", ViewModel = null });
            UpdatePage(0);
        }

        private void UpdatePage(int pageIdx)
        {
            _currentPageIndex = pageIdx;
            CurrentPage = TabItems[pageIdx].ViewModel;
        }

        public void StartDownload()
        {
            IsShowDownloadStatus = true;
            DownloadStatus = "다운로드 중..";
        }

        public bool CanStart()
        {
            return !_isShowDownloadStatus;
        }
    }
}
