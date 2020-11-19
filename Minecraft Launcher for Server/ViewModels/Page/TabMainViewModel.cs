using Minecraft_Launcher_for_Server.Updater;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Launcher_for_Server.ViewModels.Page
{
    class TabItem
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public PageViewModelBase ViewModel { get; set; }
    }

    class TabMainViewModel : ParentViewModelBase
    {
        private Launcher _launcher = new Launcher();
        private int _currentPageIndex;
        private bool _isShowDownloadStatus = false;
        private double _downloadProgress = 0;
        private string _downloadStatus = "";
        private bool _canStart = true;

        public event EventHandler DownloadCompleted;

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

        public double DownloadProgress
        {
            get => _downloadProgress;
            private set
            {
                _downloadProgress = value;
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

            _launcher.OnLog += (s, t) => Logger.Log(t);
            _launcher.OnError += (s, t) => Logger.Error(t);
            _launcher.OnExited += (s, t) => Logger.Log(" Exited (code: " + t + ")");
        }

        private void UpdatePage(int pageIdx)
        {
            _currentPageIndex = pageIdx;
            CurrentPage = TabItems[pageIdx].ViewModel;
        }

        public void StartDownload()
        {
            IsShowDownloadStatus = true;
            _canStart = false;
            DownloadStatus = "다운로드 중..";
            DownloadProgress = 0;

            ContentUpdater updater = new ContentUpdater();
            updater.OnProgress += Updater_OnProgress;
            updater.BeginDownload();
        }

        private void Updater_OnProgress(object sender, ProgressArgs e)
        {
            DownloadStatus = e.Status;
            DownloadProgress = e.Progress;

            if(IsShowDownloadStatus && e.Progress >= 100)
            {
                IsShowDownloadStatus = false;
                App.GetContext().UpdatePatchVersion();
                DownloadCompleted?.Invoke(this, null);
                StartMinecraft();
            }
        }

        public async Task StartMinecraft()
        {
            await _launcher.Start();
            _canStart = true;
            if (!Properties.Settings.Default.UseLogging)
                App.Current.Shutdown(0);
        }

        public bool CanStart()
        {
            return _canStart && !_launcher.IsRunning;
        }
    }
}
