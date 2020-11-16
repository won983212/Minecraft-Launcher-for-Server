using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minecraft_Launcher_for_Server.Components
{
    /// <summary>
    /// DownloadStatusBar.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DownloadStatusBar : UserControl
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive",
            typeof(bool), typeof(DownloadStatusBar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsActiveChanged)));

        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register("StatusText", typeof(string), typeof(DownloadStatusBar));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set { SetValue(IsActiveProperty, value); }
        }

        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set { SetValue(StatusTextProperty, value); }
        }

        private static readonly TimeSpan AnimationTimeSpan = TimeSpan.FromSeconds(0.5);
        private DoubleAnimation _animation;
        private DoubleAnimation _animationRev;

        public DownloadStatusBar()
        {
            _animation = new DoubleAnimation(60, 0, AnimationTimeSpan);
            _animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            _animationRev = new DoubleAnimation(0, 60, AnimationTimeSpan);
            _animationRev.EasingFunction = _animation.EasingFunction;

            InitializeComponent();
        }

        private void ShowMessage()
        {
            Visibility = Visibility.Visible;
            translatePanel.BeginAnimation(TranslateTransform.YProperty, _animation);
        }

        private void CloseMessage()
        {
            translatePanel.BeginAnimation(TranslateTransform.YProperty, _animationRev);

            /*var timer = new DispatcherTimer { Interval = AnimationTimeSpan };
            timer.Tick += (s, args) =>
            {
                Visibility = Visibility.Collapsed;
                IsShow = false;
                timer.Stop();
            };
            timer.Start();*/
        }

        public static void OnIsActiveChanged(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            DownloadStatusBar bar = obj as DownloadStatusBar;
            if ((bool)arg.NewValue)
                bar.ShowMessage();
            else
                bar.CloseMessage();
        }
    }
}
