using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using Community.CsharpSqlite.SQLiteClient;

namespace KKBOX_News
{
    public partial class ExternalArticlesInfoPage : PhoneApplicationPage
    {
        public ExternalArticlesInfoPage()
        {
            InitializeComponent();
            ExternalArticle = new ArticleItem();
            DataContext = this;
        }

        private void saveExternalArticleToDB()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO directoryArticlesUser{0} (directoryId, articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();", LoginPage.UserId);
                    cmd.Parameters.Add("@directoryId", 1);
                    cmd.Parameters.Add("@articleTitle", ExternalArticle.Title);
                    cmd.Parameters.Add("@articleContent", ExternalArticle.Content);
                    cmd.Parameters.Add("@articleIconPath", "123");
                    cmd.Parameters.Add("@articleLink", ExternalArticle.Link);
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                    cmd.Transaction = null;

                    //cmd.CommandText = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, 1);

                    //using (SqliteDataReader reader = cmd.ExecuteReader())
                    //{
                    //    while (reader.Read())
                    //    {
                    //        Debug.WriteLine(reader.GetInt32(1));
                    //        Debug.WriteLine(reader.GetString(2));
                    //        Debug.WriteLine(reader.GetString(3));
                    //        Debug.WriteLine(reader.GetString(4));
                    //        Debug.WriteLine(reader.GetString(5));
                    //    }

                    //}
                }
            }
        }
        
        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine(ExternalArticle.Title);
            //Debug.WriteLine(ExternalArticle.Content);
            //Debug.WriteLine(ExternalArticle.Link);
            saveExternalArticleToDB();
            ArticleNavigationPasser.Instance.Articles.Clear();
            ArticleNavigationPasser.Instance.Articles.Add(ExternalArticle);
            this.NavigationService.GoBack();
        }

        public ArticleItem ExternalArticle
        {
            get;
            set;
        }
    
    }
}