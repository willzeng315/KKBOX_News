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
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
    public class AdderItem
    {
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
        public AdderItem()
        {
            ItemTitle = "";
            Type = "";
            IsChecked = false;
        }

    }
    public partial class AddMySelectPage : PhoneApplicationPage
    {
        public AddMySelectPage()
        {
            InitializeComponent();
            loadDirectoryIntoList();
            DataContext = this;
            //adderListbox.ItemsSource = AdderListBox;
            //Debug.WriteLine(App.ViewModel.ArticleDirectories.Count);
        }

        private void loadDirectoryIntoList()
        {
            AdderListBox = new ObservableCollection<AdderItem>();
            AdderListBox.Add(new AdderItem() { ItemTitle = "", Type = "space" });
            AdderListBox.Add(new AdderItem(){ItemTitle = "新資料夾", Type = "textbox"});
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

        public ObservableCollection<AdderItem> AdderListBox
        {
            set;
            get;
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
        #endregion 

        private void OnComfirmClick(Object sender, RoutedEventArgs e)
        {

            for (int i = 3; i < AdderListBox.Count; i++) //start with my select because i=0 is space, i=1 is new directory,i=2 is space
            {
                if(AdderListBox[i].Type == "sapce")
                {
                    continue;
                }

                if (AdderListBox[i].IsChecked)
                {
                    App.ViewModel.ArticleDirectories[i - 3].ArticleItemList.Add(myArticleItem);
                }
            }

            if (AdderListBox[1].IsChecked)
            {
                MySelectedArticleDirectory mySelectedArticleDirectory = new MySelectedArticleDirectory();
                mySelectedArticleDirectory.Title = AdderListBox[1].ItemTitle;
                App.ViewModel.ArticleDirectories.Add(mySelectedArticleDirectory);

                Int32 TotalArticleDirectories = App.ViewModel.ArticleDirectories.Count;
                App.ViewModel.ArticleDirectories[TotalArticleDirectories - 1].ArticleItemList.Add(myArticleItem);
            }
            NavigationService.GoBack();
        }

        private void OnConcelClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < AdderListBox.Count; i++)
            {
                Debug.WriteLine(String.Format("{0} {1}", i, AdderListBox[i].IsChecked));
            }
            Debug.WriteLine(AdderListBox[1].ItemTitle);
        }
    }
}