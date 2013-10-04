using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace KKBOX_News
{

    public class ImageProperty
    {
        public String ImageName
        {
            set;
            get;
        }

        public String ImagePath
        {
            set;
            get;
        }

        public ImageProperty()
        {
        }

    }

    public class LocalImageManipulation
    {

        static LocalImageManipulation()
        {
            Images = new ObservableCollection<ImageProperty>();

            Images.Add(new ImageProperty() { ImageName = "KKBOX.jpg", ImagePath = "Images/KKBOX.jpg" });
            Images.Add(new ImageProperty() { ImageName = "A.jpg", ImagePath = "Images/A.jpg" });
            Images.Add(new ImageProperty() { ImageName = "B.jpg", ImagePath = "Images/B.jpg" });
            Images.Add(new ImageProperty() { ImageName = "C.jpg", ImagePath = "Images/C.jpg" });
            Images.Add(new ImageProperty() { ImageName = "D.jpg", ImagePath = "Images/D.jpg" });
            Images.Add(new ImageProperty() { ImageName = "E.jpg", ImagePath = "Images/E.jpg" });
            Images.Add(new ImageProperty() { ImageName = "F.jpg", ImagePath = "Images/F.jpg" });
        }
        public static void SaveJpgsToLocalData()
        {
          for (int i = 0; i < Images.Count; i++)
            {
                SaveJpgToLocal(Images[i].ImagePath, Images[i].ImageName);
            }
        }
        public static void SaveJpgToLocal(String JpgPath, String JpgName)
        {

            // Create virtual store and file stream. Check for duplicate tempJPEG files.
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (myIsolatedStorage.FileExists(JpgName))
                {
                    myIsolatedStorage.DeleteFile(JpgName);
                }

                IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(JpgName);

                StreamResourceInfo sri = null;
                Uri uri = new Uri(JpgPath, UriKind.Relative);
                sri = Application.GetResourceStream(uri);

                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(sri.Stream);
                WriteableBitmap wb = new WriteableBitmap(bitmap);

                // Encode WriteableBitmap object to a JPEG stream.
                Extensions.SaveJpeg(wb, fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);

                //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
                fileStream.Close();
            }
        }
        public static BitmapImage ReadJpgFromLocal(String JpgName)
        {
            BitmapImage jpgImage = new BitmapImage();

            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile(JpgName, FileMode.Open, FileAccess.Read))
                {
                    jpgImage.SetSource(fileStream);
                    return jpgImage;
                }
            }
        }
         public static ObservableCollection<ImageProperty> Images
        {
            set;
            get;
        }

    }
}
