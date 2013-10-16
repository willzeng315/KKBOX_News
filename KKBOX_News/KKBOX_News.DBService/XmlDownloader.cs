using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KKBOX_News.AppService
{
    public class XmlDownloader
    {
        public delegate void XmlLoadHandler(String result);
        public event XmlLoadHandler XmlLoadCompleted = null;

        public XmlDownloader()
        {
        }

        public void GetStringResponse(String xmlURL)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += OnDownloadStringCompleted;
            try
            {
                webClient.DownloadStringAsync(new Uri(xmlURL, UriKind.Absolute));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                if (XmlLoadCompleted != null)
                {
                    XmlLoadCompleted("");
                }
            }
        }

        private void OnDownloadStringCompleted(Object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                String strResult = "";
                try
                {
                    strResult = e.Result;
                }
                catch (Exception)
                {
                    strResult = "";
                }

                if (XmlLoadCompleted != null)
                {
                    XmlLoadCompleted(strResult);
                }
            }
            else
            {
                if (XmlLoadCompleted != null)
                {
                    XmlLoadCompleted("");
                }
            }
        }
    }
}
