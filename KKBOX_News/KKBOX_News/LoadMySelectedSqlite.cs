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
        public static Boolean CreateTables()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            isf.DeleteFile("KKBOX_NEWS.db");
            try
            {
                using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
                {
                    conn.Open();

                    using (SqliteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "CREATE TABLE directoryTable ( [id] INTEGER PRIMARY KEY, [directoryName] TEXT, [imagePath] TEXT)";
                        cmd.ExecuteNonQuery();

                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = "INSERT INTO directoryTable (directoryName, imagePath) VALUES(@directoryName, @imagePath);SELECT last_insert_rowid();";
                        cmd.Parameters.Add("@directoryName", null);
                        cmd.Parameters.Add("@imagePath", null);

                        for (int i = 0; i < 3; i++)
                        {
                            cmd.Parameters["@directoryName"].Value = "個人精選" + i;
                            cmd.Parameters["@imagePath"].Value = "123";

                            cmd.ExecuteNonQuery();
                        }
                        cmd.Transaction.Commit();
                        cmd.Transaction = null;

                        cmd.CommandText = "CREATE TABLE directoryArticles ( [id] INTEGER PRIMARY KEY, [directoryId] INTEGER, [articleTitle] TEXT, [articleContent] TEXT, [articleIconPath] TEXT, [articleLink] TEXT)";
                        cmd.ExecuteNonQuery();

                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = "INSERT INTO directoryArticles (directoryId,  articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();";
                        cmd.Parameters.Add("@directoryId", null);
                        cmd.Parameters.Add("@articleTitle", null);
                        cmd.Parameters.Add("@articleContent", null);
                        cmd.Parameters.Add("@articleIconPath", null);
                        cmd.Parameters.Add("@articleLink", null);
                        cmd.ExecuteNonQuery();

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
        public LoadMySelectedSqlite()
        {
            
        }
    }
}
