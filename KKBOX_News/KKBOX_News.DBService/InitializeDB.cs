using Community.CsharpSqlite.SQLiteClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News.AppService
{
    public class InitializeDB
    {
        private static InitializeDB _instance;

        private InitializeDB() { }

        public static InitializeDB Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new InitializeDB();
                return _instance;
            }
        }

        public Boolean IsTableExists(String tableName)
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            //isf.DeleteFile("KKBOX_NEWS.db");
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();

                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@name";
                    cmd.Parameters.Add("@name", tableName);
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.Read() ? true : false;
                    }
                }
            }
        }

        public Boolean CreateAccountTable()
        {
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
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }

        }

        public void InitialAccoutData()
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
                        cmd.Parameters["@account"].Value = "a" + (i + 1);
                        cmd.Parameters["@password"].Value = "a" + (i + 1);
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

        public Boolean CreateUserTables(Int32 userId)
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

                        cmd.CommandText = String.Format("CREATE TABLE articleBrowseRecordUser{0} ( [id] INTEGER PRIMARY KEY, [articleTitle] TEXT, [articleContent] TEXT, [articleIconPath] TEXT, [articleLink] TEXT)", userId);
                        cmd.ExecuteNonQuery();

                    }
                    conn.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void InitialUserTableData(Int32 userId)
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
    }
}
