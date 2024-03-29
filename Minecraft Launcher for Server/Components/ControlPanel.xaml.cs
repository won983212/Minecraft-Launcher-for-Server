﻿using Minecraft_Launcher_for_Server.ViewModels;
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
using System.Windows.Threading;

namespace Minecraft_Launcher_for_Server.Components
{
    /// <summary>
    /// ControlPanel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public static readonly DependencyProperty IsShowProperty = DependencyProperty.Register("IsShow",
            typeof(bool), typeof(ControlPanel), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsShowChanged)));

        public bool IsShow
        {
            get { return (bool)GetValue(IsShowProperty); }
            set { SetValue(IsShowProperty, value); }
        }

        private static readonly TimeSpan AnimationTimeSpan = TimeSpan.FromSeconds(0.1);
        private DoubleAnimation _animation;
        private DoubleAnimation _animationRev;

        public ControlPanel()
        {
            _animation = new DoubleAnimation(0, 1, AnimationTimeSpan);
            _animation.EasingFunction = new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            _animationRev = new DoubleAnimation(1, 0, AnimationTimeSpan);
            _animationRev.EasingFunction = _animation.EasingFunction;

            Visibility = Visibility.Collapsed;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            IsShow = false;
            Properties.Settings.Default.Save();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
        }

        private void ShowMessage()
        {
            Visibility = Visibility.Visible;
            rectBg.BeginAnimation(OpacityProperty, _animation);
            scalePanel.BeginAnimation(ScaleTransform.ScaleXProperty, _animation);
            scalePanel.BeginAnimation(ScaleTransform.ScaleYProperty, _animation);
            IsShow = true;
        }

        private void CloseMessage()
        {
            rectBg.BeginAnimation(OpacityProperty, _animationRev);
            scalePanel.BeginAnimation(ScaleTransform.ScaleXProperty, _animationRev);
            scalePanel.BeginAnimation(ScaleTransform.ScaleYProperty, _animationRev);

            var timer = new DispatcherTimer { Interval = AnimationTimeSpan };
            timer.Tick += (s, args) =>
            {
                Visibility = Visibility.Collapsed;
                IsShow = false;
                timer.Stop();
            };
            timer.Start();
        }

        public static void OnIsShowChanged(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            ControlPanel dialog = obj as ControlPanel;
            if ((bool)arg.NewValue)
                dialog.ShowMessage();
            else
                dialog.CloseMessage();
        }
    }
}
