using System;
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
using Community.CsharpSqlite.SQLiteClient;

namespace KKBOX_News
{
    public partial class CoverInformationEditPage : PhoneApplicationPage,INotifyPropertyChanged
    {
        const Int32 ImageLength = 200;

        public enum PageMode { NULL, READ_FROM_DIR, READ_FROM_XML };

        public CoverInformationEditPage()
        {
            InitializeComponent();
            isReturnFromPhotoChooser = false;
            DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;
            if (!isReturnFromPhotoChooser)
            {
                if (parameters.ContainsKey("Title"))
                {
                    CoverTitle = parameters["Title"];
                }
                if (parameters.ContainsKey("DirectoryIndex"))
                {
                    directoryIndex = Int32.Parse(parameters["DirectoryIndex"]);
                }
            }
            isReturnFromPhotoChooser = false;
        }

        #region Property

        private Boolean isReturnFromPhotoChooser
        {
            set;
            get;
        }

        private Int32 directoryIndex
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
            if (Object.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
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

        private void OnPhotoChooserTaskCompleted(Object sender, PhotoResult args)
        {
            if (args.TaskResult == TaskResult.OK)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(args.ChosenPhoto);

                RetrieveImageName(args);

                LocalImageManipulation.Instance.SaveJpgToIsolateStorage(bitmap, selectedImageName);

                CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(selectedImageName);
            }
        }

        private void RetrieveImageName(PhotoResult args)
        {
            Int32 imageNameBeginIndex = 0;
            for (int i = args.OriginalFileName.Length - 1; i > 0; i--)
            {
                if (args.OriginalFileName[i] == '\\')
                {
                    imageNameBeginIndex = i;
                    break;
                }
            }
            selectedImageName = args.OriginalFileName.Substring(imageNameBeginIndex + 1, (args.OriginalFileName.Length - imageNameBeginIndex) - 4);

            selectedImageName = String.Format("{0}{1}", selectedImageName, "jpg");
        }

        private void UpdateDirectoryInfoToTable()
        {
            DBManager.Instance.UpdateDirectoryToTable(directoryIndex, CoverTitle, selectedImageName);
        }

        private void OnChoosePhotoClick(Object sender, RoutedEventArgs e)
        {
            isReturnFromPhotoChooser = true;
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(OnPhotoChooserTaskCompleted);
            photoChooserTask.Show();
            //PhotoChooser photoChooser = new PhotoChooser();
            //selectedImageName =  photoChooser.GetSelectedImageName();
            //CoverImage = photoChooser.GetChooseImage();

        }

        private void OnConfirmButtonClick(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < App.ViewModel.ArticleDirectories.Count; i++)
            {
                if (App.ViewModel.ArticleDirectories[i].DirectoryIndex == directoryIndex)
                {
                    App.ViewModel.ArticleDirectories[i].Title = CoverTitle;

                    if (selectedImageName != null)
                    {
                        App.ViewModel.ArticleDirectories[i].CoverImage = LocalImageManipulation.Instance.ReadJpgFromStorage(selectedImageName);
                    }

                    UpdateDirectoryInfoToTable();
                    break;
                }
            }
                
            NavigationService.GoBack();
        }

        private void OnCancelButtonClick(Object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}