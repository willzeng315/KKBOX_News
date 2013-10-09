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
        public static Boolean IsOpenExternalWeb;
        public static Boolean IsOpenAutoUpdate;
        public static Int32 UpdateInterval;

    }

    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            if (LoadMySelectedSqlite.CreateAccountTable())
            {
                LoadMySelectedSqlite.InitialAccoutData();
            }
            DataContext = this;
            
            Debug.WriteLine("LoginPage");
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
                            UserSettings.IsOpenExternalWeb = reader.GetBoolean(3);
                            UserSettings.IsOpenAutoUpdate = reader.GetBoolean(4);
                            UserSettings.UpdateInterval = reader.GetInt32(5);
                        }
                    }
                }
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


        private void OnLoginButtonClick(Object sender, RoutedEventArgs e)
        {
            verifyUserAccount();
            //Debug.WriteLine(accountTextBox.Text);
            if (accountTextBox.Text != null)
            {
                UserId = Int32.Parse(accountTextBox.Text);
                loadUserSettingFromDB();
                if (LoadMySelectedSqlite.CreateUserTables(UserId))
                {
                    LoadMySelectedSqlite.InitialUserTableData(UserId);
                }
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        public static Int32 UserId;

    }
}