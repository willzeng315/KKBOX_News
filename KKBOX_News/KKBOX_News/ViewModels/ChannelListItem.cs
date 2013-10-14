using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News
{
    public class ChannelListItem : BindableBase
    {
        #region Property
        private String title = "";
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                SetProperty(ref title, value, "Title");
            }
        }

        public String imagePath = "";
        public String ImagePath
        {
            get
            {
                return imagePath;
            }
            set
            {
                SetProperty(ref imagePath, value, "IconImagePath");
            }
        }



        private String url = "";
        public String Url
        {
            get
            {
                return url;
            }
            set
            {
                SetProperty(ref url, value, "Url");
            }
        }

        public Boolean IsReadFromRss
        {
            get;
            set;
        }

        #endregion
        public ChannelListItem() 
        {
            IsReadFromRss = true;
        }
    }
    
}
