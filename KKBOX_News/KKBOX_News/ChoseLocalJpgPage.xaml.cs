using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KKBOX_News
{
    public class BitmapImageProperty
    {
        public String ImageName
        {
            set;
            get;
        }

        public BitmapImage LocalImage
        {
            set;
            get;
        }

        public BitmapImageProperty()
        {
        }

    }
    public partial class ChoseLocalJpgPage : PhoneApplicationPage
    {
        public ChoseLocalJpgPage()
        {
            InitializeComponent();

            DisplayImages = new ObservableCollection<BitmapImageProperty>();
            for (int i = 0; i < LocalImageManipulation.Images.Count; i++)
            {
                DisplayImages.Add(new BitmapImageProperty()
                {
                    ImageName = LocalImageManipulation.Images[i].ImageName,
                    LocalImage = LocalImageManipulation.ReadJpgFromLocal(LocalImageManipulation.Images[i].ImageName)
                });
            }

            
            DataContext = this;
        }

        private void OnImageSelectClick(Object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            BitmapImageProperty bitmapImageProperty = (BitmapImageProperty)button.DataContext;
            NavigationService.GoBack();

        }

        public ObservableCollection<BitmapImageProperty> DisplayImages
        {
            set;
            get;
        }
    }
}