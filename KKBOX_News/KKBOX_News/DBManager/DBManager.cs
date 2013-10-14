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
    public class DBManager
    {
        private static DBManager _instance;

        private DBManager() { }

        public static DBManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DBManager();
                return _instance;
            }
        }

        private Int32 recordTableFirstID
        {
            get;
            set;
        }

        public ObservableCollection<ArticleItem> LoadRecordsFromTable()
        {
            ObservableCollection<ArticleItem> recordArticles = new ObservableCollection<ArticleItem>();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM articleBrowseRecordUser{0}", LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArticleItem recordArticleItem = new ArticleItem();
                            recordArticleItem.Title = reader.GetString(1);
                            recordArticleItem.Content = reader.GetString(2);
                            recordArticleItem.IconImagePath = reader.GetString(3);
                            recordArticleItem.Link = reader.GetString(4);

                            recordArticles.Insert(0, recordArticleItem);
                        }
                    }
                }
            }
            return recordArticles;
        }

        private Boolean IsArticleInRecords(ArticleItem recordArticle)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM articleBrowseRecordUser{0}", LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            recordTableFirstID = reader.GetInt32(0);
                            if (recordArticle.Title == reader.GetString(1))
                            {
                                return true;
                            }
                        }
                        while (reader.Read())
                        {
                            if (recordArticle.Title == reader.GetString(1))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public Boolean InsertRecordToTable(ArticleItem recordArticle)
        {
            if (IsArticleInRecords(recordArticle))
            {
                return false;
            }

            const Int32 MaxDisplayRecords = 5;
            Int32 currentRecords = 0;
            ObservableCollection<ArticleItem> totalArticleRecords = new ObservableCollection<ArticleItem>();

            totalArticleRecords = LoadRecordsFromTable();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT COUNT(*) FROM articleBrowseRecordUser{0}", LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentRecords = reader.GetInt32(0);
                        }
                    }
                    if (currentRecords == MaxDisplayRecords)
                    {
                        cmd.CommandText = String.Format("DELETE FROM articleBrowseRecordUser{0} WHERE id={1}", LoginPage.UserId, recordTableFirstID);
                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO articleBrowseRecordUser{0} (articleTitle, articleContent, articleIconPath, articleLink) VALUES(@articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();", LoginPage.UserId);
                    cmd.Parameters.Add("@articleTitle", recordArticle.Title);
                    cmd.Parameters.Add("@articleContent", recordArticle.Content);
                    cmd.Parameters.Add("@articleIconPath", recordArticle.IconImagePath);
                    cmd.Parameters.Add("@articleLink", recordArticle.Link);

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
            return true;
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

        public ObservableCollection<ArticleItem> LoadDirectoryArticlesFromTable(Int32 directoryIndex)
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

        public List<ArticleItem> LoadAllArticlesFromTable()
        {
            List<ArticleItem> allArticles = new List<ArticleItem>();
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {    //Do not search external articles
                    cmd.CommandText = String.Format("SELECT * FROM directoryArticlesUser{0} WHERE directoryId>1", LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allArticles.Add(new ArticleItem()
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
            return allArticles;
        }

        public Boolean IsTableHaveArticles(String tableName, Int32 directoryIndex)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    if (directoryIndex != -1)
                    {
                        cmd.CommandText = String.Format("SELECT * FROM {0}{1} WHERE directoryId={2}", tableName, LoginPage.UserId, directoryIndex);
                    }
                    else
                    {
                        cmd.CommandText = String.Format("SELECT * FROM {0}{1}", tableName, LoginPage.UserId);
                    }

                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? true : false;
                    }
                }
            }
        }

        public void InsertExternalArticleToTable(ArticleItem externalArticle)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO directoryArticlesUser{0} (directoryId, articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();", LoginPage.UserId);
                    cmd.Parameters.Add("@directoryId", 1);
                    cmd.Parameters.Add("@articleTitle", externalArticle.Title);
                    cmd.Parameters.Add("@articleContent", externalArticle.Content);
                    cmd.Parameters.Add("@articleIconPath", "null");
                    cmd.Parameters.Add("@articleLink", externalArticle.Link);
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }

        public Boolean VerifyUserAccount(String account, String password)
        {
            Boolean isCorrect = false;
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM userAccount WHERE account=@account AND password=@password";
                    cmd.Parameters.Add("@account", account);
                    cmd.Parameters.Add("@password", password);
                    int n = cmd.ExecuteNonQuery();
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        
                        if (reader.Read())
                        {
                            LoginPage.UserId = reader.GetInt32(0);
                            isCorrect = true;
                        }
                    }
                }
            }
            return isCorrect;
        }

        public Boolean VerifyAccountExists(String account)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT account FROM userAccount";
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (account == reader.GetString(0))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                }
            }
        }

        public void LoadUserSetting()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM userAccount WHERE id={0}",LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserSettings.Instance.IsOpenExternalWeb = reader.GetBoolean(3);
                            UserSettings.Instance.IsOpenAutoUpdate = reader.GetBoolean(4);
                            UserSettings.Instance.UpdateInterval = reader.GetInt32(5);
                        }
                    }
                }
            }
        }

        public ObservableCollection<MySelectedArticleDirectory> LoadDirectoriesFromTable()
        {
            ObservableCollection<MySelectedArticleDirectory> directories = new ObservableCollection<MySelectedArticleDirectory>();

            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = String.Format("SELECT * FROM directoryTableUser{0}", LoginPage.UserId);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            directories.Add(new MySelectedArticleDirectory()
                            {
                                DirectoryIndex = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(reader.GetString(2)),
                                NonRemoved = Visibility.Collapsed,
                            });
                        }
                        while (reader.Read())
                        {
                            directories.Add(new MySelectedArticleDirectory()
                            {
                                DirectoryIndex = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(reader.GetString(2)),
                            });
                        }
                    }
                }
                conn.Close();
            }
            return directories;
        }

        public void UpdateUserSettings()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("UPDATE userAccount SET openExternalWeb=@openExternalWeb, openAutoUpdate=@openAutoUpdate, updateInterval=@updateInterval WHERE id={0}", LoginPage.UserId);
                    cmd.Parameters.Add("@openExternalWeb", UserSettings.Instance.IsOpenExternalWeb ? 1 : 0);
                    cmd.Parameters.Add("@openAutoUpdate", UserSettings.Instance.IsOpenAutoUpdate ? 1 : 0);
                    cmd.Parameters.Add("@updateInterval", UserSettings.Instance.UpdateInterval);
                    int n = cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;

                }
            }
        }

        public void CreateNewAcoount(String account, String password)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = "INSERT INTO userAccount (account, password, openExternalWeb, openAutoUpdate, updateInterval) VALUES(@account, @password, @openExternalWeb, @openAutoUpdate, @updateInterval);SELECT last_insert_rowid();";
                    cmd.Parameters.Add("@account", account);
                    cmd.Parameters.Add("@password", password);
                    cmd.Parameters.Add("@openExternalWeb", 0);
                    cmd.Parameters.Add("@openAutoUpdate", 0);
                    cmd.Parameters.Add("@updateInterval", 5);

                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }
       
    }
}
