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
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using System.IO;
using KKBOX_News.DBService;
using KKBOX_News.ViewModels;

namespace KKBOX_News
{
    public partial class AddMySelectPage : PhoneApplicationPage
    {
        public AddMySelectPage()
        {
            InitializeComponent();
            DataContext = AdderModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (coverChooser != null)
            {
                selectedImageName = coverChooser.GetSelectedImageName();
                AdderModel.SetCoverImage(coverChooser.GetChooseImage());
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
            AdderModel.AddArticleAction(selectedImageName, GetLastDirectoryIndex());
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

        private AdderMySelectPageModel adderModel;
        public AdderMySelectPageModel AdderModel
        {
            get
            {
                if (adderModel == null)
                {
                    adderModel = new AdderMySelectPageModel();
                }
                return adderModel;
            }
        }


        #endregion
    }
}