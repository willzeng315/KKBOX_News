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
        public enum PageMode { NULL,READ_FROM_DIR, READ_FROM_XML, SEARCH_ARTICLES, EXTERNAL_ARTICLES};
        public enum ConfirmButtonMode { NULL, ADD_ARTICLE, DELETE_ARTICLE };
        private PageMode currentPageMode;
        private ConfirmButtonMode currentConfirmButtonMode;

        public ArticleListPage()
        {
            InitializeComponent();
            defaultSetting();
            AppbarDefaultSettings();
            VisibilityDefaultSettings();
            SetDataContexts();
            DataContext = ArticleModel;
        }

        private void SetDataContexts()
        {
            LoadingText.DataContext = this;
            TopicPageTitle.DataContext = this;
            ButtonSet.DataContext = this;
            selectAllGrid.DataContext = this;
            searchTexBoxGrid.DataContext = this;
            externalArticleTexBoxGrid.DataContext = this;
        }

        private void AppbarDefaultSettings()
        {
            appbarMultipleManipulation = this.ApplicationBar as ApplicationBar;
            menuMultipleAdd = this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            menuMultipleDelete = this.ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
            menuMultipleDelete.IsEnabled = false;
        }

        private void VisibilityDefaultSettings()
        {
            MultipleManipulation = Visibility.Collapsed;
            SearchManipulation = Visibility.Collapsed;
            externalArticleManipulation = Visibility.Collapsed;
        }

        private void defaultSetting()
        {
            lastSelectedItemIndex = -1;

            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromSeconds(UserSettings.Instance.UpdateInterval);
            Timer.Tick += OnTimerTick;
        }

        private void OnTimerTick(Object sender, EventArgs e)
        {
            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                LoadingText.Text = "更新文章中...";
                webClientXmlDownload(xmlString);
            }
        }

        private void webClientXmlDownload(String xmlValue)
        {

            IsNotPageLoaded = true;

            Uri uri = new Uri(xmlValue, UriKind.Absolute); ;
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadXmlCompleted;
            webClient.DownloadStringAsync(uri);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;
           
            if (currentPageMode == PageMode.READ_FROM_XML)
            {
                if (UserSettings.Instance.IsOpenAutoUpdate)
                {
                    StartUpdateArticle();
                }
            }

            Debug.WriteLine(ArticleModel.KKBOXArticles.Count);
            if (ArticleModel.KKBOXArticles.Count == 0)
            {
                if (parameters.ContainsKey("XML"))
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

                    if (UserSettings.Instance.IsOpenAutoUpdate)
                    {
                        StartUpdateArticle();
                    }
                }
                else if (IsExternalArticlePage())
                {
                    ExternalArticleManipulation = Visibility.Visible;
                    menuMultipleDelete.IsEnabled = true;
                    menuMultipleAdd.IsEnabled = false;

                    directoryIndex = 1;

                    currentPageMode = PageMode.EXTERNAL_ARTICLES;

                    if (parameters.ContainsKey("DirectoryTitle"))
                    {
                        PageTitle = parameters["DirectoryTitle"];
                    }

                    LoadDirectoryArticlesFromTable();

                    SetArticleImageCollapsed();
                }
                else if (parameters.ContainsKey("DirectoryIndex"))
                {
                    Debug.WriteLine("Directory");

                    directoryIndex = Int32.Parse(parameters["DirectoryIndex"]);

                    DetermineAppBarVisibility();

                    currentPageMode = PageMode.READ_FROM_DIR;

                    menuMultipleDelete.IsEnabled = true;

                    if (parameters.ContainsKey("DirectoryTitle"))
                    {
                        PageTitle = parameters["DirectoryTitle"];
                    }

                    LoadDirectoryArticlesFromTable();

                }
                else if (parameters.ContainsKey("SearchArticle"))
                {
                    PageTitle = "搜尋文章";
                    SearchManipulation = Visibility.Visible;
                    appbarMultipleManipulation.IsVisible = false;
                }
            }
            else if (IsExternalArticlePage())
            {
                ExternalArticleManipulation = Visibility.Visible;
                menuMultipleDelete.IsEnabled = true;
                menuMultipleAdd.IsEnabled = false;

                directoryIndex = 1;

                currentPageMode = PageMode.EXTERNAL_ARTICLES;

                if (parameters.ContainsKey("DirectoryTitle"))
                {
                    PageTitle = parameters["DirectoryTitle"];
                }

                LoadDirectoryArticlesFromTable();

                SetArticleImageCollapsed();
            }

        }

        private Boolean CheckLastSelectedItemIndex()
        {
            return (lastSelectedItemIndex != -1 && ArticleModel.KKBOXArticles.Count > 0 && lastSelectedItemIndex < ArticleModel.KKBOXArticles.Count) ? true : false;
        }

        private void SetArticleImageCollapsed()
        {
            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                ArticleModel.KKBOXArticles[i].ImageVisiblity = Visibility.Collapsed;
            }
        }

        private Boolean IsExternalArticlePage()
        {
            Boolean externalArticlePage = false;
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("DirectoryIndex") && Int32.Parse(parameters["DirectoryIndex"]) == 1)
            {
                externalArticlePage = true;
            }
            return externalArticlePage;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            StopUpdateArticle();
            ExternalArticleManipulation = Visibility.Collapsed;
        }

        private void DetermineAppBarVisibility()
        {
            if (DBManager.Instance.IsDirectoryHaveArticles(directoryIndex))
            {
                appbarMultipleManipulation.IsVisible = true;
            }
            else
            {
                appbarMultipleManipulation.IsVisible = false;
            }
        }

        private void StartUpdateArticle()
        {
            Timer.Start();
        }

        private void StopUpdateArticle()
        {
            Timer.Stop();
        }

        private void LoadDirectoryArticlesFromTable()
        {
            ArticleModel.KKBOXArticles = DBManager.Instance.LoadDirectoryArticles(directoryIndex);
        }

        private void LoadArticleIntoPasser()
        {
            ArticleNavigationPasser.Instance.Articles.Clear();

            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                if (ArticleModel.KKBOXArticles[i].IsSelected)
                {
                    ArticleNavigationPasser.Instance.Articles.Add(ArticleModel.KKBOXArticles[i]);
                }
            }
        }

        private void OnDownloadXmlCompleted(Object sender, DownloadStringCompletedEventArgs EventArgs)
        {
            RssXmlParser rssArticleParser = new RssXmlParser();
            ArticleModel.KKBOXArticles = rssArticleParser.GetXmlParserResult(EventArgs);
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
                    if (lastSelectedItemIndex == listBox.SelectedIndex)
                    {
                        lastSelectedItemIndex = -1;
                    }
                }
                else
                {
                   sItem.IsExtended = true;
                   lastSelectedItemIndex = listBox.SelectedIndex;
                }
            }

            listBox.SelectedItem = null;
        }

        private void OnMenuItemAddMySelectClick(Object sender, RoutedEventArgs e)
        {
           
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;

            ArticleNavigationPasser.Instance.Articles.Clear();
            ArticleNavigationPasser.Instance.Articles.Add(articleItem);

            this.NavigationService.Navigate(new Uri("/AddMySelectPage.xaml", UriKind.Relative));
        }

        private void OnMenuItemDeleteClick(Object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ArticleItem articleItem = (ArticleItem)menuItem.DataContext;

            ArticleNavigationPasser.Instance.Articles.Clear();
            ArticleNavigationPasser.Instance.Articles.Add(articleItem);
            DeleteDirectoryArticlesFormDB();
        }

        private void SetArticleCheckBoxVisibility(Visibility visibility)
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
            return ArticleNavigationPasser.Instance.Articles.Count == 0 ? false : true;
        }

        private void ConcelAllSelect()
        {
            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                ArticleModel.KKBOXArticles[i].IsSelected = false;
            }
        }

        private void CheckAllSelect()
        {
            for (int i = 0; i < ArticleModel.KKBOXArticles.Count; i++)
            {
                ArticleModel.KKBOXArticles[i].IsSelected = true;
            }
        }

        private void ResetAllSelect()
        {
            ConcelAllSelect();
            checkBoxSelectAll.IsChecked = false;
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            MultipleManipulation = Visibility.Collapsed;
            SetArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            LoadArticleIntoPasser();
            if (currentConfirmButtonMode == ConfirmButtonMode.ADD_ARTICLE && IsAnyArticleSelected())
            {
                this.NavigationService.Navigate(new Uri("/AddMySelectPage.xaml", UriKind.Relative));
            }
            else if (currentConfirmButtonMode == ConfirmButtonMode.DELETE_ARTICLE)
            {
                DeleteDirectoryArticlesFormDB();
            }
            ResetAllSelect();

        }
        
        private void OnConcelButtonClick(Object sender, RoutedEventArgs e)
        {
            MultipleManipulation = Visibility.Collapsed;
            SetArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            ResetAllSelect();
        }

        private void OnAddMyMultiSelectMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            SetArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.ADD_ARTICLE;
        }

        private void OnAddMyMultiDeleteMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            SetArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.DELETE_ARTICLE;
        }

        private void DeleteDirectoryArticlesFormDB()
        {
            DBManager.Instance.DeleteArticleFromTable(directoryIndex);

            for (int i = 0; i < ArticleNavigationPasser.Instance.Articles.Count; i++)
            {
                ArticleModel.KKBOXArticles.Remove(ArticleNavigationPasser.Instance.Articles[i]);
            }

            ArticleNavigationPasser.Instance.Articles.Clear();
            AppbarVisibilityForHasArticles();
        }

        public static DispatcherTimer Timer;

        #region Property

        private String xmlString
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

        private Visibility externalArticleManipulation;
        public Visibility ExternalArticleManipulation
        {
            get
            {
                return externalArticleManipulation;
            }
            set
            {
                SetProperty(ref externalArticleManipulation, value, "ExternalArticleManipulation");
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
            if (Object.Equals(storage, value))
            {
                return false;
            }

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
                CheckAllSelect();
            }
            else
            {
                ConcelAllSelect();
            }
        }

        private void AppbarVisibilityForHasArticles()
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
            AppbarVisibilityForHasArticles();
        }

        private void OnAddExternalArticleButtonClick(Object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ExternalArticlesInfoPage.xaml",UriKind.Relative));
        }

        private void OnPageLinkButtonClick(Object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ArticleItem articleItem = (ArticleItem)button.DataContext;

            if (UserSettings.Instance.IsOpenExternalWeb)// External WebBrowser is in Settings[1]
            {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri(articleItem.Link, UriKind.Absolute);
                webBrowserTask.Show();
            }
            else
            {
                NavigationService.Navigate(new Uri(String.Format("/WebPage.xaml?Link={0}", articleItem.Link), UriKind.Relative));
            }
        }
    }
}