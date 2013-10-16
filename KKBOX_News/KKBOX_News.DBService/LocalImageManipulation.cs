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

namespace KKBOX_News.AppService
{
    public class LocalImageManipulation
    {
        const Int32 SquareLength = 200;

        private static LocalImageManipulation _instance;

        private LocalImageManipulation() { }

        public static LocalImageManipulation Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocalImageManipulation();
                return _instance;
            }
        }

        public void SaveBitImageAsJpgToStorage(BitmapImage bitmap, String JpgName)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!myIsolatedStorage.FileExists(JpgName))
                {
                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(JpgName);

                    WriteableBitmap wb = new WriteableBitmap(bitmap);

                    Extensions.SaveJpeg(wb, fileStream, SquareLength, SquareLength, 0, 100);

                    fileStream.Close();
                }
            }
        }

        public void SaveJpgToIsolateStorage(String JpgPath, String JpgName)
        {
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

                Extensions.SaveJpeg(wb, fileStream, SquareLength, SquareLength, 0, 100);

                fileStream.Close();
            }
        }

        public BitmapImage ReadJpgFromStorage(String JpgName)
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
    }
}
