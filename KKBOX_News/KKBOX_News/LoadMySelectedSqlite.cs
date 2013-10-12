using Community.CsharpSqlite.SQLiteClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News
{
    class LoadMySelectedSqlite
    {
        public static Boolean CreateAccountTable()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            //isf.DeleteFile("KKBOX_NEWS.db");
            try
            {
                using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
                {
                    conn.Open();

                    using (SqliteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE userAccount ( [id] INTEGER PRIMARY KEY, [account] TEXT, [password] TEXT, [openExternalWeb] INTEGER, [openAutoUpdate] INTEGER, [updateInterval] INTEGER)";
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
            
        }

        public static void InitialAccoutData()
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = "INSERT INTO userAccount (account, password, openExternalWeb, openAutoUpdate, updateInterval) VALUES(@account, @password, @openExternalWeb, @openAutoUpdate, @updateInterval);SELECT last_insert_rowid();";
                    cmd.Parameters.Add("@account", null);
                    cmd.Parameters.Add("@password", null);
                    cmd.Parameters.Add("@openExternalWeb", null);
                    cmd.Parameters.Add("@openAutoUpdate", null);
                    cmd.Parameters.Add("@updateInterval", null);

                    for (int i = 0; i < 3; i++)
                    {
                        cmd.Parameters["@account"].Value = "a" + (i+1);
                        cmd.Parameters["@password"].Value = "a" + (i+1);
                        cmd.Parameters["@openExternalWeb"].Value = 0;
                        cmd.Parameters["@openAutoUpdate"].Value = 0;
                        cmd.Parameters["@updateInterval"].Value = 5;


                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }

        }

        public static Boolean CreateUserTables(Int32 userId)
        {
            
            try
            {
                using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
                {
                    conn.Open();

                    using (SqliteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = String.Format("CREATE TABLE directoryTableUser{0} ( [id] INTEGER PRIMARY KEY, [directoryName] TEXT, [imagePath] TEXT)", userId);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = String.Format("CREATE TABLE directoryArticlesUser{0} ( [id] INTEGER PRIMARY KEY, [directoryId] INTEGER, [articleTitle] TEXT, [articleContent] TEXT, [articleIconPath] TEXT, [articleLink] TEXT)", userId);
                        cmd.ExecuteNonQuery();
                        
                        //cmd.Transaction = conn.BeginTransaction();
                        //cmd.CommandText = "INSERT INTO directoryArticles (directoryId,  articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();";
                        //cmd.Parameters.Add("@directoryId", null);
                        //cmd.Parameters.Add("@articleTitle", null);
                        //cmd.Parameters.Add("@articleContent", null);
                        //cmd.Parameters.Add("@articleIconPath", null);
                        //cmd.Parameters.Add("@articleLink", null);
                        //cmd.ExecuteNonQuery();

                        //cmd.Transaction.Commit();
                        //cmd.Transaction = null;

                        //cmd.CommandText = "SELECT * FROM directoryArticles";

                        //using (SqliteDataReader reader = cmd.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        Debug.WriteLine(reader.GetInt32(0));
                        //        Debug.WriteLine(reader.GetInt32(1));
                        //        Debug.WriteLine(reader.GetString(2));
                        //        Debug.WriteLine(reader.GetString(3));
                        //        Debug.WriteLine(reader.GetString(4));
                        //        Debug.WriteLine(reader.GetString(5));
                        //    }
                        //}

                        //cmd.Transaction = null;
                        //cmd.CommandText = "SELECT * FROM directoryTable";
                        //using (SqliteDataReader reader = cmd.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        Debug.WriteLine(reader.GetInt32(0));
                        //        Debug.WriteLine(reader.GetString(1));
                        //        Debug.WriteLine(reader.GetInt32(2));
                        //        Debug.WriteLine(reader.GetString(3));
                        //    }
                        //}
                        conn.Close();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
         }

        public static void InitialUserTableData(Int32 userId)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = String.Format("INSERT INTO directoryTableUser{0} (directoryName, imagePath) VALUES(@directoryName, @imagePath);SELECT last_insert_rowid();", userId);
                    cmd.Parameters.Add("@directoryName", null);
                    cmd.Parameters.Add("@imagePath", null);

                    cmd.Parameters["@directoryName"].Value = "外部文章";
                    cmd.Parameters["@imagePath"].Value = "KKBOX.jpg"; //Default Image

                    cmd.ExecuteNonQuery();

                    for (int i = 0; i < 3; i++)
                    {
                        cmd.Parameters["@directoryName"].Value = "個人精選" + (i + 1);
                        cmd.Parameters["@imagePath"].Value = "KKBOX.jpg"; //Default Image

                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }
            }
        }
        public LoadMySelectedSqlite()
        {
            
        }
    }
}
