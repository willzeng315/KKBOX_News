using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KKBOX_News.Resources;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using KKBOX_News.DBService;
using KKBOX_News.NetworkService;

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
                Title = AppResources.Account,
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.LogoutAccout,
                PageLink = "/LoginPage.xaml",
                Content = LoginSettings.Instance.CurrentAccount,
                Function = "logout",
                Type = SettingItemTemplate.TEMPLATE_TEXT_CONTENT
            });
            Settings.Add(new SettingListItem() 
            {
                Title = AppResources.Action,
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.ExternalWebOption,
                Content = AppResources.ExternalWebContent,
                Type = SettingItemTemplate.TEMPLATE_TXET_CHECK,
                IsChecked = UserSettings.Instance.IsOpenExternalWeb,
                Function = "OpenExternalWeb"
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.AutoUpdateOption,
                Content = AppResources.AutoUpdateOption,
                Type = SettingItemTemplate.TEMPLATE_TXET_CHECK,
                IsChecked = UserSettings.Instance.IsOpenAutoUpdate,
                Function = "OpenAutoUpdate"
            });


            Settings.Add(new SettingListItem()
            {
                Title = AppResources.AutoUpdateFrequency,
                Content = AppResources.UpdateIntervalContent,
                PageLink = "/UpdateIntervalSelector.xaml",
                UpdateInterval = String.Format("{0}{1}", UserSettings.Instance.UpdateInterval, AppResources.Minute),
                Type = SettingItemTemplate.TEMPLATE_TEXT_CONTENT
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.About,
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.AboutKKBOX,
                Link = "http://www.kkbox.com/about/tc/",
                Type = SettingItemTemplate.TEMPLATE_TEXT
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.AboutKKBOXNews,
                Link = "http://www.kkbox.com/tw/tc/column/index.html",
                Type = SettingItemTemplate.TEMPLATE_TEXT
            });
            Settings.Add(new SettingListItem()
            {
                Title = AppResources.System,
                Type = SettingItemTemplate.TEMPLATE_SPACE
            });

            AssemblyName assembly = new AssemblyName(Assembly.GetExecutingAssembly().FullName);

            Settings.Add(new SettingListItem()
            {
                Title = AppResources.Version,
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

        private void LoadTopics(String result) 
        {
            Topics = new ObservableCollection<ChannelListItem>();

            XDocument ParsedTopic = XDocument.Parse(result);
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
            String xmlUri = "https://mail.kkbox.com.tw/~willzeng/SelectChannel.xml";

            XmlDownloader topicsXml = new XmlDownloader();
            topicsXml.XmlLoadCompleted+=OnXmlLoadCompleted;
            topicsXml.GetStringResponse(xmlUri);
        }

        public void DeleteDirectory(MySelectedArticleDirectory deleteItem)
        {
            ArticleDirectories.Remove(deleteItem);
        }

        private void OnXmlLoadCompleted(String result)
        {
            LoadTopics(result);
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