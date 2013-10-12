using Community.CsharpSqlite.SQLiteClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News
{
    public class SearchLocalArticles 
    {
        public SearchLocalArticles()
        {
            searchArticleResult = new ObservableCollection<ArticleItem>();
            allDBArticles = new List<ArticleItem>();
            loadAllArticlesFromDB();
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
        
        private void loadAllArticlesFromDB()
        {
            allDBArticles.Clear();
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM directoryArticlesUser{0}",LoginPage.UserId);
                   using (SqliteDataReader reader = cmd.ExecuteReader())
                   {
                       while (reader.Read())
                       {
                           //Debug.WriteLine(reader.GetInt32(0));
                           //Debug.WriteLine(reader.GetInt32(1));
                           //Debug.WriteLine(reader.GetString(2));
                           //Debug.WriteLine(reader.GetString(3));
                           //Debug.WriteLine(reader.GetString(4));
                           //Debug.WriteLine(reader.GetString(5));
                           allDBArticles.Add(new ArticleItem()
                           {
                               Title = reader.GetString(2),
                               Content = reader.GetString(3),
                               IconImagePath = reader.GetString(4),
                               Link = reader.GetString(5)
                           });
                       }
                   }
                }
            }
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
