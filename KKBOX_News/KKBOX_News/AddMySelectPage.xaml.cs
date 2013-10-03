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
using System.Collections.ObjectModel;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Media;

namespace KKBOX_News
{
    public class AdderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate space
        {
            get;
            set;
        }

        public DataTemplate textbox
        {
            get;
            set;
        }

        public DataTemplate textblock
        {
            get;
            set;
        }

        public DataTemplate selectImage
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            AdderItem myItem = item as AdderItem;
            if (myItem != null)
            {
                switch (myItem.Type)
                {
                    case "space":
                        return space;
                    case "textbox":
                        return textbox;
                    case "textblock":
                        return textblock;
                    case "selectImage":
                        return selectImage;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
    public class AdderItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public String ItemTitle
        {
            set;
            get;
        }
        public String Type
        {
            set;
            get;
        }
        public Boolean IsChecked
        {
            set;
            get;
        }

        private BitmapImage coverImage;
        public BitmapImage CoverImage
        {
            get { return coverImage; }
            set
            {
                coverImage = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CoverImage"));
                }
            }
        }

        public AdderItem()
        {
            ItemTitle = "";
            Type = "";
            IsChecked = false;
        }

    }
    public partial class AddMySelectPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        PhotoChooserTask photoChooserTask;
        public event PropertyChangedEventHandler PropertyChanged;

        public AddMySelectPage()
        {
            InitializeComponent();
            loadDirectoryIntoList();
            DataContext = this;

            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(OnPhotoChooserTaskCompleted);
        }

