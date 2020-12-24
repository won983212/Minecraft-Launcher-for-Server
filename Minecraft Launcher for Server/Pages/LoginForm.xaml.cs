using Minecraft_Launcher_for_Server.Properties;
using Minecraft_Launcher_for_Server.ViewModels;
using Minecraft_Launcher_for_Server.ViewModels.Page;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private static readonly AuthInfoStorage _authStorage = App.MainContext.AuthInfoStorage;
        private bool _hasKeepLogged = true;

        public LoginForm()
        {
            InitializeComponent();
            App.MainContext.ServerStatus.OnConnectionStateChanged += MainContext_OnConnectionStateChanged;

            if (_authStorage.HasTokenData())
            {
                loginForm.IsEnabled = false;
                cbxKeepLogin.IsChecked = true;
            }
        }

        private void MainContext_OnConnectionStateChanged(object sender, ConnectionState e)
        {
            if (e.State == RetrieveState.Loaded)
            {
                if (_hasKeepLogged)
                {
                    Dispatcher.Invoke(() =>
                    {
                        CheckKeepLogin();
                        _hasKeepLogged = false;
                    });
                }
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    loginForm.IsEnabled = true;
                });
            }
        }

        private void AddErrorSnackbar(string message)
        {
           ((MainViewModel) (DataContext as LoginFormViewModel).RootViewModel).AddErrorSnackbar(message);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            TryLogin();
        }

        private void tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TryLogin();
            }
        }

        private void TryLogin()
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

        private async Task CheckKeepLogin()
        {
            if (_authStorage.HasTokenData())
            {
                loginForm.IsEnabled = false;
                try
                {
                    AuthInfo auth = await Task.Factory.StartNew(() => MinecraftAPI.RefreshAccessToken(_authStorage.AccessToken, _authStorage.ClientToken));
                    _authStorage.AccessToken = auth.AccessToken;
                    _authStorage.ClientToken = auth.ClientToken;
                    _authStorage.Save();
                    Login(auth);
                }
                catch (Exception e)
                {
                    Logger.Debug(e);
                    _authStorage.Clear();
                }
            }

            loginForm.IsEnabled = true;
        }

        private async void DoAuth()
        {
            loginForm.IsEnabled = false;
            await ((MainViewModel)(DataContext as LoginFormViewModel).RootViewModel).ThrowIfAPIServerClosed();

            string id = tbxUsername.Text;
            string pass = tbxPassword.Password;

            AuthInfo res = null;
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
                if (cbxKeepLogin.IsChecked == true)
                {
                    _authStorage.AccessToken = res.AccessToken;
                    _authStorage.ClientToken = res.ClientToken;
                    _authStorage.Save();
                }
                Login(res);
            }
        }

        private async void Login(AuthInfo auth)
        {
            string requestURL = URLs.APIUserInfo(auth.UUID);
            string data = await new WebClient().DownloadStringTaskAsync(requestURL);

            JObject obj = JObject.Parse(data);
            if ((bool)obj["whitelist"] == false)
            {
                AddErrorSnackbar("화이트리스트에 등록되지 않은 유저입니다. 가입버튼을 눌러 가입하시길 바랍니다.");
                return;
            }

            App.MainContext.AuthInfo = auth;
            if (DataContext != null && DataContext is LoginFormViewModel)
            {
                ((LoginFormViewModel)DataContext).LoginSuccess();
            }
        }
    }
}
