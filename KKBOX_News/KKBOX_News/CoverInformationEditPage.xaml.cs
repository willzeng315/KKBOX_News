﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;

namespace KKBOX_News
{
    public partial class CoverInformationEditPage : PhoneApplicationPage,INotifyPropertyChanged
    {
        public CoverInformationEditPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OnPhotoChooserTaskCompleted(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {

                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.ChosenPhoto);
                    CoverImage = bitmap;
                    Int32 imageNameBeginIndex = 0;
                    for (int i = e.OriginalFileName.Length - 1; i > 0; i--)
                    {
                        if (e.OriginalFileName[i] == '\\')
                        {
                            imageNameBeginIndex = i;
                            break;
                        }
                    }
                    selectedImageName = e.OriginalFileName.Substring(imageNameBeginIndex + 1, (e.OriginalFileName.Length - imageNameBeginIndex) - 4);

                    selectedImageName = String.Format("{0}{1}", selectedImageName, "jpg");


                    if (!myIsolatedStorage.FileExists(selectedImageName))
                    {
                        IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(selectedImageName);

                        WriteableBitmap wb = new WriteableBitmap(bitmap);

                        Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 100);

                        fileStream.Close();
                    }
                }
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("Title"))
            {
                CoverTitle = parameters["Title"];
            }
            if (parameters.ContainsKey("DirectoryIndex"))
            {
                directoryIndex = Int32.Parse(parameters["DirectoryIndex"]);
            }

        }

        #region Property
        private Int32 directoryIndex
        {
            get;
            set;
        }

        private Int32 selectedImageCount
        {
            get;
            set;
        }

        private String selectedImageName
        {
            get;
            set;
        }

        private String coverTitle="";
        public String CoverTitle
        {

            get
            {
                return coverTitle;
            }
            set
            {
                SetProperty(ref coverTitle, value, "CoverTitle");
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected Boolean SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void OnChoosePhotoClick(Object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(OnPhotoChooserTaskCompleted);
            photoChooserTask.Show();
        }

        private void OnComfirmClick(object sender, RoutedEventArgs e)
        {
            App.ViewModel.ArticleDirectories[directoryIndex - 1].Title = CoverTitle;
            App.ViewModel.ArticleDirectories[directoryIndex - 1].CoverImage = LocalImageManipulation.ReadJpgFromLocal(selectedImageName);
            NavigationService.GoBack();
        }

        private void OnCancelClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
      
    }
}