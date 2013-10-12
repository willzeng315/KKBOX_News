using Community.CsharpSqlite.SQLiteClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public class DBManger
    {
        private static DBManger _instance;

        private DBManger() { }

        public static DBManger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DBManger();
                return _instance;
            }
        }


        public void UpdateDirectoryToTable(Int32 directoryIndex, String coverTitle, String imageName)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("UPDATE directoryTableUser{0} SET directoryName=@directoryName WHERE id={1}", LoginPage.UserId, directoryIndex);
                    cmd.Parameters.Add("@directoryName", coverTitle);
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;


                    if (imageName != null)
                    {
                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = String.Format("UPDATE directoryTableUser{0} SET imagePath=@imagePath WHERE id={1}", LoginPage.UserId, directoryIndex);
                        cmd.Parameters.Add("@imagePath", imageName);
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        cmd.Transaction = null;
                    }
                }
            }
        }

        public void UpdateValueToArticleTable(Int32 directoryIndex, List<String> args)
        {

            //using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            //{
            //    conn.Open();

            //    using (SqliteCommand cmd = conn.CreateCommand())
            //    {
            //        cmd.Transaction = conn.BeginTransaction();
            //        cmd.CommandText = String.Format("UPDATE directoryTableUser{0} SET directoryName=@directoryName WHERE id={1}", LoginPage.UserId, directoryIndex);
            //        cmd.Parameters.Add("@directoryName", CoverTitle);
            //        cmd.ExecuteNonQuery();
            //        cmd.Transaction.Commit();
            //        cmd.Transaction = null;


            //        if (selectedImageName != null)
            //        {
            //            cmd.Transaction = conn.BeginTransaction();
            //            cmd.CommandText = String.Format("UPDATE directoryTableUser{0} SET imagePath=@imagePath WHERE id={1}", LoginPage.UserId, directoryIndex);
            //            cmd.Parameters.Add("@imagePath", selectedImageName);
            //            cmd.ExecuteNonQuery();
            //            cmd.Transaction.Commit();
            //            cmd.Transaction = null;
            //        }
            //    }
            //}
        }

        public void InsertDirectoryToTable(String directoryName, String imagePath)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO directoryTableUser{0} (directoryName, imagePath) VALUES(@directoryName, @imagePath);SELECT last_insert_rowid();", LoginPage.UserId);

                    cmd.Parameters.Add("@directoryName", directoryName);
                    cmd.Parameters.Add("@imagePath", imagePath);

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }

        public void InsertArticleToTable(Int32 directoryIndex)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO directoryArticlesUser{0} (directoryId, articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();", LoginPage.UserId);

                    cmd.Parameters.Add("@directoryId", null);
                    cmd.Parameters.Add("@articleTitle", null);
                    cmd.Parameters.Add("@articleContent", null);
                    cmd.Parameters.Add("@articleIconPath", null);
                    cmd.Parameters.Add("@articleLink", null);

                    for (int j = 0; j < ArticleNavigationPasser.Instance.Articles.Count; j++)
                    {
                        cmd.Parameters["@directoryId"].Value = directoryIndex; //ID start with 1
                        cmd.Parameters["@articleTitle"].Value = ArticleNavigationPasser.Instance.Articles[j].Title;
                        cmd.Parameters["@articleContent"].Value = ArticleNavigationPasser.Instance.Articles[j].Content;
                        cmd.Parameters["@articleIconPath"].Value = ArticleNavigationPasser.Instance.Articles[j].IconImagePath;
                        cmd.Parameters["@articleLink"].Value = ArticleNavigationPasser.Instance.Articles[j].Link;

                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }

        public void DeleteDirectoryFromTable(Int32 directoryIndex)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("DELETE FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, directoryIndex);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = String.Format("DELETE FROM directoryTableUser{0} WHERE id={1}", LoginPage.UserId, directoryIndex);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteArticleFromTable(Int32 directoryIndex)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("DELETE FROM directoryArticlesUser{0} WHERE directoryId={1} AND articleTitle=@articleTitle", LoginPage.UserId, directoryIndex);
                    cmd.Parameters.Add("@articleTitle", null);

                    for (int i = 0; i < ArticleNavigationPasser.Instance.Articles.Count; i++)
                    {
                        cmd.Parameters["@articleTitle"].Value = ArticleNavigationPasser.Instance.Articles[i].Title;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public ObservableCollection<ArticleItem> LoadDirectoryArticles(Int32 directoryIndex)
        {
            ObservableCollection<ArticleItem> directoryArticles = new ObservableCollection<ArticleItem>();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    String querySrting = "";

                    querySrting = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, directoryIndex);

                    cmd.CommandText = querySrting;

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        

                        while (reader.Read())
                        {
                            ArticleItem directoryArticleItem = new ArticleItem();
                            directoryArticleItem.Title = reader.GetString(2);
                            directoryArticleItem.Content = reader.GetString(3);
                            directoryArticleItem.IconImagePath = reader.GetString(4);
                            directoryArticleItem.Link = reader.GetString(5);
                            directoryArticleItem.DeleteMenuVisiblity = Visibility.Visible;
                            if (directoryIndex == 1) // set external article can not add to my selected
                            {
                                directoryArticleItem.AddMenuVisiblity = Visibility.Collapsed;
                            }

                            directoryArticles.Add(directoryArticleItem);
                        }

                    }
                }
            }
            return directoryArticles;
        }

        public Boolean IsDirectoryHaveArticles(Int32 directoryIndex)
        {

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    String querySrting = "";

                    querySrting = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId={1}", LoginPage.UserId, directoryIndex);

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
    }
}
