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
using Community.CsharpSqlite.SQLiteClient;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KKBOX_News
{
    public class UserSettings
    {
        private static UserSettings _instance;

        private UserSettings() { }

        public static UserSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UserSettings();
                return _instance;
            }
        }
        public Boolean IsOpenExternalWeb;
        public Boolean IsOpenAutoUpdate;
        public Int32 UpdateInterval;
    }

    public class LoginSettings
    {
        private static LoginSettings _instance;

        private LoginSettings() { }

        public static LoginSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LoginSettings();
                return _instance;
            }
        }

        public String LastAccount;
        public String LastPassword;
        public String CurrentAccount;
        public Boolean IsSaveAccountAndPassword;
        public Boolean Login;
    }

    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            CheckUserAccountTableExistAndCreate();
            DataContext = this;
            Debug.WriteLine("LoginPage");
        }

        private void CheckUserAccountTableExistAndCreate()
        {
            if (!InitializeDB.Instance.IsTableExists("userAccount"))
            {
                CreateUserAccount();
                LoadUserSettings();
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
            if (!InitializeDB.Instance.IsTableExists(String.Format("directoryTableUser{0}",UserId)))
            {
                InitializeDB.Instance.CreateUserTables(UserId);
                InitializeDB.Instance.InitialUserTableData(UserId);
            }
        }

        private void OnLoginButtonClick(Object sender, RoutedEventArgs e)
        {
            String account = accountTextBox.Text;
            String password = passwordTextBox.Password;

            if (isNotAccountOrPasswordEmpty() && DBManager.Instance.VerifyUserAccount(account,password))
            {
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
                MessageBox.Show("帳號或密碼錯誤");
            }
        }

        private void OnRegistrationButtonClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RegistrationPage.xaml", UriKind.Relative));
        }
        
        public static Int32 UserId;

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