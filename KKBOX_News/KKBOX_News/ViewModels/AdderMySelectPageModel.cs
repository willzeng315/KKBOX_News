using KKBOX_News.DBService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KKBOX_News.ViewModels
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

    public class AdderMySelectPageModel
    {
        public AdderMySelectPageModel()
        {
            AdderListBox = new ObservableCollection<AdderItem>();
            LoadDirectoryIntoList();
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

        public void AddArticleAction(String imageName, Int32 directoryIndex)
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
                NewDirectory(imageName, directoryIndex);
            }
            ArticleNavigationPasser.Instance.Articles.Clear();
        }

        private void NewDirectory(String imageName, Int32 directoryIndex)
        {
            MySelectedArticleDirectory mySelectedArticleDirectory = new MySelectedArticleDirectory();

            mySelectedArticleDirectory.Title = AdderListBox[1].ItemTitle;

            if (imageName == null)
            {
                imageName = "KKBOX.jpg"; // prevent user want to create new folder but not choose image 
            }

            mySelectedArticleDirectory.CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(imageName);
            mySelectedArticleDirectory.DirectoryIndex = directoryIndex;// GetLastDirectoryIndex();

            App.ViewModel.ArticleDirectories.Add(mySelectedArticleDirectory);

            DBManager.Instance.InsertDirectoryToTable(AdderListBox[1].ItemTitle, imageName);

            DBManager.Instance.InsertArticleToTable(mySelectedArticleDirectory.DirectoryIndex);
        }

        public void SetCoverImage(BitmapImage cover)
        {
            AdderListBox[2].CoverImage = cover;
        }

        public ObservableCollection<AdderItem> AdderListBox
        {
            get;
            set;
        }
    }
}
