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

        public void SaveJpgToIsolateStorage(BitmapImage bitmap, String JpgPath)
        {
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!myIsolatedStorage.FileExists(JpgPath))
                {
                    IsolatedStorageFileStream fileStream = myIsolatedStorage.CreateFile(JpgPath);

                    WriteableBitmap wb = new WriteableBitmap(bitmap);

                    Extensions.SaveJpeg(wb, fileStream, SquareLength, SquareLength, 0, 100);

                    fileStream.Close();
                }
            }
        }

        public void SaveJpgToIsolateStorage(String JpgPath, String JpgName)
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
                Extensions.SaveJpeg(wb, fileStream, SquareLength, SquareLength, 0, 100);

                //wb.SaveJpeg(fileStream, wb.PixelWidth, wb.PixelHeight, 0, 85);
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
