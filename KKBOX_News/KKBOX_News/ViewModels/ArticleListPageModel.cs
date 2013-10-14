using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public class ArticleItem : BindableBase
    {
        public ArticleItem()
        {
            CheckBoxVisiblity = Visibility.Collapsed;
            DeleteMenuVisiblity = Visibility.Collapsed;
            AddMenuVisiblity = Visibility.Visible;
            ImageVisiblity = Visibility.Visible;
            IsSelected = false;
        }
        #region Property
        private String iconImagePath = "";
        public String IconImagePath
        {
            get
            {
                return iconImagePath;
            }
            set
            {
                SetProperty(ref iconImagePath, value, "IconImagePath");
            }
        }

        private String title = "";
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

        private String content = "";
        public String Content
        {
            get
            {
                return content;
            }
            set
            {
                SetProperty(ref content, value, "Content");
            }
        }
        private String link = "";
        public String Link
        {
            get
            {
                return link;
            }
            set
            {
                SetProperty(ref link, value, "Link");
            }
        }

        private Boolean isExtended;
        public Boolean IsExtended
        {
            get
            {
                return isExtended;
            }
            set
            {
                SetProperty(ref isExtended, value, "IsExtended");
            }
        }

        private Boolean isSelected;
        public Boolean IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value, "IsSelected");
            }
        }

        private Visibility checkBoxVisiblity;
        public Visibility CheckBoxVisiblity
        {
            get
            {
                return checkBoxVisiblity;
            }
            set
            {
                SetProperty(ref checkBoxVisiblity, value, "CheckBoxVisiblity");
            }
        }

        private Visibility deleteMenuVisiblity;
        public Visibility DeleteMenuVisiblity
        {
            get
            {
                return deleteMenuVisiblity;
            }
            set
            {
                SetProperty(ref deleteMenuVisiblity, value, "DeleteMenuVisiblity");
            }
        }

        private Visibility addMenuVisiblity;
        public Visibility AddMenuVisiblity
        {
            get
            {
                return addMenuVisiblity;
            }
            set
            {
                SetProperty(ref addMenuVisiblity, value, "AddMenuVisiblity");
            }
        }

        private Visibility imageVisiblity;
        public Visibility ImageVisiblity
        {
            get
            {
                return imageVisiblity;
            }
            set
            {
                SetProperty(ref imageVisiblity, value, "ImageVisiblity");
            }
        }

    }
        #endregion
    public class ArticleListPageModel : BindableBase
    {
        public ArticleListPageModel()
        {
            KKBOXArticles = new ObservableCollection<ArticleItem>();
        }

        private ObservableCollection<ArticleItem> kkboxArticles = null;
        public ObservableCollection<ArticleItem> KKBOXArticles
        {
            get
            {
                return kkboxArticles;
            }
            set
            {
                SetProperty(ref kkboxArticles, value, "KKBOXArticles");
            }
        }
    }
}
