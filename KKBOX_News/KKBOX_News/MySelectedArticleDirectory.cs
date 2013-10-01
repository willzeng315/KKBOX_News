using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KKBOX_News
{
    public class ListItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate One
        {
            get;
            set;
        }

        public DataTemplate Two
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(Object item, DependencyObject container)
        {
            
            MySelectedArticleDirectory myItem = item as MySelectedArticleDirectory;
            if (myItem != null)
            {
                if (myItem.Type == "One")
                {
                    return One;
                }
                else
                {
                    return Two;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }


    public class MySelectedArticleDirectory 
    {
        public List<ArticleItem> LeftArticleItemList;
        public List<ArticleItem> RightArticleItemList;

        public String RightImagePath
        {
            get;
            set;
        }

        public String LeftImagePath
        {
            get;
            set;
        }

        public String RightTitle
        {
            get;
            set;
        }

        public String LeftTitle
        {
            get;
            set;
        }

        public String Type
        {
            get;
            set;
        }

        public MySelectedArticleDirectory()
        {

            LeftArticleItemList = new List<ArticleItem>();
            RightArticleItemList = new List<ArticleItem>();
        }
    }
}
