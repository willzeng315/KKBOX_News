using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KKBOX_News.AppService;

namespace KKBOX_News
{
    public class SearchLocalArticles
    {
        public SearchLocalArticles()
        {
            searchArticleResult = new ObservableCollection<ArticleItem>();
            allDBArticles = new List<ArticleItem>();
            allDBArticles = DBManager.Instance.LoadAllArticlesFromTable();
        }

        public ObservableCollection<ArticleItem> SearchArticleContainKeyWord(String keyWord)
        {
            searchArticleResult.Clear();
            for (int i = 0; i < allDBArticles.Count; i++)
            {
                if (allDBArticles[i].Title.Contains(keyWord))
                {
                    Boolean isResultContainArticle = false;
                    for (int j = 0; j < searchArticleResult.Count; j++)
                    {
                        if (searchArticleResult[j].Title.Contains(allDBArticles[i].Title))
                        {
                            isResultContainArticle = true;
                            break;
                        }
                    }
                    if (!isResultContainArticle)
                    {
                        searchArticleResult.Add(allDBArticles[i]);
                    }
                }
            }
            return searchArticleResult;
        }

        private List<ArticleItem> allDBArticles
        {
            get;
            set;
        }

        private ObservableCollection<ArticleItem> searchArticleResult
        {
            get;
            set;
        }
    }
}
