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
        private ObservableCollection<ChannelListItem> topics = null;
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
        

    }
}