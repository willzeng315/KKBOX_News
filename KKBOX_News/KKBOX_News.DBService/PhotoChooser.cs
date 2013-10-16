using KKBOX_News.AppService;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KKBOX_News.AppService
{
    public class PhotoChooser
    {
        const Int32 SquareLength = 200;

        public delegate void PhotoChoseHandler(BitmapImage cover, String imageName);
        public PhotoChoseHandler PhotoChoseCompleted = null;

        public PhotoChooser()
        {
            PhotoChooserTask photoChooserTask;
            photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += new EventHandler<PhotoResult>(OnPhotoChooserTaskCompleted);
            photoChooserTask.Show();
        }

        private void RetrieveImageName(String imagePath)
        {
            Int32 imageNameBeginIndex = 0;

            for (int i = imagePath.Length - 1; i > 0; i--)
            {
                if (imagePath[i] == '\\')
                {
                    imageNameBeginIndex = i;
                    break;
                }
            }
            selectedImageName = imagePath.Substring(imageNameBeginIndex + 1, (imagePath.Length - imageNameBeginIndex) - 4);

            selectedImageName = String.Format("{0}{1}", selectedImageName, "jpg");
        }

        private void OnPhotoChooserTaskCompleted(Object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(e.ChosenPhoto);

                RetrieveImageName(e.OriginalFileName);

                LocalImageManipulation.Instance.SaveBitImageAsJpgToStorage(bitmap, selectedImageName);

                chooseImage = LocalImageManipulation.Instance.ReadJpgFromStorage(selectedImageName);

                if (PhotoChoseCompleted != null)
                {
                    PhotoChoseCompleted(chooseImage, selectedImageName);
                }

            }
        }

        private BitmapImage chooseImage;

        private String selectedImageName;
    }
}
