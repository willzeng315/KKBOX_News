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
            createUserAccount();
            loadUserSettings();
            DataContext = this;
            //List<String> a =new List<string>();
            //a.Add("abc");

           // DBManger.Instance.UpdateValueToArticleTable("123", 1, a);

            Debug.WriteLine("LoginPage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private void createUserAccount()
        {
            if (InitializeDB.Instance.CreateAccountTable())
            {
                InitializeDB.Instance.InitialAccoutData();
            }
        }

        private void loadUserSettingFromDB()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM userAccount WHERE id={0}",UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserSettings.Instance.IsOpenExternalWeb = reader.GetBoolean(3);
                            UserSettings.Instance.IsOpenAutoUpdate = reader.GetBoolean(4);
                            UserSettings.Instance.UpdateInterval = reader.GetInt32(5);
                        }
                    }
                }
            }
        }

        private void loadUserSettings()
        {
            IsSaveAccountAndPassword = LoginSettings.Instance.IsSaveAccountAndPassword;
            if (IsSaveAccountAndPassword)
            {
                accountTextBox.Text = LoginSettings.Instance.LastAccount;
                passwordTextBox.Password = LoginSettings.Instance.LastPassword;
            }
            
        }

        private Boolean verifyUserAccount()
        {
            String account = accountTextBox.Text;
            String password = passwordTextBox.Password;
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM userAccount WHERE account=@account AND password=@password";
                    cmd.Parameters.Add("@account", account);
                    cmd.Parameters.Add("@password", password);
                    int n = cmd.ExecuteNonQuery();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserId = reader.GetInt32(0);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        private Boolean isNotAccountOrPasswordEmpty()
        {
            if (accountTextBox.Text == "" || passwordTextBox.Password == "")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void saveAccountAndPassword(Boolean isSave)
        {
            if (isSave)
            {
                LoginSettings.Instance.LastAccount = accountTextBox.Text;
                LoginSettings.Instance.LastPassword = passwordTextBox.Password;
            }
        }

        private void createUserTables()
        {
            if (InitializeDB.Instance.CreateUserTables(UserId))
            {
                InitializeDB.Instance.InitialUserTableData(UserId);
            }
        }

        private void OnLoginButtonClick(Object sender, RoutedEventArgs e)
        {
            if (isNotAccountOrPasswordEmpty() && verifyUserAccount())
            {
                loadUserSettingFromDB();
                createUserTables();
                saveAccountAndPassword(IsSaveAccountAndPassword);

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