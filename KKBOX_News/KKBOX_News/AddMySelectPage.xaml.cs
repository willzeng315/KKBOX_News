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
using KKBOX_News.ViewModels;
using KKBOX_News.AppService;

namespace KKBOX_News
{
    public partial class AddMySelectPage : PhoneApplicationPage
    {
        public AddMySelectPage()
        {
            InitializeComponent();
            DataContext = AdderModel;
        }

        private void OnChoosePhotoClick(Object sender, RoutedEventArgs e)
        {
            PhotoChooser coverChooser = new PhotoChooser();
            coverChooser.PhotoChoseCompleted += OnPhotoChoseCompleted;
        }

        private void OnPhotoChoseCompleted(BitmapImage cover, String imageName)
        {
            selectedImageName = imageName;
            AdderModel.SetCoverImage(cover);
        }

        private void OnListBoxSelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            listbox.SelectedIndex = -1;
        }

        private void OnConfirmButtonClick(Object sender, RoutedEventArgs e)
        {
            AdderModel.AddArticleAction(selectedImageName);
            NavigationService.GoBack();
        }

        private void OnConcelButtonClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        #region property

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