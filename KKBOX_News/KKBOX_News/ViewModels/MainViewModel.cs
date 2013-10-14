using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KKBOX_News.Resources;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Community.CsharpSqlite.SQLiteClient;
using System.Windows;

namespace KKBOX_News
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            LoadDirectoris();
            LoadSettingList();
        }

        private void LoadSettingList()
        {
            Settings = new ObservableCollection<SettingListItem>();
            Settings.Add(new SettingListItem()
            {
                Title = "帳戶",
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = "登出帳戶",
                PageLink = "/LoginPage.xaml",
                Content = LoginSettings.Instance.CurrentAccount,
                Type = SettingItemTemplate.TEMPLATE_TEXT_CONTENT
            });
            Settings.Add(new SettingListItem() 
            { 
                Title = "行為",
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = "以外部瀏覽器開啟文章",
                Content = "應用程式內直接顯示網頁",
                Type = SettingItemTemplate.TEMPLATE_TXET_CHECK,
                IsChecked = UserSettings.Instance.IsOpenExternalWeb,
                FunctionOfCheck = "OpenExternalWeb"
            });
            Settings.Add(new SettingListItem()
            {
                Title = "開啟自動更新",
                Content = "開啟自動更新資訊",
                Type = SettingItemTemplate.TEMPLATE_TXET_CHECK,
                IsChecked = UserSettings.Instance.IsOpenAutoUpdate,
                FunctionOfCheck = "OpenAutoUpdate"
            });


            Settings.Add(new SettingListItem()
            {
                Title = "自動更新頻率",
                Content = "設定每次自動更新的間隔時間",
                PageLink = "/UpdateIntervalSelector.xaml",
                UpdateInterval = String.Format("{0}{1}", UserSettings.Instance.UpdateInterval, "分"),
                Type = SettingItemTemplate.TEMPLATE_TEXT_CONTENT
            });
            Settings.Add(new SettingListItem()
            {
                Title = "關於",
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = "關於KKBOX",
                Link = "http://www.kkbox.com/about/tc/",
                Type = SettingItemTemplate.TEMPLATE_TEXT
            });
            Settings.Add(new SettingListItem()
            {
                Title = "關於KKBOX 音樂誌",
                Link = "http://www.kkbox.com/tw/tc/column/index.html",
                Type = SettingItemTemplate.TEMPLATE_TEXT
            });
            Settings.Add(new SettingListItem()
            {
                Title = "系統",
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });

            AssemblyName assembly = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            Settings.Add(new SettingListItem()
            {
                Title = "版本",
                Content = assembly.Version.ToString(),
                Type = SettingItemTemplate.TEMPLATE_TEXT_CONTENT
            });
        }

        private void LoadDirectoris()
        {
            ArticleDirectories = new ObservableCollection<MySelectedArticleDirectory>();
            try
            {
                ArticleDirectories = DBManager.Instance.LoadDirectoriesFromTable();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
                
        }

        private void LoadTopics(DownloadStringCompletedEventArgs args) 
        {
            Topics = new ObservableCollection<ChannelListItem>();

            XDocument ParsedTopic = XDocument.Parse(args.Result);
            TopicsInXml = ParsedTopic.Element("kkbox_news").Elements("channel");

            foreach (XElement channel in TopicsInXml)
            {
                Topics.Add(new ChannelListItem() { Title = channel.Element("title").Value, ImagePath = channel.Element("icon").Value, Url = channel.Element("url").Value });
            }

            Topics.Add(new ChannelListItem() { Title = "瀏覽紀錄", IsReadFromRss = false, ImagePath = "Images/lib_browse.png" });
            IsTopicsXmlLoaded = true;
        }

        public void SelectTopicParser()
        {
            IsTopicsXmlLoaded = false;
            Uri uri = new Uri("https://mail.kkbox.com.tw/~willzeng/SelectChannel.xml");

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadTopicXmlCompleted;
            webClient.DownloadStringAsync(uri);
        }

        void OnDownloadTopicXmlCompleted(Object sender, DownloadStringCompletedEventArgs args)//loadTipics
        {
            LoadTopics(args);
        }
        #region Property

        private IEnumerable<XElement> TopicsInXml;
        
        private Boolean isTopicsXmlLoaded;
        public Boolean IsTopicsXmlLoaded
        {
            get
            {
                return isTopicsXmlLoaded;
            }
            set
            {
                SetProperty(ref isTopicsXmlLoaded, value, "IsTopicsXmlLoaded");
            }
        }

        private ObservableCollection<ChannelListItem> topics;
        public ObservableCollection<ChannelListItem> Topics
        {
            get
            {
                return topics;
            }
            set
            {
                SetProperty(ref topics, value, "Topics");
            }
        }

        private ObservableCollection<MySelectedArticleDirectory> articleDirectories;
        public ObservableCollection<MySelectedArticleDirectory> ArticleDirectories
        {
            get
            {
                return articleDirectories;
            }
            set
            {
                SetProperty(ref articleDirectories, value, "ArticleDirectories");
            }
        }

        private ObservableCollection<SettingListItem> settings;
        public ObservableCollection<SettingListItem> Settings
        {
            get
            {
                return settings;
            }
            set
            {
                SetProperty(ref settings, value, "Settings");
            }
        }
        #endregion
    }
}