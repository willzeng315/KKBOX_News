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
using System.Windows.Threading;
using KKBOX_News.Resources;
using KKBOX_News.AppService;

namespace KKBOX_News
{
    public enum PageMode { NULL, READ_FROM_DIR, READ_FROM_XML, SEARCH_ARTICLES, EXTERNAL_ARTICLES, BROWSE_RECORDS };

    public partial class ArticleListPage : PhoneApplicationPage, INotifyPropertyChanged
    {

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
            menuMultipleAdd.Text = AppResources.JoinMySelectAppbarMenu;
            menuMultipleDelete.Text = AppResources.DeleteAppbarMenu;
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
                LoadingText.Text = AppResources.UpdateText;
                WebClientXmlDownload(xmlString);
            }
        }
        private void WebClientXmlDownload(String xmlValue)
        {
            IsNotPageLoaded = true;

            XmlDownloader selectTopic = new XmlDownloader();
            selectTopic.XmlLoadCompleted += OnXmlLoadCompleted;
            selectTopic.GetStringResponse(xmlValue);
        }

        public void OnXmlLoadCompleted(String result)
        {
            ArticleModel.LoadRssArticles(result);
            IsNotPageLoaded = false;
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

            if (IsRssArticlePage())
            {
                currentPageMode = PageMode.READ_FROM_XML;

                ArticleModel.CurrentPageMode = PageMode.READ_FROM_XML;

                if (UserSettings.Instance.IsOpenAutoUpdate)
                {
                    StartUpdateArticle();
                }
            }
            else if (IsExternalArticlePage())
            {
                currentPageMode = PageMode.EXTERNAL_ARTICLES;
                ArticleModel.CurrentPageMode = PageMode.EXTERNAL_ARTICLES;
            }
            else if (IsDirectoryArticlePage())
            {
                currentPageMode = PageMode.READ_FROM_DIR;
                ArticleModel.CurrentPageMode = PageMode.READ_FROM_DIR;
            }
            else if (IsSearchArticlePage())
            {
                currentPageMode = PageMode.SEARCH_ARTICLES;
                ArticleModel.CurrentPageMode = PageMode.SEARCH_ARTICLES;

            }
            else if (IsBrowseRecordPage())
            {
                currentPageMode = PageMode.BROWSE_RECORDS;
                ArticleModel.CurrentPageMode = PageMode.BROWSE_RECORDS;
            }

            ArticleModel.LoadArticle();
        }

        private Boolean IsSearchArticlePage()
        {
            Boolean searchArticlePage = false;

            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("SearchArticleMode"))
            {
                PageTitle = AppResources.SearchArticleTitle;
                SearchManipulation = Visibility.Visible;
                appbarMultipleManipulation.IsVisible = false;
                searchArticlePage = true;

            }
            return searchArticlePage;
        }

        private Boolean IsBrowseRecordPage()
        {
            Boolean browseRecordPage = false;

            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            directoryIndex = -1;
            ArticleModel.DirectoryIndex = -1;

            DetermineAppBarVisibility("articleBrowseRecordUser");

            if (parameters.ContainsKey("BrowseRecord"))
            {
                browseRecordPage = true;

                if (parameters.ContainsKey("Title"))
                {
                    PageTitle = parameters["Title"];
                }

            }

            return browseRecordPage;
        }

        private Boolean IsExternalArticlePage()
        {
            Boolean externalArticlePage = false;

            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("DirectoryIndex") && Int32.Parse(parameters["DirectoryIndex"]) == 1)
            {
                directoryIndex = 1;
                ArticleModel.DirectoryIndex = 1;
                ExternalArticleManipulation = Visibility.Visible;
                menuMultipleDelete.IsEnabled = true;
                menuMultipleAdd.IsEnabled = false;

                DetermineAppBarVisibility("directoryArticlesUser");

                externalArticlePage = true;

                if (parameters.ContainsKey("DirectoryTitle"))
                {
                    PageTitle = parameters["DirectoryTitle"];
                }
            }
            return externalArticlePage;
        }

        private Boolean IsDirectoryArticlePage()
        {
            Boolean directoryArticlePage = false;
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("DirectoryIndex") && Int32.Parse(parameters["DirectoryIndex"]) > 1)
            {
                directoryIndex = Int32.Parse(parameters["DirectoryIndex"]);

                ArticleModel.DirectoryIndex = Int32.Parse(parameters["DirectoryIndex"]);

                DetermineAppBarVisibility("directoryArticlesUser");

                menuMultipleDelete.IsEnabled = true;

                directoryArticlePage = true;

                if (parameters.ContainsKey("DirectoryTitle"))
                {
                    PageTitle = parameters["DirectoryTitle"];
                }
            }
            return directoryArticlePage;
        }

        private Boolean IsRssArticlePage()
        {
            Boolean rssArticlePage = false;
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("Xml") && ArticleModel.KKBOXArticles.Count == 0)
            {
                xmlString = parameters["Xml"];

                LoadingText.Text = AppResources.LoadingText;

                WebClientXmlDownload(xmlString);

                rssArticlePage = true;

                if (parameters.ContainsKey("Title"))
                {
                    PageTitle = parameters["Title"];
                }
            }
            return rssArticlePage;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            StopUpdateArticle();
            ExternalArticleManipulation = Visibility.Collapsed;
        }

        private void DetermineAppBarVisibility(String tableName)
        {
            appbarMultipleManipulation.IsVisible = (DBManager.Instance.IsTableHaveArticles(tableName, directoryIndex)) ? true : false;
        }

        private void StartUpdateArticle()
        {
            Timer.Start();
        }

        private void StopUpdateArticle()
        {
            Timer.Stop();
        }

        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                ArticleItem sItem = (ArticleItem)listBox.SelectedItem;

                if (lastSelectedItemIndex != -1 && lastSelectedItemIndex != listBox.SelectedIndex)
                {
                    ArticleModel.SetLastItemShrink(lastSelectedItemIndex);
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

        private Boolean IsAnyArticleSelected()
        {
            return ArticleNavigationPasser.Instance.Articles.Count == 0 ? false : true;
        }

        private void ResetAllSelect()
        {
            ArticleModel.ConcelAllSelect();
            checkBoxSelectAll.IsChecked = false;
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            MultipleManipulation = Visibility.Collapsed;
            ArticleModel.SetArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            ArticleModel.LoadArticleIntoPasser();
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
            ArticleModel.SetArticleCheckBoxVisibility(Visibility.Collapsed);
            appbarMultipleManipulation.IsVisible = true;
            ResetAllSelect();
        }

        private void OnAddMyMultiSelectMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            ArticleModel.SetArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.ADD_ARTICLE;
        }

        private void OnAddMyMultiDeleteMenuClick(Object sender, EventArgs e)
        {
            MultipleManipulation = Visibility.Visible;
            ArticleModel.SetArticleCheckBoxVisibility(Visibility.Visible);
            appbarMultipleManipulation.IsVisible = false;
            currentConfirmButtonMode = ConfirmButtonMode.DELETE_ARTICLE;
        }

        private void DeleteDirectoryArticlesFormDB()
        {
            ArticleModel.DeleteDirectoryArticles();
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
                ArticleModel.CheckAllSelect();
            }
            else
            {
                ArticleModel.ConcelAllSelect();
            }
        }

        private void AppbarVisibilityForHasArticles()
        {
            if (ArticleModel.IsArtilcesZero())
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
            String keyword = searchKeywordTextBox.Text;
            ArticleModel.SearchSelectedArticle(keyword);
            AppbarVisibilityForHasArticles();
        }

        private void OnAddExternalArticleButtonClick(Object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/ExternalArticlesInfoPage.xaml", UriKind.Relative));
        }

        private void OnPageLinkButtonClick(Object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ArticleItem articleItem = (ArticleItem)button.DataContext;
            if (directoryIndex == 0)
            {
                DBManager.Instance.InsertRecordToTable(articleItem);
            }

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