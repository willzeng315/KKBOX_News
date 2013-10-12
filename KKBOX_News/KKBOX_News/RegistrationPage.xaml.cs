using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Community.CsharpSqlite.SQLiteClient;
using System.Diagnostics;

namespace KKBOX_News
{
    public partial class RegistrationPage : PhoneApplicationPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private Boolean verifyPasswordEqual()
        {
            if (passwordTextBox.Password !="" && (passwordTextBox.Password == passwordCheckTextBox.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private Boolean verifyAccountExists(String account)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT account FROM userAccount";
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //Debug.WriteLine(reader.GetString(0));
                            if (account == reader.GetString(0))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                }
            }
        }

        private void createNewAcoount()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = "INSERT INTO userAccount (account, password, openExternalWeb, openAutoUpdate, updateInterval) VALUES(@account, @password, @openExternalWeb, @openAutoUpdate, @updateInterval);SELECT last_insert_rowid();";
                    cmd.Parameters.Add("@account", accountTextBox.Text);
                    cmd.Parameters.Add("@password", passwordTextBox.Password);
                    cmd.Parameters.Add("@openExternalWeb", 0);
                    cmd.Parameters.Add("@openAutoUpdate", 0);
                    cmd.Parameters.Add("@updateInterval", 5);

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }

        private void OnRegistrationButtonClick(object sender, RoutedEventArgs e)
        {
            if (!verifyAccountExists(accountTextBox.Text))
            {
                MessageBox.Show("帳號已存在, 請改用其他帳號");
            }
            if (!verifyPasswordEqual())
            {
                MessageBox.Show("密碼與確認密碼不相符");
            }
            if (verifyAccountExists(accountTextBox.Text) && verifyPasswordEqual())
            {
                createNewAcoount();
                this.NavigationService.GoBack();
            }
        }
    }
}