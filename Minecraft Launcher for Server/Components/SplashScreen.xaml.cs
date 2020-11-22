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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minecraft_Launcher_for_Server.Components
{
    /// <summary>
    /// SplashScreen.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SplashScreen : UserControl
    {
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", 
            typeof(bool), typeof(SplashScreen), new FrameworkPropertyMetadata(false));

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set { SetValue(IsActiveProperty, value); }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }
    }
}
