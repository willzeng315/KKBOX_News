using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.Diagnostics;
using KKBOX_News.Resources;

namespace KKBOX_News
{
    public partial class UpdateIntervalSelector : PhoneApplicationPage
    {
        public UpdateIntervalSelector()
        {
            InitializeComponent();
            LoadMinutesSet();
            DataContext = this;
        }

        private void LoadMinutesSet()
        {
            Minutes = new ObservableCollection<string>();
            Minutes.Add("1");
            Minutes.Add("3");
            Minutes.Add("5");
        }

        public ObservableCollection<String> Minutes
        {
            get;
            set;
        }

        private void OnConfirmClick(Object sender, RoutedEventArgs e)
        {
            if (minuteSelector.SelectedItem != null)
            {
                App.ViewModel.Settings[5].UpdateInterval = String.Format("{0}{1}", minuteSelector.SelectedItem.ToString(), AppResources.Minute);//value.ToString() + "分";
                UserSettings.Instance.UpdateInterval = Int32.Parse(minuteSelector.SelectedItem.ToString());
            }
            NavigationService.GoBack();
        }

        private void OnConcelClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}