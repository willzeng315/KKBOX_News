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
using KKBOX_News.ViewModels;
using Microsoft.Phone.Tasks;
using Community.CsharpSqlite.SQLiteClient;

namespace KKBOX_News
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 建構函式
        public MainPage()
        {
            InitializeComponent();

            // 將清單方塊控制項的資料內容設為範例資料
            DataContext = App.ViewModel;
        }

        // 載入 ViewModel 項目的資料
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsTopicsXmlLoaded)
            {
                App.ViewModel.SelectTopicParser();
            }
        }

        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox topics = sender as ListBox;
            ChannelListItem channelItem = topics.SelectedItem as ChannelListItem;

            if (topics.SelectedIndex != -1)
            {
                String sDestination = String.Format("/ArticleListPage.xaml?XML={0}&Title={1}", channelItem.Url, channelItem.Title);
                this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
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
            deleteDirectoryFromDB(mySelectedArticleDirectory.DirectoryIndex, mySelectedArticleDirectory);
            
        }

        private void deleteDirectoryFromDB(Int32 DirIndex, MySelectedArticleDirectory mySelectedArticleDirectory)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = String.Format("DELETE FROM directoryArticles WHERE directoryId={0}", DirIndex);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = String.Format("DELETE FROM directoryTable WHERE id={0}", DirIndex);
                    cmd.ExecuteNonQuery();
                    App.ViewModel.ArticleDirectories.Remove(mySelectedArticleDirectory);
                }
            }
        }



        private void OnSelectedDirectoyClick(object sender, RoutedEventArgs e)
        {
            Button image = (Button)sender;
            MySelectedArticleDirectory mySelectedArticleDirectory = (MySelectedArticleDirectory)image.DataContext;

            //if (isDirectoryHasContent(mySelectedArticleDirectory.DirectoryIndex))
            //{
                String sDestination = String.Format("/ArticleListPage.xaml?DirectoryIndex={0}&DirectoryTitle={1}", mySelectedArticleDirectory.DirectoryIndex, mySelectedArticleDirectory.Title);
                this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
            //}
            
        }

    }
}