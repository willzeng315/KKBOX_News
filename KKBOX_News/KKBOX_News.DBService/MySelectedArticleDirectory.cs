using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace KKBOX_News.AppService
{

    public class MySelectedArticleDirectory : BindableBase
    {
        #region Property
        //public List<ArticleItem> ArticleItemList;

        public Int32 DirectoryIndex
        {
            get;
            set;
        }

        public String ImagePath
        {
            get;
            set;
        }

        public Visibility NonRemoved
        {
            get;
            set;
        }

        private String title ="";
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value, "Title");
            }
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

        #endregion
        public MySelectedArticleDirectory()
        {
            NonRemoved = Visibility.Visible;
        }
    }
}
