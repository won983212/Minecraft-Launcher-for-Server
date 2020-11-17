using Minecraft_Launcher_for_Server.ViewModels;
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

namespace Minecraft_Launcher_for_Server.Pages
{
    /**
     * LoginForm의 일부는 보안상의 이유로 MVVM패턴을 적용하지 않음.
     */
    public partial class LoginForm : UserControl
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void AddErrorSnackbar(string message)
        {
           ((MainViewModel) (DataContext as LoginFormViewModel).RootViewModel).AddErrorSnackbar(message);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginFormViewModel vm = DataContext as LoginFormViewModel;
            if (!vm.CanLogin)
            {
                AddErrorSnackbar("서버와 연결되지 않았습니다.");
            }
            else if (string.IsNullOrWhiteSpace(tbxUsername.Text))
            {
                AddErrorSnackbar("이메일을 입력해주세요!");
            }
            else if (string.IsNullOrWhiteSpace(tbxPassword.Password))
            {
                AddErrorSnackbar("비밀번호를 입력해주세요!");
            }
            else
            {
                DoAuth();
            }
        }

        private async void DoAuth()
        {
            loginForm.IsEnabled = false;
            string id = tbxUsername.Text;
            string pass = tbxPassword.Password;

            AuthResponse res = null;
            try
            {
                res = await Task.Factory.StartNew(() => MinecraftAPI.DoAuth(id, pass));
            }
            catch (BadAuthenticationException e)
            {
                AddErrorSnackbar(e.ErrorMessage);
            }

            loginForm.IsEnabled = true;

            if (res != null)
            {
                // TODO Login successful
            }
        }
    }
}
