using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KKBOX_News.Resources;
using System.Net;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace KKBOX_News.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private IEnumerable<XElement> TopicsInXml;

        public MainViewModel()
        {
            
            //SelectTopicParser();
            loadDirectoris();
        }

        private void loadDirectoris()
        {
            ArticleDirectories = new ObservableCollection<MySelectedArticleDirectory>();
            ArticleDirectories.Add(new MySelectedArticleDirectory()
            {
                RightTitle = "RightTitle",
                LeftTitle = "LeftTitle",
                RightImagePath = "Images/green.png",
                LeftImagePath = "Images/green.png",
                Type = "Two"
            });
            ArticleDirectories.Add(new MySelectedArticleDirectory()
            {
                RightTitle = "RightTitle",
                LeftTitle = "LeftTitle",
                RightImagePath = "Images/green.png",
                LeftImagePath = "Images/green.png",
                Type = "Two"
            });
            ArticleDirectories.Add(new MySelectedArticleDirectory()
            {
                LeftTitle = "LeftTitle",
                LeftImagePath = "Images/green.png",
                Type = "One"
            });
        }

        public void SelectTopicParser()
        {
            Uri uri = new Uri("https://mail.kkbox.com.tw/~willzeng/SelectChannel.xml");

            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadTopicXmlCompleted;
            webClient.DownloadStringAsync(uri);
        }

        void OnDownloadTopicXmlCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            Topics = new ObservableCollection<ChannelListItem>();

            XDocument ParsedTopic = XDocument.Parse(args.Result);
            TopicsInXml = ParsedTopic.Element("kkbox_news").Elements("channel");
            
            foreach (XElement channel in TopicsInXml)
            {
                Topics.Add(new ChannelListItem() { Title = channel.Element("title").Value, IconImagePath = channel.Element("icon").Value, Url = channel.Element("url").Value });
            }

            IsTopicsXmlLoaded = true;
        }

        public Boolean IsTopicsXmlLoaded
        {
            get;
            private set;
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
        

    }
}