        private void OnPhotoChooserTaskCompleted(Object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bitmap = new BitmapImage();
                Debug.WriteLine(e.OriginalFileName);
                bitmap.SetSource(e.ChosenPhoto);
                AdderListBox[2].CoverImage = bitmap;
                //CoverImage.Source = bitmap;
            }
            else if (e.TaskResult == TaskResult.Cancel)
                MessageBox.Show("您沒有選擇圖片", "警告", MessageBoxButton.OK);
            else
                MessageBox.Show("圖片選擇中發生錯誤:\n" + e.Error.Message, "Fail", MessageBoxButton.OK);
        }

        private void OnChoosePhotoClick(Object sender, RoutedEventArgs e)
        {
            photoChooserTask.Show();
        }

        private void loadDirectoryIntoList()
        {
            AdderListBox = new ObservableCollection<AdderItem>();
            AdderListBox.Add(new AdderItem() { ItemTitle = "", Type = "space" });
            AdderListBox.Add(new AdderItem() { ItemTitle = "新資料夾", Type = "textbox"});
            AdderListBox.Add(new AdderItem() { Type = "selectImage" });
            AdderListBox.Add(new AdderItem(){ItemTitle = "", Type = "space"});
            for (int i = 0; i < App.ViewModel.ArticleDirectories.Count; i++)
            {
                AdderListBox.Add(new AdderItem() { ItemTitle = App.ViewModel.ArticleDirectories[i].Title, Type = "textblock" });
            }

        }
        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
           IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            myArticleItem = new ArticleItem();
            

            if (parameters.ContainsKey("Title"))
            {
                myArticleItem.Title = parameters["Title"];
            }
            if (parameters.ContainsKey("Content"))
            {
                myArticleItem.Content = parameters["Content"];
            }
            if (parameters.ContainsKey("Link"))
            {
                myArticleItem.Link = parameters["Link"];
            }
            if (parameters.ContainsKey("ImagePath"))
            {
                myArticleItem.IconImagePath = parameters["ImagePath"];
            }


        }
        #region property
        private ArticleItem myArticleItem
        {
            get;
            set;
        }

        public String SelectedItemTitle
        {
            get;
            set;
        }

        public String SelectedItemContent
        {
            get;
            set;
        }

        public String SelectedItemLink
        {
            get;
            set;
        }

        public String SelectedItemImagePath
        {
            get;
            set;
        }

        private ObservableCollection<AdderItem> adderListBox;
        public ObservableCollection<AdderItem> AdderListBox
        {
            get { return adderListBox; }
            set
            {
                adderListBox = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("AdderListBox"));
                }
            }
        }
        #endregion 

        private void OnComfirmClick(Object sender, RoutedEventArgs e)
        {
            using (SqliteConnection conn = new SqliteConnection("Version=3,uri=file:KKBOX_NEWS.db"))
            {
                conn.Open();
                using (SqliteCommand cmd = conn.CreateCommand())
                {
                    cmd.Transaction = conn.BeginTransaction();
                    cmd.CommandText = "INSERT INTO directoryArticles (directoryId, articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();";
                    cmd.Parameters.Add("@directoryId", null);
                    cmd.Parameters.Add("@articleTitle", null);
                    cmd.Parameters.Add("@articleContent", null);
                    cmd.Parameters.Add("@articleIconPath", null);
                    cmd.Parameters.Add("@articleLink", null);
                    for (int i = 3; i < AdderListBox.Count; i++) //start with my select because i=0 is space, i=1 is new directory,i=2 is space
                    {
                        if (AdderListBox[i].Type == "sapce")
                        {
                            continue;
                        }

                        if (AdderListBox[i].IsChecked)
                        {
                            //App.ViewModel.ArticleDirectories[i - 3].ArticleItemList.Add(myArticleItem);

                            cmd.Parameters["@directoryId"].Value = i - 2; //ID start with 1
                            cmd.Parameters["@articleTitle"].Value = myArticleItem.Title;
                            cmd.Parameters["@articleContent"].Value = myArticleItem.Content;
                            cmd.Parameters["@articleIconPath"].Value = myArticleItem.IconImagePath;
                            cmd.Parameters["@articleLink"].Value = myArticleItem.Link;

                            cmd.ExecuteNonQuery();
                            
                        }
                    }
                    cmd.Transaction.Commit();
                    cmd.Transaction = null;
                }


                if (AdderListBox[1].IsChecked) //new directory 
                {
                    MySelectedArticleDirectory mySelectedArticleDirectory = new MySelectedArticleDirectory();
                    mySelectedArticleDirectory.Title = AdderListBox[1].ItemTitle;
                    mySelectedArticleDirectory.DirectoryIndex = App.ViewModel.ArticleDirectories.Count+1;
                    App.ViewModel.ArticleDirectories.Add(mySelectedArticleDirectory);

                    Int32 TotalArticleDirectories = App.ViewModel.ArticleDirectories.Count;
                    //App.ViewModel.ArticleDirectories[TotalArticleDirectories - 1].ArticleItemList.Add(myArticleItem);

                    using (SqliteCommand cmd = conn.CreateCommand())
                    {
                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = "INSERT INTO directoryTable (directoryName, imagePath) VALUES(@directoryName, @imagePath);SELECT last_insert_rowid();";

                        cmd.Parameters.Add("@directoryName", null);
                        cmd.Parameters.Add("@imagePath", null);

                        cmd.Parameters["@directoryName"].Value = AdderListBox[1].ItemTitle;
                        cmd.Parameters["@imagePath"].Value = "123";

                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        cmd.Transaction = null;
                        //cmd.CommandText = "SELECT * FROM directoryTable";
                        //using (SqliteDataReader reader = cmd.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        Debug.WriteLine(reader.GetInt32(0));
                        //        Debug.WriteLine(reader.GetString(1));
                        //        Debug.WriteLine(reader.GetString(2));
                        //    }
                        //}

                        cmd.Transaction = conn.BeginTransaction();
                        cmd.CommandText = "INSERT INTO directoryArticles (directoryId, articleTitle, articleContent, articleIconPath, articleLink) VALUES(@directoryId, @articleTitle, @articleContent, @articleIconPath, @articleLink);SELECT last_insert_rowid();";
                        cmd.Parameters.Add("@directoryId", null);
                        cmd.Parameters.Add("@articleTitle", null);
                        cmd.Parameters.Add("@articleContent", null);
                        cmd.Parameters.Add("@articleIconPath", null);
                        cmd.Parameters.Add("@articleLink", null);

                        cmd.Parameters["@directoryId"].Value = TotalArticleDirectories; //ID start with 1
                        cmd.Parameters["@articleTitle"].Value = myArticleItem.Title;
                        cmd.Parameters["@articleContent"].Value = myArticleItem.Content;
                        cmd.Parameters["@articleIconPath"].Value = myArticleItem.IconImagePath;
                        cmd.Parameters["@articleLink"].Value = myArticleItem.Link;

                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        cmd.Transaction = null;
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
                    }
                }

            }
            NavigationService.GoBack();

        }
        private void OnConcelClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        

 
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < AdderListBox.Count; i++)
            //{
            //    Debug.WriteLine(String.Format("{0} {1}", i, AdderListBox[i].IsChecked));
            //}
            //Debug.WriteLine(AdderListBox[1].ItemTitle);
        }
    }
}