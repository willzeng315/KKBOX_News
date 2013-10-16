using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KKBOX_News.Resources;
using KKBOX_News.AppService;

namespace KKBOX_News
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            CheckUserAccountTableExistAndCreate();
            LoadUserSettings();
            DataContext = this;
        }

        private void CheckUserAccountTableExistAndCreate()
        {
            if (!InitializeDB.Instance.IsTableExists("userAccount"))
            {
                CreateUserAccount();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private void CreateUserAccount()
        {
            if (InitializeDB.Instance.CreateAccountTable())
            {
                InitializeDB.Instance.InitialAccoutData();
            }
        }

        private void LoadUserSettings()
        {
            IsSaveAccountAndPassword = LoginSettings.Instance.IsSaveAccountAndPassword;
            if (IsSaveAccountAndPassword)
            {
                accountTextBox.Text = LoginSettings.Instance.LastAccount;
                passwordTextBox.Password = LoginSettings.Instance.LastPassword;
            }

        }

        private Boolean isNotAccountOrPasswordEmpty()
        {
            return (accountTextBox.Text == "" || passwordTextBox.Password == "") ? false : true;
        }

        private void SaveAccountAndPassword(Boolean isSave)
        {
            if (isSave)
            {
                LoginSettings.Instance.LastAccount = accountTextBox.Text;
                LoginSettings.Instance.LastPassword = passwordTextBox.Password;
            }
        }

        private void CheckUserTablesExistsAndCreate()
        {
            if (!InitializeDB.Instance.IsTableExists(String.Format("directoryTableUser{0}", userId)))
            {
                InitializeDB.Instance.CreateUserTables(userId);
                InitializeDB.Instance.InitialUserTableData(userId);
            }
        }

        private void OnLoginButtonClick(Object sender, RoutedEventArgs e)
        {
            String account = accountTextBox.Text;
            String password = passwordTextBox.Password;

            if (isNotAccountOrPasswordEmpty() && DBManager.Instance.VerifyUserAccount(account, password))
            {
                userId = DBManager.Instance.UserId;

                DBManager.Instance.LoadUserSetting();
                CheckUserTablesExistsAndCreate();
                SaveAccountAndPassword(IsSaveAccountAndPassword);

                LoginSettings.Instance.IsSaveAccountAndPassword = IsSaveAccountAndPassword;
                LoginSettings.Instance.Login = true;
                LoginSettings.Instance.CurrentAccount = accountTextBox.Text;

                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
            {
                MessageBox.Show(AppResources.AccountPasswordError);
            }
        }

        private void OnRegistrationButtonClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RegistrationPage.xaml", UriKind.Relative));
        }

        private Int32 userId;

        #region Property

        private Boolean isSaveAccountAndPassword;
        public Boolean IsSaveAccountAndPassword
        {
            get
            {
                return isSaveAccountAndPassword;
            }
            set
            {
                isSaveAccountAndPassword = value;
            }
        }
        #endregion
    }
}