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
using Community.CsharpSqlite.SQLiteClient;
using System.Windows.Threading;

namespace KKBOX_News
{
    public partial class ArticleListPage : PhoneApplicationPage,INotifyPropertyChanged
    {
        public enum PageMode { NULL,READ_FROM_DIR, READ_FROM_XML};

        private PageMode currentPageMode;
       
        public ArticleListPage()
        {
            InitializeComponent();
            defaultSetting();
            LoadingText.DataContext = this;
            TopicPageTitle.DataContext = this;
            DataContext = Model;
            
        }


        private void defaultSetting()
        {
            isLinkClick = false;
            lastSelectedItemIndex = -1;
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMinutes(ArticleUpdateTimeInterval);
            Timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(Object sender, EventArgs e)
        {
            Debug.WriteLine("更新文章中...");
            Debug.WriteLine("ArticleUpdateTimeInterval");
            Debug.WriteLine(ArticleUpdateTimeInterval);
            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                LoadingText.Text = "更新文章中...";
                webClientXmlDownload(xmlString);
            }
        }

        #region LoadArticles
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

                //ContentString = ContentString;// String.Format("{0},{1}", ContentString, "...");

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
        #endregion

        private void webClientXmlDownload(String xmlValue)
        {

            IsNotPageLoaded = true;

            Uri uri = new Uri(xmlValue, UriKind.Absolute); ;
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadStringCompleted;
            webClient.DownloadStringAsync(uri);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            Debug.WriteLine("go back!");

            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                Debug.WriteLine("startUpdateArticle");

                if (App.ViewModel.IsAutoUpdate)
                {
                    startUpdateArticle();
                }
            }

            if (lastSelectedItemIndex != -1 && items != null)
            {
                items[lastSelectedItemIndex].IsExtended = true;
            }
            else if (parameters.ContainsKey("XML") && items == null)
            {

                Debug.WriteLine("XML");

                currentPageMode = PageMode.READ_FROM_XML;

                if (parameters.ContainsKey("XML"))
                {
                    xmlString = parameters["XML"];
                }
                if (parameters.ContainsKey("Title"))
                {
                    PageTitle = parameters["Title"];
                }

                LoadingText.Text = "載入中...";

                webClientXmlDownload(xmlString);

                if (App.ViewModel.IsAutoUpdate)
                {
                    startUpdateArticle();
                }
                
            }
            else if (parameters.ContainsKey("DirectoryIndex"))
            {
                currentPageMode = PageMode.READ_FROM_DIR;

                if (parameters.ContainsKey("DirectoryTitle"))
                {
                    PageTitle = parameters["DirectoryTitle"];
                }
                if (parameters.ContainsKey("DirectoryIndex"))
                {
                    IsNotPageLoaded = true;
                    LoadDirectoryArticlesFromDB(Int32.Parse(parameters["DirectoryIndex"]));
                    IsNotPageLoaded = false;
                }

            }

        }

        private void startUpdateArticle()
        {
            Timer.Start();
        }

        private void stopUpdateArticle()
        {
            Timer.Stop();
        }

        private void LoadDirectoryArticlesFromDB(Int32 DirectoryIndex)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    String querySrting = "";

                    querySrting = String.Format("SELECT * FROM directoryArticles WHERE directoryId={0}", DirectoryIndex);

                    cmd.CommandText = querySrting;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        items = new ObservableCollection<ArticleItem>();

                        while (reader.Read())
                        {
                            ArticleItem selectedArticleItem = new ArticleItem();
                            selectedArticleItem.Title = reader.GetString(2);
                            selectedArticleItem.Content = reader.GetString(3);
                            selectedArticleItem.IconImagePath = reader.GetString(4);
                            selectedArticleItem.Link = reader.GetString(5);

                            items.Add(selectedArticleItem);
                        }
                        Model.Items = items;
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            stopUpdateArticle();
        }

        private void loadXmlParserResult(DownloadStringCompletedEventArgs EventArgs)
        {
            String sXML = EventArgs.Result;
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
        }

        private void OnDownloadStringCompleted(Object sender, DownloadStringCompletedEventArgs EventArgs)
        {
            loadXmlParserResult(EventArgs);
            IsNotPageLoaded = false;
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
                    if (App.ViewModel.IsOpenExternalWeb)// External WebBrowser is in Settings[1]
                    {
                        WebBrowserTask webBrowserTask = new WebBrowserTask();
                        webBrowserTask.Uri = new Uri(sItem.Link, UriKind.Absolute);
                        webBrowserTask.Show();
                    }
                    else
                    {
                        NavigationService.Navigate(new Uri(String.Format("/WebPage.xaml?Link={0}", sItem.Link), UriKind.Relative));
                    }
                    isLinkClick = false;
                }

            }

            listBox.SelectedItem = null;
        }
       
        private void OnTextBlockManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            isLinkClick = true;
        }

        private void OnAddMySelectClick(Object sender, RoutedEventArgs e)
        {
           
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;
            

            String sDestination = String.Format("/AddMySelectPage.xaml?Title={0}&Content={1}&Link={2}&ImagePath={3}",
                articleItem.Title, articleItem.Content, articleItem.Link, articleItem.IconImagePath);

            this.NavigationService.Navigate(new Uri(sDestination, UriKind.Relative));
        }

        public static DispatcherTimer Timer;

        #region Property

        private String xmlString
        {
            get;
            set;
        }

        private Boolean isLinkClick
        {
            get;
            set;
        }

        private ObservableCollection<ArticleItem> items
        {
            get;
            set;
        }

        private Int32 lastSelectedItemIndex
        {
            get;
            set;
        }

        private static Int32 articleUpdateTimeInterval;
        public static Int32 ArticleUpdateTimeInterval
        {
            get
            {
                return articleUpdateTimeInterval;
            }
            set
            {
                if (App.ViewModel.Settings != null)
                {
                    if (value == 0)
                    {
                        App.ViewModel.Settings[3].UpdateInterval = "";
                    }
                    else
                    {
                        App.ViewModel.Settings[3].UpdateInterval = value.ToString() + "分"; // UpdateInterval
                    }
                }
                articleUpdateTimeInterval = value;
            }
        }

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

        private Boolean isNotPageLoaded;
        public Boolean IsNotPageLoaded
        {
            get
            {
                return isNotPageLoaded;
            }
            set
            {
                SetProperty(ref isNotPageLoaded, value, "IsNotPageLoaded");
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
        #endregion
    }
}