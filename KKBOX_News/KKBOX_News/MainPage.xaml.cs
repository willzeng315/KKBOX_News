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
            //TopicListBox.SelectedItem = null;
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

        private void OnSelectedDirectoyClick(Object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            Debug.WriteLine(sender.GetType().ToString());
            Image image = (Image)sender;
            MySelectedArticleDirectory mySelectedArticleDirectory = (MySelectedArticleDirectory)image.DataContext;
            //if (mySelectedArticleDirectory.ArticleItemList != null)
            //{
            //    for(int i = 0 ; i < mySelectedArticleDirectory.ArticleItemList.Count ; i++)
            //    {
            //        Debug.WriteLine(mySelectedArticleDirectory.ArticleItemList[i].Title);
            //    }
            //}
            String sDestination = String.Format("/ArticleListPage.xaml?DirectoryIndex={0}&DirectoryTitle={1}", mySelectedArticleDirectory.DirectoryIndex, mySelectedArticleDirectory.Title);
            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
            //Debug.WriteLine(mySelectedArticleDirectory.DirectoryIndex);
        }

        private void OnSettingSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            if (((ListBox)sender).SelectedItem != null)
            {
                SettingListItem settingListItem = (SettingListItem)((ListBox)sender).SelectedItem;
                if (settingListItem.Link != null)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(settingListItem.Link, UriKind.Absolute);
                    webBrowserTask.Show();
                }
            }
            ((ListBox)sender).SelectedItem = null;
        }

        private void OnPhoneApplicationPageLoaded(object sender, RoutedEventArgs e)
        {
            //SqliteConnection connection = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db");
            
            //Debug.WriteLine( LoadMySelectedSqlite.CreateTables().ToString());
        }

    }
}