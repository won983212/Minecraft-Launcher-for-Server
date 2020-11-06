using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// FloatingHintPasswordBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FloatingHintPasswordBox : UserControl, INotifyPropertyChanged
    {
        public bool IsEmpty
        {
            get => passBox.Password.Length == 0;
        }

        public string Password
        {
            get => passBox.Password;
        }

        public FloatingHintPasswordBox()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void passBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEmpty"));
        }
    }
}
