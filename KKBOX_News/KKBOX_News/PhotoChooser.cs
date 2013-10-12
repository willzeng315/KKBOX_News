using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KKBOX_News
{
    public class PhotoChooser
    {
        const Int32 SquareLength = 200;

        public PhotoChooser()
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(OnPhotoChooserTaskCompleted);
            photoChooserTask.Show();
        }

        private void OnPhotoChooserTaskCompleted(Object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(e.ChosenPhoto);

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

                        Extensions.SaveJpeg(wb, fileStream, SquareLength, SquareLength, 0, 100);

                        fileStream.Close();
                    }

                    chooseImage = LocalImageManipulation.Instance.ReadJpgFromStorage(selectedImageName);
                }
            }
        }

        public BitmapImage GetChooseImage()
        {
            return chooseImage;
        }

        public String GetSelectedImageName()
        {
            return selectedImageName;
        }

        private BitmapImage chooseImage;

        private String selectedImageName;

    }
}
