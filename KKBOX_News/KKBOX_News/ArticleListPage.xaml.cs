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
    public class SimpleArticleItem
    {
        public String Title
        {
            get;
            set;
        }

        public Int32 DeleteId
        {
            get;
            set;
        }
    }

    public partial class ArticleListPage : PhoneApplicationPage,INotifyPropertyChanged
    {
        public enum PageMode { NULL,READ_FROM_DIR, READ_FROM_XML, SEARCH_ARTICLES};
        public enum ConfirmButtonMode { NULL, ADD_ARTICLE, DELETE_ARTICLE };
        private PageMode currentPageMode;
        private ConfirmButtonMode currentConfirmButtonMode;

        public ArticleListPage()
        {
            InitializeComponent();
            defaultSetting();
            LoadingText.DataContext = this;
            TopicPageTitle.DataContext = this;
            ButtonSet.DataContext = this;
            selectAllGrid.DataContext = this;
            searchTexBoxGrid.DataContext = this;
            DataContext = ArticleModel;
        }

        private void defaultSetting()
        {
            isLinkClick = false;
            lastSelectedItemIndex = -1;
            MultipleManipulation = Visibility.Collapsed;
            SearchManipulation = Visibility.Collapsed;
            simpleArticles = new List<SimpleArticleItem>();
            ArticleUpdateTimeInterval = UserSettings.UpdateInterval;

            appbarMultipleManipulation = this.ApplicationBar as ApplicationBar;
            menuMultipleDelete = this.ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            menuMultipleDelete.IsEnabled = false;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(ArticleUpdateTimeInterval);
            Timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(Object sender, EventArgs e)
        {
            Debug.WriteLine("更新文章中...");
            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                LoadingText.Text = "更新文章中...";
                webClientXmlDownload(xmlString);
            }
        }

        #region LoadArticlesFromXml
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

            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                if (App.ViewModel.IsAutoUpdate)
                {
                    startUpdateArticle();
                }
            }

            if (lastSelectedItemIndex != -1 && ArticleModel.KKBOXArticles != null && lastSelectedItemIndex < ArticleModel.KKBOXArticles.Count)
            {
                ArticleModel.KKBOXArticles[lastSelectedItemIndex].IsExtended = true;
            }
            else if (parameters.ContainsKey("XML") && selectedArticles == null)
            {
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
                directoryIndex = Int32.Parse(parameters["DirectoryIndex"]);

                determineAppBarVisibility();

                currentPageMode = PageMode.READ_FROM_DIR;

                menuMultipleDelete.IsEnabled = true;

                if (parameters.ContainsKey("DirectoryTitle"))
                {
                    PageTitle = parameters["DirectoryTitle"];
                }
                if (parameters.ContainsKey("DirectoryIndex"))
                {
                    IsNotPageLoaded = true;
                    LoadDirectoryArticlesFromDB(directoryIndex);
                    IsNotPageLoaded = false;
                }
            }
            else if (parameters.ContainsKey("SearchArticle"))
            {
                PageTitle = "搜尋文章";
                SearchManipulation = Visibility.Visible;
                appbarMultipleManipulation.IsVisible = false;
            }

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            stopUpdateArticle();
        }

        private void determineAppBarVisibility()
        {
            if (isDirectoryHasContent(directoryIndex))
            {
                appbarMultipleManipulation.IsVisible = true;
            }
            else
            {
                appbarMultipleManipulation.IsVisible = false;
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

                    querySrting = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, DirectoryIndex);

                    cmd.CommandText = querySrting;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        selectedArticles = new ObservableCollection<ArticleItem>();

                        while (reader.Read())
                        {
                            ArticleItem selectedArticleItem = new ArticleItem();
                            selectedArticleItem.Title = reader.GetString(2);
                            selectedArticleItem.Content = reader.GetString(3);
                            selectedArticleItem.IconImagePath = reader.GetString(4);
                            selectedArticleItem.Link = reader.GetString(5);
                            selectedArticleItem.DeleteMenuVisiblity = Visibility.Visible;

                            selectedArticles.Add(selectedArticleItem);
                        }
                        ArticleModel.KKBOXArticles = selectedArticles;
                    }
                }
            }
        }

        private void loadXmlParserResult(DownloadStringCompletedEventArgs EventArgs)
        {
            String sXML = EventArgs.Result;
            XDocument root = XDocument.Parse(sXML);
            XElement channelRoot = root.Element("rss").Element("channel");
            IEnumerable<XElement> elements = channelRoot.Elements("item");
            selectedArticles = new ObservableCollection<ArticleItem>();
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
                selectedArticles.Add(newItem);
            }
            ArticleModel.KKBOXArticles = selectedArticles;
        }

        private void loadArticleIntoPasser()
        {
            ArticleNavigationPasser.Articles.Clear();

            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                if (ArticleModel.KKBOXArticles[i].IsSelected)
                {
                    ArticleNavigationPasser.Articles.Add(ArticleModel.KKBOXArticles[i]);
                }
            }
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
                    ArticleModel.KKBOXArticles[lastSelectedItemIndex].IsExtended = false;
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

        private void OnTextBlockManipulationStarted(Object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            isLinkClick = true;
        }

        private void OnMenuItemAddMySelectClick(Object sender, RoutedEventArgs e)
        {
           
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;

            ArticleNavigationPasser.Articles.Clear();
            ArticleNavigationPasser.Articles.Add(articleItem);

            this.NavigationService.Navigate(new Uri("/AddMySelectPage.xaml", UriKind.Relative));
        }

        private void OnMenuItemDeleteClick(Object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;

            ArticleNavigationPasser.Articles.Clear();
            ArticleNavigationPasser.Articles.Add(articleItem);
            deleteDirectoryArticlesFormDB();
        }

        private void setArticleCheckBoxVisibility(Visibility visibility)
        {
            if (ArticleModel.KKBOXArticles != null)
            {
                for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
                {
                    ArticleModel.KKBOXArticles[i].CheckBoxVisiblity = visibility;
                }
            }
        }

        private Boolean IsAnyArticleSelected()
        {
            if (ArticleNavigationPasser.Articles.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private Boolean isDirectoryHasContent(Int32 DirIndex)
        {

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    String querySrting = "";

                    querySrting = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, DirIndex);

                    cmd.CommandText = querySrting;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        private void concelAllSelect()
        {
            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                ArticleModel.KKBOXArticles[i].IsSelected = false;
            }
        }

        private void checkAllSelect()
        {
            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                ArticleModel.KKBOXArticles[i].IsSelected = true;
            }
        }

        private void resetAllSelect()
        {
            concelAllSelect();
            checkBoxSelectAll.IsChecked = false;
        }

        private void OnComfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            MultipleManipulation = Visibility.Collapsed;
            setArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            loadArticleIntoPasser();
            if (currentConfirmButtonMode == ConfirmButtonMode.ADD_ARTICLE && IsAnyArticleSelected())
            {
                this.NavigationService.Navigate(new Uri("/AddMySelectPage.xaml", UriKind.Relative));
            }
            else if (currentConfirmButtonMode == ConfirmButtonMode.DELETE_ARTICLE)
            {
                deleteDirectoryArticlesFormDB();
            }
            resetAllSelect();

        }
        
        private void OnConcelButtonClick(Object sender, RoutedEventArgs e)
        {
            MultipleManipulation = Visibility.Collapsed;
            setArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            resetAllSelect();
        }

        private void OnAddMyMultiSelectMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            setArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.ADD_ARTICLE;
        }

        private void OnAddMyMultiDeleteMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            setArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.DELETE_ARTICLE;
        }

        private void loadSimpleArticlesFromDB()
        {
            simpleArticles.Clear();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    //String querySrting = "";
                    //querySrting = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, directoryIndex);
                    cmd.CommandText = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, directoryIndex);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            simpleArticles.Add(new SimpleArticleItem()
                            {
                                DeleteId = reader.GetInt32(0),
                                Title = reader.GetString(2)
                            });
                        }
                    }
                }
            }
        }

        private void deleteDirectoryArticlesFormDB()
        {
            loadSimpleArticlesFromDB();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    for (int i = 0; i < ArticleNavigationPasser.Articles.Count; i++)
                    {
                        for (int j = 0; j < simpleArticles.Count; j++)
                        {
                            if (ArticleNavigationPasser.Articles[i].Title == simpleArticles[j].Title)
                            {
                                cmd.CommandText = String.Format("DELETE FROM directoryArticlesUser{0} WHERE id={1}", LoginPage.UserId, simpleArticles[j].DeleteId);
                                cmd.ExecuteNonQuery();
                                ArticleModel.KKBOXArticles.Remove(ArticleNavigationPasser.Articles[i]);
                                break;
                            }
                        }
                    }
                }
            }
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

        private ObservableCollection<ArticleItem> selectedArticles
        {
            get;
            set;
        }

        private Int32 lastSelectedItemIndex
        {
            get;
            set;
        }

        private Int32 directoryIndex
        {
            get;
            set;
        }

        private List<SimpleArticleItem> simpleArticles
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
                        App.ViewModel.Settings[5].UpdateInterval = "";
                    }
                    else
                    {
                        App.ViewModel.Settings[5].UpdateInterval = value.ToString() + "分"; // UpdateInterval
                    }
                }
                articleUpdateTimeInterval = value;
            }
        }

        private ArticleListPageModel articleModel;
        public ArticleListPageModel ArticleModel
        {
            get
            {
                if (articleModel == null)
                {
                    articleModel = new ArticleListPageModel();
                }
                return articleModel;
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

        private Visibility searchManipulation;
        public Visibility SearchManipulation
        {
            get
            {
                return searchManipulation;
            }
            set
            {
                SetProperty(ref searchManipulation, value, "SearchManipulation");
            }
        }

        private Visibility multipleManipulation;
        public Visibility MultipleManipulation
        {
            get
            {
                return multipleManipulation;
            }
            set
            {
                SetProperty(ref multipleManipulation, value, "MultipleManipulation");
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

        private void OnSelectAllCheckBoxClick(Object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if ((Boolean)checkBox.IsChecked)
            {
                checkAllSelect();
            }
            else
            {
                concelAllSelect();
            }
        }

        private void appbarVisibilityForSearchResult()
        {
            if (ArticleModel.KKBOXArticles.Count == 0)
            {
                appbarMultipleManipulation.IsVisible = false;
            }
            else
            {
                appbarMultipleManipulation.IsVisible = true;
            }
        }

        private void OnSearchArticlesButtonClick(Object sender, RoutedEventArgs e)
        {
            SearchLocalArticles locaArticles = new SearchLocalArticles();
            String keyword = searchKeywordTextBox.Text;
            ArticleModel.KKBOXArticles = locaArticles.SearchArticleContainKeyWord(keyword);
            appbarVisibilityForSearchResult();
        }
    }
}