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
using KKBOX_News.DBService;

namespace KKBOX_News
{
    public partial class ExternalArticlesInfoPage : PhoneApplicationPage
    {
        public ExternalArticlesInfoPage()
        {
            InitializeComponent();
            ExternalArticle = new ArticleItem();
            ExternalArticle.Link = "http://";
            
            DataContext = this;
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            DBManager.Instance.InsertExternalArticleToTable(ExternalArticle);
            this.NavigationService.GoBack();
        }

        public ArticleItem ExternalArticle
        {
            get;
            set;
        }
    
    }
}