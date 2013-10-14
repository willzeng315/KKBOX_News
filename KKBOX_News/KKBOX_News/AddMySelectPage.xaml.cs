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
using System.IO.IsolatedStorage;
using System.IO;

namespace KKBOX_News
{
    public enum AdderItemTemplate
    {
        TEMPLATE_SPACE,
        TEMPLATE_TXETBOX,
        TEMPLATE_TXETBLOCK,
        TEMPLATE_COVERIMAGE,
    }

    public class AdderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Space
        {
            get;
            set;
        }

        public DataTemplate TextboxCheck
        {
            get;
            set;
        }

        public DataTemplate TextblockCheck
        {
            get;
            set;
        }

        public DataTemplate CoverImage
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
                    case AdderItemTemplate.TEMPLATE_SPACE:
                        return Space;
                    case AdderItemTemplate.TEMPLATE_TXETBOX:
                        return TextboxCheck;
                    case AdderItemTemplate.TEMPLATE_TXETBLOCK:
                        return TextblockCheck;
                    case AdderItemTemplate.TEMPLATE_COVERIMAGE:
                        return CoverImage;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
    public class AdderItem : BindableBase
    {
        public String ItemTitle
        {
            set;
            get;
        }
        public AdderItemTemplate Type
        {
            set;
            get;
        }
        public Boolean IsChecked
        {
            set;
            get;
        }

        public Int32 DirectoryId
        {
            set;
            get;
        }

        private BitmapImage coverImage;
        public BitmapImage CoverImage
        {
            get
            {
                return coverImage;
            }
            set
            {
                SetProperty(ref coverImage, value, "CoverImage");
            }
        }

        public AdderItem()
        {
            ItemTitle = "";
            IsChecked = false;
        }

    }

    public partial class AddMySelectPage : PhoneApplicationPage
    {
        public AddMySelectPage()
        {
            InitializeComponent();
            LoadDirectoryIntoList();
            DataContext = this;
        }

        private void LoadDirectoryIntoList()
        {
            AdderListBox = new ObservableCollection<AdderItem>();
            AdderListBox.Add(new AdderItem() { ItemTitle = "", Type = AdderItemTemplate.TEMPLATE_SPACE });
            AdderListBox.Add(new AdderItem() { ItemTitle = "新資料夾", Type = AdderItemTemplate.TEMPLATE_TXETBOX });
            AdderListBox.Add(new AdderItem() { Type = AdderItemTemplate.TEMPLATE_COVERIMAGE });
            AdderListBox.Add(new AdderItem() { ItemTitle = "", Type = AdderItemTemplate.TEMPLATE_SPACE });
            for (int i = 1; i < App.ViewModel.ArticleDirectories.Count; i++)
            {
                AdderListBox.Add(new AdderItem()
                {
                    ItemTitle = App.ViewModel.ArticleDirectories[i].Title,
                    DirectoryId = App.ViewModel.ArticleDirectories[i].DirectoryIndex,
                    Type = AdderItemTemplate.TEMPLATE_TXETBLOCK
                });
            }

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (coverChooser != null)
            {
                selectedImageName = coverChooser.GetSelectedImageName();
                AdderListBox[2].CoverImage = coverChooser.GetChooseImage();
            }
        }

        private void OnChoosePhotoClick(Object sender, RoutedEventArgs e)
        {
            coverChooser = new PhotoChooser();
        }

        private Int32 GetLastDirectoryIndex()
        {
            Int32 DirectoryIndex = 2; //dirID = 1 is externalArticle

            if (App.ViewModel.ArticleDirectories.Count > 0)
            {
                DirectoryIndex = App.ViewModel.ArticleDirectories[App.ViewModel.ArticleDirectories.Count - 1].DirectoryIndex + 1;
            }
            return DirectoryIndex;
        }

        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            listbox.SelectedIndex = -1;
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            for (int i = 3; i < AdderListBox.Count; i++) //start with my select because i=0 is space, i=1 is new directory,i=2 is space
            {
                if (AdderListBox[i].Type == AdderItemTemplate.TEMPLATE_SPACE)
                {
                    continue;
                }

                if (AdderListBox[i].IsChecked)
                {
                    DBManager.Instance.InsertArticleToTable(AdderListBox[i].DirectoryId);
                }
            }

            if (AdderListBox[1].IsChecked) //new directory 
            {
                MySelectedArticleDirectory mySelectedArticleDirectory = new MySelectedArticleDirectory();

                mySelectedArticleDirectory.Title = AdderListBox[1].ItemTitle;

                if (selectedImageName == null)
                {
                    selectedImageName = "KKBOX.jpg"; // prevent user want to create new folder but not choose image 
                }

                mySelectedArticleDirectory.CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(selectedImageName);
                mySelectedArticleDirectory.DirectoryIndex = GetLastDirectoryIndex();

                App.ViewModel.ArticleDirectories.Add(mySelectedArticleDirectory);

                DBManager.Instance.InsertDirectoryToTable(AdderListBox[1].ItemTitle, selectedImageName);

                DBManager.Instance.InsertArticleToTable(mySelectedArticleDirectory.DirectoryIndex);

            }
            ArticleNavigationPasser.Instance.Articles.Clear();

            NavigationService.GoBack();
        }

        private void OnConcelButtonClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        #region property

        private PhotoChooser coverChooser
        {
            get;
            set;
        }

        private String selectedImageName
        {
            get;
            set;
        }

        public ObservableCollection<AdderItem> AdderListBox
        {
            get;
            set;
        }

        #endregion
    }
}