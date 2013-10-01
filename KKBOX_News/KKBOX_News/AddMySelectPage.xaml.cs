using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace KKBOX_News
{
    public partial class AddMySelectPage : PhoneApplicationPage
    {
        public AddMySelectPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {


            IDictionary<String, String> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("Title"))
            {
                Title = parameters["Title"];
            }
            if (parameters.ContainsKey("Content"))
            {
                Content = parameters["Content"];
            }
            if (parameters.ContainsKey("Link"))
            {
                Link = parameters["Link"];
            }
            if (parameters.ContainsKey("ImagePath"))
            {
                ImagePath = parameters["ImagePath"];
            }

        }
        #region property
        public String Title
        {
            get;
            set;
        }

        public String Content
        {
            get;
            set;
        }

        public String Link
        {
            get;
            set;
        }

        public String ImagePath
        {
            get;
            set;
        }
        #endregion 
    }
}