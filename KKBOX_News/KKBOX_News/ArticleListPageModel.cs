using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News
{
    public class ArticleItem : BindableBase
    {
        public ArticleItem()
        {
        }

        public String iconImagePath = "";
        public String IconImagePath
        {
            get
            {
                return iconImagePath;
            }
            set
            {
                iconImagePath = value;
                Notify("IconImagePath");
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
                title = value;
                Notify("Title");
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
                content = value;
                Notify("Content");
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
                link = value;
                Notify("Link");
            }
        }
        private bool isExtended;
        public bool IsExtended
        {
            get
            {
                return isExtended;
            }
            set
            {
                isExtended = value;
                Notify("IsExtended");
            }
        }
    }

    public class ArticleListPageModel : BindableBase
    {
        public ArticleListPageModel()
        {
            Items = new ObservableCollection<ArticleItem>();
        }

        private ObservableCollection<ArticleItem> items = null;
        public ObservableCollection<ArticleItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                Notify("Items");
            }
        }
    }
}
