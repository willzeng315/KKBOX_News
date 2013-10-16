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
using KKBOX_News.Resources;
using KKBOX_News.AppService;

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
                MessageBox.Show(AppResources.AccountExist);
            }
            if (!verifyPasswordEqual())
            {
                MessageBox.Show(AppResources.PWNotEqual);
            }
            if (!DBManager.Instance.VerifyAccountExists(sAccount) && verifyPasswordEqual())
            {
                DBManager.Instance.CreateNewAcoount(sAccount, sPassword);
                this.NavigationService.GoBack();
            }
        }
    }
}