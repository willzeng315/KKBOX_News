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

namespace KKBOX_News
{
    public partial class UpdateIntervalSelector : PhoneApplicationPage
    {
        public UpdateIntervalSelector()
        {
            InitializeComponent();
            loadMinutesSet();
            DataContext = this;
        }

        private void loadMinutesSet()
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

        private void OnComfirmClick(Object sender, RoutedEventArgs e)
        {
            if(minuteSelector.SelectedItem !=null)
            {
                Debug.WriteLine(Int32.Parse(minuteSelector.SelectedItem.ToString()));
                ArticleListPage.ArticleUpdateTimeInterval = Int32.Parse(minuteSelector.SelectedItem.ToString());
            }
            NavigationService.GoBack();
        }

        private void OnConcelClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}