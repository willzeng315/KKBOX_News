using KKBOX_News.DBService;
using KKBOX_News.NetworkService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public class ArticleListPageModel : BindableBase
    {
        public PageMode CurrentPageMode;

        public ArticleListPageModel()
        {
            KKBOXArticles = new ObservableCollection<ArticleItem>();
            DirectoryIndex = -1;
            CurrentPageMode = PageMode.NULL;
        }

        public void LoadArticle()
        {
            if (CurrentPageMode == PageMode.NULL)
            {
                return;
            }
            else
            {
                switch (CurrentPageMode)
                {
                    case PageMode.READ_FROM_XML:

                        break;
                    case PageMode.READ_FROM_DIR:
                        LoadDirectoryArticlesFromTable();
  
                        break;
                    case PageMode.EXTERNAL_ARTICLES:
                        LoadDirectoryArticlesFromTable();
                        SetArticleImageCollapsed();
                        break;
                    case PageMode.SEARCH_ARTICLES:

                        break;
                    case PageMode.BROWSE_RECORDS:
                        LoadBrowseArticleRecordsFromTable();
                        break;

                }
            }
        }

        public void SetLastItemShrink(Int32 lastItemIndex)
        {
            KKBOXArticles[lastItemIndex].IsExtended = false;
        }

        private void SetArticleImageCollapsed()
        {
            for (int i = 0; i < KKBOXArticles.Count; i++)
            {
                KKBOXArticles[i].ImageVisiblity = Visibility.Collapsed;
            }
        }

        private void LoadBrowseArticleRecordsFromTable()
        {
            KKBOXArticles = DBManager.Instance.LoadRecordsFromTable();
        }

        private void LoadDirectoryArticlesFromTable()
        {
            KKBOXArticles = DBManager.Instance.LoadDirectoryArticlesFromTable(DirectoryIndex);
        }

        public void LoadArticleIntoPasser()
        {
            ArticleNavigationPasser.Instance.Articles.Clear();

            for (int i = 0; i < KKBOXArticles.Count; i++)
            {
                if (KKBOXArticles[i].IsSelected)
                {
                    ArticleNavigationPasser.Instance.Articles.Add(KKBOXArticles[i]);
                }
            }
        }

        public void SetArticleCheckBoxVisibility(Visibility visibility)
        {
            if (KKBOXArticles != null)
            {
                for (int i = 0; i < KKBOXArticles.Count; i++)
                {
                    KKBOXArticles[i].CheckBoxVisiblity = visibility;
                }
            }
        }

        public void DeleteDirectoryArticles()
        {
            DBManager.Instance.DeleteArticleFromTable(DirectoryIndex);

            for (int i = 0; i < ArticleNavigationPasser.Instance.Articles.Count; i++)
            {
                KKBOXArticles.Remove(ArticleNavigationPasser.Instance.Articles[i]);
            }

            ArticleNavigationPasser.Instance.Articles.Clear();
        }

        public void SearchSelectedArticle(String keyword)
        {
            SearchLocalArticles locaArticles = new SearchLocalArticles();
            KKBOXArticles = locaArticles.SearchArticleContainKeyWord(keyword);
        }

        public Boolean IsArtilcesZero()
        {
            return (KKBOXArticles.Count == 0) ? true : false;
        }

        public void ConcelAllSelect()
        {
            for (int i = 0; i < KKBOXArticles.Count; i++)
            {
                KKBOXArticles[i].IsSelected = false;
            }
        }

        public void CheckAllSelect()
        {
            for (int i = 0; i < KKBOXArticles.Count; i++)
            {
                KKBOXArticles[i].IsSelected = true;
            }
        }

        public void LoadRssArticles(String result)
        {
            RssXmlParser rssArticleParser = new RssXmlParser();
            KKBOXArticles = rssArticleParser.GetXmlParserResult(result);
        }

        public Int32 DirectoryIndex
        {
            get;
            set;
        }

        private ObservableCollection<ArticleItem> kkboxArticles = null;
        public ObservableCollection<ArticleItem> KKBOXArticles
        {
            get
            {
                return kkboxArticles;
            }
            set
            {
                SetProperty(ref kkboxArticles, value, "KKBOXArticles");
            }
        }
    }
}
