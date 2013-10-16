using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Diagnostics;
using Microsoft.Phone.Tasks;
using KKBOX_News.AppService;

namespace KKBOX_News
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationService.RemoveBackEntry();
        }

        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox topics = sender as ListBox;
            ChannelListItem channelItem = topics.SelectedItem as ChannelListItem;

            if (topics.SelectedIndex != -1)
            {
                if (channelItem.IsReadFromRss)
                {
                    String sDestination = String.Format("/ArticleListPage.xaml?Xml={0}&Title={1}", channelItem.Url, channelItem.Title);
                    this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
                }
                else
                {
                    this.NavigationService.Navigate(new Uri(String.Format("/ArticleListPage.xaml?BrowseRecord&Title={0}", channelItem.Title), UriKind.Relative));
                }
            }
            topics.SelectedIndex = -1;
        }

        private void OnSettingSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedItem != null)
            {
                SettingListItem settingListItem = (SettingListItem)((ListBox)sender).SelectedItem;
                if (settingListItem.Link != "")
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(settingListItem.Link, UriKind.Absolute);
                    webBrowserTask.Show();
                }
                else if (settingListItem.PageLink != null)
                {
                    if (settingListItem.Function == "logout")
                    {
                        App.SaveSettings();
                    }
                    this.NavigationService.Navigate(new Uri(settingListItem.PageLink, UriKind.Relative));
                }
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void OnDirectoryEditMenuClick(Object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            MySelectedArticleDirectory mySelectedArticleDirectory = (MySelectedArticleDirectory)menuItem.DataContext;
            String sDestination = String.Format("/CoverInformationEditPage.xaml?Title={0}&DirectoryIndex={1}", mySelectedArticleDirectory.Title, mySelectedArticleDirectory.DirectoryIndex);

            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
        }

        private void OnDirectoryDeleteMenuClick(Object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            MySelectedArticleDirectory mySelectedArticleDirectory = (MySelectedArticleDirectory)menuItem.DataContext;
            DeleteDirectory(mySelectedArticleDirectory.DirectoryIndex);
        }

        private void DeleteDirectory(Int32 directoryIndex)
        {
            DBManager.Instance.DeleteDirectoryFromTable(directoryIndex);
            App.ViewModel.ReLoadDirectory();
        }

        private void OnSelectedDirectoyClick(Object sender, RoutedEventArgs e)
        {
            Button image = (Button)sender;
            MySelectedArticleDirectory mySelectedArticleDirectory = (MySelectedArticleDirectory)image.DataContext;

            String sDestination = String.Format("/ArticleListPage.xaml?DirectoryIndex={0}&DirectoryTitle={1}", mySelectedArticleDirectory.DirectoryIndex, mySelectedArticleDirectory.Title);
            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
        }

        private void OnSearchArticleButtonClick(Object sender, RoutedEventArgs e)
        {
            String sDestination = String.Format("/ArticleListPage.xaml?SearchArticleMode");
            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
        }
    }
}