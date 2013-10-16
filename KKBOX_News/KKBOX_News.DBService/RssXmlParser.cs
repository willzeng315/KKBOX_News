using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;

namespace KKBOX_News.AppService
{
    public class RssXmlParser
    {
        public RssXmlParser()
        {
            selectedArticles = new ObservableCollection<ArticleItem>();
        }

        public ObservableCollection<ArticleItem> GetXmlParserResult(String sXML)
        {
            selectedArticles = new ObservableCollection<ArticleItem>();

            try
            {
                XDocument root = XDocument.Parse(sXML);
                XElement channelRoot = root.Element("rss").Element("channel");
                IEnumerable<XElement> elements = channelRoot.Elements("item");
                foreach (XElement eleItem in elements)
                {
                    String RssTitle = eleItem.Element("title").Value;
                    String RssDescription = eleItem.Element("description").Value;
                    String RssIconPath = ImageRetriever(RssDescription);
                    String RssContent = ContentRetriever(RssDescription);
                    String RssLink = LinkRetriever(RssDescription);

                    ArticleItem newItem = new ArticleItem();

                    newItem.Title = RssTitle;
                    newItem.Content = RssContent;
                    newItem.IconImagePath = RssIconPath;
                    newItem.Link = RssLink;
                    newItem.IsExtended = false;
                    selectedArticles.Add(newItem);
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

            return selectedArticles;
        }

        private String ImageRetriever(String description)
        {
            String ImageSource = "";
            const Int32 IndexShift = 9; //Length of "img src='"

            if (description != null)
            {
                Int32 startIndex = description.ToString().IndexOf("img src='");
                Int32 EndIndex = description.ToString().IndexOf("jpg");
                Int32 srcLen = EndIndex - startIndex;

                if (srcLen > 0)
                {
                    ImageSource = description.ToString().Substring(startIndex + IndexShift, srcLen);
                }
            }
            return ImageSource;
        }

        private String ContentRetriever(String description)
        {
            String ContentString = "";

            if (description != null)
            {
                ContentString = Regex.Replace(description.ToString(), "<[^>]+>", String.Empty);

                ContentString = ContentString.Replace("\r", "").Replace("\n", "");

                ContentString = HttpUtility.HtmlDecode(ContentString);

                Int32 ContentLenth = ContentString.IndexOf("更多文章");

                if (ContentLenth != -1)
                {
                    ContentString = ContentString.Substring(0, ContentLenth);
                }
            }
            return ContentString;
        }

        private String LinkRetriever(String description)
        {
            String LinkSource = "";
            const Int32 IndexShift = 4; // Length of "http"

            if (description != null)
            {
                Int32 startIndex = description.ToString().IndexOf("http");
                Int32 EndIndex = description.ToString().IndexOf("html");
                Int32 srcLen = EndIndex - startIndex;

                if (startIndex < EndIndex && srcLen > 0)
                {
                    LinkSource = description.ToString().Substring(startIndex, srcLen + IndexShift);
                }
            }
            return LinkSource;
        }

        private ObservableCollection<ArticleItem> selectedArticles
        {
            get;
            set;
        }
    }
}
