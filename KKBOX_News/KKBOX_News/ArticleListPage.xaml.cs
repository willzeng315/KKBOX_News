using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KKBOX_News
{
    public partial class ArticleListPage : PhoneApplicationPage,INotifyPropertyChanged
    {

        private Boolean isLinkClick = false;
        private ObservableCollection<ArticleItem> items;
        private Int32 lastSelectedItemIndex = -1;
        private ArticleListPageModel model;
        public ArticleListPageModel Model
        {
            get
            {
                if (model == null)
                {
                    model = new ArticleListPageModel();
                }
                return model;
            }
        }

        public ArticleListPage()
        {
            InitializeComponent();
            IsNotRssPageLoaded = true;
            LoadingText.DataContext = this;
            TopicPageTitle.DataContext = this;
            DataContext = Model;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (items != null)
            {
                items[lastSelectedItemIndex].IsExtended = true;
                Debug.WriteLine(lastSelectedItemIndex);
            }
            else
            {
                IDictionary<String, String> parameters = this.NavigationContext.QueryString;
                String xmlValue = "";
                if (parameters.ContainsKey("XML"))
                {
                    xmlValue = parameters["XML"];
                }
                if(parameters.ContainsKey("Title"))
                {
                    PageTitle = parameters["Title"];
                }

                Uri uri = new Uri(xmlValue, UriKind.Absolute); ;
                WebClient webClient = new WebClient();
                webClient.DownloadStringCompleted += OnDownloadStringCompleted;
                webClient.DownloadStringAsync(uri);
            }
       }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            
        }


        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            String sXML = e.Result;
            XDocument root = XDocument.Parse(sXML);
            XElement channelRoot = root.Element("rss").Element("channel");
            IEnumerable<XElement> elements = channelRoot.Elements("item");
            items = new ObservableCollection<ArticleItem>();
            foreach (XElement eleItem in elements)
            {
                String sTitle = eleItem.Element("title").Value;
                String sDescription = eleItem.Element("description").Value;
                String sIconPath = ImageRetriever(sDescription);
                String sContent = ContentRetriever(sDescription);
                String sLink = LinkRetriever(sDescription);
                ArticleItem newItem = new ArticleItem();
                newItem.Title = sTitle;
                newItem.Content = sContent;
                newItem.IconImagePath = sIconPath;
                newItem.Link = sLink;
                newItem.IsExtended = false;
                items.Add(newItem);
            }
            Model.Items = items;

            IsNotRssPageLoaded = false;
        }
        private String ImageRetriever(String sDescription)
        {
            String ImageSource = "";
            const Int32 IndexShift = 9; //Length of "img src='"

            if (sDescription != null)
            {
                Int32 startIndex = sDescription.ToString().IndexOf("img src='");
                Int32 EndIndex = sDescription.ToString().IndexOf("jpg");
                Int32 srcLen = EndIndex - startIndex;

                if (startIndex > EndIndex || srcLen < 0)
                {
                    return null;
                }

                ImageSource = sDescription.ToString().Substring(startIndex + IndexShift, srcLen);
                return ImageSource;
            }
            return ImageSource;
        }

        private String ContentRetriever(String sDescription)
        {
            String ContentString = "";
            
            if (sDescription != null)
            {
                ContentString = Regex.Replace(sDescription.ToString(), "<[^>]+>", String.Empty);

                ContentString = ContentString.Replace("\r", "").Replace("\n", "");

                ContentString = HttpUtility.HtmlDecode(ContentString);

                Int32 ContentLenth = ContentString.IndexOf("更多文章");

                if (ContentLenth != -1)
                {
                    ContentString = ContentString.Substring(0, ContentLenth);
                }

                ContentString = String.Format("{0},{1}", ContentString, "...");
                
            }
            return ContentString;
        }

        private String LinkRetriever(String sDescription)
        {
            String LinkSource = "";
            const Int32 IndexShift = 4; // Length of "http"

            if (sDescription != null)
            {
                Int32 startIndex = sDescription.ToString().IndexOf("http");
                Int32 EndIndex = sDescription.ToString().IndexOf("html");
                Int32 srcLen = EndIndex - startIndex;

                if (startIndex < EndIndex && srcLen > 0)
                {
                    LinkSource = sDescription.ToString().Substring(startIndex, srcLen + IndexShift);
                }
            }
            return LinkSource;

        }
        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {

                ArticleItem sItem = (ArticleItem)listBox.SelectedItem;

                if (lastSelectedItemIndex != -1 && lastSelectedItemIndex != listBox.SelectedIndex)
                {
                    items[lastSelectedItemIndex].IsExtended = false;
                    lastSelectedItemIndex = listBox.SelectedIndex;
                }

                if (sItem.IsExtended == true)
                {
                    sItem.IsExtended = false;
                }
                else
                {
                   sItem.IsExtended = true;
                   lastSelectedItemIndex = listBox.SelectedIndex;
                }

                if (isLinkClick)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(sItem.Link, UriKind.Absolute);
                    webBrowserTask.Show();
                    isLinkClick = false;
                }

            }

            listBox.SelectedItem = null;
        }
       
        private void TextBlock_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            isLinkClick = true;
            //Debug.WriteLine("Manipulation");  
        }

        private Boolean isNotRssPageLoaded;
        public Boolean IsNotRssPageLoaded
        {
            get
            {
                return isNotRssPageLoaded;
            }
            set
            {
                SetProperty(ref isNotRssPageLoaded, value, "IsNotRssPageLoaded");
            }
        }
        private String pageTitle;
        public String PageTitle
        {
            get
            {
                return pageTitle;
            }
            set
            {
                SetProperty(ref pageTitle, value, "PageTitle");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected Boolean SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void OnAddMySelectClick(Object sender, RoutedEventArgs e)
        {
           
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;
            
            Debug.WriteLine(articleItem.Title);

            String sDestination = String.Format("/AddMySelectPage.xaml?Title={0}&Content={1}&Link={2}&ImagePath={3}",
                articleItem.Title, articleItem.Content, articleItem.Link, articleItem.IconImagePath);

            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
        }
    }
}