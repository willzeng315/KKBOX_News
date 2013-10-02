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
    public partial class WebPage : PhoneApplicationPage
    {
        public WebPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            IDictionary<String, String> parameters = this.NavigationContext.QueryString;
            String Link = "";
            if (parameters.ContainsKey("Link"))
            {
                Link = parameters["Link"];
            }
            webBrowser1.Navigate(new Uri(Link, UriKind.Absolute));     
        }
    }
}