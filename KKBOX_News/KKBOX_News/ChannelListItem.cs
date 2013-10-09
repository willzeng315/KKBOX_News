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

        public String iconImagePath = "";
        public String IconImagePath
        {
            get
            {
                return iconImagePath;
            }
            set
            {
                SetProperty(ref iconImagePath, value, "IconImagePath");
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
        #endregion
        public ChannelListItem() { }
    }
    
}
