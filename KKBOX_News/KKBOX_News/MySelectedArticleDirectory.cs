using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{

    public class MySelectedArticleDirectory 
    {
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

      
        public String Title
        {
            get;
            set;
        }

        public MySelectedArticleDirectory()
        {
            //ArticleItemList = new List<ArticleItem>();
            Title = "新個人精選";
            ImagePath = "Images/green.png";
        }
    }
}
