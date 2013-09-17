using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Microsoft.Phone.Tasks;
using System.Diagnostics;

namespace KKBOX_News
{
    public partial class ArticleListPage : PhoneApplicationPage
    {
        private bool IsLinkClick = false;
        private ArticleListPageModel _model;
        public ArticleListPageModel Model
        {
            get
            {
                if (_model == null)
                {
                    _model = new ArticleListPageModel();
                }
                return _model;
            }
        }

        public ArticleListPage()
        {
            InitializeComponent();
            DataContext = Model;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.

            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            string xmlValue = "";
            if (parameters.ContainsKey("XML"))
            {
                xmlValue = parameters["XML"];
            }

            Uri uri = new Uri(xmlValue, UriKind.Absolute); ;
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += webClient_DownloadStringCompleted;
            webClient.DownloadStringAsync(uri);
            Debug.WriteLine("125345236");

        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.


        }


        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            String sXML = e.Result;
            XDocument root = XDocument.Parse(sXML);
            XElement channelRoot = root.Element("rss").Element("channel");
            IEnumerable<XElement> elements = channelRoot.Elements("item");
            ObservableCollection<ArticleItem> items = new ObservableCollection<ArticleItem>();
            foreach (XElement eleItem in elements)
            {
                String sTitle = eleItem.Element("title").Value;
                String sDescription = eleItem.Element("description").Value;
                String sIconPath = ImageRetriever(sDescription);
                String sContent = ContentRetriever(sDescription);
                String sLink = LinkRetriever(sDescription);
                bool sIsItemClik = false;
                ArticleItem newItem = new ArticleItem();
                newItem.Title = sTitle;
                newItem.Content = sContent;
                newItem.IconImagePath = sIconPath;
                newItem.Link = sLink;
                newItem.IsExtended = false;//"Visible";
                //newItem.IsItemClik = sIsItemClik;
                items.Add(newItem);
            }
            Model.Items = items;
        }
        private string ImageRetriever(string sDescription)
        {
            if (sDescription == null) return null;

            string ImageSource = "";

            int startIndex = sDescription.ToString().IndexOf("img src='");
            int EndIndex = sDescription.ToString().IndexOf("jpg");
            int srcLen = EndIndex - startIndex - 6;

            if (startIndex > EndIndex || srcLen < 0)
                return null;

            ImageSource = sDescription.ToString().Substring(startIndex + 9, srcLen);

            return ImageSource;
        }
        private string ContentRetriever(string sDescription)
        {
            if (sDescription == null) return null;

            int maxLength = 200;
            int strLength = 0;
            string fixedString = "";
            fixedString = Regex.Replace(sDescription.ToString(), "<[^>]+>", string.Empty);

            // Remove newline characters.
            fixedString = fixedString.Replace("\r", "").Replace("\n", "");

            // Remove encoded HTML characters.
            fixedString = HttpUtility.HtmlDecode(fixedString);
            int ContentLenth = fixedString.IndexOf("更多文章");
            if (ContentLenth != -1)
            {
                fixedString = fixedString.Substring(0, ContentLenth);
            }

            strLength = fixedString.ToString().Length;

            if (strLength == 0)
            {
                return null;
            }

            else if (strLength >= maxLength)
            {
                fixedString = fixedString.Substring(0, maxLength);
                fixedString = fixedString.Substring(0, fixedString.LastIndexOf(" "));
            }

            fixedString += "...";

            return fixedString;
        }
        private string LinkRetriever(string sDescription)
        {
            if (sDescription == null) return null;

            string LinkSource = "";

            int startIndex = sDescription.ToString().IndexOf("http");
            int EndIndex = sDescription.ToString().IndexOf("html");
            int srcLen = EndIndex - startIndex;

            if (startIndex > EndIndex || srcLen < 0)
                return null;

            LinkSource = sDescription.ToString().Substring(startIndex, srcLen+4);

            return LinkSource;

        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                // Get the SyndicationItem that was tapped.
                ArticleItem sItem = (ArticleItem)listBox.SelectedItem;

                sItem.IsExtended = true;
                
                // Set up the page navigation only if a link actually exists in the feed item.
                //if (sItem.Links.Count > 0 && IsLinkClick)
                //{
                //    // Get the associated URI of the feed item.
                //    Uri uri = sItem.Links.FirstOrDefault().Uri;

                //    // Create a new WebBrowserTask Launcher to navigate to the feed item. 
                //    // An alternative solution would be to use a WebBrowser control, but WebBrowserTask is simpler to use. 
                if (IsLinkClick)
                {
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = new Uri(sItem.Link, UriKind.Absolute);
                    webBrowserTask.Show();
                    IsLinkClick = false;
                }
               // Debug.WriteLine("Selection");

                //}
            }
            listBox.SelectedItem = null;
        }

        private void TextBlock_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            IsLinkClick = true;
            //Debug.WriteLine("Manipulation");  

            //afsdf
        }
    }
}