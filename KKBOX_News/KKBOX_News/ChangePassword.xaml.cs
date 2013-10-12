using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace KKBOX_News
{
    public partial class ChangePassword : PhoneApplicationPage
    {
        public ChangePassword()
        {
            InitializeComponent();
        }


        private Boolean verifyPasswordEqual()
        {
            if (newPasswordTextBox.Password != "" && (newPasswordTextBox.Password == newPasswordCheckTextBox.Password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {

        }
    }
}