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
            return (passwordTextBox.Password != "" && (passwordTextBox.Password == passwordCheckTextBox.Password)) ? true : false;
        }

        private void OnRegistrationButtonClick(object sender, RoutedEventArgs e)
        {
            String sAccount = accountTextBox.Text;
            String sPassword = passwordTextBox.Password;
            if (DBManager.Instance.VerifyAccountExists(sAccount))
            {
                MessageBox.Show("帳號已存在, 請改用其他帳號");
            }
            if (!verifyPasswordEqual())
            {
                MessageBox.Show("密碼與確認密碼不相符");
            }
            if (!DBManager.Instance.VerifyAccountExists(sAccount) && verifyPasswordEqual())
            {
                DBManager.Instance.CreateNewAcoount(sAccount,sPassword);
                this.NavigationService.GoBack();
            }
        }
    }
}